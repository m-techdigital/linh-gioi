#!/usr/bin/env python3
from __future__ import annotations

import os
import re
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


def check_panel_sizes(ui: str) -> None:
    for match in re.finditer(r'NewPanel\((\d+)\)', ui):
        value = int(match.group(1))
        if value > 960:
            errors.append(f'panel max width exceeds 1280x720 review target: NewPanel({value})')
    min_widths = [int(v) for v in re.findall(r'minWidth\s*=\s*(\d+)', ui)]
    if min_widths and max(min_widths) > 360:
        errors.append(f'fixed minWidth too large for visible review: {max(min_widths)}')
    if 'ShowLobbyMode()' in ui and '_authPanel.style.display = DisplayStyle.None;' not in ui:
        errors.append('lobby mode must not keep the full auth panel visible beside character hall')


def main() -> int:
    ui = read('client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs')
    check_panel_sizes(ui)
    require(
        'client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs',
        'Auth / Gate Entry',
        'Character Hall',
        'World HUD',
        'Save Position',
        'Back to Lobby',
        'API status',
        'Quit',
        'KeyCode.Escape',
        'Application.Quit',
    )
    require(
        'tools/run_m4_visible_ui_review.sh',
        '--rebuild',
        '--open-existing',
        '--stop',
        '-screen-fullscreen 0',
        '-screen-width 1280',
        '-screen-height 720',
        'M4_VISIBLE_UI_MANUAL_CHECKLIST',
    )
    script = ROOT / 'tools/run_m4_visible_ui_review.sh'
    if script.exists() and not os.access(script, os.X_OK):
        errors.append('tools/run_m4_visible_ui_review.sh is not executable')
    if '|| true' in read('tools/run_m4_visible_ui_review.sh'):
        errors.append('manual visible UI review script must not contain || true')
    require('docs/execution/M4-VISIBLE-UI-REVIEW-COMMAND-v0.14.0.md', 'Login', 'Character Hall', 'World HUD', '--rebuild', '--open-existing', '--stop')
    require('client/Unity/Assets/Game/World/Runtime/M4PlayableVerticalSliceSmokeRunner.cs', '--lgo-m4-playable-vertical-slice-smoke')
    require('client/Unity/Assets/Game/Art/Runtime/M4VisualFoundationSmokeRunner.cs', '--lgo-m4-visual-foundation-smoke')

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
        print('M4 VISIBLE UI VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M4 VISIBLE UI VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
