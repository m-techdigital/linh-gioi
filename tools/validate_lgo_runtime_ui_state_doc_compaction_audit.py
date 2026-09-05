#!/usr/bin/env python3
"""Validate the NEXT-ACTION quick-resume compaction guard."""

from __future__ import annotations

import subprocess
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
ERRORS: list[str] = []


def read_text(relative: str) -> str:
    path = ROOT / relative
    if not path.exists():
        ERRORS.append(f"Missing required file: {relative}")
        return ""
    return path.read_text(encoding="utf-8")


def require(text: str, needle: str, context: str) -> None:
    if needle not in text:
        ERRORS.append(f"{context} missing marker/text: {needle}")


def check_next_action() -> None:
    text = read_text("docs/execution/NEXT-ACTION.md")
    require(text, "## Quick Resume", "NEXT-ACTION")
    require(text, "Active task: `", "NEXT-ACTION")
    require(text, "Historical marker registry stays in this file", "NEXT-ACTION")
    require(text, "`LGO-EXECUTION-LEDGER-ROLLUP-VIEW-v1.0`", "NEXT-ACTION")
    require(text, "LGO_RUNTIME_UI_STATE_DOC_COMPACTION_AUDIT_READY", "NEXT-ACTION")
    require(text, "LGO_LOGIN_PANEL_VISUAL_BALANCE_READY", "NEXT-ACTION historical registry")
    require(text, "LGO_COMBAT_BUTTON_COOLDOWN_VISUAL_EVIDENCE_REFRESH_READY", "NEXT-ACTION previous marker")


def check_task_doc() -> None:
    text = read_text("docs/tasks/LGO-RUNTIME-UI-STATE-DOC-COMPACTION-AUDIT-v1.0.md")
    require(text, "LGO_RUNTIME_UI_STATE_DOC_COMPACTION_AUDIT_READY", "task doc")
    require(text, "Documentation/tooling only.", "task doc")
    require(text, "Preserve historical marker coverage", "task doc")
    require(text, "Do not change gameplay, UI runtime behavior, assets, protocol, GameData schemas, ADR, or design tokens.", "task doc")
    require(text, "LGO-EXECUTION-LEDGER-ROLLUP-VIEW-v1.0", "task doc")


def check_ledger() -> None:
    text = read_text("docs/execution/TASK-LEDGER.md")
    require(text, "LGO-RUNTIME-UI-STATE-DOC-COMPACTION-AUDIT v1.0", "TASK-LEDGER")
    require(text, "LGO_RUNTIME_UI_STATE_DOC_COMPACTION_AUDIT_READY", "TASK-LEDGER")
    require(text, "LGO-EXECUTION-LEDGER-ROLLUP-VIEW-v1.0", "TASK-LEDGER")


def check_closure_hook() -> None:
    text = read_text("tools/lgo_playable_closure_check.sh")
    require(text, "tools/validate_lgo_runtime_ui_state_doc_compaction_audit.py", "closure py_compile")
    require(text, "runtime_ui_state_doc_compaction_audit", "closure source-only phase")


def check_frozen_surfaces() -> None:
    frozen = [
        "protocol",
        "gamedata/schemas",
        "docs/adr",
        "client/Unity/Assets/Game/UI/design-tokens.json",
    ]
    result = subprocess.run(
        ["git", "--no-pager", "diff", "--name-only", "--", *frozen],
        cwd=ROOT,
        check=False,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
    )
    if result.returncode != 0:
        ERRORS.append(f"git frozen-surface diff failed: {result.stderr.strip()}")
        return
    changed = [line for line in result.stdout.splitlines() if line.strip()]
    if changed:
        ERRORS.append("Frozen surfaces changed: " + ", ".join(changed))


def main() -> int:
    check_next_action()
    check_task_doc()
    check_ledger()
    check_closure_hook()
    check_frozen_surfaces()
    if ERRORS:
        print("LGO RUNTIME UI STATE DOC COMPACTION AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(f"- {error}", file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_STATE_DOC_COMPACTION_AUDIT_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
