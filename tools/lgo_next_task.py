#!/usr/bin/env python3
from __future__ import annotations

import csv
import json
import re
import subprocess
import sys
from dataclasses import dataclass
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BACKLOG = ROOT / "docs/execution/LGO-NEXT-50-TASKS-BACKLOG-v1.0.md"
LEDGER = ROOT / "docs/execution/TASK-LEDGER.md"
NEXT_ACTION = ROOT / "docs/execution/NEXT-ACTION.md"
OWNER_STOPPED_ACTIVE_TASKS = {"OUTFIT_BODY_RIG_SOURCE_PROTOTYPE"}
ACTIVE_STATE_EXECUTION_BLOCKERS = ("KRITA_AUTOMATED_REOPEN_EXPORT_BLOCKED",)


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
    return active_next_action_task_from_text(NEXT_ACTION.read_text(encoding="utf-8", errors="replace"))


def section_from_text(text: str, heading_prefix: str) -> str:
    lines = text.splitlines()
    start = None
    for index, line in enumerate(lines):
        if line.startswith(heading_prefix):
            start = index
            break
    if start is None:
        return ""
    out: list[str] = []
    for line in lines[start + 1:]:
        if line.startswith("## "):
            break
        out.append(line)
    return "\n".join(out).strip()


def active_task_state_from_text(text: str) -> dict:
    section = section_from_text(text, "## Active task state")
    if not section:
        return {}
    match = re.search(r"```json\s*(.*?)\s*```", section, re.DOTALL)
    payload = match.group(1) if match else section
    try:
        data = json.loads(payload)
    except json.JSONDecodeError:
        return {}
    return data if isinstance(data, dict) else {}


def active_state_execution_blocker_from_text(text: str) -> str | None:
    active_state = active_task_state_from_text(text)
    blockers = active_state.get("blockers")
    if isinstance(blockers, list):
        known_blocker = next((marker for marker in ACTIVE_STATE_EXECUTION_BLOCKERS if marker in blockers), None)
        if known_blocker:
            return known_blocker
    status = active_state.get("status")
    if isinstance(status, str) and status.startswith(("BLOCKED_", "FIX_REQUIRED", "NEED_OWNER_DECISION")):
        return status
    return None


def execution_blocker_owner_note(blocker: str) -> str:
    if blocker == "BLOCKED_SPINE_TOOLING":
        return (
            "Trial 4.3.26 và Mix and Match evaluation đã chạy; cần provision/kích hoạt Spine Professional 4.3.x. "
            "Sau đó mới tích hợp runtime 4.3 đã kiểm chứng vào client/Unity và lặp lại Player proof chính thức."
        )
    if blocker == "KRITA_AUTOMATED_REOPEN_EXPORT_BLOCKED":
        return (
            "Krita automation đã bị đóng sau các probe có giới hạn; không thử thêm runner/path/signature "
            "hoặc tạo ảnh thay source. Cần host GUI/Scripter điều khiển được hoặc native layered source mới."
        )
    return "Active state đang chặn thực thi; xử lý blocker đã ghi trong NEXT-ACTION trước khi chọn task khác."


def active_next_action_task_from_text(text: str) -> str | None:
    active_state = active_task_state_from_text(text)
    active_task = active_state.get("activeTask")
    if isinstance(active_task, str) and active_task.strip():
        active_task = active_task.strip()
        if active_task in OWNER_STOPPED_ACTIVE_TASKS:
            return "OWNER_STOPPED_PATH_REVIEW_REQUIRED"
        return active_task

    active_lock = section_from_text(text, "## ACTIVE GOAL LOCK")
    active_context = "\n".join(text.splitlines()[:1]) + "\n" + active_lock
    if (
        active_lock
        and "six-pose registered outfit path" in active_context
        and "NEED_OWNER_DECISION" in active_lock
        and "ROUTE_SELECTION_REQUIRED" in active_lock
    ):
        return "SIX_POSE_REGISTERED_OUTFIT_SURFACE_CONTRACT_DECISION"
    if (
        active_lock
        and "six-pose registered outfit path" in active_context
        and "SOURCE_VISUAL_FIX_REQUIRED_LAYER_COVERAGE_COMPLETE" in active_lock
    ):
        return "SIX_POSE_REGISTERED_OUTFIT_SOURCE_VISUAL_POLISH"
    if active_lock and "six-pose registered outfit path" in active_context:
        return "SIX_POSE_REGISTERED_OUTFIT_SOURCE_AUTHORING"
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
    execution_blocker = active_state_execution_blocker_from_text(
        NEXT_ACTION.read_text(encoding="utf-8", errors="replace")
    )
    if execution_blocker:
        print("LGO_NEXT_TASK_ADVISOR_FIX_REQUIRED")
        print(f"blocker={execution_blocker}")
        print(f"owner_note={execution_blocker_owner_note(execution_blocker)}")
        return 0
    active_task = active_next_action_task()
    if active_task == "SIX_POSE_REGISTERED_OUTFIT_SOURCE_AUTHORING":
        print("LGO_NEXT_TASK_ADVISOR_READY")
        print(f"id={active_task}")
        print("purpose=Tiếp tục Task 2 Step 2: author 11 source target Pháp Lv1 cho six-pose registered outfit path")
        print("allowed=docs, tools, external selected source repair directories with DO-NOT-PACK provenance")
        print("forbidden=stopped skeletal/cutout path, flat-panel direct-fit production, per-pixel nudging loop, Player pack before source gates")
        print("closure=repair-layer audit/source-board/provenance evidence updated; runtimePromotionAllowed remains false until visual source gates pass")
        return 0
    if active_task == "SIX_POSE_REGISTERED_OUTFIT_SURFACE_CONTRACT_DECISION":
        print("LGO_NEXT_TASK_ADVISOR_READY")
        print(f"id={active_task}")
        print("purpose=Chốt surface contract route Pháp Lv1 trước khi author thêm asset: SLEEVELESS_PHAP_LV1 hoặc SLEEVED_PHAP_LV1")
        print("allowed=docs, validators, contract evidence; no new image candidates before route/ownership is selected")
        print("forbidden=per-pose mask/pixel polish, stopped skeletal/cutout path, flat-panel direct-fit production, Player pack")
        print("closure=contract validation PASS for the selected route; source-board plan updated from contract")
        return 0
    if active_task == "OWNER_STOPPED_PATH_REVIEW_REQUIRED":
        print("LGO_NEXT_TASK_ADVISOR_OWNER_STOPPED_PATH")
        print(f"id={active_task}")
        print("owner_note=Active state trỏ vào OUTFIT_BODY_RIG_SOURCE_PROTOTYPE đã bị owner dừng; không được chạy Blender/body-card/rig. Khôi phục active task về whole-body six-pose authoring trước khi làm tiếp.")
        return 0
    if active_task == "SIX_POSE_REGISTERED_OUTFIT_POSE_SET_AUTHORING":
        print("LGO_NEXT_TASK_ADVISOR_READY")
        print(f"id={active_task}")
        print("purpose=Author áo Pháp có tay, belt và guard theo sáu pose body nguyên khối; sau mẫu đầu tạo item thứ hai bằng cùng pose template")
        print("allowed=one native layered KRA/ORA source that reopens and exports true-alpha six-pose front/back overlays; shared source-space profile; ImageGen concept/reference only")
        print("forbidden=split-body rig/cards, ImageGen PNG as source, RGB/checkerboard alpha cleanup, rejected-evidence reuse, sleeve-add/capsule, pixel deletion, flat-panel direct-fit, per-item body measurement or runtime offsets")
        print("closure=native source reopen/export verified; item đầu chạy đẹp trong Player trên đủ sáu pose; item thứ hai reuse template không đo lại body; visual review ghi lỗi và thời gian thực")
        return 0
    if active_task == "SIX_POSE_REGISTERED_OUTFIT_SOURCE_VISUAL_POLISH":
        print("LGO_NEXT_TASK_ADVISOR_READY")
        print(f"id={active_task}")
        print("purpose=Polish visual source Pháp Lv1 sau khi layer coverage đã đủ; ưu tiên outer_top run/jump để khớp style idle áo tay trắng")
        print("allowed=docs, tools, external selected source repair directories with DO-NOT-PACK provenance, source-board/mixed-board evidence")
        print("forbidden=stopped skeletal/cutout path, flat-panel direct-fit production, per-pixel nudging loop, Player pack before visual source gates")
        print("closure=source boards A/B and mixed/off-slot boards visually acceptable; repair audit remains failureCount 0; runtimePromotionAllowed still false until pack gate")
        return 0
    candidates = [task for task in tasks if not closure_already_satisfied(task, text) and is_safe_without_owner(task, text)]
    if not candidates:
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
