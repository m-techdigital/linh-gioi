#!/usr/bin/env python3
from __future__ import annotations

import json
import re
import subprocess
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
PROJECT_STATE = ROOT / "docs/execution/PROJECT-STATE.md"
NEXT_ACTION = ROOT / "docs/execution/NEXT-ACTION.md"
LEDGER_ROLLUP = ROOT / "docs/execution/TASK-LEDGER-ROLLUP.md"
OWNER_STOPPED_ACTIVE_TASKS = {"OUTFIT_BODY_RIG_SOURCE_PROTOTYPE"}


def section_prefix(text: str, heading_prefix: str) -> str:
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


def active_task_state(next_action: str) -> dict:
    active = section_prefix(next_action, "## Active task state")
    if not active:
        return {}
    match = re.search(r"```json\s*(.*?)\s*```", active, re.DOTALL)
    payload = match.group(1) if match else active
    try:
        parsed = json.loads(payload)
    except json.JSONDecodeError:
        return {}
    return parsed if isinstance(parsed, dict) else {}


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


def first_lines(text: str, limit: int, *, show_truncation: bool = True) -> str:
    lines = text.splitlines()
    trimmed = lines[:limit]
    if show_truncation and len(lines) > limit:
        trimmed.append(f"... truncated {len(lines) - limit} lines; open source file only if needed.")
    return "\n".join(trimmed).strip()


def first_existing_section(text: str, headings: tuple[str, ...]) -> str:
    for heading in headings:
        extracted = section(text, heading)
        if extracted:
            return extracted
    return ""


def limited_section(text: str, heading: str, limit: int) -> str:
    extracted = section(text, heading)
    return first_lines(extracted, limit) if extracted else ""


def limited_section_any(text: str, headings: tuple[str, ...], limit: int) -> str:
    extracted = first_existing_section(text, headings)
    return first_lines(extracted, limit) if extracted else ""


def quiet_limited_section(text: str, heading: str, limit: int) -> str:
    extracted = section(text, heading)
    return first_lines(extracted, limit, show_truncation=False) if extracted else ""


def quiet_limited_section_any(text: str, headings: tuple[str, ...], limit: int) -> str:
    extracted = first_existing_section(text, headings)
    return first_lines(extracted, limit, show_truncation=False) if extracted else ""


def active_goal_lock_section(text: str) -> str:
    lines = text.splitlines()
    start = None
    for index, line in enumerate(lines):
        if line.startswith("## ACTIVE GOAL LOCK"):
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


def resume_section(next_action: str) -> str:
    active = active_goal_lock_section(next_action)
    if active:
        lines = [line for line in active.splitlines() if not line.startswith("Next valid work:")]
        return first_lines("\n".join(lines), 12, show_truncation=False)
    return quiet_limited_section_any(next_action, ("## Quick Resume", "## Trạng thái"), 12)


def next_task_section(next_action: str) -> str:
    state = active_task_state(next_action)
    active_task = state.get("activeTask")
    if isinstance(active_task, str) and active_task:
        if active_task in OWNER_STOPPED_ACTIVE_TASKS:
            return "Active task: OWNER_STOPPED_PATH_REVIEW_REQUIRED"
        status = state.get("status")
        return "\n".join(line for line in (f"Active task: {active_task}", f"status={status}" if status else "") if line)
    active = active_goal_lock_section(next_action)
    if active and "Next valid work:" in active:
        lines = [line for line in active.splitlines() if line.startswith("Next valid work:")]
        return "\n".join(lines[:3]).strip()
    return limited_section_any(next_action, ("## Next task", "## Việc tiếp theo"), 6)


def current_blocker_section(next_action: str) -> str:
    state = active_task_state(next_action)
    blockers = state.get("blockers")
    active_task = state.get("activeTask")
    if active_task in OWNER_STOPPED_ACTIVE_TASKS:
        return "Current blocker from active task state: OWNER_STOPPED_PATH. Restore the whole-body six-pose task before any implementation."
    if active_task == "LGO_RIGID_OUTFIT_PILOT_01" and state.get("status") == "FIX_REQUIRED_VISUAL_OUTFIT1":
        return (
            "Current gate: FIX_REQUIRED_VISUAL_OUTFIT1. Re-author/redesign one rigid-native bind source and prove natural "
            "run/jump plus required motion in Unity Player. Do not start outfit #2, mix, class or level work; stop for owner "
            "review only after a genuinely final outfit #1 visual candidate exists."
        )
    if state.get("status") == "NEED_HUMAN_VISUAL_REVIEW":
        if state.get("phase") in {
            "FINAL_OUTFIT1_CANDIDATE_READY_FOR_OWNER_REVIEW",
            "FINAL_OUTFIT1_FAST_COMBO_READY_FOR_OWNER_REVIEW",
            "FINAL_OUTFIT1_V3_READY_FOR_OWNER_REVIEW",
        } and active_task == "LGO_RIGID_OUTFIT_PILOT_01":
            return (
                "Current gate: NEED_HUMAN_VISUAL_REVIEW. Review build/rigid-outfit-pilot/"
                "final-1-combo-review-v3/fast-combo-proof.mp4 and combo-contact-sheet.png. "
                "Stop before outfit #2, mix, class or level work."
            )
        if state.get("phase") == "ARTICULATED_MOTION_CANDIDATE_OWNER_REVIEW" and active_task == "LGO_CHIBI_SIDE_MODULAR_MOTION_PROOF_01":
            return (
                "Current gate from active task state: NEED_HUMAN_VISUAL_REVIEW. Review the true-side chibi Blender MP4, "
                "dense run/front-tuck boards and outfit toggle board; Unity/runtime promotion remains locked."
            )
        if state.get("phase") == "OWNER_CONCEPT_REVIEW" and active_task == "LGO_CHARACTER_CHIBI_MODULAR_CONCEPT_01":
            return (
                "Current gate from active task state: NEED_HUMAN_VISUAL_REVIEW. Review the mini chibi modular concept before "
                "source/motion design or Unity implementation."
            )
        current_evidence = state.get("currentDesignEvidence") or state.get("currentPlayerEvidence")
        stop_condition = state.get("stopCondition")
        if isinstance(current_evidence, str) and current_evidence:
            stop_note = f" Gate: {stop_condition}." if isinstance(stop_condition, str) and stop_condition else ""
            return (
                "Current gate from active task state: NEED_HUMAN_VISUAL_REVIEW. Review "
                f"{current_evidence}.{stop_note} Do not proceed beyond this visual gate."
            )
        return "Current gate from active task state: NEED_HUMAN_VISUAL_REVIEW. Review the paired six-pose character boards before source reconstruction or runtime promotion."
    if state.get("status") == "NEED_OWNER_DECISION":
        if state.get("phase") == "POSE_CONTROL_READY_ART_TRANSFER_BLOCKED":
            return (
                "Current gate from active task state: NEED_OWNER_DECISION. Six-pose geometry is reusable, but art transfer still needs "
                "an artist-controlled redraw or a complete pose-plus-identity conditioning workflow. Do not resume prompt-only generation."
            )
        return "Current gate from active task state: NEED_OWNER_DECISION. Resolve the decision recorded in NEXT-ACTION before implementation."
    if isinstance(blockers, list) and blockers:
        return "Current blocker from active task state: " + ", ".join(str(item) for item in blockers)
    active = active_goal_lock_section(next_action)
    if active and "NEED_OWNER_DECISION" in active and "ROUTE_SELECTION_REQUIRED" in active:
        return "Current blocker is surface contract route decision: ROUTE_SELECTION_REQUIRED. Do not create more image candidates or pack Player until SLEEVELESS_PHAP_LV1 or SLEEVED_PHAP_LV1 is selected and the contract validator passes."
    if active and "SOURCE_VISUAL_FIX_REQUIRED_LAYER_COVERAGE_COMPLETE" in active:
        return "Current blocker is source visual polish: SOURCE_VISUAL_FIX_REQUIRED_LAYER_COVERAGE_COMPLETE. Continue outer_top run/jump visual fixes and regenerate source/mixed boards before any Player pack."
    if active:
        current_gate = state.get("currentGate")
        phase = state.get("phase")
        if isinstance(current_gate, str) and current_gate:
            phase_note = f" in phase {phase}" if isinstance(phase, str) and phase else ""
            return f"No current blocker from active lock; continue {current_gate}{phase_note} unless that gate fails."
        return "No current blocker from active lock; continue the next result named in NEXT-ACTION unless its gate fails."
    return limited_section_until_any(next_action, ("## Current blocker", "## Blocker"), ("Evidence:",), 5)


def limited_section_until_any(text: str, headings: tuple[str, ...], stop_markers: tuple[str, ...], limit: int) -> str:
    extracted = first_existing_section(text, headings)
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


def next_task_advisor_section(next_action: str, generic_advisor=None) -> str:
    state = active_task_state(next_action)
    active_task = state.get("activeTask")
    if isinstance(active_task, str) and active_task:
        rows = ["LGO_NEXT_TASK_ACTIVE_GOAL_LOCK", f"activeTask={active_task}"]
        for key in ("phase", "status", "currentGate"):
            value = state.get(key)
            if isinstance(value, str) and value:
                rows.append(f"{key}={value}")
        return "\n".join(rows)
    return (generic_advisor or advisor)()


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
    print(quiet_limited_section(project_state, "## Continuous workflow status", 6) or first_lines(project_state, 8, show_truncation=False) or "PROJECT_STATE_MISSING")
    print()
    print(resume_section(next_action) or "QUICK_RESUME_MISSING")
    print()
    print(next_task_section(next_action) or "NEXT_TASK_MISSING")
    print()
    print(current_blocker_section(next_action) or "CURRENT_BLOCKER_MISSING")
    print()
    print("## Next Task Advisor")
    print(next_task_advisor_section(next_action) or "ADVISOR_OUTPUT_MISSING")
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
