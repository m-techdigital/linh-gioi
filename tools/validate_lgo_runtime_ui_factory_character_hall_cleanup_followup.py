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
            ERRORS.append(f"{rel} contains removed/dead helper marker: {marker}")


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
        "_characterList = NewCharacterListPanel(layout);",
        "RuntimeCharacterHallResponsiveLayout.Apply(",
        "NewCharacterHallStatusLabel",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeCharacterHallResponsiveLayout.cs",
        "RuntimeUiFactory.ApplyCharacterListResponsive(characterList, layout, width);",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "private static void ApplyCharacterHallListDensity",
        "RuntimeUiFactory.ApplyCharacterListDensity(list, layout.CharacterHallDensity);",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "NewCharacterListPanel(RuntimeUiLayoutProfile layout)",
        "ApplyCharacterListResponsive(VisualElement list, RuntimeUiLayoutProfile layout, int viewportWidth)",
        "ApplyCharacterListDensity(list, layout.CharacterHallDensity);",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-FACTORY-CHARACTER-HALL-CLEANUP-FOLLOWUP-v1.0.md",
        "LGO_RUNTIME_UI_FACTORY_CHARACTER_HALL_CLEANUP_FOLLOWUP_READY",
        "No gameplay",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_factory_character_hall_cleanup_followup",
        "validate_lgo_runtime_ui_factory_character_hall_cleanup_followup.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-FACTORY-CHARACTER-HALL-CLEANUP-FOLLOWUP-v1.0",
        "LGO_RUNTIME_UI_FACTORY_CHARACTER_HALL_CLEANUP_FOLLOWUP_READY",
        "LGO-WORLD-HUD-RUNTIME-UI-REUSE-AUDIT-v1.0",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-FACTORY-CHARACTER-HALL-CLEANUP-FOLLOWUP v1.0",
        "LGO_RUNTIME_UI_FACTORY_CHARACTER_HALL_CLEANUP_FOLLOWUP_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI FACTORY CHARACTER HALL CLEANUP FOLLOWUP VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_FACTORY_CHARACTER_HALL_CLEANUP_FOLLOWUP_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
