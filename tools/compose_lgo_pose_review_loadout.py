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


def digest(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def compose_loadout(packs: dict[str, Path], selection: dict[str, str], output: Path) -> dict:
    if set(selection) != set(SLOTS):
        raise ValueError('Selection must contain exactly the ten canonical slots')
    unknown = sorted(set(selection.values()) - set(packs))
    if unknown:
        raise ValueError('Unknown pack aliases: ' + ', '.join(unknown))
    if output.exists():
        raise FileExistsError('Output already exists: ' + str(output))

    authority = packs[next(iter(packs))]
    output.mkdir(parents=True)
    try:
        for name in ('atlas-review.png', 'atlas-review.json'):
            shutil.copy2(authority / name, output / name)
        body = {'atlas': digest(output / 'atlas-review.png'),
                'manifest': digest(output / 'atlas-review.json')}
        items = []
        for slot in SLOTS:
            alias = selection[slot]
            source = packs[alias] / DIRECTORIES[slot]
            manifest = json.loads((source / 'atlas-review.json').read_text())
            if manifest.get('reviewSlot') != slot:
                raise ValueError('Slot manifest mismatch: ' + slot)
            if manifest.get('fitFamily') != 'vo_male_v3':
                raise ValueError('Fit family mismatch: ' + slot)
            if (manifest.get('basePoseAtlasSha256') != body['atlas']
                    or manifest.get('basePoseManifestSha256') != body['manifest']):
                raise ValueError('Body authority mismatch: ' + slot)
            item_id = manifest.get('itemId')
            unlock_level = manifest.get('unlockLevel')
            if not item_id or not isinstance(unlock_level, int) or unlock_level < 1:
                raise ValueError('Item identity missing: ' + slot)
            shutil.copytree(source, output / DIRECTORIES[slot])
            items.append({'slotId': slot, 'itemId': item_id, 'sourcePack': alias,
                          'unlockLevel': unlock_level, 'fitFamily': manifest['fitFamily'],
                          'atlasSha256': manifest['atlasSha256']})
        loadout = {'status': 'REVIEW_ONLY', 'runtimeEligible': False,
                   'resolver': 'slotId -> itemId', 'atomicPreflight': True,
                   'bodyProfile': 'vo_male_v3', 'skeletonVersion': 'vo_source_pose_v3',
                   'bodyHashes': body, 'items': items}
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
