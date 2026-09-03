#!/usr/bin/env python3
from __future__ import annotations

import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []


def read(path: str) -> str:
    target = ROOT / path
    if not target.exists():
        errors.append(f'missing: {path}')
        return ''
    return target.read_text(encoding='utf-8')


def require(path: str, *markers: str) -> None:
    content = read(path)
    for marker in markers:
        if marker not in content:
            errors.append(f'{path} missing marker: {marker}')


def require_ref(asmdef_path: str, reference: str) -> None:
    content = read(asmdef_path)
    if not content:
        return
    data = json.loads(content)
    if reference not in data.get('references', []):
        errors.append(f'{asmdef_path} must reference {reference}')


def main() -> int:
    require(
        'client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs',
        'Input.GetAxisRaw("Horizontal")',
        'KeyCode.Q',
        'KeyCode.E',
        'BuildSaveRequest',
        'SetSmokePosition',
        'FormatPosition',
    )
    require(
        'client/Unity/Assets/Game/World/Runtime/M4PlayableVerticalSliceSmokeRunner.cs',
        '--lgo-m4-playable-vertical-slice-smoke',
        'LoginDevAsync',
        'ListCharactersAsync',
        'CreateCharacterAsync',
        'LoadCharacterAsync',
        'SaveCharacterPositionAsync',
        '--lgo-m4-expect-existing',
        'restart loaded position mismatch',
    )
    require(
        'client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs',
        'Dev key',
        'No character yet',
        'Enter World',
        'Save Position',
        'Back to Lobby',
        'AccountApiClient',
    )
    require('client/Unity/Assets/Game/Bootstrap/Runtime/GameBootstrap.cs', 'M4PlayableVerticalSliceSmokeRunner.ShouldRun()', 'M4PlayableClientController.Attach(gameObject)')
    require('tools/run_m4_playable_vertical_slice_once.sh', 'm4_playable_vertical_slice_runtime.py', 'UNVERIFIED_ENVIRONMENT')
    require('tools/m4_playable_vertical_slice_runtime.py', 'M4_PLAYABLE_VERTICAL_SLICE_RUNTIME_SMOKE_PASS', '--lgo-m4-playable-vertical-slice-smoke')
    require('m4-manifest.json', 'M4-0 / v0.9.0', 'M4_PLAYABLE_VERTICAL_SLICE_FOUNDATION_SOURCE_READY')
    require_ref('client/Unity/Assets/Game/World/LinhGioi.World.asmdef', 'LinhGioi.Account')
    require_ref('client/Unity/Assets/Game/UI/LinhGioi.UI.asmdef', 'LinhGioi.World')
    require_ref('client/Unity/Assets/Game/UI/LinhGioi.UI.asmdef', 'LinhGioi.Account')
    require_ref('client/Unity/Assets/Game/Bootstrap/LinhGioi.Bootstrap.asmdef', 'LinhGioi.UI')
    require_ref('client/Unity/Assets/Game/Bootstrap/LinhGioi.Bootstrap.asmdef', 'LinhGioi.World')

    if errors:
        print('M4 PLAYABLE VERTICAL SLICE STATIC VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M4 PLAYABLE VERTICAL SLICE STATIC VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
