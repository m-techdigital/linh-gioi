#!/usr/bin/env python3
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []

FORBIDDEN_CHANGED_PREFIXES = ['protocol/', 'gamedata/schemas/', 'docs/adr/']
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
        'LGO Gate Keeper NPC Interactable',
        'LGO Training Stone Interactable',
        'LGO Shadow Slime Non Combat Marker',
        'InteractionRange',
        'ObjectiveText',
        'InteractionText',
        'KeyCode.F',
        'KeyCode.Space',
        'TriggerInteractionForSmoke',
        'SetSmokePositionNearTrainingStone',
        'InteractionAcknowledged',
        'Objective complete: first spirit training loop acknowledged.',
    )
    require(
        'client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs',
        'Objective: enter the world and find the training stone.',
        'Move near the Gate Keeper or Training Stone.',
        'RefreshWorldLoopLabels',
        'InteractionStateChanged',
        'Save Position',
        'Back to Lobby',
        'Quit',
    )
    require(
        'client/Unity/Assets/Game/World/Runtime/M5FirstPlayableLoopSmokeRunner.cs',
        '--lgo-m5-first-playable-loop-smoke',
        'LoginDevAsync',
        'ListCharactersAsync',
        'CreateCharacterAsync',
        'LoadCharacterAsync',
        'TriggerInteractionForSmoke',
        'SaveCharacterPositionAsync',
        'M5 first playable loop smoke status=',
    )
    require(
        'client/Unity/Assets/Game/Bootstrap/Runtime/GameBootstrap.cs',
        'M5FirstPlayableLoopSmokeRunner.ShouldRun()',
    )
    require(
        'tools/m5_first_playable_loop_runtime.py',
        'M5_FIRST_PLAYABLE_LOOP_RUNTIME_SMOKE_PASS',
        '--lgo-m5-first-playable-loop-smoke',
        'Objective complete',
        'savePositionStillWorks',
    )
    require_file('tools/run_m5_first_playable_loop_once.sh', executable=True)
    require(
        'tools/run_m5_first_playable_loop_once.sh',
        'm5_first_playable_loop_runtime.py',
        'UNVERIFIED_ENVIRONMENT',
    )
    require_file('tools/lgo_playable_closure_check.sh', executable=True)
    require(
        'tools/lgo_playable_closure_check.sh',
        'LGO_PLAYABLE_CLOSURE_SOURCE_GATES_PASS',
        'LGO_PLAYABLE_CLOSURE_RUNTIME_GATES_PASS',
        'LGO_PLAYABLE_CLOSURE_RUNTIME_UNVERIFIED_ENVIRONMENT',
        'LGO_PLAYABLE_CLOSURE_PACKAGE_READY',
        'LGO_PLAYABLE_CLOSURE_FIX_REQUIRED',
        'validate_m5_first_playable_loop.py',
        'run_m5_first_playable_loop_once.sh',
    )
    require(
        'tools/run_m4_visible_ui_review.sh',
        'visible-ui-review-summary.json',
        'VISIBLE_UI_SCREENSHOT_UNAVAILABLE',
        'VISIBLE_UI_SCREENSHOT_CAPTURED',
    )
    require('docs/tasks/M5-FIRST-PLAYABLE-LOOP-FOUNDATION-v0.15.0.md', 'first playable loop foundation', 'not full combat', 'M5_FIRST_PLAYABLE_LOOP_RUNTIME_SMOKE_PASS')
    require('docs/execution/LGO-PLAYABLE-CLOSURE-COMMAND-v0.15.0.md', './tools/lgo_playable_closure_check.sh --source-only', './tools/run_m4_visible_ui_review.sh --rebuild')
    require('README.md', 'M5_FIRST_PLAYABLE_LOOP_SOURCE_READY')
    require('START-HERE.md', 'M5 First Playable Loop Foundation')
    require('VERSIONING.md', 'source_package_version = 0.15.0', 'client_version = 0.5.0-m5')
    require('docs/execution/PROJECT-STATE.md', 'M5_FIRST_PLAYABLE_LOOP_SOURCE_READY')

    for path in git_lines('diff', '--name-only'):
        if path == 'client/Unity/Assets/Game/UI/design-tokens.json':
            errors.append(f'frozen surface modified: {path}')
        for prefix in FORBIDDEN_CHANGED_PREFIXES:
            if path.startswith(prefix):
                errors.append(f'frozen surface modified: {path}')
    for line in git_lines('status', '--short', '--untracked-files=all'):
        path = line[3:] if len(line) >= 4 else line
        for prefix in FORBIDDEN_OUTPUT_PREFIXES:
            if path.startswith(prefix):
                errors.append(f'generated/cache/build output under source status: {path}')

    if errors:
        print('M5 FIRST PLAYABLE LOOP VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M5 FIRST PLAYABLE LOOP VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
