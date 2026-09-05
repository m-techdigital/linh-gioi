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


def forbid(rel: str, *markers: str) -> None:
    text = read(rel)
    for marker in markers:
        if marker in text:
            ERRORS.append(f"{rel} still contains session-menu padding drift marker: {marker}")


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
        "SessionMenuPaddingHorizontal => IsMobile ? 12 : IsTablet ? 16 : 22",
        "SessionMenuPaddingTop => IsMobile ? 10 : IsTablet ? 14 : 18",
        "SessionMenuPaddingBottom => IsMobile ? 10 : IsTablet ? 14 : 20",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "RuntimeUiSkin.ApplyPadding(_sessionMenuPanel, layout.SessionMenuPaddingHorizontal, layout.SessionMenuPaddingHorizontal, layout.SessionMenuPaddingTop, layout.SessionMenuPaddingBottom)",
    )
    forbid(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "RuntimeUiSkin.ApplyPadding(_sessionMenuPanel, mobile ? 12 : tablet ? 16 : 22",
        "_sessionMenuPanel.style.paddingLeft = mobile ? 12 : tablet ? 16 : 22",
    )
    require(
        "docs/design/RUNTIME-UI-SESSION-MENU-PADDING-PROFILE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_SESSION_MENU_PADDING_PROFILE_AUDIT_READY",
        "The session menu is a recurring shell",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-SESSION-MENU-PADDING-PROFILE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_SESSION_MENU_PADDING_PROFILE_AUDIT_READY",
        "No `VISUAL_RUNTIME_PASS` claim",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_session_menu_padding_profile_audit",
        "validate_lgo_runtime_ui_session_menu_padding_profile_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-SESSION-MENU-PADDING-PROFILE-AUDIT-v1.0",
        "LGO_RUNTIME_UI_SESSION_MENU_PADDING_PROFILE_AUDIT_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-SESSION-MENU-PADDING-PROFILE-AUDIT v1.0",
        "LGO_RUNTIME_UI_SESSION_MENU_PADDING_PROFILE_AUDIT_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI SESSION MENU PADDING PROFILE AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_SESSION_MENU_PADDING_PROFILE_AUDIT_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
