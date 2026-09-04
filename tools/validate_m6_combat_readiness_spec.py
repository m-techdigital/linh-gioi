#!/usr/bin/env python3
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []
FORBIDDEN_CHANGED_PREFIXES = [
    'protocol/',
    'gamedata/schemas/',
    'docs/adr/',
    'client/Unity/Assets/Game/UI/design-tokens.json',
]
FORBIDDEN_CODE_PREFIXES = [
    'client/Unity/Assets/Game/',
    'server/',
    'protocol/',
    'gamedata/schemas/',
]
ALLOWED_CODE_FILES = {
    'tools/validate_m6_combat_readiness_spec.py',
    'tools/lgo_playable_closure_check.sh',
}
M6_ALLOWED_AFTER_CONTRACT_FILES = {
    'client/Unity/Assets/Game/Bootstrap/Runtime/GameBootstrap.cs',
    'client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs',
    'client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs',
    'client/Unity/Assets/Game/World/Runtime/M6MinimalLocalCombatSmokeRunner.cs',
    'client/Unity/Assets/Game/World/Runtime/M6MinimalLocalCombatSmokeRunner.cs.meta',
    'tools/run_m6_minimal_local_combat_once.sh',
    'tools/validate_m6_contract_review.py',
    'tools/validate_m6_minimal_local_combat.py',
    'tools/validate_master_roadmap.py',
    'tools/validate_m5_vfx_feedback_placeholder.py',
    'tools/validate_m6_skill_preview_sandbox.py',
    'tools/validate_m6_target_dummy_readability.py',
    'tools/validate_m6_combat_ux_feedback.py',
    'tools/validate_m6_combat_visual_reference_pack.py',
    'tools/validate_m6_combat_visual_readability.py',
    'tools/validate_m6_combat_input_feedback_stability.py',
}


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
        'docs/tasks/M6-COMBAT-READINESS-SPEC-v0.32.0.md',
        'M6_COMBAT_READINESS_SPEC_CLOSED_v0.32.0',
        'Real combat is not implemented.',
        'Required contract review before real combat',
        'Protocol ownership',
        'GameData ownership',
        'Runtime note: v0.32.0 is docs-only.',
    )
    require(
        'docs/execution/prompts/M6-COMBAT-FOUNDATION-LONG-TASK.md',
        'Do not start until `M6_COMBAT_READINESS_SPEC_CLOSED_v0.32.0` is accepted',
        'Confirm whether protocol changes are approved',
        'Confirm whether GameData schema changes are approved',
        'Unity build',
        'Runtime smoke',
    )
    require(
        'tools/lgo_playable_closure_check.sh',
        'validate_m6_combat_readiness_spec.py',
        'm6_combat_readiness_spec',
    )
    require(
        'HANDOFF-LG-M6-COMBAT-READINESS-SPEC-v0.32.0.md',
        'M6_COMBAT_READINESS_SPEC_CLOSED_v0.32.0',
        'docs-only',
        'Real combat was not implemented',
    )
    require_file('tools/validate_m6_combat_readiness_spec.py', executable=True)

    for path in git_lines('diff', '--name-only'):
        for prefix in FORBIDDEN_CHANGED_PREFIXES:
            if path == prefix or path.startswith(prefix):
                errors.append(f'frozen surface modified: {path}')
        m6_local_allowed = (
            path in M6_ALLOWED_AFTER_CONTRACT_FILES and
            'M6_MINIMAL_LOCAL_COMBAT_ALLOWED_WITHOUT_CONTRACT_CHANGE_v0.33.0' in read('docs/tasks/M6-CONTRACT-REVIEW-v0.33.0.md') and
            'M6_MINIMAL_LOCAL_COMBAT_FOUNDATION_SOURCE_READY_v0.34.0' in read('docs/tasks/M6-MINIMAL-LOCAL-COMBAT-FOUNDATION-v0.34.0.md')
        )
        if path not in ALLOWED_CODE_FILES and not m6_local_allowed:
            for prefix in FORBIDDEN_CODE_PREFIXES:
                if path.startswith(prefix):
                    errors.append(f'v0.32 docs-only spec changed implementation path: {path}')

    if errors:
        print('M6 COMBAT READINESS SPEC VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M6 COMBAT READINESS SPEC VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
