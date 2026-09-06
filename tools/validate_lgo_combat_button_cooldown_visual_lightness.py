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
        "internal static void ApplyCombatButtonSkin(Button button, Texture2D texture, bool coolingDown)",
        "if (coolingDown)",
        "button.style.backgroundImage = StyleKeyword.None;",
        "button.style.backgroundColor = new Color(0.018f, 0.055f, 0.070f, 0.76f);",
        "button.style.color = RuntimeArtCatalog.Muted;",
        "RuntimeUiSkin.ApplyEdgeFrame(",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "RuntimeCombatHudPresentation.ApplyAssetState(",
        "LocalCombatCoolingDown",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeCombatHudPresentation.cs",
        "coolingDown ? CombatPlaceholderAssets.CombatButtonCooldownTexture : CombatPlaceholderAssets.CombatButtonNormalTexture",
        'localCombatButton.text = coolingDown ? "Hồi chiêu" : previewingSkill ? "Thử bia luyện" : "Tấn công thử";',
    )
    require(
        "docs/tasks/LGO-COMBAT-BUTTON-COOLDOWN-VISUAL-LIGHTNESS-PASS-v1.0.md",
        "LGO_COMBAT_BUTTON_COOLDOWN_VISUAL_LIGHTNESS_READY",
        "No combat mechanic",
        "No visual asset import",
        "No `VISUAL_RUNTIME_PASS` claim",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "combat_button_cooldown_visual_lightness",
        "validate_lgo_combat_button_cooldown_visual_lightness.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-COMBAT-BUTTON-COOLDOWN-VISUAL-LIGHTNESS-PASS-v1.0",
        "LGO_COMBAT_BUTTON_COOLDOWN_VISUAL_LIGHTNESS_READY",
        "LGO-COMBAT-BUTTON-COOLDOWN-VISUAL-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-COMBAT-BUTTON-COOLDOWN-VISUAL-LIGHTNESS-PASS v1.0",
        "LGO_COMBAT_BUTTON_COOLDOWN_VISUAL_LIGHTNESS_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO COMBAT BUTTON COOLDOWN VISUAL LIGHTNESS VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_COMBAT_BUTTON_COOLDOWN_VISUAL_LIGHTNESS_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
