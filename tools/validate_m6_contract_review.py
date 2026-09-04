#!/usr/bin/env python3
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []
DECISION = 'M6_MINIMAL_LOCAL_COMBAT_ALLOWED_WITHOUT_CONTRACT_CHANGE_v0.33.0'
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
    'server/scripts/__pycache__/',
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
    result = subprocess.run(['git', '--no-pager', *args], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if result.returncode != 0:
        errors.append('git command failed: git --no-pager ' + ' '.join(args) + ' ' + result.stderr.strip())
        return []
    return result.stdout.splitlines()


def main() -> int:
    require_file('tools/validate_m6_contract_review.py', executable=True)
    require(
        'docs/tasks/M6-CONTRACT-REVIEW-v0.33.0.md',
        DECISION,
        'client-local prototype',
        'Stage 2 may proceed',
        'server-authoritative combat',
        'real damage balancing',
        'loot/reward',
        'inventory/equipment',
        'PvP',
        'anti-cheat',
        'future protocol/GameData changes',
        'Code Quality / Duplication / Ownership Audit',
    )
    require(
        'M6-CONTRACT-REVIEW-FINAL-REPORT-v0.33.0.md',
        DECISION,
        'no protocol or GameData schema changes',
        'Frozen surfaces unchanged',
    )
    require(
        'HANDOFF-LG-M6-CONTRACT-REVIEW-v0.33.0.md',
        DECISION,
        'UI input',
        'Target selection',
        'Smoke marker',
        'Code Quality / Duplication / Ownership Audit',
    )
    require('LGO-M6-CONTRACT-REVIEW-v0.33.0-DELETIONS.txt', 'DELETED', 'none')

    if (ROOT / 'CONTRACT_CHANGE_REQUEST-M6-COMBAT-v0.33.0.md').exists():
        errors.append('contract change request exists even though Stage 2 is marked allowed')

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
        print('M6 CONTRACT REVIEW VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M6 CONTRACT REVIEW VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
