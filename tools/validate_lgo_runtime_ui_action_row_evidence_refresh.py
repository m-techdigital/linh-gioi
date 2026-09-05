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
        "docs/tasks/LGO-RUNTIME-UI-ACTION-ROW-EVIDENCE-REFRESH-v1.0.md",
        "LGO_RUNTIME_UI_ACTION_ROW_EVIDENCE_REFRESH_READY",
        "build/visual-evidence/latest/character-select.png",
        "build/visual-evidence/latest/session-menu.png",
        "build/visual-evidence/latest/npc-dialogue.png",
        "build/visual-evidence/latest/target-dummy-state.png",
        "NewActionRow",
        "NewIconStatusRow",
        "VISUAL_RUNTIME_PASS",
        "not claimed",
        "LGO-RUNTIME-UI-RESPONSIVE-STYLE-APPLICATION-AUDIT-v1.0",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-ACTION-ROW-EVIDENCE-REFRESH-v1.0",
        "LGO_RUNTIME_UI_ACTION_ROW_EVIDENCE_REFRESH_READY",
        "LGO-RUNTIME-UI-RESPONSIVE-STYLE-APPLICATION-AUDIT-v1.0",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-ACTION-ROW-EVIDENCE-REFRESH v1.0",
        "LGO_RUNTIME_UI_ACTION_ROW_EVIDENCE_REFRESH_READY",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_action_row_evidence_refresh",
        "validate_lgo_runtime_ui_action_row_evidence_refresh.py",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI ACTION ROW EVIDENCE REFRESH VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_ACTION_ROW_EVIDENCE_REFRESH_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
