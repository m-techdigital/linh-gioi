#!/usr/bin/env python3
from __future__ import annotations

import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []


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
        'client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs',
        'LGO Session Menu Overlay',
        'Menu phiên',
        'Đang tạm dừng trong sân luyện.',
        'ToggleSessionMenu',
        'HideSessionMenu',
        'SetSessionMenuVisible',
        'Tiếp tục',
        'Lưu vị trí',
        'Về điện nhân vật',
        'Thoát',
        'KeyCode.Escape',
    )
    require(
        'tools/lgo_playable_closure_check.sh',
        'validate_m5_session_menu.py',
        'm5_session_menu',
    )
    require('docs/tasks/M5-SESSION-MENU-v0.27.0.md', 'M5_SESSION_MENU_RUNTIME_CLOSED_LOCAL_v0.27.0', 'No combat')

    for path in git_lines('diff', '--name-only'):
        if path == 'client/Unity/Assets/Game/UI/design-tokens.json':
            errors.append(f'frozen surface modified: {path}')
        for prefix in ['protocol/', 'gamedata/schemas/', 'docs/adr/']:
            if path.startswith(prefix):
                errors.append(f'frozen surface modified: {path}')
    for line in git_lines('status', '--short', '--untracked-files=all'):
        path = line[3:] if len(line) >= 4 else line
        for prefix in ['build/', 'client/Unity/Library/', 'client/Unity/Temp/', 'client/Unity/Logs/', 'client/Unity/Assets/Game/Generated/', 'client/Unity/Assets/Game/Protocol/Generated/']:
            if path.startswith(prefix):
                errors.append(f'generated/cache/build output under source status: {path}')

    if errors:
        print('M5 SESSION MENU VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M5 SESSION MENU VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
