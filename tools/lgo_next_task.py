#!/usr/bin/env python3
from __future__ import annotations

import csv
import re
import subprocess
import sys
from dataclasses import dataclass
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BACKLOG = ROOT / "docs/execution/LGO-NEXT-50-TASKS-BACKLOG-v1.0.md"
LEDGER = ROOT / "docs/execution/TASK-LEDGER.md"


@dataclass(frozen=True)
class Task:
    ident: str
    purpose: str
    allowed: str
    forbidden: str
    validators: str
    runtime: str
    dependency: str
    closure: str


def run_git(*args: str) -> str:
    result = subprocess.run(["git", "--no-pager", *args], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if result.returncode != 0:
        raise RuntimeError(result.stderr.strip() or "git failed")
    return result.stdout


def parse_backlog() -> list[Task]:
    rows: list[Task] = []
    for line in BACKLOG.read_text(encoding="utf-8").splitlines():
        if not line.startswith("| LGO-TASK-"):
            continue
        cells = [cell.strip() for cell in line.strip("|").split("|")]
        if len(cells) != 8:
            continue
        rows.append(Task(*cells))
    return rows


def closed_text() -> str:
    text = LEDGER.read_text(encoding="utf-8", errors="replace")
    reports = "\n".join(path.name for path in ROOT.glob("*REPORT*.md"))
    handoffs = "\n".join(path.name for path in ROOT.glob("HANDOFF-*.md"))
    return "\n".join([text, reports, handoffs])


def dirty_worktree_is_ambiguous() -> bool:
    lines = [line for line in run_git("status", "--short", "--untracked-files=all").splitlines() if line.strip()]
    return len(lines) > 250


def is_safe_without_owner(task: Task, text: str) -> bool:
    unsafe_words = (
        "protocol change request",
        "gamedata combat request",
        "Auth implementation",
        "DB migration foundation",
        "Account/player/character persistence",
        "Progression/training persistence",
        "Inventory",
        "Economy",
        "Friend",
        "Chat",
        "Party",
        "Guild",
        "Admin-prod",
    )
    if any(word.lower() in task.purpose.lower() for word in unsafe_words):
        return False
    if "implementation" in task.purpose.lower() and "combat" not in task.purpose.lower() and "website" not in task.purpose.lower():
        return False
    if "docs" not in task.allowed and "tools" not in task.allowed and "approved paths only" not in task.allowed:
        return False
    return True


def main() -> int:
    if not BACKLOG.is_file() or not LEDGER.is_file():
        print("LGO_NEXT_TASK_SELECTION_FAILED missing backlog or ledger", file=sys.stderr)
        return 1
    tasks = parse_backlog()
    text = closed_text()
    if dirty_worktree_is_ambiguous():
        print("LGO_NEXT_TASK_ADVISOR_DIRTY_WORKTREE_REVIEW_REQUIRED")
        print("Recommended: finish/commit/review current untracked asset/doc inventory before broad new implementation.")
        return 0
    candidates = [task for task in tasks if task.closure not in text and is_safe_without_owner(task, text)]
    if not candidates:
        print("LGO_NEXT_TASK_ADVISOR_NO_SAFE_AUTONOMOUS_TASK")
        return 0
    task = candidates[0]
    print("LGO_NEXT_TASK_ADVISOR_READY")
    print(f"id={task.ident}")
    print(f"purpose={task.purpose}")
    print(f"allowed={task.allowed}")
    print(f"forbidden={task.forbidden}")
    print(f"closure={task.closure}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
