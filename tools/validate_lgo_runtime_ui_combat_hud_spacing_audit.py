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


def reject(rel: str, *markers: str) -> None:
    text = read(rel)
    for marker in markers:
        if marker in text:
            ERRORS.append(f"{rel} still contains forbidden marker: {marker}")


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
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSpacing.cs",
        "CombatStatusFontSize",
        "CombatRangeStatusFontSize",
        "CombatActionRowMarginTop",
        "CombatActionRowMarginBottom",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "RuntimeUiSpacing.CombatStatusFontSize",
        "RuntimeUiSpacing.CombatRangeStatusFontSize",
        "RuntimeUiSpacing.CombatActionRowMarginTop",
        "RuntimeUiSpacing.CombatActionRowMarginBottom",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        'NewCompactStatusLabel("Bia luyện: chưa vào sân.", RuntimeArtCatalog.Gold, 13);',
        'NewCompactStatusLabel("Tầm: chưa vào sân.", RuntimeArtCatalog.Muted, 12);',
        'NewCompactStatusLabel("Chưa phải chiến đấu thật.", RuntimeArtCatalog.Spirit, 13);',
        'NewActionRow("LGO Local Combat Action Row", Justify.FlexStart, 6, 0, _localCombatButton)',
    )
    require(
        "docs/design/RUNTIME-UI-COMBAT-HUD-SPACING-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_COMBAT_HUD_SPACING_READY",
        "M4PlayableClientController",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-COMBAT-HUD-SPACING-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_COMBAT_HUD_SPACING_READY",
        "LGO-RUNTIME-UI-COMBAT-HUD-SPACING-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_combat_hud_spacing_audit",
        "validate_lgo_runtime_ui_combat_hud_spacing_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-COMBAT-HUD-SPACING-EVIDENCE-REFRESH-v1.0",
        "LGO_RUNTIME_UI_COMBAT_HUD_SPACING_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-COMBAT-HUD-SPACING-AUDIT v1.0",
        "LGO_RUNTIME_UI_COMBAT_HUD_SPACING_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI COMBAT HUD SPACING AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_COMBAT_HUD_SPACING_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
