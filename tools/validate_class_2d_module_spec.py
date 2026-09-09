#!/usr/bin/env python3
"""Validate the Vo document contract and reference pack naming, never visual quality."""
from __future__ import annotations

import argparse
from pathlib import Path
import re
import struct

SLOTS = (
    'main_weapon', 'head_hair', 'inner_top', 'outer_top', 'lower_body',
    'waist_belt', 'arm_guard', 'footwear', 'shoulder_chest_guard', 'class_accessory',
)
LEVELS = ('lv001', *(f'lv{i:03}' for i in range(10, 101, 10)))
CLASSES = ('vo', 'kiem', 'phap', 'co', 'linh')
GENDERS = ('male', 'female')
DOCS = (
    'docs/art/LGO-CLASS-2D-MODULE-STANDARD-v1.0.md',
    'docs/art/LGO-CLASS-EQUIPMENT-SLOTS-v1.0.md',
    'docs/art/LGO-CLASS-PROGRESSION-RULES-v1.0.md',
    'docs/art/LGO-CLASS-IMAGE-PROMPT-TEMPLATES-v1.0.md',
    'docs/art/LGO-ASSET-SEPARATION-CHECKLIST-v1.0.md',
    'docs/art/classes/vo/LGO-VO-2D-MODULE-SPEC-v1.0.md',
    'HANDOFF-LGO-CLASS-2D-MODULE-STANDARD-v1.0.md',
)
PAIRS = (
    ('main_weapon', 'arm_guard'), ('inner_top', 'outer_top'),
    ('lower_body', 'waist_belt'), ('outer_top', 'shoulder_chest_guard'),
)
NAMING = ('assets/reference/classes/{class_id}/{gender}/equipment/{slot_id}/'
          '{class_id}_{gender}_{slot_id}_{level}.png')
BOARDS = tuple(f'vo_{name}_v1.png' for name in (
    'module_map', 'male_equipment_grid', 'female_equipment_grid',
    'male_outfit_progression', 'female_outfit_progression', 'skill_vfx_progression',
))


def validate(root: Path, asset_root: Path, require_assets: bool) -> tuple[list[str], int, int]:
    errors: list[str] = []
    texts = {}
    for name in DOCS:
        try:
            texts[name] = (root / name).read_text(encoding='utf-8')
        except (OSError, UnicodeError) as exc:
            errors.append(f'Tài liệu thiếu/không đọc được: {name}: {exc}')
    standard = texts.get(DOCS[0], '')
    for token in (*SLOTS, *LEVELS, *CLASSES, *GENDERS):
        if not re.search(rf'(?<!\w){re.escape(token)}(?!\w)', standard):
            errors.append(f'Chuẩn thiếu ID: {token}')
    phrases = ['không dính da thịt', NAMING]
    phrases.extend(f'Không gộp `{a}` với `{b}`.' for a, b in PAIRS)
    for phrase in phrases:
        if phrase not in standard:
            errors.append(f'Chuẩn thiếu quy tắc: {phrase}')
    expected = {
        f'{gender}/equipment/{slot}/vo_{gender}_{slot}_{level}.png'
        for gender in GENDERS for slot in SLOTS for level in LEVELS
    } | {f'boards/{name}' for name in BOARDS}
    # Owner narrowed this batch to Vo. Other classes are reserved, not asset gates.
    class_root = asset_root / 'vo'
    found: set[str] = set()
    if class_root.exists():
        for path in sorted(class_root.rglob('*')):
            if not path.is_file():
                continue
            relative = path.relative_to(class_root).as_posix()
            found.add(relative)
            if relative not in expected:
                errors.append(f'Tên/folder không đúng chuẩn Võ: {relative}')
                continue
            try:
                with path.open('rb') as stream:
                    header = stream.read(24)
                if (len(header) != 24 or header[:8] != b'\x89PNG\r\n\x1a\n'
                        or header[12:16] != b'IHDR'
                        or 0 in struct.unpack('>II', header[16:24])):
                    errors.append(f'Header PNG không hợp lệ: {relative}')
            except OSError as exc:
                errors.append(f'Không đọc được ảnh: {relative}: {exc}')
    missing = expected - found
    if require_assets and missing:
        sample = ', '.join(sorted(missing)[:4])
        errors.append(f'Thiếu {len(missing)}/226 ảnh reference Võ; ví dụ: {sample}')
    return errors, len(texts), len(expected & found)


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--root', type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument('--asset-root', type=Path,
                        help='Folder classes ở ngoài repo hoặc trong fixture; chỉ đọc.')
    parser.add_argument('--require-assets', action='store_true',
                        help='Yêu cầu đủ 220 item + 6 board Võ; không chứng nhận visual.')
    args = parser.parse_args()
    asset_root = args.asset_root or args.root / 'assets/reference/classes'
    errors, docs, images = validate(args.root, asset_root, args.require_assets)
    print(f'Scope=vo; tài liệu đã đọc={docs}/{len(DOCS)}; ảnh đúng đường dẫn={images}/226')
    if errors:
        print('LGO_CLASS_2D_MODULE_SPEC_FAIL')
        for error in errors:
            print(f'- {error}')
        return 1
    print('LGO_CLASS_2D_MODULE_SPEC_PASS (chỉ gate tài liệu/tên file)')
    if not args.require_assets:
        print('ASSET_COMPLETENESS_NOT_GATED: dùng --require-assets để kiểm đủ ảnh.')
    print('Không kiểm decode toàn ảnh, alpha, da thịt, tách lớp, rig hoặc Unity runtime.')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
