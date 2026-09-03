#!/usr/bin/env python3
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []

IMAGE_FILES = [
    'docs/reference-art/v0.16.5/lgo-visual-reference-overview-v0165.png',
    'docs/reference-art/v0.16.5/lgo-key-visual-moodboard-v0165.png',
    'docs/reference-art/v0.16.5/lgo-world-hub-2d5-v0165.png',
    'docs/reference-art/v0.16.5/lgo-gate-character-ui-v0165.png',
    'docs/reference-art/v0.16.5/lgo-playable-hud-mockup-v0165.png',
    'docs/reference-art/v0.16.5/lgo-character-npc-monster-style-v0165.png',
    'docs/reference-art/v0.16.5/lgo-item-skill-vfx-icons-v0165.png',
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


def read(path: str) -> str:
    target = ROOT / path
    if not target.is_file():
        errors.append(f'missing file: {path}')
        return ''
    return target.read_text(encoding='utf-8', errors='replace')


def require_markers(path: str, markers: list[str]) -> None:
    content = read(path)
    for marker in markers:
        if marker not in content:
            errors.append(f'{path} missing marker: {marker}')


def git_lines(*args: str) -> list[str]:
    result = subprocess.run(['git', '--no-pager', *args], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if result.returncode != 0:
        errors.append('git command failed: git --no-pager ' + ' '.join(args) + ' ' + result.stderr.strip())
        return []
    return result.stdout.splitlines()


def check_png(path: str) -> None:
    target = ROOT / path
    if not target.is_file():
        errors.append(f'missing reference image: {path}')
        return
    header = target.read_bytes()[:8]
    if header != b'\x89PNG\r\n\x1a\n':
        errors.append(f'reference image is not PNG: {path}')


def main() -> int:
    for image in IMAGE_FILES:
        check_png(image)

    require_markers(
        'docs/art/LGO-VISUAL-REFERENCE-PACK-v0.16.5.md',
        [
            'LGO_VISUAL_REFERENCE_PACK_ACCEPTED_v0.16.5',
            'dark navy surface',
            'cyan spirit energy',
            'warm gold guidance',
            'purple shadow threat',
            'small red/orange alert/accent',
            'spirit gate',
            'talisman',
            'rune trim',
            'lantern warmth',
            'cultivation circle',
            'shadow realm edge',
            'must not be copied one-to-one',
            'not final production art',
            'Not final UI',
        ],
    )
    require_markers(
        'docs/reference-art/v0.16.5/README.md',
        [
            'LGO Visual Reference Pack v0.16.5',
            'reference only',
            'lgo-visual-reference-overview-v0165.png',
            'lgo-item-skill-vfx-icons-v0165.png',
        ],
    )

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

    if errors:
        print('VISUAL REFERENCE PACK VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('VISUAL REFERENCE PACK VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
