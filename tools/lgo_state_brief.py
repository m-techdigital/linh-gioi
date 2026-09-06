#!/usr/bin/env python3
from __future__ import annotations

import subprocess
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
PROJECT_STATE = ROOT / "docs/execution/PROJECT-STATE.md"
NEXT_ACTION = ROOT / "docs/execution/NEXT-ACTION.md"
LEDGER_ROLLUP = ROOT / "docs/execution/TASK-LEDGER-ROLLUP.md"


def read(path: Path) -> str:
    return path.read_text(encoding="utf-8", errors="replace") if path.is_file() else ""


def section(text: str, heading: str) -> str:
    lines = text.splitlines()
    start = None
    for index, line in enumerate(lines):
        if line == heading:
            start = index
            break
    if start is None:
        return ""
    out: list[str] = []
    for line in lines[start:]:
        if out and line.startswith("## "):
            break
        out.append(line)
    return "\n".join(out).strip()


def first_lines(text: str, limit: int) -> str:
    lines = text.splitlines()
    trimmed = lines[:limit]
    if len(lines) > limit:
        trimmed.append(f"... truncated {len(lines) - limit} lines; open source file only if needed.")
    return "\n".join(trimmed).strip()


def limited_section(text: str, heading: str, limit: int) -> str:
    extracted = section(text, heading)
    return first_lines(extracted, limit) if extracted else ""


def limited_section_until(text: str, heading: str, stop_markers: tuple[str, ...], limit: int) -> str:
    extracted = section(text, heading)
    if not extracted:
        return ""
    lines: list[str] = []
    for line in extracted.splitlines():
        if lines and line.strip() in stop_markers:
            break
        lines.append(line)
    return first_lines("\n".join(lines), limit)


def advisor() -> str:
    result = subprocess.run(
        ["python3.12", "tools/lgo_next_task.py"],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.STDOUT,
        check=False,
    )
    return result.stdout.strip()


def change_budget() -> str:
    result = subprocess.run(
        ["python3.12", "tools/report_lgo_change_budget.py"],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.STDOUT,
        check=False,
    )
    lines = []
    for line in result.stdout.splitlines():
        if line.startswith(("LGO_CHANGE_BUDGET_STATUS", "LGO_CHANGE_BUDGET_FILES", "LGO_CHANGE_BUDGET_CHANGED_LINES", "LGO_CHANGE_BUDGET_WARN")):
            lines.append(line)
    return "\n".join(lines[:8]).strip()


def main() -> int:
    next_action = read(NEXT_ACTION)
    project_state = read(PROJECT_STATE)
    rollup = read(LEDGER_ROLLUP)
    print("LGO_STATE_BRIEF_BEGIN")
    print("owner_note=Đây là bản state ngắn để giảm token: chỉ gồm resume, task tiếp theo, blocker và ledger gần nhất.")
    print()
    print("## Project State")
    print(limited_section(project_state, "## Continuous workflow status", 6) or first_lines(project_state, 8) or "PROJECT_STATE_MISSING")
    print()
    print(limited_section(next_action, "## Quick Resume", 8) or "QUICK_RESUME_MISSING")
    print()
    print(limited_section(next_action, "## Next task", 6) or "NEXT_TASK_MISSING")
    print()
    print(limited_section_until(next_action, "## Current blocker", ("Evidence:",), 5) or "CURRENT_BLOCKER_MISSING")
    print()
    print("## Next Task Advisor")
    print(advisor() or "ADVISOR_OUTPUT_MISSING")
    print()
    print("## Change Budget")
    print(change_budget() or "CHANGE_BUDGET_UNAVAILABLE")
    print()
    print("## Task Ledger Rollup")
    print(first_lines(rollup, 22) or "TASK_LEDGER_ROLLUP_MISSING")
    print("LGO_STATE_BRIEF_END")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
