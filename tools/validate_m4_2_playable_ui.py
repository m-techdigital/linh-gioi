#!/usr/bin/env python3
from __future__ import annotations

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []


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


def main() -> int:
    require('docs/design/LGO-DESIGN-DIRECTION-LOCK-v0.11.0.md', 'Vietnamese spiritual fantasy', 'SaaS dashboard')
    require('docs/design/LGO-PLAYABLE-UI-WIREFRAME-SPEC-v0.11.0.md', 'Login / Dev Entry', 'Character Select / Create', 'World HUD')
    require(
        'client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs',
        'Auth / Gate Entry',
        'Character Hall',
        'World HUD',
        'Open Gate',
        'Create Cultivator',
        'Enter World',
        'Save Position',
        'Back to Lobby',
        'WASD/arrows + Q/E',
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
    for path in changed:
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
