#!/usr/bin/env python3
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []

REFERENCE_IMAGES = [
    'lgo-m6-combat-readability-board-v0360.png',
    'lgo-m6-target-dummy-state-sheet-v0360.png',
    'lgo-m6-skill-feedback-sheet-v0360.png',
    'lgo-m6-combat-hud-mockup-v0360.png',
    'lgo-m6-enemy-telegraph-sheet-v0360.png',
    'lgo-m6-hit-cooldown-feedback-sheet-v0360.png',
    'lgo-m6-combat-reference-composite-v0360.png',
]
FROZEN_PREFIXES = [
    'protocol/',
    'gamedata/schemas/',
    'docs/adr/',
    'client/Unity/Assets/Game/UI/design-tokens.json',
]
FORBIDDEN_OUTPUT_PREFIXES = [
    'build/',
    'client/Unity/Library/',
    'client/Unity/Temp/',
    'client/Unity/Logs/',
    'client/Unity/Assets/Game/Generated/',
    'client/Unity/Assets/Game/Protocol/Generated/',
    'tools/__pycache__/',
]
CHECKED_V037_DOCS = [
    'docs/tasks/M6-COMBAT-VISUAL-READABILITY-POLISH-v0.37.0.md',
    'HANDOFF-LG-M6-COMBAT-VISUAL-READABILITY-POLISH-v0.37.0.md',
    'M6-COMBAT-VISUAL-READABILITY-POLISH-FINAL-REPORT-v0.37.0.md',
]


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
    result = subprocess.run(
        ['git', '--no-pager', *args],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if result.returncode != 0:
        errors.append('git command failed: git --no-pager ' + ' '.join(args))
        return []
    return result.stdout.splitlines()


def main() -> int:
    require_file('tools/validate_m6_combat_visual_readability.py', executable=True)
    require_file('tools/validate_m6_combat_visual_reference_pack.py', executable=True)
    require(
        'docs/tasks/M6-COMBAT-VISUAL-READABILITY-POLISH-v0.37.0.md',
        'M6_COMBAT_VISUAL_READABILITY_POLISH_SOURCE_READY_v0.37.0',
        'reference-only',
        'target highlight',
        'hit flash',
        'cooldown display',
        'target label',
        'local-only prototype label',
        'tooltip/help text',
        'M6_RUNTIME_USABLE_COMBAT_ASSET_PACK',
        'Code Quality / Duplication / Ownership Audit',
    )
    require(
        'client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs',
        'LGO Target Dummy Focus Ring v0.37',
        'LGO Target Dummy Cooldown Ring v0.37',
        'TargetDummyVisualStateText',
        'Dấu hiệu mục tiêu',
        'Sẵn sàng',
        'Đang hồi chiêu',
        'Chưa phải chiến đấu thật',
        'mô phỏng cục bộ',
    )
    require(
        'client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs',
        'Luyện mục tiêu cục bộ',
        'Nhãn nguyên mẫu cục bộ',
        'Đánh thử cục bộ',
        'Tấn công thử',
        'không phải chiến đấu thật',
        'TargetDummyVisualStateText',
    )
    require(
        'tools/lgo_playable_closure_check.sh',
        'validate_m6_combat_visual_readability.py',
        'm6_combat_visual_readability',
    )
    require(
        'HANDOFF-LG-M6-COMBAT-VISUAL-READABILITY-POLISH-v0.37.0.md',
        'M6_COMBAT_VISUAL_READABILITY_POLISH_SOURCE_READY_v0.37.0',
        'Frozen Surface Audit',
        'Code Quality / Duplication / Ownership Audit',
        'runtime-usable combat art is required',
    )
    require(
        'M6-COMBAT-VISUAL-READABILITY-POLISH-FINAL-REPORT-v0.37.0.md',
        'reference-only',
        'No production art claim',
        'No server-authoritative combat',
    )
    require('LGO-M6-COMBAT-VISUAL-READABILITY-POLISH-v0.37.0-DELETIONS.txt', 'DELETED', 'none')
    for image in REFERENCE_IMAGES:
        require_file('docs/reference-art/v0.36.0/' + image)
    require(
        'docs/reference-art/future-reference-v0.36.0/README.md',
        'Future Reference Images v0.36.0',
        'not required for M6 v0.37',
        'Do not ingest these as production art',
    )

    combined_v037_docs = '\n'.join(read(path) for path in CHECKED_V037_DOCS)
    if 'docs/reference-art/future-reference-v0.36.0/lgo-extra-' in combined_v037_docs:
        errors.append('v0.37 docs reference future-reference image files')
    for forbidden in ['production art complete', 'server-authoritative combat implemented', 'real combat implemented']:
        if forbidden in combined_v037_docs.lower():
            errors.append(f'forbidden claim in v0.37 docs: {forbidden}')

    runtime_ui = read('client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs')
    for english_label in ['Target Dummy', 'Cooldown:', 'Attack', 'Hit Flash']:
        if '"' + english_label in runtime_ui:
            errors.append(f'player-facing runtime UI may contain English combat label: {english_label}')

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
        print('M6 COMBAT VISUAL READABILITY VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M6 COMBAT VISUAL READABILITY VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
