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


def require_png(rel: str, min_size: int = 24_000) -> None:
    path = ROOT / rel
    if not path.is_file():
        ERRORS.append(f"missing screenshot: {rel}")
        return
    if path.stat().st_size < min_size:
        ERRORS.append(f"screenshot too small: {rel} size={path.stat().st_size}")


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
    ui = read("client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs")
    for marker in ("LGO Session Menu Focus Cleanup v1", "LGO Session Menu Compact Focus Frame v1"):
        if marker not in ui:
            ERRORS.append(f"client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs missing marker: {marker}")
    if (
        "_worldHud.style.visibility = sessionVisible && compactViewport ? Visibility.Hidden : Visibility.Visible;" not in ui
        and "SetElementVisibility(_worldHud, !(sessionVisible && compactViewport));" not in ui
    ):
        ERRORS.append("client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs missing world HUD compact visibility marker")
    if (
        "_headerActions.style.visibility = sessionVisible && compactViewport ? Visibility.Hidden : Visibility.Visible;" not in ui
        and "SetElementVisibility(_headerActions, !(sessionVisible && compactViewport));" not in ui
    ):
        ERRORS.append("client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs missing header actions compact visibility marker")
    require(
        "docs/tasks/LGO-SESSION-MENU-FOCUS-EVIDENCE-REFRESH-v1.0.md",
        "LGO_SESSION_MENU_FOCUS_EVIDENCE_REFRESH_READY",
        "desktop/session-menu.png",
        "tablet/session-menu.png",
        "mobile/session-menu.png",
        "No VISUAL_RUNTIME_PASS claim",
        "LGO-CHARACTER-HALL-MOBILE-COPY-DENSITY-PASS-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "session_menu_focus_evidence_refresh",
        "validate_lgo_session_menu_focus_evidence_refresh.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-SESSION-MENU-FOCUS-EVIDENCE-REFRESH-v1.0",
        "LGO_SESSION_MENU_FOCUS_EVIDENCE_REFRESH_READY",
        "LGO-CHARACTER-HALL-MOBILE-COPY-DENSITY-PASS-v1.0",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-SESSION-MENU-FOCUS-EVIDENCE-REFRESH v1.0",
        "LGO_SESSION_MENU_FOCUS_EVIDENCE_REFRESH_READY",
    )
    require(
        "build/visual-evidence/profiles/index.md",
        "desktop/session-menu.png",
        "tablet/session-menu.png",
        "mobile/session-menu.png",
    )
    require(
        "build/visual-evidence/profiles/index.json",
        '"desktop"',
        '"tablet"',
        '"mobile"',
        "session-menu.png",
    )
    require_png("build/visual-evidence/profiles/desktop/session-menu.png")
    require_png("build/visual-evidence/profiles/tablet/session-menu.png")
    require_png("build/visual-evidence/profiles/mobile/session-menu.png")
    check_frozen()
    if ERRORS:
        print("LGO SESSION MENU FOCUS EVIDENCE REFRESH VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_SESSION_MENU_FOCUS_EVIDENCE_REFRESH_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
