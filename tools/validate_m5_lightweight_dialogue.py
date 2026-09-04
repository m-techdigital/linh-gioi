#!/usr/bin/env python3
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []
FORBIDDEN_CHANGED_PREFIXES = ['protocol/', 'gamedata/schemas/', 'docs/adr/']
FORBIDDEN_OUTPUT_PREFIXES = ['build/', 'client/Unity/Library/', 'client/Unity/Temp/', 'client/Unity/Logs/', 'client/Unity/Assets/Game/Generated/', 'client/Unity/Assets/Game/Protocol/Generated/']
FORBIDDEN_MARKERS = ['QuestDatabase', 'RewardSystem', 'Inventory', 'RelationshipSystem', 'ChatSystem', 'HitPoints', 'Damage']


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
    elif executable and not os.access(target, os.X_OK):
        errors.append(f'file is not executable: {path}')


def git_lines(*args: str) -> list[str]:
    result = subprocess.run(['git', '--no-pager', *args], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if result.returncode != 0:
        errors.append('git command failed: git --no-pager ' + ' '.join(args) + ' ' + result.stderr.strip())
        return []
    return result.stdout.splitlines()


def main() -> int:
    require('client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs', 'GateKeeperDialogueLines', 'DialogueActive', 'DialogueCompleted', 'ContinueDialogue', 'CloseDialogue', 'Objective: listen to the Gate Keeper.', 'Objective: stabilize the Training Stone.')
    require('client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs', '_dialoguePanel', '_dialogueContinueButton', '_dialogueCloseButton', 'RefreshDialoguePanel', 'SetDialogueVisible')
    require('client/Unity/Assets/Game/World/Runtime/M5LightweightDialogueSmokeRunner.cs', '--lgo-m5-lightweight-dialogue-smoke', 'openedDialogue', 'dialogueCompleted', 'savePositionStillWorks')
    require('client/Unity/Assets/Game/Bootstrap/Runtime/GameBootstrap.cs', 'M5LightweightDialogueSmokeRunner.ShouldRun()', 'M5LightweightDialogueSmokeRunner.RunFromCommandLineAsync')
    require('tools/m5_lightweight_dialogue_runtime.py', 'M5_LIGHTWEIGHT_NPC_DIALOGUE_RUNTIME_SMOKE_PASS', 'UNITY_M5_LIGHTWEIGHT_DIALOGUE_PASS')
    require('tools/run_m5_lightweight_dialogue_once.sh', 'm5_lightweight_dialogue_runtime.py')
    require('tools/lgo_playable_closure_check.sh', 'validate_m5_lightweight_dialogue.py', 'm5_lightweight_dialogue_runtime')
    require('docs/tasks/M5-LIGHTWEIGHT-NPC-DIALOGUE-v0.24.0.md', 'M5_LIGHTWEIGHT_NPC_DIALOGUE_SOURCE_READY_v0.24.0', 'local-only dialogue')
    require_file('tools/run_m5_lightweight_dialogue_once.sh', executable=True)
    require_file('tools/m5_lightweight_dialogue_runtime.py', executable=True)

    joined = read('client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs') + read('client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs')
    for marker in FORBIDDEN_MARKERS:
        if marker in joined:
            errors.append(f'forbidden system marker present: {marker}')
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
        print('M5 LIGHTWEIGHT NPC DIALOGUE VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M5 LIGHTWEIGHT NPC DIALOGUE VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
