#!/usr/bin/env python3
from __future__ import annotations

import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []
EXPECTED_IMAGES = [
    'docs/reference-art/v0.20.0/lgo-art-direction-overview-v0200.png',
    'docs/reference-art/v0.20.0/lgo-playable-hero-pose-sheet-v0200.png',
    'docs/reference-art/v0.20.0/lgo-npc-direction-sheet-v0200.png',
    'docs/reference-art/v0.20.0/lgo-monster-direction-sheet-v0200.png',
    'docs/reference-art/v0.20.0/lgo-ui-component-skin-sheet-v0200.png',
    'docs/reference-art/v0.20.0/lgo-window-popup-sheet-v0200.png',
    'docs/reference-art/v0.20.0/lgo-item-skill-vfx-sheet-v0200.png',
    'docs/reference-art/v0.20.0/lgo-environment-prop-sheet-v0200.png',
]
FORBIDDEN_CHANGED_PREFIXES = ['protocol/', 'gamedata/schemas/', 'docs/adr/']
FORBIDDEN_OUTPUT_PREFIXES = [
    'build/',
    'client/Unity/Library/',
    'client/Unity/Temp/',
    'client/Unity/Logs/',
    'client/Unity/Assets/Game/Generated/',
    'client/Unity/Assets/Game/Protocol/Generated/',
]


def git_lines(*args: str) -> list[str]:
    result = subprocess.run(['git', '--no-pager', *args], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if result.returncode != 0:
        errors.append('git command failed: git --no-pager ' + ' '.join(args) + ' ' + result.stderr.strip())
        return []
    return result.stdout.splitlines()


def read(path: str) -> str:
    target = ROOT / path
    if not target.exists():
        errors.append(f'missing: {path}')
        return ''
    return target.read_text(encoding='utf-8', errors='replace')


def require(path: str, *markers: str) -> None:
    content = read(path)
    for marker in markers:
        if marker not in content:
            errors.append(f'{path} missing marker: {marker}')


def main() -> int:
    for image in EXPECTED_IMAGES:
        target = ROOT / image
        if not target.is_file():
            errors.append(f'missing image: {image}')
        elif target.stat().st_size <= 0:
            errors.append(f'empty image: {image}')

    require(
        'docs/art/LGO-ART-DIRECTION-PACK-v0.20.0.md',
        'LGO_ART_DIRECTION_PACK_ACCEPTED_v0.20.0',
        'Overview board feeds global art language',
        'Hero pose sheet feeds v0.21',
        'NPC sheet feeds v0.21',
        'Monster sheet feeds v0.21',
        'UI component sheet feeds v0.22',
        'Window and popup sheet feeds v0.22',
        'Item, skill, and VFX sheet feeds v0.23',
        'Environment prop sheet feeds v0.19-v0.23',
        'Dark navy',
        'Spirit cyan',
        'Warm gold',
        'Jade and teal',
        'Purple shadow',
        'Small red/orange alert',
        'Do not copy images 1:1',
        'Not final production art',
        'Not licensed production asset',
        'Not final animation sheet',
        'Not final UI skin',
    )
    require(
        'docs/reference-art/v0.20.0/README.md',
        'LGO_ART_DIRECTION_PACK_ACCEPTED_v0.20.0',
        'lgo-monster-direction-sheet-v0200.png',
        'lgo-shadow-slime-monster-direction-v0200.png',
        'reference-only',
    )

    tags = set(git_lines('tag', '--list', 'lgo-m5-world-hub-readability-*v0.19.0'))
    if not ({'lgo-m5-world-hub-readability-closed-local-v0.19.0', 'lgo-m5-world-hub-readability-source-closed-v0.19.0'} & tags):
        errors.append('missing v0.19.0 baseline tag')

    for path in git_lines('diff', '--name-only'):
        if path == 'client/Unity/Assets/Game/UI/design-tokens.json':
            errors.append(f'frozen surface modified: {path}')
        for prefix in FORBIDDEN_CHANGED_PREFIXES:
            if path.startswith(prefix):
                errors.append(f'frozen surface modified: {path}')

    for line in git_lines('status', '--short', '--untracked-files=all'):
        path = line[3:] if len(line) >= 4 else line
        for prefix in FORBIDDEN_OUTPUT_PREFIXES:
            if path.startswith(prefix):
                errors.append(f'generated/cache/build output under source status: {path}')
        if path.endswith('.DS_Store') or path.startswith('__MACOSX/'):
            errors.append(f'package hygiene artifact under source status: {path}')

    if errors:
        print('LGO ART DIRECTION PACK VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('LGO ART DIRECTION PACK VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
