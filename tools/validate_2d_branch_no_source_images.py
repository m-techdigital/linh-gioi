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


RUNTIME_ART_PACKS = [
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/VoLv1MapAvatarArt',
        'id': 'vo-lv1-map-avatar-v1',
        'status': 'DRAFT_RUNTIME_REVIEW',
        'assets': {
            'vo-lv1-map-avatar-atlas.png': (512, 512, 'paper-doll-preview-atlas'),
        },
        'generators': {'reviewed_owner_wip'},
        'max_bytes': 200_000,
        'status_error': 'Vo Lv1 map avatar must remain draft until Player motion review',
    },
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/CongDongLamMap01AArt',
        'id': 'cong-dong-lam-map01a-art-draft-v1',
        'status': 'DRAFT_NOT_RUNTIME_APPROVED',
        'assets': {'far-background.png': (1024, 576, 'far-background'),
                   'props-atlas.png': (1024, 1024, 'runtime-atlas'),
                   'sheet-props.png': (256, 256, 'runtime-atlas'),
                   'modules-atlas.png': (1024, 1024, 'runtime-atlas'),
                   'npcs-atlas.png': (512, 512, 'runtime-atlas'),
                   'landmarks-atlas.png': (1024, 1024, 'runtime-atlas'),
                   'combat-atlas.png': (512, 512, 'runtime-atlas')},
        'generators': {'image_gen', 'extracted_owner_source'},
        'max_bytes': 6_000_000,
        'status_error': 'Map01A art must remain an explicit draft until Player review',
    },
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/DongMonIllustrated',
        'id': 'dongmon-illustrated-draft-v1',
        'status': 'DRAFT_OWNER_REVIEW',
        'assets': {
            'skyline.png': (1536, 864, 'far-background'),
            'props-atlas.png': (2048, 2048, 'runtime-atlas'),
        },
        'generators': {'image_gen'},
        'max_bytes': 4_000_000,
        'status_error': 'Pack must remain an explicit draft',
    },
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/VoLv1ApprovedRuntimeArt',
        'id': 'vo-lv1-approved-runtime-art-v1',
        'status': 'APPROVED_RUNTIME_ART',
        'assets': {
            'vo-lv1-starter-atlas.png': (2048, 2048, 'paper-doll-atlas'),
            'vo-lv1-skill-atlas.png': (1024, 1024, 'skill-vfx-atlas'),
        },
        'generators': {'approved_original_2d_art', 'image_gen'},
        'max_bytes': 4_000_000,
        'status_error': 'Vo Lv1 runtime art must remain approved runtime art',
    },
]


def _expected_assets(spec: dict[str, object]) -> dict[str, tuple[int, int, str]]:
    pack = str(spec['pack'])
    assets = spec['assets']
    if not isinstance(assets, dict):
        raise ValueError('Invalid runtime art spec')
    return {pack + '/' + name: value for name, value in assets.items()}


def _validate_runtime_pack(root: Path, spec: dict[str, object]) -> set[str]:
    pack = str(spec['pack'])
    manifest = root / pack / 'manifest.json'
    if not manifest.exists():
        return set()
    data = json.loads(manifest.read_text())
    expected = _expected_assets(spec)
    if data['id'] != spec['id'] or data['status'] != spec['status']:
        raise ValueError(str(spec['status_error']))
    entries = data['assets']
    if len(entries) != len(expected) or {a['path'] for a in entries} != set(expected):
        raise ValueError('Only declared runtime textures are allowed for ' + pack)
    generators = spec['generators']
    if not isinstance(generators, set):
        raise ValueError('Invalid runtime art generators')
    for entry in entries:
        path = root / entry['path']
        if path.is_symlink() or not path.is_file():
            raise ValueError('Missing or symlinked runtime texture: ' + entry['path'])
        raw = path.read_bytes()
        w, h, role = expected[entry['path']]
        if (entry['generator'] not in generators or entry['referenceOnly'] is not False
                or entry['role'] != role or len(raw) > int(spec['max_bytes'])
                or hashlib.sha256(raw).hexdigest() != entry['sha256']
                or raw[:8] != b'\x89PNG\r\n\x1a\n'
                or struct.unpack('>II', raw[16:24]) != (w, h)):
            raise ValueError('Invalid provenance/hash/PNG budget: ' + entry['path'])
    return set(expected)


def runtime_allowlist(root: Path) -> set[str]:
    allowed: set[str] = set()
    for spec in RUNTIME_ART_PACKS:
        try:
            allowed.update(_validate_runtime_pack(root, spec))
        except (KeyError, TypeError, OSError, json.JSONDecodeError, struct.error) as exc:
            raise ValueError('Invalid runtime art manifest: ' + str(exc)) from exc
    return allowed

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
