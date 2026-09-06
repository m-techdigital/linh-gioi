#!/usr/bin/env python3
from __future__ import annotations

import subprocess
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
ERRORS: list[str] = []


def read(relative: str) -> str:
    path = ROOT / relative
    if not path.exists():
        ERRORS.append(f"Missing required file: {relative}")
        return ""
    return path.read_text(encoding="utf-8", errors="replace")


def require(text: str, needle: str, context: str) -> None:
    if needle not in text:
        ERRORS.append(f"{context} missing marker/text: {needle}")


def check_rollup() -> None:
    text = read("docs/execution/TASK-LEDGER-ROLLUP.md")
    require(text, "LGO_EXECUTION_LEDGER_ROLLUP_VIEW_READY", "TASK-LEDGER-ROLLUP")
    require(text, "Source of truth: `docs/execution/TASK-LEDGER.md` remains append-only.", "TASK-LEDGER-ROLLUP")
    require(text, "Regenerate this file with `python3.12 tools/report_lgo_task_ledger_rollup.py`", "TASK-LEDGER-ROLLUP")
    require(text, "This report does not claim runtime or visual PASS.", "TASK-LEDGER-ROLLUP")
    require(text, "## Recent Tasks", "TASK-LEDGER-ROLLUP recent section")
    require(text, "| Recent | Task ID | Status / Decision | Next allowed step |", "TASK-LEDGER-ROLLUP recent table")
    require(text, "LGO-RUNTIME-QUALITY-NEXT-COMPACT-BATCH-v1.0", "TASK-LEDGER-ROLLUP current next task")


def check_next_action() -> None:
    text = read("docs/execution/NEXT-ACTION.md")
    require(text, "LGO_EXECUTION_LEDGER_ROLLUP_VIEW_READY", "NEXT-ACTION")
    require(text, "TASK-LEDGER-ROLLUP.md", "NEXT-ACTION")
    require(text, "LGO-COMMIT-CADENCE-POLICY-HARDENING-v1.0", "NEXT-ACTION")


def check_task_doc() -> None:
    text = read("docs/tasks/LGO-EXECUTION-LEDGER-ROLLUP-VIEW-v1.0.md")
    require(text, "LGO_EXECUTION_LEDGER_ROLLUP_VIEW_READY", "task doc")
    require(text, "append-only", "task doc")
    require(text, "python3.12 tools/report_lgo_task_ledger_rollup.py --check", "task doc")
    require(text, "LGO-COMMIT-CADENCE-POLICY-HARDENING-v1.0", "task doc")


def check_ledger() -> None:
    text = read("docs/execution/TASK-LEDGER.md")
    require(text, "LGO-EXECUTION-LEDGER-ROLLUP-VIEW v1.0", "TASK-LEDGER")
    require(text, "LGO_EXECUTION_LEDGER_ROLLUP_VIEW_READY", "TASK-LEDGER")


def check_report_fresh() -> None:
    result = subprocess.run(
        ["python3.12", "tools/report_lgo_task_ledger_rollup.py", "--check"],
        cwd=ROOT,
        check=False,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
    )
    if result.returncode != 0:
        ERRORS.append((result.stdout + result.stderr).strip() or "TASK-LEDGER rollup is stale")


def check_closure_hook() -> None:
    text = read("tools/lgo_playable_closure_check.sh")
    require(text, "tools/report_lgo_task_ledger_rollup.py", "closure py_compile")
    require(text, "tools/validate_lgo_execution_ledger_rollup_view.py", "closure py_compile")
    require(text, "execution_ledger_rollup_view", "closure source-only phase")


def check_frozen_surfaces() -> None:
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
    check_rollup()
    check_next_action()
    check_task_doc()
    check_ledger()
    check_report_fresh()
    check_closure_hook()
    check_frozen_surfaces()
    if ERRORS:
        print("LGO EXECUTION LEDGER ROLLUP VIEW VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(f"- {error}", file=sys.stderr)
        return 1
    print("LGO_EXECUTION_LEDGER_ROLLUP_VIEW_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
