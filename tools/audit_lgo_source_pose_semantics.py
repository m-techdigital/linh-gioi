#!/usr/bin/env python3.12
"""Check pack integrity, body identity and excessive accessory coverage.

These structural checks do not establish garment completeness, correct pixel
ownership, design fidelity or visual quality in the Player.
"""
import argparse
import json
from pathlib import Path

import numpy as np
from PIL import Image

from compose_lgo_pose_review_loadout import body_identity, digest, item_identity

POSES = ('idle', 'run_contact_a', 'run_a', 'run_contact_b', 'run_b', 'jump_tuck')
SLOT_DIRS = ('main-weapon-review', 'head-hair-review', 'inner-top-review', 'outer-top-review',
             'lower-body-review', 'waist-belt-review', 'arm-guard-review', 'footwear-review',
             'shoulder-chest-guard-review', 'class-accessory-review')
ACCESSORY_MAX_FULL_ALPHA_SHARE = 0.15


def source_mask(part: dict) -> np.ndarray:
    path = Path(part['source'])
    with Image.open(path) as image:
        alpha = np.asarray(image.convert('RGBA'))[:, :, 3]
    if alpha.shape != (1536, 1024):
        raise ValueError('Source is outside canonical 1024x1536 canvas: ' + str(path))
    return alpha > 8


def load_pose_masks(pack: Path, pose: str) -> tuple[np.ndarray, dict[str, np.ndarray]]:
    body = json.loads((pack / 'atlas-review.json').read_text())
    body_parts = [part for part in body['sprites'] if part['id'] == pose]
    if len(body_parts) != 1:
        raise ValueError(f'Expected one body sprite for {pose}: {pack}')
    body_mask = source_mask(body_parts[0])
    slots = {}
    for directory in SLOT_DIRS:
        manifest = json.loads((pack / directory / 'atlas-review.json').read_text())
        parts = [part for part in manifest['sprites'] if part['id'] == pose]
        if not parts:
            raise ValueError(f'Missing {directory}/{pose}: {pack}')
        mask = np.zeros_like(body_mask)
        for part in parts:
            mask |= source_mask(part)
        slots[directory.removesuffix('-review').replace('-', '_')] = mask
    return body_mask, slots


def audit_pack(label: str, pack: Path) -> dict:
    pack = pack.resolve()
    body = json.loads((pack / 'atlas-review.json').read_text())
    errors, rows = [], []
    body_hash = (digest(pack / 'atlas-review.png'), digest(pack / 'atlas-review.json'))
    family, gender = body_identity(body)
    if body.get('atlasSha256') != body_hash[0]:
        errors.append('BODY_ATLAS_HASH_MISMATCH')
    class_ids, genders, levels = set(), set(), set()
    for directory in SLOT_DIRS:
        manifest = json.loads((pack / directory / 'atlas-review.json').read_text())
        if body_hash != (manifest.get('basePoseAtlasSha256'), manifest.get('basePoseManifestSha256')):
            errors.append('SLOT_BODY_AUTHORITY_MISMATCH:' + directory)
        if manifest.get('atlasSha256') != digest(pack / directory / 'atlas-review.png'):
            errors.append('SLOT_ATLAS_HASH_MISMATCH:' + directory)
        if manifest.get('fitFamily') != family:
            errors.append('SLOT_FIT_FAMILY_MISMATCH:' + directory)
        try:
            item_class, item_gender = item_identity(manifest)
            class_ids.add(item_class); genders.add(item_gender)
            if body.get('classId') and item_class != body['classId']:
                errors.append('SLOT_CLASS_MISMATCH:' + directory)
            if item_gender != gender:
                errors.append('SLOT_GENDER_MISMATCH:' + directory)
        except ValueError:
            errors.append('ITEM_ID_INVALID:' + directory)
        level = manifest.get('unlockLevel')
        if type(level) is not int or level < 1:
            errors.append('ITEM_LEVEL_INVALID:' + directory)
        else:
            levels.add(level)
    if len(class_ids) != 1: errors.append('CLASS_ID_INCONSISTENT')
    if len(genders) != 1: errors.append('GENDER_INCONSISTENT')
    for pose in POSES:
        base, slots = load_pose_masks(pack, pose)
        full = base.copy()
        for mask in slots.values(): full |= mask
        share = float(slots['class_accessory'].sum() / max(1, full.sum()))
        rows.append({'pose': pose, 'fullAlphaPixels': int(full.sum()),
                     'classAccessoryAlphaPixels': int(slots['class_accessory'].sum()),
                     'classAccessoryFullAlphaShare': round(share, 4)})
        if share > ACCESSORY_MAX_FULL_ALPHA_SHARE:
            errors.append(f'CLASS_ACCESSORY_OWNS_OUTFIT:{pose}:{share:.4f}')
    return {'label': label, 'pack': str(pack), 'classId': next(iter(class_ids), ''),
            'gender': gender, 'fitFamily': family,
            'level': next(iter(levels)) if len(levels) == 1 else None, 'levels': sorted(levels),
            'samplingDivisor': body.get('samplingDivisor'), 'bodyAuthority': body_hash,
            'poses': rows, 'errors': errors, 'status': 'PASS' if not errors else 'FIX_REQUIRED',
            'visualStatus': 'NOT_ASSESSED', 'garmentCompleteness': 'NOT_ASSESSED'}


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--pack', action='append', required=True, help='LABEL=PACK_DIRECTORY')
    parser.add_argument('--output', type=Path, required=True)
    parser.add_argument('--allow-failures', action='store_true')
    args = parser.parse_args()
    records = []
    for value in args.pack:
        if '=' not in value: parser.error('--pack must be LABEL=PACK_DIRECTORY')
        label, path = value.split('=', 1)
        records.append(audit_pack(label, Path(path)))
    report = {'status': 'PASS' if all(row['status'] == 'PASS' for row in records) else 'FIX_REQUIRED',
              'accessoryMaxFullAlphaShare': ACCESSORY_MAX_FULL_ALPHA_SHARE, 'records': records}
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n')
    print(f"LGO_SOURCE_POSE_SEMANTIC_AUDIT_{report['status']} packs={len(records)} output={args.output}")
    if report['status'] != 'PASS' and not args.allow_failures: raise SystemExit(1)


if __name__ == '__main__':
    main()
