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


def git_lines(*args: str) -> list[str]:
    result = subprocess.run(['git', '--no-pager', *args], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if result.returncode != 0:
        errors.append('git command failed: git --no-pager ' + ' '.join(args) + ' ' + result.stderr.strip())
        return []
    return result.stdout.splitlines()


def main() -> int:
    require(
        'client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs',
        'private const float MoveSpeed = 3.6f;',
        'private const float RotateSpeed = 105f;',
        'CameraFollowOffset',
        'RefreshCameraFrame',
        'Vector3.Lerp',
        'Mathf.Clamp01(Time.deltaTime * 7f)',
        'Quaternion.Euler(43f, 0f, 0f)',
        'camera.orthographic = true',
        'camera.orthographicSize = CurrentCameraOrthographicSize()',
        'private static float CurrentCameraOrthographicSize()',
        'return 5.45f',
        'return 6.15f',
        'return 7.0f',
    )
    require(
        'client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs',
        '_worldDebugStrip = NewBadgeStrip(',
        '("Di chuyển", "WASD hoặc phím mũi tên")',
        '("Xoay", "Q / E")',
        '("Tương tác", "F hoặc Space")',
        '("Menu", "Esc")',
        'Esc mở menu phiên trong thế giới; Thoát đóng phiên hiện tại.',
        'Sân Luyện An Toàn',
    )
    require(
        'client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs',
        'internal static VisualElement NewBadgeStrip',
        'internal static VisualElement NewBadge',
    )
    require(
        'tools/lgo_playable_closure_check.sh',
        'validate_m5_input_camera_polish.py',
        'm5_input_camera_polish',
    )
    require('docs/tasks/M5-INPUT-CAMERA-POLISH-v0.26.0.md', 'M5_INPUT_CAMERA_POLISH_RUNTIME_CLOSED_LOCAL_v0.26.0', 'No combat')

    world = read('client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs')
    forbidden_world_markers = ['NavMesh', 'Pathfinding', 'InputAction', 'Cinemachine', 'TargetLock']
    for marker in forbidden_world_markers:
        if marker in world:
            errors.append(f'out-of-scope camera/input marker present: {marker}')

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
        print('M5 INPUT CAMERA POLISH VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M5 INPUT CAMERA POLISH VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
