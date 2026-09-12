#!/usr/bin/env python3
"""Compose one source-pose review loadout from independently versioned slot items."""
from __future__ import annotations

import argparse
import hashlib
import json
import shutil
from pathlib import Path

SLOTS = (
    'main_weapon', 'head_hair', 'inner_top', 'outer_top', 'lower_body',
    'waist_belt', 'arm_guard', 'footwear', 'shoulder_chest_guard', 'class_accessory',
)
DIRECTORIES = {slot: slot.replace('_', '-') + '-review' for slot in SLOTS}
COMPLETE_GARMENT_LAYER_PROFILE = 'lgo_complete_garment_layers_v1'
# Same actor range as the legacy renderer (body=24, equipment=25..34).
# Complete overlapping clothes need semantic order instead of slot-list order.
FRONT_ORDERS = dict(zip(('inner_top', 'lower_body', 'footwear', 'outer_top',
                        'waist_belt', 'arm_guard', 'shoulder_chest_guard',
                        'class_accessory', 'head_hair', 'main_weapon'), range(25, 35)))


def validate_layer_profile(manifest: dict, slot: str) -> str:
    profile = manifest.get('layerOrderProfile') or 'legacy'
    if profile == 'legacy':
        return profile
    if profile != COMPLETE_GARMENT_LAYER_PROFILE:
        raise ValueError('Unknown layer order profile: ' + profile)
    if not manifest.get('sprites'):
        raise ValueError('Layer profile needs component sprites: ' + slot)
    orders = {'front': FRONT_ORDERS[slot], 'back': FRONT_ORDERS[slot] - 11}
    for part in manifest['sprites']:
        if part.get('componentId') not in orders or part.get('order') != orders[part['componentId']]:
            raise ValueError('Layer order mismatch: ' + slot + '/' + str(part.get('componentId')))
    return profile


def digest(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def body_identity(manifest: dict) -> tuple[str, str]:
    # Match the legacy div4 runtime default; new authorities declare their profile.
    family = manifest.get('fitFamily') or 'vo_male_v3'
    gender = manifest.get('gender') or 'male'
    if gender not in ('male', 'female'):
        raise ValueError('Invalid body gender: ' + str(gender))
    return family, gender


def item_identity(manifest: dict) -> tuple[str, str]:
    parts = manifest.get('itemId', '').split('_', 2)
    if len(parts) != 3 or parts[0] not in ('vo', 'kiem', 'phap', 'co', 'linh') or parts[1] not in ('male', 'female'):
        raise ValueError('Invalid item identity: ' + str(manifest.get('itemId')))
    for field, value in (('classId', parts[0]), ('gender', parts[1])):
        if manifest.get(field) and manifest[field] != value:
            raise ValueError('Item identity metadata mismatch: ' + field)
    return parts[0], parts[1]


def compose_loadout(packs: dict[str, Path], selection: dict[str, str], output: Path) -> dict:
    if set(selection) != set(SLOTS):
        raise ValueError('Selection must contain exactly the ten canonical slots')
    unknown = sorted(set(selection.values()) - set(packs))
    if unknown:
        raise ValueError('Unknown pack aliases: ' + ', '.join(unknown))
    if output.exists():
        raise FileExistsError('Output already exists: ' + str(output))

    authority = packs[next(iter(packs))]
    authority_manifest = json.loads((authority / 'atlas-review.json').read_text())
    family, gender = body_identity(authority_manifest)
    if authority_manifest.get('atlasSha256') != digest(authority / 'atlas-review.png'):
        raise ValueError('Body atlas hash mismatch')
    class_id = authority_manifest.get('classId')
    output.mkdir(parents=True)
    try:
        for name in ('atlas-review.png', 'atlas-review.json'):
            shutil.copy2(authority / name, output / name)
        body = {'atlas': digest(output / 'atlas-review.png'),
                'manifest': digest(output / 'atlas-review.json')}
        items = []
        layer_profiles = set()
        for slot in SLOTS:
            alias = selection[slot]
            source = packs[alias] / DIRECTORIES[slot]
            manifest = json.loads((source / 'atlas-review.json').read_text())
            if manifest.get('reviewSlot') != slot:
                raise ValueError('Slot manifest mismatch: ' + slot)
            layer_profiles.add(validate_layer_profile(manifest, slot))
            if manifest.get('fitFamily') != family:
                raise ValueError('Fit family mismatch: ' + slot)
            if (manifest.get('basePoseAtlasSha256') != body['atlas']
                    or manifest.get('basePoseManifestSha256') != body['manifest']):
                raise ValueError('Body authority mismatch: ' + slot)
            item_class, item_gender = item_identity(manifest)
            if item_gender != gender:
                raise ValueError('Gender mismatch: ' + slot)
            class_id = class_id or item_class
            if item_class != class_id:
                raise ValueError('Class mismatch: ' + slot)
            atlas_hash = digest(source / 'atlas-review.png')
            if manifest.get('atlasSha256') != atlas_hash:
                raise ValueError('Item atlas hash mismatch: ' + slot)
            item_id = manifest.get('itemId')
            unlock_level = manifest.get('unlockLevel')
            if type(unlock_level) is not int or unlock_level < 1:
                raise ValueError('Item identity missing: ' + slot)
            shutil.copytree(source, output / DIRECTORIES[slot])
            items.append({'slotId': slot, 'itemId': item_id, 'sourcePack': alias,
                          'unlockLevel': unlock_level, 'fitFamily': manifest['fitFamily'],
                          'atlasSha256': atlas_hash})
        if len(layer_profiles) != 1:
            raise ValueError('Layer order profile mismatch across loadout')
        loadout = {'status': 'REVIEW_ONLY', 'runtimeEligible': False,
                   'resolver': 'slotId -> itemId', 'atomicPreflight': True,
                   'bodyProfile': authority_manifest.get('bodyProfile') or family,
                   'fitFamily': family, 'gender': gender, 'classId': class_id,
                   'bodyHashes': body, 'items': items}
        if authority_manifest.get('skeletonVersion'):
            loadout['skeletonVersion'] = authority_manifest['skeletonVersion']
        if layer_profiles != {'legacy'}:
            loadout['layerOrderProfile'] = next(iter(layer_profiles))
        (output / 'review-loadout.json').write_text(json.dumps(loadout, indent=2) + '\n')
        return loadout
    except Exception:
        shutil.rmtree(output)
        raise


def parse_pairs(values: list[str], label: str) -> dict[str, str]:
    result = {}
    for value in values:
        if '=' not in value:
            raise ValueError(label + ' must use name=value: ' + value)
        key, item = value.split('=', 1)
        if not key or not item or key in result:
            raise ValueError('Invalid or duplicate ' + label + ': ' + value)
        result[key] = item
    return result


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--pack', action='append', required=True, help='alias=/absolute/pack')
    parser.add_argument('--select', action='append', required=True, help='slot=alias')
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    packs = {alias: Path(path) for alias, path in parse_pairs(args.pack, 'pack').items()}
    loadout = compose_loadout(packs, parse_pairs(args.select, 'selection'), args.output)
    print(json.dumps({'output': str(args.output), 'items': len(loadout['items']),
                      'levels': sorted({item['unlockLevel'] for item in loadout['items']})}))


if __name__ == '__main__':
    main()
