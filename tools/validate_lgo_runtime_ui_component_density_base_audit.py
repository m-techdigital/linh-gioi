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
            ERRORS.append(f"{rel} still contains local density marker: {marker}")


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
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiDensityProfile.cs",
        "LGO Runtime UI Component Density Base v1",
        "internal static RuntimeUiDensityProfile CharacterHall(RuntimeUiLayoutProfile layout)",
        "StatusLabelMobileDensityPaddingHorizontal",
        "CharacterListDensityPaddingHorizontal",
        "EmptyCharacterCardMobileDensityPaddingHorizontal",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiLayoutProfile.cs",
        "internal RuntimeUiDensityProfile CharacterHallDensity => RuntimeUiDensityProfile.CharacterHall(this);",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "internal static void ApplyCharacterListDensity(VisualElement list, RuntimeUiDensityProfile density)",
        "internal static void ApplyEmptyCharacterCardDensity(VisualElement card, RuntimeUiDensityProfile density)",
        "internal static Label NewStatusLabel(string text, Color color, RuntimeUiDensityProfile density)",
    )
    controller = require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "NewCharacterHallStatusLabel",
        "ApplyCharacterHallListDensity",
        "layout.CharacterHallDensity",
    )
    if controller.count("ApplyCharacterHallListDensity(_characterList, layout);") < 2:
        ERRORS.append("Character Hall list density should be applied during build and responsive refresh")
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "layout.CharacterListPaddingHorizontal",
        "layout.CharacterListPaddingVertical",
        "layout.EmptyCharacterCardPaddingHorizontal",
        "layout.EmptyCharacterCardPaddingVertical",
    )
    require(
        "docs/design/RUNTIME-UI-COMPONENT-DENSITY-BASE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_COMPONENT_DENSITY_BASE_READY",
        "RuntimeUiDensityProfile",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-COMPONENT-DENSITY-BASE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_COMPONENT_DENSITY_BASE_READY",
        "LGO-RUNTIME-UI-COMPONENT-DENSITY-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-COMPONENT-DENSITY-EVIDENCE-REFRESH-v1.0",
        "LGO_RUNTIME_UI_COMPONENT_DENSITY_BASE_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-COMPONENT-DENSITY-BASE-AUDIT v1.0",
        "LGO_RUNTIME_UI_COMPONENT_DENSITY_BASE_READY",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_component_density_base_audit",
        "validate_lgo_runtime_ui_component_density_base_audit.py",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI COMPONENT DENSITY BASE AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_COMPONENT_DENSITY_BASE_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
