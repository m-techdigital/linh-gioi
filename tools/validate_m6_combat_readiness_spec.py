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
    'client/Unity/Assets/Game/World/Runtime/M5FirstPlayableLoopSmokeRunner.cs',
    'client/Unity/Assets/Game/World/Runtime/M5GuidedTrainingLoopSmokeRunner.cs',
    'client/Unity/Assets/Game/World/Runtime/M5LightweightDialogueSmokeRunner.cs',
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
    'tools/validate_m6_combat_ux_readability_polish.py',
    'tools/lgo_m4_closure_check.sh',
    'docs/tasks/M6-COMBAT-UX-READABILITY-POLISH-v0.53.0.md',
    'docs/design/M6-COMBAT-UX-READABILITY-POLISH-v0.53.0.md',
    'docs/execution/checklists/M6-COMBAT-UX-READABILITY-POLISH-CHECKLIST-v0.53.0.md',
    'M6-COMBAT-UX-READABILITY-POLISH-FINAL-REPORT-v0.53.0.md',
    'HANDOFF-LG-M6-COMBAT-UX-READABILITY-POLISH-v0.53.0.md',
    'LGO-M6-COMBAT-UX-READABILITY-POLISH-v0.53.0-CHANGED-FILES.txt',
    'LGO-M6-COMBAT-UX-READABILITY-POLISH-v0.53.0-DELETIONS.txt',
    'tools/validate_m6_combat_gamedata_balance.py',
    'tools/validate_lgo_art_pack_v1.py',
    'client/Unity/Assets/Game/Art/Runtime/CombatPlaceholderAssets.cs',
    'docs/tasks/M6-COMBAT-GAMEDATA-BALANCE-ADVERSARIAL-v0.54.0.md',
    'docs/design/M6-COMBAT-GAMEDATA-BALANCE-NOTES-v0.54.0.md',
    'docs/execution/checklists/M6-COMBAT-GAMEDATA-BALANCE-CHECKLIST-v0.54.0.md',
    'M6-COMBAT-GAMEDATA-BALANCE-FINAL-REPORT-v0.54.0.md',
    'HANDOFF-LG-M6-COMBAT-GAMEDATA-BALANCE-v0.54.0.md',
    'LGO-M6-COMBAT-GAMEDATA-BALANCE-v0.54.0-CHANGED-FILES.txt',
    'LGO-M6-COMBAT-GAMEDATA-BALANCE-v0.54.0-DELETIONS.txt',
    'tools/run_m6_server_authoritative_combat_pilot.sh',
    'tools/validate_m6_server_authoritative_combat_pilot.py',
}
RUNTIME_ASSET_WEIGHT_HYGIENE_PREFIXES = (
    'client/Unity/Assets/Game/Art/Runtime/LgoVisualAssetRegistryV2.cs',
    'client/Unity/Assets/Game/Art/Runtime/V3B/',
    'docs/reference-art/v3b/metadata/',
    'docs/art/v3/',
    'docs/art/RUNTIME-ASSET-WEIGHT-HYGIENE.md',
    'docs/tasks/LGO-RUNTIME-ASSET-WEIGHT-HYGIENE-v1.0.md',
    'tools/prepare_lgo_art_v3b_candidates.py',
    'tools/validate_lgo_art_v3b_candidates.py',
    'tools/validate_lgo_runtime_asset_weight.py',
)
LOGIN_V3B_RUNTIME_ONLY_FILES = {
    'client/Unity/Assets/Game/Art/Runtime/LgoVisualAssetRegistryV3B.cs',
    'client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/Login/logo_linh_gioi_online_v3b_light_runtime_candidate.png',
    'client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/Login/logo_linh_gioi_online_v3b_light_runtime_candidate.png.meta',
    'client/Unity/Assets/Game/Bootstrap/Runtime/GameBootstrap.cs',
    'client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs',
    'client/Unity/Assets/Game/UI/Runtime/RuntimePngWriter.cs',
    'client/Unity/Assets/Game/UI/Runtime/RuntimePngWriter.cs.meta',
    'client/Unity/Assets/Game/UI/Runtime/VisualRuntimeEvidenceRunner.cs',
    'client/Unity/Assets/Game/UI/Runtime/VisualRuntimeEvidenceRunner.cs.meta',
    'docs/art/v3b/LOGIN-GATE-ENTRY-ASSET-PACK-v3b-runtime.md',
    'docs/art/v3b/LOGIN-GATE-ENTRY-ASSET-MAPPING-v3b-runtime.csv',
    'docs/execution/LGO-VISUAL-RUNTIME-EVIDENCE-HARNESS.md',
    'docs/execution/checklists/LOGIN-GATE-ENTRY-VISUAL-CHECKLIST-v1.md',
    'docs/tasks/LOGIN-GATE-ENTRY-VISUAL-v1.md',
    'tools/lgo_visual_runtime_review.sh',
    'tools/run_m4_visible_ui_review.sh',
    'tools/validate_lgo_login_gate_entry_visual_v1.py',
    'tools/validate_lgo_runtime_asset_weight.py',
    'tools/validate_m4_visible_ui.py',
    'tools/validate_m5_visual_evidence.py',
}
LOGIN_V3B_REMOVED_PREFIXES = (
    'client/Unity/Assets/Game/Art/Runtime/V3BA',
    'client/Unity/Assets/Game/Art/Runtime/FinalLogin',
)
LOGIN_V3B_REMOVED_FILES = {
    'client/Unity/Assets/Game/Art/Runtime/V3BA.meta',
    'client/Unity/Assets/Game/Art/Runtime/FinalLogin.meta',
    'client/Unity/Assets/Game/Art/Runtime/LgoVisualAssetRegistryV3BA.cs',
    'client/Unity/Assets/Game/Art/Runtime/LgoVisualAssetRegistryV3BA.cs.meta',
    'client/Unity/Assets/Game/Art/Runtime/LgoFinalLoginAssetRegistry.cs',
    'client/Unity/Assets/Game/Art/Runtime/LgoFinalLoginAssetRegistry.cs.meta',
    'docs/art/v3b/LOGIN-GATE-ENTRY-ASSET-PACK-v3b-a.md',
    'docs/art/v3b/LOGIN-GATE-ENTRY-ASSET-MAPPING-v3b-a.csv',
}
RUNTIME_UI_SKIN_ADOPTION_FILES = {
    'client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs',
    'client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs',
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


def runtime_asset_weight_hygiene_is_active() -> bool:
    return (
        'LGO_RUNTIME_ASSET_WEIGHT_HYGIENE_READY'
        in read('docs/tasks/LGO-RUNTIME-ASSET-WEIGHT-HYGIENE-v1.0.md')
        and 'validate_lgo_runtime_asset_weight.py' in read('tools/lgo_playable_closure_check.sh')
    )


def login_v3b_runtime_only_is_active() -> bool:
    return (
        'LGO_LOGIN_GATE_ENTRY_V3B_RUNTIME_ONLY'
        in read('docs/art/v3b/LOGIN-GATE-ENTRY-ASSET-PACK-v3b-runtime.md')
        and 'LGO_VISUAL_RUNTIME_EVIDENCE_HARNESS_READY'
        in read('docs/execution/LGO-VISUAL-RUNTIME-EVIDENCE-HARNESS.md')
        and 'check_no_v3ba_runtime'
        in read('tools/validate_lgo_login_gate_entry_visual_v1.py')
    )


def runtime_ui_skin_adoption_is_active() -> bool:
    return (
        'LGO_RUNTIME_UI_SKIN_ADOPTION_AUDIT_READY'
        in read('docs/tasks/LGO-RUNTIME-UI-SKIN-ADOPTION-AUDIT-PASS-v1.0.md')
        and 'validate_lgo_runtime_ui_skin_adoption_audit.py'
        in read('tools/lgo_playable_closure_check.sh')
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
    runtime_ui_skin_adoption_active = runtime_ui_skin_adoption_is_active()
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
        runtime_asset_weight_allowed = (
            runtime_asset_weight_hygiene_is_active()
            and any(path == prefix or path.startswith(prefix) for prefix in RUNTIME_ASSET_WEIGHT_HYGIENE_PREFIXES)
        )
        login_v3b_allowed = (
            login_v3b_runtime_only_is_active()
            and (
                path in LOGIN_V3B_RUNTIME_ONLY_FILES
                or path in LOGIN_V3B_REMOVED_FILES
                or any(path == prefix or path.startswith(prefix + '/') for prefix in LOGIN_V3B_REMOVED_PREFIXES)
            )
        )
        runtime_ui_skin_adoption_allowed = (
            runtime_ui_skin_adoption_active
            and path in RUNTIME_UI_SKIN_ADOPTION_FILES
        )
        if path not in ALLOWED_CODE_FILES and not m6_local_allowed and not runtime_asset_weight_allowed and not login_v3b_allowed and not runtime_ui_skin_adoption_allowed:
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
