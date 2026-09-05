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
            ERRORS.append(f"{rel} contains repeated/removed marker: {marker}")


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
        "internal static Label NewHiddenStatusLabel(string text, Color color)",
        "internal static Label NewHiddenMutedLabel(string text)",
        "var label = NewMutedLabel(text);",
        "label.style.display = DisplayStyle.None;",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        '_layoutProfileLabel = NewHiddenStatusLabel("Bố cục:',
        'var combatNote = NewHiddenMutedLabel("Nhãn nguyên mẫu cục bộ:',
        '_combatVisualState = NewHiddenStatusLabel("Dấu hiệu mục tiêu:',
        '_combatCooldown = NewHiddenStatusLabel("Hồi chiêu:',
        '_combatAuthority = NewHiddenStatusLabel("Mô phỏng cục bộ:',
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "_layoutProfileLabel.style.display = DisplayStyle.None;",
        "combatNote.style.display = DisplayStyle.None;",
        "_combatVisualState.style.display = DisplayStyle.None;",
        "_combatCooldown.style.display = DisplayStyle.None;",
        "_combatAuthority.style.display = DisplayStyle.None;",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-STATUS-COMPOSITION-CLEANUP-v1.0.md",
        "LGO_RUNTIME_UI_STATUS_COMPOSITION_CLEANUP_READY",
        "No gameplay",
        "No `VISUAL_RUNTIME_PASS` claim",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_status_composition_cleanup",
        "validate_lgo_runtime_ui_status_composition_cleanup.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-STATUS-COMPOSITION-CLEANUP-v1.0",
        "LGO_RUNTIME_UI_STATUS_COMPOSITION_CLEANUP_READY",
        "LGO-RUNTIME-UI-STATUS-COMPOSITION-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-STATUS-COMPOSITION-CLEANUP v1.0",
        "LGO_RUNTIME_UI_STATUS_COMPOSITION_CLEANUP_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI STATUS COMPOSITION CLEANUP VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_STATUS_COMPOSITION_CLEANUP_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
