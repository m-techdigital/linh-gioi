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
            ERRORS.append(f"{rel} still contains marker: {marker}")


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
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "using System;",
        "internal static TextField NewTextField",
        "internal static void ApplyLobbyInputStyle",
        "internal static Button NewPrimaryButton",
        "internal static Button NewCompactPrimaryButton",
        "internal static Button NewQuietButton",
        "internal static Button NewSecondaryButton",
        "internal static Button NewCompactSecondaryButton",
        "internal static Button NewIconButton",
        "internal static Toggle NewLocalSettingToggle",
        "internal static Button NewListButton",
        "internal static VisualElement NewRuntimeIcon",
        "internal static VisualElement NewCombatCooldownIcon",
        "internal static void ApplyCombatPanelSkin",
        "internal static void ApplyV2PanelSkin",
        "RuntimeUiSkin.ApplyBaseButtonFrame(button);",
        "RuntimeUiSkin.ApplyCombatCooldownIconFrame(icon);",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "using static LinhGioi.UI.RuntimeUiFactory;",
        "_loginButton = NewPrimaryButton",
        "_showPositionToggle = NewLocalSettingToggle",
        "_combatCooldownIcon = NewCombatCooldownIcon();",
        "ApplyCombatPanelSkin(_localCombatPanel);",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "private static TextField NewTextField",
        "private static Button NewPrimaryButton",
        "private static Button NewSecondaryButton",
        "private static Toggle NewLocalSettingToggle",
        "private static VisualElement NewCombatCooldownIcon",
        "private static void ApplyCombatPanelSkin",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-BUTTON-FACTORY-ADOPTION-PASS-v1.0.md",
        "LGO_RUNTIME_UI_BUTTON_FACTORY_ADOPTION_READY",
        "Button actions are still passed in explicitly",
        "No gameplay change",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_button_factory_adoption",
        "validate_lgo_runtime_ui_button_factory_adoption.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-BUTTON-FACTORY-ADOPTION-PASS-v1.0",
        "LGO_RUNTIME_UI_BUTTON_FACTORY_ADOPTION_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-BUTTON-FACTORY-ADOPTION-PASS v1.0",
        "LGO_RUNTIME_UI_BUTTON_FACTORY_ADOPTION_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI BUTTON FACTORY ADOPTION VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_BUTTON_FACTORY_ADOPTION_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
