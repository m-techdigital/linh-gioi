#!/usr/bin/env python3
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []
IMAGES = [
    'lgo-m6-combat-readability-board-v0360.png',
    'lgo-m6-target-dummy-state-sheet-v0360.png',
    'lgo-m6-skill-feedback-sheet-v0360.png',
    'lgo-m6-combat-hud-mockup-v0360.png',
    'lgo-m6-enemy-telegraph-sheet-v0360.png',
    'lgo-m6-hit-cooldown-feedback-sheet-v0360.png',
    'lgo-m6-combat-reference-composite-v0360.png',
]
FROZEN_PREFIXES = ['protocol/', 'gamedata/schemas/', 'docs/adr/', 'client/Unity/Assets/Game/UI/design-tokens.json']
FORBIDDEN_OUTPUT_PREFIXES = ['build/', 'client/Unity/Library/', 'client/Unity/Temp/', 'client/Unity/Logs/', 'client/Unity/Assets/Game/Generated/', 'client/Unity/Assets/Game/Protocol/Generated/', 'tools/__pycache__/']


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


def require_file(path: str, executable: bool = False) -> None:
    target = ROOT / path
    if not target.is_file():
        errors.append(f'missing file: {path}')
        return
    if executable and not os.access(target, os.X_OK):
        errors.append(f'file is not executable: {path}')


def git_lines(*args: str) -> list[str]:
    result = subprocess.run(['git', '--no-pager', *args], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if result.returncode != 0:
        errors.append('git command failed: git --no-pager ' + ' '.join(args))
        return []
    return result.stdout.splitlines()


def main() -> int:
    require_file('tools/validate_m6_combat_visual_reference_pack.py', executable=True)
    require('docs/reference-art/v0.36.0/README.md', *IMAGES, 'visual reference pack only', 'Player-facing UI copy must be Vietnamese')
    require('docs/reference-art/v0.36.0/CODEX-USAGE.md', *IMAGES, 'Do not copy English labels', 'Player-facing runtime UI must be Vietnamese')
    for image in IMAGES:
        require_file('docs/reference-art/v0.36.0/' + image)
    require('docs/art/LGO-COMBAT-VISUAL-REFERENCE-PACK-v0.36.0.md', 'M6_COMBAT_VISUAL_REFERENCE_PACK_ACCEPTED_v0.36.0', 'Tấn công thử', 'Mục tiêu luyện tập', 'Trúng mục tiêu', 'Hồi chiêu', 'Mô phỏng cục bộ', 'Chưa phải chiến đấu thật', 'Do not implement a feature solely because it appears in a reference image', 'Do not claim production art', 'Do not claim server-authoritative combat')
    require('docs/design/LGO-COMBAT-READABILITY-RULES-v0.36.0.md', 'Target Highlight Rules', 'Hit Feedback Rules', 'Cooldown Feedback Rules', 'Telegraph Warning Rules', 'Maximum visual noise rule', 'Mô phỏng cục bộ')
    require('docs/tasks/M6-COMBAT-VISUAL-REFERENCE-PACK-v0.36.0.md', 'M6_COMBAT_VISUAL_REFERENCE_PACK_ACCEPTED_v0.36.0', 'Future-reference files', 'not v0.37 scope', 'Code Quality / Duplication / Ownership Audit')
    require('HANDOFF-LG-M6-COMBAT-VISUAL-REFERENCE-PACK-v0.36.0.md', 'Code Quality / Duplication / Ownership Audit', 'Frozen Surface Audit')
    require('LGO-M6-COMBAT-VISUAL-REFERENCE-PACK-v0.36.0-DELETIONS.txt', 'DELETED', 'none')

    for path in git_lines('diff', '--name-only'):
        for prefix in FROZEN_PREFIXES:
            if path == prefix or path.startswith(prefix):
                errors.append(f'frozen surface modified: {path}')
    for line in git_lines('status', '--short', '--untracked-files=all'):
        path = line[3:] if len(line) >= 4 else line
        for prefix in FORBIDDEN_OUTPUT_PREFIXES:
            if path.startswith(prefix):
                errors.append(f'generated/cache/build output under source status: {path}')

    if errors:
        print('M6 COMBAT VISUAL REFERENCE PACK VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M6 COMBAT VISUAL REFERENCE PACK VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
