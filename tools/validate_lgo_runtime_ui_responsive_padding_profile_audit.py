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


def require(rel: str, *markers: str) -> str:
    text = read(rel)
    for marker in markers:
        if marker not in text:
            ERRORS.append(f"{rel} missing marker: {marker}")
    return text


def forbid(rel: str, *markers: str) -> None:
    text = read(rel)
    for marker in markers:
        if marker in text:
            ERRORS.append(f"{rel} still contains controller-local responsive padding: {marker}")


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
        "LobbyPanelPaddingHorizontal",
        "LobbyPanelPaddingTop",
        "LobbyPanelPaddingBottom",
        "CharacterListPaddingVertical",
        "EmptyCharacterCardPaddingHorizontal",
        "EmptyCharacterCardPaddingVertical",
        "CreatePanelPaddingHorizontal",
        "CreatePanelPaddingTop",
        "CreatePanelPaddingBottom",
        "WorldHudPaddingHorizontal",
        "WorldHudPaddingVertical",
        "WorldHudDialoguePaddingHorizontal",
        "WorldHudDialoguePaddingVertical",
        "WorldGuidanceCardPaddingVertical",
        "DialoguePanelPaddingHorizontal",
        "DialoguePanelPaddingVertical",
        "DialogueProgressPaddingVertical",
        "StatusPaddingHorizontal",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "layout.LobbyPanelPaddingHorizontal",
        "layout.CharacterListPaddingVertical",
        "layout.EmptyCharacterCardPaddingHorizontal",
        "layout.CreatePanelPaddingHorizontal",
        "layout.WorldHudPaddingHorizontal",
        "layout.WorldHudDialoguePaddingHorizontal",
        "layout.DialoguePanelPaddingHorizontal",
        "layout.StatusPaddingHorizontal(worldVisible)",
        "ApplyTopStatusResponsive(layout, worldVisible, width)",
    )
    forbid(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "RuntimeUiSkin.ApplyPadding(_lobbyPanel, mobile ? 12 : 18",
        "RuntimeUiSkin.ApplyPadding(_createPanel, mobile ? 12 : 16",
        "RuntimeUiSkin.ApplyPadding(_worldHud, mobile ? 8 : 12",
        "RuntimeUiSkin.ApplyPadding(_dialoguePanel, mobile ? 10 : 14",
        "_characterList.style.paddingTop = mobile ? 8 : 12",
        "_status.style.paddingLeft = worldVisible && mobile ? 14 : 18",
        "RuntimeUiLayoutProfile.FromScreen(_forcedLayoutProfile, Screen.width, Screen.height).StatusPaddingHorizontal",
    )
    require(
        "docs/design/RUNTIME-UI-RESPONSIVE-PADDING-PROFILE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_RESPONSIVE_PADDING_PROFILE_AUDIT_READY",
        "`RuntimeUiLayoutProfile` owns profile-specific numeric layout decisions",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-RESPONSIVE-PADDING-PROFILE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_RESPONSIVE_PADDING_PROFILE_AUDIT_READY",
        "No `VISUAL_RUNTIME_PASS` claim",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_responsive_padding_profile_audit",
        "validate_lgo_runtime_ui_responsive_padding_profile_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-RESPONSIVE-PADDING-PROFILE-AUDIT-v1.0",
        "LGO_RUNTIME_UI_RESPONSIVE_PADDING_PROFILE_AUDIT_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-RESPONSIVE-PADDING-PROFILE-AUDIT v1.0",
        "LGO_RUNTIME_UI_RESPONSIVE_PADDING_PROFILE_AUDIT_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI RESPONSIVE PADDING PROFILE AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_RESPONSIVE_PADDING_PROFILE_AUDIT_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
