#!/usr/bin/env python3
from __future__ import annotations

import os
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


def check_executable(rel: str) -> None:
    path = ROOT / rel
    if not path.is_file():
        ERRORS.append(f"missing executable: {rel}")
    elif not os.access(path, os.X_OK):
        ERRORS.append(f"not executable: {rel}")


def check_frozen_diff() -> None:
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
        return
    changed = [line for line in result.stdout.splitlines() if line.strip()]
    for rel in changed:
        ERRORS.append(f"frozen surface changed: {rel}")


def main() -> int:
    require(
        "docs/execution/CONTINUOUS-DEVELOPMENT-OPERATING-MODE.md",
        "LGO_CONTINUOUS_DEVELOPMENT_MODE_READY_v1.0",
        "Operating Loop",
        "Stop Conditions",
        "Asset Work Policy",
        "Commit Discipline",
    )
    require(
        "docs/execution/05-WHAT-TO-DO-NOW.md",
        "M6_COMBAT_HARDENING_CONTINUATION_CLOSED_LOCAL_v0.56.0",
        "lgo_continuous_cycle.py",
        "RUNTIME-ASSET-SIZE-BUDGET.md",
    )
    require(
        "docs/execution/NEXT-TASK-SELECTION-RULES.md",
        "LGO_NEXT_TASK_SELECTION_RULES_READY_v1.0",
        "Selection Priority",
        "Dirty Worktree Rule",
        "Internet Research Rule",
        "auth/session/DB work",
    )
    require(
        "docs/art/RUNTIME-ASSET-SIZE-BUDGET.md",
        "LGO_RUNTIME_ASSET_SIZE_BUDGET_READY_v1.0",
        "RUNTIME_CANDIDATE_SIZE_BUDGETED",
        "STRUCTURAL_RUNTIME_PLACEHOLDER_V2",
        "PRODUCTION_FINAL_REVIEW_REQUIRED",
        "Do not crop composite sheets into final runtime assets",
    )
    require(
        "tools/lgo_continuous_cycle.py",
        "lgo_playable_closure_check.sh",
        "validate_package_hygiene.py",
        "validate_lgo_art_v3b_candidates.py",
        "DEFERRED_DIRTY_WORKTREE_REVIEW_REQUIRED",
        "lgo_worktree_audit.py",
        "LGO_CONTINUOUS_CYCLE_",
    )
    require(
        "tools/lgo_next_task.py",
        "LGO_NEXT_TASK_ADVISOR_READY",
        "LGO_NEXT_TASK_ADVISOR_DIRTY_WORKTREE_REVIEW_REQUIRED",
        "LGO_NEXT_TASK_ADVISOR_NO_SAFE_AUTONOMOUS_TASK",
        "closure_already_satisfied",
        "M6_COMBAT_FOUNDATION_CLOSED_LOCAL_v0.55.0",
    )
    require(
        "tools/lgo_worktree_audit.py",
        "LGO_WORKTREE_AUDIT_READY",
        "frozen_protocol",
        "unity_runtime_art",
        "Commit Guidance",
    )
    check_executable("tools/lgo_continuous_cycle.py")
    check_executable("tools/lgo_next_task.py")
    check_executable("tools/lgo_worktree_audit.py")
    check_frozen_diff()
    if ERRORS:
        print("LGO CONTINUOUS DEVELOPMENT MODE VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_CONTINUOUS_DEVELOPMENT_MODE_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
