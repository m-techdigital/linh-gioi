#!/usr/bin/env python3
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []
FORBIDDEN_CHANGED_PREFIXES = ['protocol/', 'gamedata/schemas/', 'docs/adr/']
FORBIDDEN_WORLD_MARKERS = ['HitPoints', 'Damage', 'Loot', 'Inventory', 'EnemyAttack', 'Projectile']


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
    require(
        'client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs',
        'ReadabilityDummyPosition',
        'LGO Target Dummy Readability Marker',
        'LGO Target Dummy Non Combat Base',
        'Sân luyện an toàn / Mục tiêu luyện tập',
        'Readability Dummy east',
    )
    require(
        'client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs',
        'Readability Dummy east',
        'Shadow Slime far east',
    )
    require(
        'tools/lgo_playable_closure_check.sh',
        'validate_m6_target_dummy_readability.py',
        'm6_target_dummy_readability',
    )
    require(
        'docs/tasks/M6-TARGET-DUMMY-READABILITY-v0.31.0.md',
        'M6_TARGET_DUMMY_READABILITY_RUNTIME_CLOSED_LOCAL_v0.31.0',
        'No combat system',
        'readability marker',
    )
    require_file('tools/validate_m6_target_dummy_readability.py', executable=True)

    world = read('client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs')
    for marker in FORBIDDEN_WORLD_MARKERS:
        if marker in world:
            errors.append(f'target dummy readability contains forbidden gameplay marker: {marker}')
    if 'Cooldown' in world:
        m6_doc = read('docs/tasks/M6-MINIMAL-LOCAL-COMBAT-FOUNDATION-v0.34.0.md')
        if 'M6_MINIMAL_LOCAL_COMBAT_FOUNDATION_SOURCE_READY_v0.34.0' not in m6_doc or 'local/non-authoritative' not in m6_doc:
            errors.append('target dummy readability contains Cooldown without M6 local/non-authoritative approval')
    for path in git_lines('diff', '--name-only'):
        if path == 'client/Unity/Assets/Game/UI/design-tokens.json':
            errors.append(f'frozen surface modified: {path}')
        for prefix in FORBIDDEN_CHANGED_PREFIXES:
            if path.startswith(prefix):
                errors.append(f'frozen surface modified: {path}')

    if errors:
        print('M6 TARGET DUMMY READABILITY VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M6 TARGET DUMMY READABILITY VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
