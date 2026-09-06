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
        'RuntimeUiFactory.NewSessionMenuShell("Menu phiên", "LGO Session Menu Overlay")',
        'Tiếp tục',
        'Lưu vị trí',
        'Về điện nhân vật',
        'Thoát',
        'KeyCode.Escape',
        '_sessionActions',
        '_sessionLocationRow',
        '_sessionObjectiveRow',
        'RuntimeSessionMenuLayout.ApplyActions(_sessionActions',
        'RuntimeSessionMenuLayout.ApplyDetails(_sessionLocationRow, _sessionObjectiveRow, layout);',
        'RuntimeSessionMenuLayout.ApplyStatus(_sessionMenuStatus, layout);',
    )
    require(
        'client/Unity/Assets/Game/UI/Runtime/RuntimeSessionMenuLayout.cs',
        'internal static void ApplyActions',
        'internal static void ApplyStatus',
        'internal static void ApplyDetails',
        'RuntimeUiOverflowGuard.ApplyBoundedScroll(scroll, layout.SessionMenuContentMaxHeight, 0f);',
        'scroll.verticalScrollerVisibility = ScrollerVisibility.Hidden',
        'RuntimeUiOverflowGuard.ApplyViewportOverlaySurface(',
        'RuntimeUiOverlayPlacement.Center',
        'RuntimeUiOverlayVerticalPlacement.Center',
        'layout.SessionMenuInsetHorizontal',
        'layout.SessionMenuInsetVertical',
        'status.style.display = DisplayStyle.None',
        'objectiveRow.style.display = DisplayStyle.None',
        'LGO Session Menu Demo Vertical Actions v1',
        'actions.style.justifyContent = Justify.Center',
        'RuntimeUiOverflowGuard.ApplyResponsiveColumns(actions, 1, layout.SessionMenuActionGap, buttons)',
        'actions.style.maxWidth = layout.SessionMenuActionMaxWidth',
        'RuntimeUiSkin.ApplyButtonTier(button, layout.IsMobile ? RuntimeUiButtonTier.Compact : RuntimeUiButtonTier.Standard)',
        'button.style.flexGrow = 0',
    )
    require(
        'client/Unity/Assets/Game/UI/Runtime/RuntimeUiLayoutProfile.cs',
        'SessionMenuInsetHorizontal',
        'SessionMenuInsetVertical',
        'SessionMenuActionGap',
        'SessionMenuActionMaxWidth',
        'SessionMenuContentMaxHeight',
        'Mathf.Clamp(Width * 0.30f, 340f, 380f)',
        '(Height - SessionMenuMaxHeight) * 0.5f',
    )
    require(
        'client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs',
        'internal static VisualElement NewSessionMenuShell',
        'LGO Session Menu Title',
        'NewOrnamentRule(RuntimeArtCatalog.Gold)',
    )
    require(
        'client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs',
        'internal enum RuntimeUiButtonTier',
        'ApplyButtonTier(Button button, RuntimeUiButtonTier tier',
        'ApplyEdgeFrame(panel, RuntimeArtCatalog.Gold, RuntimeArtCatalog.Gold, RuntimeArtCatalog.Gold, RuntimeArtCatalog.Gold)',
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
