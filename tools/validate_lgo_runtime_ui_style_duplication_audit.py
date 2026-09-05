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
            ERRORS.append(f"{rel} still contains duplicated style marker: {marker}")


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
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs",
        "ApplySettingToggleState(Toggle toggle, bool enabled)",
        "ApplyEmptyCharacterCardFrame(VisualElement card)",
        "ApplyLocalSettingsPanelFrame(VisualElement panel)",
        "ApplyCombatCooldownIconFrame(VisualElement icon)",
        "ApplyCombatCooldownIconState(VisualElement icon, bool coolingDown)",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "RuntimeUiSkin.ApplyLocalSettingsPanelFrame(_settingsPanel);",
        "RuntimeUiSkin.ApplyEmptyCharacterCardFrame(emptyCard);",
        "RuntimeUiSkin.ApplyCombatCooldownIconState(_combatCooldownIcon, coolingDown);",
        "_showPositionToggle = NewLocalSettingToggle",
        "_combatCooldownIcon = NewCombatCooldownIcon();",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "RuntimeUiSkin.ApplySettingToggleState(toggle, evt.newValue);",
        "RuntimeUiSkin.ApplyCombatCooldownIconFrame(icon);",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "emptyCard.style.backgroundColor = new Color(0.01f, 0.04f, 0.10f, 0.76f);",
        "_settingsPanel.style.backgroundColor = RuntimeArtCatalog.SurfaceRaised;",
        "toggle.style.borderLeftColor = evt.newValue ? RuntimeArtCatalog.Spirit : RuntimeArtCatalog.Muted;",
        "_combatCooldownIcon.style.borderTopColor = coolingDown ? RuntimeArtCatalog.Gold : RuntimeArtCatalog.Spirit;",
        "_combatCooldownIcon.style.borderLeftColor = coolingDown ? RuntimeArtCatalog.Danger : RuntimeArtCatalog.Spirit;",
    )
    require(
        "docs/design/RUNTIME-UI-SKIN-USAGE-GUIDE-v1.0.md",
        "ApplyEmptyCharacterCardFrame",
        "ApplyLocalSettingsPanelFrame",
        "ApplyCombatCooldownIconState",
    )
    require(
        "docs/design/RUNTIME-UI-STYLE-DUPLICATION-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_STYLE_DUPLICATION_AUDIT_READY",
        "Centralized In RuntimeUiSkin",
        "Remaining Direct Style Use",
        "Next Refactor Candidates",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-STYLE-DUPLICATION-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_STYLE_DUPLICATION_AUDIT_READY",
        "No gameplay",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_style_duplication_audit",
        "validate_lgo_runtime_ui_style_duplication_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-STYLE-DUPLICATION-AUDIT-v1.0",
        "LGO_RUNTIME_UI_STYLE_DUPLICATION_AUDIT_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-STYLE-DUPLICATION-AUDIT v1.0",
        "LGO_RUNTIME_UI_STYLE_DUPLICATION_AUDIT_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI STYLE DUPLICATION AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_STYLE_DUPLICATION_AUDIT_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
