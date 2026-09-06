#!/usr/bin/env python3
from __future__ import annotations

import subprocess
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
ERRORS: list[str] = []


def read(rel: str) -> str:
    path = ROOT / rel
    if not path.is_file():
        ERRORS.append(f"missing file: {rel}")
        return ""
    return path.read_text(encoding="utf-8", errors="replace")


def require(rel: str, *markers: str) -> None:
    text = read(rel)
    for marker in markers:
        if marker not in text:
            ERRORS.append(f"{rel} missing marker: {marker}")


def check_frozen() -> None:
    result = subprocess.run(
        [
            "git",
            "--no-pager",
            "diff",
            "--name-only",
            "--",
            "protocol",
            "gamedata/schemas",
            "docs/adr",
            "client/Unity/Assets/Game/UI/design-tokens.json",
        ],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if result.returncode != 0:
        ERRORS.append(result.stderr.strip() or "git frozen diff failed")
    elif result.stdout.strip():
        ERRORS.append("frozen surface changed")


def main() -> int:
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiLayoutProfile.cs",
        "internal readonly struct RuntimeUiLayoutProfile",
        "internal static RuntimeUiLayoutProfile FromScreen",
        "var screenTargetWidth = screenWidth > 0 ? screenWidth : DefaultViewportWidth",
        "var screenTargetHeight = screenHeight > 0 ? screenHeight : DefaultViewportHeight",
        "var width = layoutWidth > 0 ? layoutWidth : screenTargetWidth",
        "var height = layoutHeight > 0 ? layoutHeight : screenTargetHeight",
        "var screenShortSide = Mathf.Min(screenTargetWidth, screenTargetHeight)",
        "var screenLongSide = Mathf.Max(screenTargetWidth, screenTargetHeight)",
        "screenShortSide <= MobileMaxShortSide && screenLongSide <= MobileMaxLongSide",
        "screenShortSide <= TabletMaxShortSide && screenLongSide <= TabletMaxLongSide",
        "LoginLogoWidth",
        "LoginCardWidth",
        "LoginButtonFontSize",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "private RuntimeUiLayoutProfile CurrentLayoutProfile()",
        "internal int LayoutViewportWidth",
        "internal int LayoutViewportHeight",
        "_root.resolvedStyle.width",
        "_root.resolvedStyle.height",
        "return RuntimeUiLayoutProfile.FromScreen(_forcedLayoutProfile, Screen.width, Screen.height, LayoutViewportWidth, LayoutViewportHeight);",
        "settings.scaleMode = PanelScaleMode.ScaleWithScreenSize;",
        "settings.referenceResolution = new Vector2Int(1200, 800);",
        "settings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;",
        "var layout = CurrentLayoutProfile();",
        "var width = layout.Width;",
        "var profile = layout.Name;",
        "var mobile = layout.IsMobile;",
        "var tablet = layout.IsTablet;",
        "RuntimeLoginResponsiveLayout.Apply(",
        "RuntimeCharacterHallResponsiveLayout.Apply(",
    )
    require(
        "client/Unity/Assets/Resources/LGORuntimePanelSettings.asset",
        "m_Name: LGORuntimePanelSettings",
        "themeUss: {fileID: -4733365628477956816, guid: 7bd04c36bc73e4bb09ab497d57011628, type: 3}",
        "m_ScaleMode: 1",
        "m_ReferenceResolution: {x: 1200, y: 800}",
        "m_ScreenMatchMode: 0",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/VisualRuntimeEvidenceRunner.cs",
        "uiViewportWidth = _controller != null ? _controller.LayoutViewportWidth : Screen.width",
        "uiViewportHeight = _controller != null ? _controller.LayoutViewportHeight : Screen.height",
        "public int uiViewportWidth;",
        "public int uiViewportHeight;",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeCharacterHallResponsiveLayout.cs",
        "var height = layout.Height;",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeLoginResponsiveLayout.cs",
        "loginLogo.style.width = layout.LoginLogoWidth;",
        "loginButton.style.fontSize = layout.LoginButtonFontSize;",
    )
    require(
        "docs/design/RUNTIME-UI-RESPONSIVE-LAYOUT-HELPER-REVIEW-v1.0.md",
        "LGO_RUNTIME_UI_RESPONSIVE_LAYOUT_HELPER_REVIEW_READY",
        "`RuntimeUiLayoutProfile` now owns",
        "The controller still owns applying those values",
        "LGO-RUNTIME-UI-RESPONSIVE-CONSTANTS-AUDIT-v1.0",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-RESPONSIVE-LAYOUT-HELPER-REVIEW-v1.0.md",
        "LGO_RUNTIME_UI_RESPONSIVE_LAYOUT_HELPER_REVIEW_READY",
        "Added `RuntimeUiLayoutProfile`",
        "No gameplay change",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_responsive_layout_helper_review",
        "validate_lgo_runtime_ui_responsive_layout_helper_review.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-RESPONSIVE-LAYOUT-HELPER-REVIEW-v1.0",
        "LGO_RUNTIME_UI_RESPONSIVE_LAYOUT_HELPER_REVIEW_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-RESPONSIVE-LAYOUT-HELPER-REVIEW v1.0",
        "LGO_RUNTIME_UI_RESPONSIVE_LAYOUT_HELPER_REVIEW_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI RESPONSIVE LAYOUT HELPER REVIEW VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_RESPONSIVE_LAYOUT_HELPER_REVIEW_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
