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
        'docs/art/LGO-VISUAL-REFERENCE-PACK-v0.16.5.md',
        'LGO_VISUAL_REFERENCE_PACK_ACCEPTED_v0.16.5',
        'Gate Keeper',
        'Training Stone',
        'cyan spirit energy',
    )
    require(
        'client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs',
        'GuidedTrainingStep',
        'FindGateKeeper',
        'FindTrainingStone',
        'Objective: talk to the Gate Keeper.',
        'Objective: stabilize the Training Stone.',
        'Objective complete: spirit pulse stabilized.',
        'Gate Keeper: your path is open. Follow the cyan spirit pulse.',
        'SetSmokePositionNearGateKeeper',
        'SetSmokePositionNearTrainingStone',
        'LGO Shadow Slime Non Combat Marker',
    )
    require(
        'client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs',
        'Objective: talk to the Gate Keeper.',
        'Move near the Gate Keeper.',
        'Save Position',
        'Back to Lobby',
        'Quit',
    )
    require(
        'client/Unity/Assets/Game/World/Runtime/M5GuidedTrainingLoopSmokeRunner.cs',
        '--lgo-m5-guided-training-loop-smoke',
        'LoginDevAsync',
        'LoadCharacterAsync',
        'SetSmokePositionNearGateKeeper',
        'SetSmokePositionNearTrainingStone',
        'gateKeeperInteractionTriggered',
        'trainingStoneInteractionTriggered',
        'SaveCharacterPositionAsync',
    )
    require('client/Unity/Assets/Game/Bootstrap/Runtime/GameBootstrap.cs', 'M5GuidedTrainingLoopSmokeRunner.ShouldRun()')
    require_file('tools/run_m5_guided_training_loop_once.sh', executable=True)
    require_file('tools/m5_guided_training_loop_runtime.py', executable=False)
    require(
        'tools/m5_guided_training_loop_runtime.py',
        'M5_GUIDED_TRAINING_LOOP_RUNTIME_SMOKE_PASS',
        '--lgo-m5-guided-training-loop-smoke',
        'Gate Keeper',
        'Training Stone',
        'Objective complete',
        'savePositionStillWorks',
    )
    require(
        'tools/lgo_playable_closure_check.sh',
        'validate_m5_guided_training_loop.py',
        'run_m5_guided_training_loop_once.sh',
        'M5_GUIDED_TRAINING_LOOP_RUNTIME_SMOKE_PASS',
    )
    require('docs/tasks/M5-GUIDED-TRAINING-LOOP-v0.17.0.md', 'M5_GUIDED_TRAINING_LOOP_RUNTIME_CLOSED_LOCAL_v0.17.0', 'not full combat')
    require('m5-manifest.json', 'M5 Guided Training Loop', '0.17.0', 'M5_GUIDED_TRAINING_LOOP_SOURCE_READY')

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
        print('M5 GUIDED TRAINING LOOP VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M5 GUIDED TRAINING LOOP VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
