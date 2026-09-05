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
NEXT_ACTION = ROOT / "docs/execution/NEXT-ACTION.md"


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


def active_next_action_task() -> str | None:
    if not NEXT_ACTION.is_file():
        return None
    text = NEXT_ACTION.read_text(encoding="utf-8", errors="replace")
    match = re.search(r"Active task:\s*`([^`]+)`", text)
    if match:
        return match.group(1).strip()
    match = re.search(r"## Next task\s*\n+`([^`]+)`", text)
    if match:
        return match.group(1).strip()
    return None


def closure_already_satisfied(task: Task, text: str) -> bool:
    if task.closure in text:
        return True
    if task.ident in {"LGO-TASK-001", "LGO-TASK-002", "LGO-TASK-003", "LGO-TASK-004", "LGO-TASK-005", "LGO-TASK-006", "LGO-TASK-007"}:
        return "M6_COMBAT_FOUNDATION_CLOSED_LOCAL_v0.55.0" in text or "M6_COMBAT_HARDENING_CONTINUATION_CLOSED_LOCAL_v0.56.0" in text
    return False


def is_safe_without_owner(task: Task, text: str) -> bool:
    unsafe_words = (
        "protocol change request",
        "gamedata combat request",
        "Auth",
        "DB",
        "login",
        "register",
        "Token",
        "session",
        "migration",
        "persistence",
        "Backup",
        "restore",
        "Concurrency",
        "audit design",
        "Item",
        "Inventory",
        "Equipment",
        "Rewards",
        "Economy",
        "Profile",
        "Presence",
        "Friend",
        "Chat",
        "Party",
        "Guild",
        "Leaderboard",
        "event board",
        "World event",
        "boss event",
        "event spec",
        "live ops",
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
        "admin-dev",
        "Content validation admin",
        "Public website",
        "website spec",
        "Player portal",
        "portal spec",
        "Auth/DB",
    )
    if any(word.lower() in task.purpose.lower() for word in unsafe_words):
        return False
    if "implementation" in task.purpose.lower() and "combat" not in task.purpose.lower() and "website" not in task.purpose.lower():
        return False
    if "docs" not in task.allowed and "tools" not in task.allowed and "approved paths only" not in task.allowed:
        return False
    return True


def priority(task: Task) -> int:
    preferred = {
        "LGO-TASK-047": 1,
        "LGO-TASK-048": 2,
        "LGO-TASK-049": 3,
        "LGO-TASK-050": 4,
        "LGO-TASK-042": 5,
        "LGO-TASK-045": 6,
    }
    if task.ident in preferred:
        return preferred[task.ident]
    match = re.search(r"LGO-TASK-(\d+)", task.ident)
    return 100 + (int(match.group(1)) if match else 999)


def main() -> int:
    if not BACKLOG.is_file() or not LEDGER.is_file():
        print("LGO_NEXT_TASK_SELECTION_FAILED missing backlog or ledger", file=sys.stderr)
        return 1
    tasks = parse_backlog()
    text = closed_text()
    if dirty_worktree_is_ambiguous():
        print("LGO_NEXT_TASK_ADVISOR_DIRTY_WORKTREE_REVIEW_REQUIRED")
        print("owner_note=Cần rà soát worktree trước khi mở batch lớn vì số file thay đổi đang quá nhiều.")
        return 0
    candidates = [task for task in tasks if not closure_already_satisfied(task, text) and is_safe_without_owner(task, text)]
    if not candidates:
        active_task = active_next_action_task()
        if active_task and active_task != "DONE":
            print("LGO_NEXT_TASK_ADVISOR_READY")
            print(f"id={active_task}")
            print("purpose=Tiếp tục compact runtime/UI/source quality batch từ docs/execution/NEXT-ACTION.md")
            print("allowed=allowed paths from NEXT-ACTION.md and current governance")
            print("forbidden=frozen surfaces and out-of-roadmap production systems")
            print("closure=update TASK-LEDGER/NEXT-ACTION/status.json after validation and evidence")
            return 0
        print("LGO_NEXT_TASK_ADVISOR_NO_SAFE_AUTONOMOUS_TASK")
        print("owner_note=Chưa thấy task tự động an toàn; cần xem lại roadmap hoặc quyết định owner.")
        return 0
    task = sorted(candidates, key=priority)[0]
    print("LGO_NEXT_TASK_ADVISOR_READY")
    print(f"id={task.ident}")
    print(f"purpose={task.purpose}")
    print(f"allowed={task.allowed}")
    print(f"forbidden={task.forbidden}")
    print(f"closure={task.closure}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
