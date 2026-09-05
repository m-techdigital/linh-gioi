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


def main() -> int:
    next_action = read(NEXT_ACTION)
    project_state = read(PROJECT_STATE)
    rollup = read(LEDGER_ROLLUP)
    print("LGO_STATE_BRIEF_BEGIN")
    print("owner_note=Đây là bản state ngắn để giảm token: chỉ gồm resume, task tiếp theo, blocker và ledger gần nhất.")
    print()
    print("## Project State")
    print(first_lines(project_state, 18) or "PROJECT_STATE_MISSING")
    print()
    print(limited_section(next_action, "## Quick Resume", 8) or "QUICK_RESUME_MISSING")
    print()
    print(limited_section(next_action, "## Next task", 6) or "NEXT_TASK_MISSING")
    print()
    print(limited_section(next_action, "## Current blocker", 5) or "CURRENT_BLOCKER_MISSING")
    print()
    print("## Next Task Advisor")
    print(advisor() or "ADVISOR_OUTPUT_MISSING")
    print()
    print("## Task Ledger Rollup")
    print(first_lines(rollup, 22) or "TASK_LEDGER_ROLLUP_MISSING")
    print("LGO_STATE_BRIEF_END")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
