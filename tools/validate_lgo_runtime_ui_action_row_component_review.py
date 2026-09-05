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
            ERRORS.append(f"{rel} still contains rejected duplicate marker: {marker}")


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
        "internal static VisualElement NewActionRow",
        "return NewActionRow(\"LGO Runtime Action Row\", Justify.FlexStart, 6, 0, buttons);",
        "row.style.justifyContent = justifyContent;",
        "internal static VisualElement NewIconStatusRow",
        "statusColumn.style.marginLeft = RuntimeUiSpacing.StatusLabelPaddingHorizontal;",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "NewActionRow(\"LGO Character Hall Action Row\"",
        "NewActionRow(\"LGO Dialogue Action Row\"",
        "NewActionRow(\"LGO World Action Footer V3B\"",
        "NewActionRow(\"LGO Session Menu Action Row\"",
        "NewActionRow(\"LGO Skill Preview Action Row\"",
        "NewIconStatusRow(\"LGO World Combat Readiness Row V3B\"",
        "NewActionRow(\"LGO Local Combat Action Row\"",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "sessionActions.style.justifyContent = Justify.Center;",
        "sessionActions.style.marginBottom = 12;",
        "combatStatusColumn.style.marginLeft = 10;",
    )
    require(
        "docs/design/RUNTIME-UI-ACTION-ROW-COMPONENT-REVIEW-v1.0.md",
        "LGO_RUNTIME_UI_ACTION_ROW_COMPONENT_REVIEW_READY",
        "NewActionRow",
        "NewIconStatusRow",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-ACTION-ROW-COMPONENT-REVIEW-v1.0.md",
        "LGO_RUNTIME_UI_ACTION_ROW_COMPONENT_REVIEW_READY",
        "No gameplay",
        "LGO-RUNTIME-UI-ACTION-ROW-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_action_row_component_review",
        "validate_lgo_runtime_ui_action_row_component_review.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-ACTION-ROW-COMPONENT-REVIEW-v1.0",
        "LGO_RUNTIME_UI_ACTION_ROW_COMPONENT_REVIEW_READY",
        "LGO-RUNTIME-UI-ACTION-ROW-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-ACTION-ROW-COMPONENT-REVIEW v1.0",
        "LGO_RUNTIME_UI_ACTION_ROW_COMPONENT_REVIEW_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI ACTION ROW COMPONENT REVIEW VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_ACTION_ROW_COMPONENT_REVIEW_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
