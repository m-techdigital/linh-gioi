#!/usr/bin/env python3
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []
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
    require(
        'docs/tasks/M6-COMBAT-INPUT-FEEDBACK-STABILITY-v0.38.0.md',
        'M6_COMBAT_INPUT_FEEDBACK_STABILITY_SOURCE_READY_v0.38.0',
        'PlayableWorldController',
        'Cooldown/readiness logic is not duplicated in the UI',
        'Chưa thể tấn công',
        'Đang hồi chiêu',
        'Sẵn sàng',
        'Trúng mục tiêu',
        'Mô phỏng cục bộ',
        'Code Quality / Duplication / Ownership Audit',
    )
    require(
        'client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs',
        'LocalCombatCoolingDown',
        'RecoverLocalCombatCooldownForSmoke',
        'Chưa thể tấn công',
        'Đang hồi chiêu',
        'Sẵn sàng',
        'Mô phỏng cục bộ',
    )
    require(
        'client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs',
        'TriggerLocalCombat',
        'TargetDummyVisualStateText',
        'Tấn công thử',
        'Nhãn nguyên mẫu cục bộ',
    )
    require(
        'client/Unity/Assets/Game/World/Runtime/M6MinimalLocalCombatSmokeRunner.cs',
        'cooldownBlockedAfterRepeatedInput',
        'cooldownBlockedFeedbackText',
        'cooldownRecoveredText',
        'attackAfterCooldownRecovered',
        'repeated attack did not produce deterministic cooldown block feedback',
        'cooldown recovery did not restore deterministic local attack feedback',
        'M6_MINIMAL_LOCAL_COMBAT_RUNTIME_SMOKE_PASS',
    )
    require('tools/lgo_playable_closure_check.sh', 'validate_m6_combat_input_feedback_stability.py', 'm6_combat_input_feedback_stability')
    require('HANDOFF-LG-M6-COMBAT-INPUT-FEEDBACK-STABILITY-v0.38.0.md', 'M6_COMBAT_INPUT_FEEDBACK_STABILITY_SOURCE_READY_v0.38.0', 'Frozen Surface Audit', 'Code Quality / Duplication / Ownership Audit')
    require('M6-COMBAT-INPUT-FEEDBACK-STABILITY-FINAL-REPORT-v0.38.0.md', 'No server combat', 'No protocol change', 'Runtime PASS')
    require('LGO-M6-COMBAT-INPUT-FEEDBACK-STABILITY-v0.38.0-DELETIONS.txt', 'DELETED', 'none')
    require_file('tools/validate_m6_combat_input_feedback_stability.py', executable=True)

    for path in git_lines('diff', '--name-only'):
        for prefix in FROZEN_PREFIXES:
            if path == prefix or path.startswith(prefix):
                errors.append(f'frozen surface modified: {path}')
    for line in git_lines('status', '--short', '--untracked-files=all'):
        status = line[:2]
        if 'D' in status:
            continue
        path = line[3:] if len(line) >= 4 else line
        for prefix in FORBIDDEN_OUTPUT_PREFIXES:
            if path.startswith(prefix):
                errors.append(f'generated/cache/build output under source status: {path}')

    if errors:
        print('M6 COMBAT INPUT FEEDBACK STABILITY VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M6 COMBAT INPUT FEEDBACK STABILITY VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
