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
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "ApplyCombatButtonSkin(_localCombatButton, coolingDown ? CombatPlaceholderAssets.CombatButtonCooldownTexture",
        "CombatPlaceholderAssets.CombatButtonNormalTexture, coolingDown);",
        '_localCombatButton.text = coolingDown ? "Hồi chiêu" : "Tấn công thử";',
        "Đang hồi chiêu: bấm vẫn cho phản hồi từ chối hồi chiêu",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "internal static void ApplyCombatButtonSkin(Button button, Texture2D texture, bool coolingDown)",
        "RuntimeUiSpacing.CombatButtonCooldownMinWidth",
        "RuntimeUiSpacing.CombatButtonReadyMinWidth",
        "RuntimeUiSpacing.CombatButtonMinHeight",
        "RuntimeUiSpacing.CombatButtonCooldownFontSize",
        "RuntimeUiSpacing.CombatButtonReadyFontSize",
    )
    require(
        "docs/tasks/LGO-COMBAT-BUTTON-STATE-READABILITY-POLISH-v1.0.md",
        "LGO_COMBAT_BUTTON_STATE_READABILITY_POLISH_READY",
        "No combat mechanic change",
        "LGO-COMBAT-BUTTON-STATE-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "combat_button_state_readability_polish",
        "validate_lgo_combat_button_state_readability_polish.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-COMBAT-BUTTON-STATE-READABILITY-POLISH-v1.0",
        "LGO_COMBAT_BUTTON_STATE_READABILITY_POLISH_READY",
        "LGO-COMBAT-BUTTON-STATE-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-COMBAT-BUTTON-STATE-READABILITY-POLISH v1.0",
        "LGO_COMBAT_BUTTON_STATE_READABILITY_POLISH_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO COMBAT BUTTON STATE READABILITY POLISH VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_COMBAT_BUTTON_STATE_READABILITY_POLISH_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
