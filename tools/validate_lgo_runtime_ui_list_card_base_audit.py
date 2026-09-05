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
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSpacing.cs",
        "EmptyCharacterHintMarginTop",
        "ListButtonMinWidth",
        "ListButtonMinHeight",
        "ListButtonPaddingLeft",
    )
    factory = require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "NewEmptyCharacterCard(RuntimeUiLayoutProfile layout, Label title, Label hint)",
        "RuntimeUiSkin.ApplyEmptyCharacterCardFrame(card);",
        "hint.style.marginTop = RuntimeUiSpacing.EmptyCharacterHintMarginTop;",
        "var button = NewSecondaryButton(name + \"\\n\" + classId, action);",
        "RuntimeUiSkin.ApplyButtonMetrics(button, RuntimeUiSpacing.ListButtonMinWidth, RuntimeUiSpacing.ListButtonMinHeight);",
        "button.style.paddingLeft = RuntimeUiSpacing.ListButtonPaddingLeft;",
    )
    controller = require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "var emptyCard = NewEmptyCharacterCard(layout, emptyTitle, empty);",
        "_characterList.Add(NewListButton(character.name, \"Kiếm tu sơ nhập\", () => SelectCharacter(captured)));",
    )
    if "RuntimeUiSkin.ApplyEmptyCharacterCardFrame(emptyCard);" in controller:
        ERRORS.append("M4PlayableClientController still owns empty character card frame setup")
    if "empty.style.marginTop = 6;" in controller:
        ERRORS.append("M4PlayableClientController still owns empty character hint margin")
    require(
        "tools/validate_lgo_runtime_ui_style_duplication_audit.py",
        "NewEmptyCharacterCard(RuntimeUiLayoutProfile layout, Label title, Label hint)",
    )
    require(
        "docs/design/RUNTIME-UI-LIST-CARD-BASE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_LIST_CARD_BASE_READY",
        "RuntimeUiFactory.NewEmptyCharacterCard",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-LIST-CARD-BASE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_LIST_CARD_BASE_READY",
        "LGO-RUNTIME-UI-LIST-CARD-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_list_card_base_audit",
        "validate_lgo_runtime_ui_list_card_base_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-LIST-CARD-EVIDENCE-REFRESH-v1.0",
        "LGO_RUNTIME_UI_LIST_CARD_BASE_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-LIST-CARD-BASE-AUDIT v1.0",
        "LGO_RUNTIME_UI_LIST_CARD_BASE_READY",
    )
    _ = factory
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI LIST CARD BASE AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_LIST_CARD_BASE_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
