#!/usr/bin/env python3
from __future__ import annotations

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []
V040_CONTRACT_FILES = {
    'protocol/combat.proto',
    'gamedata/schemas/skill.schema.json',
    'gamedata/skills/wind_slash.yaml',
    'gamedata/compiled/gamedata-manifest.json',
    'tests/gamedata/test_gamedata_pipeline.py',
    'tests/gamedata/__pycache__/test_gamedata_pipeline.cpython-312.pyc',
}


def read(path: str) -> str:
    target = ROOT / path
    if not target.exists():
        errors.append(f'missing: {path}')
        return ''
    return target.read_text(encoding='utf-8')


def require(path: str, *markers: str) -> None:
    content = read(path)
    for marker in markers:
        if marker not in content:
            errors.append(f'{path} missing marker: {marker}')


def v040_contract_is_active() -> bool:
    return (
        'M6_COMBAT_PROTOCOL_GAMEDATA_CONTRACT_ACCEPTED_v0.40.0'
        in read('docs/tasks/M6-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-v0.40.0.md')
        and (ROOT / 'CONTRACT_CHANGE_REQUEST-M6-SERVER-COMBAT-v0.39.0.md').is_file()
    )


def main() -> int:
    require('docs/design/LGO-DESIGN-DIRECTION-LOCK-v0.11.0.md', 'Vietnamese spiritual fantasy', 'SaaS dashboard')
    require('docs/design/LGO-PLAYABLE-UI-WIREFRAME-SPEC-v0.11.0.md', 'Login / Dev Entry', 'Character Select / Create', 'World HUD')
    require(
        'client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs',
        'Linh Môn',
        'Điện Nhân Vật',
        'Sân Luyện An Toàn',
        'Vào Thế Giới',
        'Tạo Tu Sĩ',
        'Vào sân luyện',
        'Lưu vị trí',
        'Về điện nhân vật',
        'WASD hoặc phím mũi tên',
        'Q / E',
        'F hoặc Space',
        'Menu',
        'RuntimeArtCatalog',
        'LoginDevAsync',
        'ListCharactersAsync',
        'CreateCharacterAsync',
        'LoadCharacterAsync',
        'SaveCharacterPositionAsync',
    )
    require('tools/validate_m4_2_playable_ui.py', 'M4-2 PLAYABLE UI REDESIGN VALIDATION PASS')

    forbidden = [
        'protocol/',
        'gamedata/schemas/',
        'docs/adr/',
        'client/Unity/Assets/Game/UI/design-tokens.json',
    ]
    changed = []
    try:
        import subprocess
        changed = subprocess.check_output(['git', 'diff', '--name-only'], cwd=ROOT, text=True).splitlines()
    except Exception:
        changed = []
    v040_active = v040_contract_is_active()
    for path in changed:
        if v040_active and path in V040_CONTRACT_FILES:
            continue
        if path in forbidden or any(path.startswith(prefix) for prefix in forbidden if prefix.endswith('/')):
            errors.append(f'frozen surface modified: {path}')

    if errors:
        print('M4-2 PLAYABLE UI REDESIGN VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M4-2 PLAYABLE UI REDESIGN VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
