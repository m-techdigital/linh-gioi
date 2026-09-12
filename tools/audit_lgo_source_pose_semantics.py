#!/usr/bin/env python3.12
"""Audit semantic ownership and shared-body invariants of source-pose review packs."""
import argparse
import json
from pathlib import Path

import numpy as np
from PIL import Image

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
    body_hash = None
    class_ids, genders, levels = set(), set(), set()
    for directory in SLOT_DIRS:
        manifest = json.loads((pack / directory / 'atlas-review.json').read_text())
        body_hash = body_hash or (manifest.get('basePoseAtlasSha256'), manifest.get('basePoseManifestSha256'))
        if body_hash != (manifest.get('basePoseAtlasSha256'), manifest.get('basePoseManifestSha256')):
            errors.append('SLOT_BODY_AUTHORITY_MISMATCH:' + directory)
        item = manifest.get('itemId', '').split('_')
        if len(item) >= 3:
            class_ids.add(item[0]); genders.add(item[1])
        levels.add(manifest.get('unlockLevel'))
    if len(class_ids) != 1: errors.append('CLASS_ID_INCONSISTENT')
    if len(genders) != 1: errors.append('GENDER_INCONSISTENT')
    if len(levels) != 1: errors.append('ITEM_LEVEL_INCONSISTENT')
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
            'gender': next(iter(genders), ''), 'level': next(iter(levels), 0),
            'samplingDivisor': body.get('samplingDivisor'), 'bodyAuthority': body_hash,
            'poses': rows, 'errors': errors, 'status': 'PASS' if not errors else 'FIX_REQUIRED'}


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
