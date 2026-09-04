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
    'server/realtime/src/main/java/com/linhgioi/server/realtime/combat/CombatValidationService.java',
    'server/realtime/src/test/java/com/linhgioi/server/realtime/combat/CombatValidationServiceTest.java',
    'server/realtime/src/main/java/com/linhgioi/server/realtime/combat/CombatSmokeServer.java',
    'client/Unity/Assets/Game/World/Runtime/M6UnityJavaCombatE2ERunner.cs',
    'client/Unity/Assets/Game/World/Runtime/M6UnityJavaCombatE2ERunner.cs.meta',
    'tools/run_m6_unity_java_combat_e2e.sh',
    'tools/validate_m6_unity_java_combat_e2e.py',
    'tools/run_m6_server_authoritative_combat_pilot.sh',
    'tools/validate_m6_server_authoritative_combat_pilot.py',
}
V040_CONTRACT_FILES = {
    'protocol/combat.proto',
    'gamedata/schemas/skill.schema.json',
    'gamedata/skills/wind_slash.yaml',
    'gamedata/compiled/gamedata-manifest.json',
    'tests/gamedata/test_gamedata_pipeline.py',
    'tests/gamedata/__pycache__/test_gamedata_pipeline.cpython-312.pyc',
    'tools/validate_m6_combat_protocol_gamedata_contract.py',
    'docs/tasks/M6-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-v0.40.0.md',
    'docs/design/LGO-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-v0.40.0.md',
    'M6-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-FINAL-REPORT-v0.40.0.md',
    'HANDOFF-LG-M6-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-v0.40.0.md',
    'LGO-M6-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-v0.40.0-CHANGED-FILES.txt',
    'LGO-M6-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-v0.40.0-DELETIONS.txt',
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


def v040_contract_is_active() -> bool:
    return (
        'M6_COMBAT_PROTOCOL_GAMEDATA_CONTRACT_ACCEPTED_v0.40.0'
        in read('docs/tasks/M6-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-v0.40.0.md')
        and (ROOT / 'CONTRACT_CHANGE_REQUEST-M6-SERVER-COMBAT-v0.39.0.md').is_file()
    )


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
    require(
        'docs/design/M6-COMBAT-READINESS-SPEC-v0.48.0.md',
        'NO_CONTRACT_CHANGE_REQUIRED_FOR_M6_V0_49_LOCAL_PROTOTYPE',
        'Combat Ownership Model',
        'Required Runtime Gates For Actual Combat',
        'Entry Criteria For M6 v0.49',
        'Forbidden Scope For v0.49',
        'Decision Tree',
    )
    require(
        'docs/design/M6-COMBAT-CONTRACT-IMPACT-REVIEW-v0.48.0.md',
        'NO_CONTRACT_CHANGE_REQUIRED',
        'No protobuf mutation is required',
        'No schema mutation is required',
        'No `CONTRACT_CHANGE_REQUEST-M6-COMBAT-v0.48.0.md` is created',
    )
    require(
        'docs/tasks/M6-LOCAL-COMBAT-PROTOTYPE-v0.49.0.md',
        'existing `CombatIntent`',
        'No protocol changes',
        'No GameData schema changes',
        'No private DTO/schema workaround',
    )
    require(
        'docs/execution/checklists/M6-COMBAT-ENTRY-CHECKLIST-v0.48.0.md',
        'NO_CONTRACT_CHANGE_REQUIRED',
        'CONTRACT_CHANGE_REQUIRED',
        'BLOCKED_CONTRACT',
        'FIX_REQUIRED',
    )
    require(
        'HANDOFF-LG-M6-COMBAT-READINESS-v0.48.0.md',
        'M6_COMBAT_READINESS_ACCEPTED_v0.48.0',
        'Contract change required: no',
        'No real combat',
    )
    require(
        'M6-COMBAT-READINESS-FINAL-REPORT-v0.48.0.md',
        'M6_COMBAT_READINESS_ACCEPTED_v0.48.0',
        'Contract change required: no',
        'No gameplay implementation',
    )
    require(
        'LGO-M6-COMBAT-READINESS-v0.48.0-CHANGED-FILES.txt',
        'docs/design/M6-COMBAT-READINESS-SPEC-v0.48.0.md',
        'tools/validate_m6_combat_readiness_spec.py',
    )
    require(
        'LGO-M6-COMBAT-READINESS-v0.48.0-DELETIONS.txt',
        'DELETED',
        'none',
    )
    require_file('tools/validate_m6_combat_readiness_spec.py', executable=True)

    v040_active = v040_contract_is_active()
    for path in git_lines('diff', '--name-only'):
        if v040_active and path in V040_CONTRACT_FILES:
            continue
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
