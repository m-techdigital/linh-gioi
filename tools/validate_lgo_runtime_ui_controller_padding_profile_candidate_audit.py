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


def reject(rel: str, *markers: str) -> None:
    text = read(rel)
    for marker in markers:
        if marker in text:
            ERRORS.append(f"{rel} still contains rejected marker: {marker}")


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
        "internal int AuthPanelPaddingBottom => 8;",
        "internal RuntimeUiDensityProfile CharacterHallDensity => RuntimeUiDensityProfile.CharacterHall(this);",
        "internal int PositionChipPaddingHorizontal => 10;",
        "internal int LocalCombatPanelPaddingHorizontal => 12;",
        "internal int SettingsPanelPaddingHorizontal => 14;",
        "internal int WorldGuidanceCardPaddingHorizontal => 8;",
        "internal int DialogueProgressPaddingHorizontal => 10;",
        "internal int StatusPaddingVertical => 6;",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "private RuntimeUiLayoutProfile CurrentLayoutProfile()",
        "var initialLayout = CurrentLayoutProfile();",
        "RuntimeUiSkin.ApplyPadding(_authPanel, 0, 0, layout.AuthPanelPaddingTop, layout.AuthPanelPaddingBottom);",
        "RuntimeUiSkin.ApplyPadding(_loginCard, layout.LoginCardPadding, layout.LoginCardPadding, layout.LoginCardPaddingTop, layout.LoginCardPaddingBottom);",
        "RuntimeUiSkin.ApplyPadding(serverRow, layout.LoginServerRowPaddingHorizontal, layout.LoginServerRowPaddingHorizontal, layout.LoginServerRowPaddingVertical, layout.LoginServerRowPaddingVertical);",
        "RuntimeUiSkin.ApplyPadding(_lobbyPanel, layout.LobbyPanelPaddingHorizontal, layout.LobbyPanelPaddingHorizontal, layout.LobbyPanelPaddingTop, layout.LobbyPanelPaddingBottom);",
        "ApplyCharacterHallListDensity(_characterList, layout);",
        "ApplyEmptyCharacterCardDensity(_emptyCharacterCard, layout.CharacterHallDensity);",
        "RuntimeUiSkin.ApplyPadding(_createPanel, layout.CreatePanelPaddingHorizontal, layout.CreatePanelPaddingHorizontal, layout.CreatePanelPaddingTop, layout.CreatePanelPaddingBottom);",
        "RuntimeUiSkin.ApplyPadding(_position, layout.PositionChipPaddingHorizontal, layout.PositionChipPaddingHorizontal, layout.PositionChipPaddingVertical, layout.PositionChipPaddingVertical);",
        "RuntimeUiSkin.ApplyPadding(_sessionMenuPanel, layout.SessionMenuPaddingHorizontal, layout.SessionMenuPaddingHorizontal, layout.SessionMenuPaddingTop, layout.SessionMenuPaddingBottom);",
        "RuntimeUiSkin.ApplyPadding(_localCombatPanel, layout.LocalCombatPanelPaddingHorizontal, layout.LocalCombatPanelPaddingHorizontal, layout.LocalCombatPanelPaddingVertical, layout.LocalCombatPanelPaddingVertical);",
        "RuntimeUiSkin.ApplyPadding(_settingsPanel, layout.SettingsPanelPaddingHorizontal, layout.SettingsPanelPaddingHorizontal, layout.SettingsPanelPaddingTop, layout.SettingsPanelPaddingBottom);",
        "RuntimeUiSkin.ApplyPadding(_worldGuidanceCard, layout.WorldGuidanceCardPaddingHorizontal, layout.WorldGuidanceCardPaddingHorizontal, layout.WorldGuidanceCardPaddingVertical, layout.WorldGuidanceCardPaddingVertical);",
        "RuntimeUiSkin.ApplyPadding(_dialogueProgress, layout.DialogueProgressPaddingHorizontal, layout.DialogueProgressPaddingHorizontal, layout.DialogueProgressPaddingVertical, layout.DialogueProgressPaddingVertical);",
        "RuntimeUiSkin.ApplyPadding(_status, layout.StatusPaddingHorizontal(worldVisible), layout.StatusPaddingHorizontal(worldVisible), layout.StatusPaddingVertical, layout.StatusPaddingVertical);",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "_root.style.paddingLeft = 28;",
        "_authPanel.style.paddingBottom = 8;",
        "_loginCard.style.paddingLeft = 28;",
        "serverRow.style.paddingLeft = 22;",
        "_lobbyPanel.style.paddingLeft = 18;",
        "_characterList.style.paddingLeft = 14;",
        "_createPanel.style.paddingLeft = 16;",
        "_position.style.paddingLeft = 10;",
        "_sessionMenuPanel.style.paddingLeft = 22;",
        "_localCombatPanel.style.paddingLeft = 12;",
        "_settingsPanel.style.paddingLeft = 14;",
        "emptyCard.style.paddingLeft = 14;",
        "_worldGuidanceCard.style.paddingTop = layout.WorldGuidanceCardPaddingVertical;",
        "_dialogueProgress.style.paddingTop = layout.DialogueProgressPaddingVertical;",
        "_status.style.paddingLeft = layout.StatusPaddingHorizontal(worldVisible);",
    )
    require(
        "docs/design/RUNTIME-UI-CONTROLLER-PADDING-PROFILE-CANDIDATE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_CONTROLLER_PADDING_PROFILE_CANDIDATE_READY",
        "RuntimeUiLayoutProfile",
        "Single-edge alignment",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-CONTROLLER-PADDING-PROFILE-CANDIDATE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_CONTROLLER_PADDING_PROFILE_CANDIDATE_READY",
        "No gameplay",
        "LGO-RUNTIME-UI-CONTROLLER-PADDING-PROFILE-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_controller_padding_profile_candidate_audit",
        "validate_lgo_runtime_ui_controller_padding_profile_candidate_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-CONTROLLER-PADDING-PROFILE-CANDIDATE-AUDIT-v1.0",
        "LGO_RUNTIME_UI_CONTROLLER_PADDING_PROFILE_CANDIDATE_READY",
        "LGO-RUNTIME-UI-CONTROLLER-PADDING-PROFILE-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-CONTROLLER-PADDING-PROFILE-CANDIDATE-AUDIT v1.0",
        "LGO_RUNTIME_UI_CONTROLLER_PADDING_PROFILE_CANDIDATE_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI CONTROLLER PADDING PROFILE CANDIDATE AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_CONTROLLER_PADDING_PROFILE_CANDIDATE_AUDIT_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
