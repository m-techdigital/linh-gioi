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
        "screenWidth > 0 ? screenWidth : 1280",
        "screenHeight > 0 ? screenHeight : 720",
        'width <= 760 || height <= 520 ? "mobile" : width <= 1100 ? "tablet" : "desktop"',
        "LoginLogoWidth",
        "LoginCardWidth",
        "LoginButtonFontSize",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "var layout = RuntimeUiLayoutProfile.FromScreen(_forcedLayoutProfile, Screen.width, Screen.height);",
        "var width = layout.Width;",
        "var height = layout.Height;",
        "var profile = layout.Name;",
        "var mobile = layout.IsMobile;",
        "var tablet = layout.IsTablet;",
        "var loginLogoWidth = layout.LoginLogoWidth;",
        "var loginButtonFont = layout.LoginButtonFontSize;",
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
