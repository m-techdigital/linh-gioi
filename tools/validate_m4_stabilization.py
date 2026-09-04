#!/usr/bin/env python3
from __future__ import annotations

import json
import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []
V040_CONTRACT_FILES = {
    'protocol/combat.proto',
    'gamedata/schemas/skill.schema.json',
    'gamedata/skills/wind_slash.yaml',
    'gamedata/compiled/gamedata-manifest.json',
    'tests/gamedata/test_gamedata_pipeline.py',
    'tests/gamedata/__pycache__/test_gamedata_pipeline.cpython-312.pyc',
}

REQUIRED_DOCS = [
    'README.md',
    'START-HERE.md',
    'VERSIONING.md',
    'docs/execution/PROJECT-STATE.md',
    'docs/execution/M4-CLOSURE-COMMAND-v0.13.0.md',
    'docs/execution/M4-VISIBLE-UI-REVIEW-COMMAND-v0.14.0.md',
    'docs/tasks/M4-PLAYABLE-SLICE-STABILIZATION-v0.13.0.md',
    'docs/tasks/M4-VISIBLE-UI-USABILITY-AND-REVIEW-HARNESS-v0.14.0.md',
    'docs/design/LGO-DESIGN-DIRECTION-LOCK-v0.11.0.md',
    'docs/design/LGO-PLAYABLE-UI-WIREFRAME-SPEC-v0.11.0.md',
    'docs/art/LGO-VISUAL-IDENTITY-GUIDE-v0.10.0.md',
    'docs/art/LGO-RUNTIME-ART-MANIFEST-v0.10.0.md',
]

REQUIRED_TOOLS = [
    'tools/lgo_m4_closure_check.sh',
    'tools/run_m4_visible_ui_review.sh',
    'tools/validate_project_state.py',
    'tools/validate_m4_playable_source.py',
    'tools/validate_m4_visual_foundation.py',
    'tools/validate_m4_2_playable_ui.py',
    'tools/validate_m4_stabilization.py',
    'tools/validate_m4_visible_ui.py',
    'tools/run_m4_playable_vertical_slice_once.sh',
    'tools/run_m4_visual_foundation_once.sh',
    'tools/m4_playable_vertical_slice_runtime.py',
    'tools/m4_visual_foundation_runtime.py',
    'tools/protobuf/darwin-arm64/protoc',
    'tools/protobuf/darwin-arm64/SHA256',
]

REQUIRED_ART_ASSETS = [
    'client/Unity/Assets/Game/Art/Characters/lgo_character_hero_sword_placeholder.svg',
    'client/Unity/Assets/Game/Art/NPCs/lgo_npc_keeper_placeholder.svg',
    'client/Unity/Assets/Game/Art/Monsters/lgo_monster_shadow_slime_placeholder.svg',
    'client/Unity/Assets/Game/Art/Items/lgo_item_iron_sword_icon.svg',
    'client/Unity/Assets/Game/Art/Items/lgo_item_spirit_stone_icon.svg',
    'client/Unity/Assets/Game/Art/Items/lgo_item_healing_gourd_icon.svg',
    'client/Unity/Assets/Game/Art/Skills/lgo_skill_wind_slash_icon.svg',
    'client/Unity/Assets/Game/Art/Skills/lgo_skill_shadow_bind_icon.svg',
    'client/Unity/Assets/Game/Art/Skills/lgo_skill_spirit_guard_icon.svg',
    'client/Unity/Assets/Game/Art/Maps/lgo_map_training_ground_tile.svg',
    'client/Unity/Assets/Game/Art/UI/lgo_ui_frame_panel.svg',
    'client/Unity/Assets/Game/Art/UI/lgo_ui_button_rune.svg',
    'client/Unity/Assets/Game/Art/UI/lgo_ui_status_api_icon.svg',
    'client/Unity/Assets/Game/Art/VFX/lgo_vfx_spirit_burst_marker.svg',
    'client/Unity/Assets/Game/Art/Shared/lgo_shared_palette_motifs.svg',
]

FORBIDDEN_CHANGED_PREFIXES = [
    'protocol/',
    'gamedata/schemas/',
    'docs/adr/',
]

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
    if not target.exists():
        errors.append(f'missing: {path}')
        return ''
    return target.read_text(encoding='utf-8', errors='replace')


def v040_contract_is_active() -> bool:
    return (
        'M6_COMBAT_PROTOCOL_GAMEDATA_CONTRACT_ACCEPTED_v0.40.0'
        in read('docs/tasks/M6-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-v0.40.0.md')
        and (ROOT / 'CONTRACT_CHANGE_REQUEST-M6-SERVER-COMBAT-v0.39.0.md').is_file()
    )


def require(path: str, *markers: str) -> None:
    content = read(path)
    for marker in markers:
        if marker not in content:
            errors.append(f'{path} missing marker: {marker}')


def require_file(path: str) -> None:
    if not (ROOT / path).is_file():
        errors.append(f'missing file: {path}')


def changed_paths() -> list[str]:
    result = subprocess.run(
        ['git', '--no-pager', 'diff', '--name-only'],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if result.returncode != 0:
        errors.append('cannot inspect git diff for frozen surface audit: ' + result.stderr.strip())
        return []
    return result.stdout.splitlines()


def tracked_or_untracked_paths() -> list[str]:
    result = subprocess.run(
        ['git', '--no-pager', 'status', '--short', '--untracked-files=all'],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if result.returncode != 0:
        errors.append('cannot inspect git status for output hygiene: ' + result.stderr.strip())
        return []
    paths: list[str] = []
    for line in result.stdout.splitlines():
        if len(line) >= 4:
            paths.append(line[3:])
    return paths


def validate_manifest_coverage() -> None:
    manifest = read('docs/art/LGO-RUNTIME-ART-MANIFEST-v0.10.0.md')
    visual_manifest = read('m4-visual-manifest.json')
    try:
        visual = json.loads(visual_manifest)
    except json.JSONDecodeError as exc:
        errors.append(f'm4-visual-manifest.json invalid JSON: {exc}')
        visual = {}
    visual_text = json.dumps(visual, sort_keys=True)
    for asset in REQUIRED_ART_ASSETS:
        require_file(asset)
        if asset not in manifest:
            errors.append(f'runtime art manifest missing asset: {asset}')
        if asset not in visual_text:
            errors.append(f'm4 visual manifest missing asset: {asset}')


def main() -> int:
    for path in REQUIRED_DOCS:
        require_file(path)
    for path in REQUIRED_TOOLS:
        require_file(path)

    closure = ROOT / 'tools/lgo_m4_closure_check.sh'
    if closure.exists() and not os.access(closure, os.X_OK):
        errors.append('tools/lgo_m4_closure_check.sh is not executable')

    require(
        'tools/lgo_m4_closure_check.sh',
        '--source-only',
        '--runtime',
        '--package-ready',
        'LGO_M4_CLOSURE_SOURCE_GATES_PASS',
        'LGO_M4_CLOSURE_RUNTIME_GATES_PASS',
        'LGO_M4_CLOSURE_RUNTIME_UNVERIFIED_ENVIRONMENT',
        'LGO_M4_CLOSURE_PACKAGE_READY',
        'validate_m4_visible_ui.py',
        'latest-summary.json',
    )
    require(
        'client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs',
        'Auth / Gate Entry',
        'Character Hall',
        'World HUD',
        'Open Gate',
        'Enter World',
        'Save Position',
        'Back to Lobby',
    )
    require(
        'client/Unity/Assets/Game/World/Runtime/M4PlayableVerticalSliceSmokeRunner.cs',
        '--lgo-m4-playable-vertical-slice-smoke',
    )
    require(
        'tools/m4_playable_vertical_slice_runtime.py',
        'M4_PLAYABLE_VERTICAL_SLICE_RUNTIME_SMOKE_PASS',
    )
    require(
        'client/Unity/Assets/Game/Art/Runtime/M4VisualFoundationSmokeRunner.cs',
        '--lgo-m4-visual-foundation-smoke',
    )
    require(
        'tools/m4_visual_foundation_runtime.py',
        'M4_VISUAL_PLACEHOLDER_FOUNDATION_SMOKE_PASS',
    )
    require(
        'docs/execution/PROJECT-STATE.md',
        'M4-2/M4-3 Playable UI And Art Quality Pass',
        'M4_PLAYABLE_UI_ART_QUALITY_SOURCE_READY',
    )
    require(
        'VERSIONING.md',
        'source_package_version = m6-combat-foundation-v0.55.0',
        'client_version = 0.6.0-m6-combat-foundation',
        'm6_governance_baseline = M6_COMBAT_READINESS_SPEC_CLOSED_v0.32.0',
        'M4_VISIBLE_UI_USABILITY_SOURCE_READY',
        'M5_FIRST_PLAYABLE_LOOP_SOURCE_READY',
        'M5_VISUAL_EVIDENCE_UX_REVIEW_READY',
        'M5_GUIDED_TRAINING_LOOP_SOURCE_READY',
    )
    validate_manifest_coverage()

    v040_active = v040_contract_is_active()
    for path in changed_paths():
        if v040_active and path in V040_CONTRACT_FILES:
            continue
        if path == 'client/Unity/Assets/Game/UI/design-tokens.json':
            errors.append(f'frozen surface modified: {path}')
        for prefix in FORBIDDEN_CHANGED_PREFIXES:
            if path.startswith(prefix):
                errors.append(f'frozen surface modified: {path}')

    for path in tracked_or_untracked_paths():
        for prefix in FORBIDDEN_OUTPUT_PREFIXES:
            if path.startswith(prefix):
                errors.append(f'generated/cache/build output under source status: {path}')

    if errors:
        print('M4 STABILIZATION VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M4 STABILIZATION VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
