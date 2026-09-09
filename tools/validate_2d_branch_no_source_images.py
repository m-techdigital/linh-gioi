#!/usr/bin/env python3
from __future__ import annotations

from pathlib import Path
import sys
import hashlib
import json
import struct

ROOT = Path(__file__).resolve().parents[1]
IMAGE_SUFFIXES = {
    '.png', '.jpg', '.jpeg', '.webp', '.bmp', '.gif', '.psd', '.tga', '.tif', '.tiff'
}
SKIP_PARTS = {
    '.git', 'build', 'Library', 'Temp', 'Obj', 'Logs', 'UserSettings', 'Packages'
}


PACK = 'client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/DongMonIllustrated'


def runtime_allowlist(root: Path) -> set[str]:
    manifest = root / PACK / 'manifest.json'
    if not manifest.exists():
        return set()
    try:
        data = json.loads(manifest.read_text())
        expected = {PACK + '/skyline.png': (1536, 864, 'far-background'),
                    PACK + '/props-atlas.png': (2048, 2048, 'runtime-atlas')}
        if data['id'] != 'dongmon-illustrated-draft-v1' or data['status'] != 'DRAFT_OWNER_REVIEW':
            raise ValueError('Pack must remain an explicit draft')
        entries = data['assets']
        if len(entries) != 2 or {a['path'] for a in entries} != set(expected):
            raise ValueError('Only the two declared new runtime textures are allowed')
        for entry in entries:
            path = root / entry['path']
            if path.is_symlink() or not path.is_file():
                raise ValueError('Missing or symlinked runtime texture: ' + entry['path'])
            raw = path.read_bytes()
            w, h, role = expected[entry['path']]
            if (entry['generator'] != 'image_gen' or entry['referenceOnly'] is not False
                    or entry['role'] != role or len(raw) > 4_000_000
                    or hashlib.sha256(raw).hexdigest() != entry['sha256']
                    or raw[:8] != b'\x89PNG\r\n\x1a\n'
                    or struct.unpack('>II', raw[16:24]) != (w, h)):
                raise ValueError('Invalid provenance/hash/PNG budget: ' + entry['path'])
        return set(expected)
    except (KeyError, TypeError, OSError, json.JSONDecodeError, struct.error) as exc:
        raise ValueError('Invalid runtime art manifest: ' + str(exc)) from exc


def main() -> int:
    try:
        allowed = runtime_allowlist(ROOT)
    except ValueError as exc:
        print('LGO_2D_BRANCH_NO_SOURCE_IMAGES_FAIL', str(exc))
        return 1
    found: list[str] = []
    for path in ROOT.rglob('*'):
        if not path.is_file():
            continue
        rel = path.relative_to(ROOT)
        if any(part in SKIP_PARTS for part in rel.parts):
            continue
        if path.suffix.lower() in IMAGE_SUFFIXES and rel.as_posix() not in allowed:
            found.append(rel.as_posix())

    if found:
        print('LGO_2D_BRANCH_NO_SOURCE_IMAGES_FAIL')
        for rel in sorted(found)[:240]:
            print(rel)
        if len(found) > 240:
            print(f'... and {len(found) - 240} more')
        return 1

    print('LGO_2D_BRANCH_NO_SOURCE_IMAGES_PASS')
    return 0


if __name__ == '__main__':
    sys.exit(main())
