#!/usr/bin/env python3
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []
FROZEN_PREFIXES = ['protocol/', 'gamedata/schemas/', 'docs/adr/', 'client/Unity/Assets/Game/UI/design-tokens.json']


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
    require('docs/tasks/M6-MINIMAL-LOCAL-COMBAT-FOUNDATION-v0.34.0.md', 'M6_MINIMAL_LOCAL_COMBAT_FOUNDATION_SOURCE_READY_v0.34.0', 'Chỉ là mô phỏng cục bộ', 'no loot/reward', 'Code Quality / Duplication / Ownership Audit')
    require('client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs', 'TryLocalCombatPrototype', 'SetSmokePositionNearTargetDummy', 'Mục tiêu luyện tập', 'Trúng mục tiêu', 'Chỉ là mô phỏng cục bộ', 'TargetDummyHitFlash')
    require('client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs', 'Luyện mục tiêu cục bộ', 'Tấn công thử', 'Chưa phải chiến đấu thật', 'TriggerLocalCombat')
    require('client/Unity/Assets/Game/World/Runtime/M6MinimalLocalCombatSmokeRunner.cs', '--lgo-m6-minimal-local-combat-smoke', 'M6_MINIMAL_LOCAL_COMBAT_RUNTIME_SMOKE_PASS')
    require('client/Unity/Assets/Game/Bootstrap/Runtime/GameBootstrap.cs', 'M6MinimalLocalCombatSmokeRunner.ShouldRun')
    require('tools/lgo_playable_closure_check.sh', 'validate_m6_minimal_local_combat.py', 'run_m6_minimal_local_combat_once.sh', 'M6_MINIMAL_LOCAL_COMBAT_RUNTIME_SMOKE_PASS')
    require_file('tools/run_m6_minimal_local_combat_once.sh', executable=True)
    require_file('tools/validate_m6_minimal_local_combat.py', executable=True)
    require('M6-MINIMAL-LOCAL-COMBAT-FOUNDATION-FINAL-REPORT-v0.34.0.md', 'M6_MINIMAL_LOCAL_COMBAT_FOUNDATION_SOURCE_READY_v0.34.0', 'local-only', 'Runtime')
    require('HANDOFF-LG-M6-MINIMAL-LOCAL-COMBAT-FOUNDATION-v0.34.0.md', 'Code Quality / Duplication / Ownership Audit', 'Frozen Surface Audit')
    require('LGO-M6-MINIMAL-LOCAL-COMBAT-FOUNDATION-v0.34.0-DELETIONS.txt', 'DELETED', 'none')

    for path in git_lines('diff', '--name-only'):
        for prefix in FROZEN_PREFIXES:
            if path == prefix or path.startswith(prefix):
                errors.append(f'frozen surface modified: {path}')

    if errors:
        print('M6 MINIMAL LOCAL COMBAT VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M6 MINIMAL LOCAL COMBAT VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
