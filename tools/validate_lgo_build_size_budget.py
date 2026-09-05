#!/usr/bin/env python3
from __future__ import annotations

import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
ERRORS: list[str] = []

LIMITS = {
    "client/Unity/Assets": 16 * 1024 * 1024,
    "client/Unity/Assets/Game/Art/Runtime": 8 * 1024 * 1024,
    "client/Unity/Assets/Game/Art/Runtime/V3B": 4 * 1024 * 1024,
}

IGNORED_DIRS = {"Library", "Temp", "Logs", "obj", "__pycache__"}


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


def tree_size(rel: str) -> int:
    path = ROOT / rel
    if not path.exists():
        ERRORS.append(f"missing budget path: {rel}")
        return 0
    total = 0
    for child in path.rglob("*"):
        if any(part in IGNORED_DIRS for part in child.relative_to(path).parts):
            continue
        if child.is_file():
            total += child.stat().st_size
    return total


def check_limits() -> None:
    for rel, limit in LIMITS.items():
        size = tree_size(rel)
        if size > limit:
            ERRORS.append(f"{rel} exceeds source budget: {size} > {limit}")


def check_report_runs() -> None:
    result = subprocess.run(
        ["python3.12", "tools/report_lgo_build_size_budget.py"],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if result.returncode != 0:
        ERRORS.append(result.stderr.strip() or "build size report failed")
        return
    for marker in (
        "LGO_BUILD_SIZE_BUDGET_REPORT_READY",
        "runtime_art_v3b",
        "Runtime Files Over 256KB",
        "repository/tooling weight",
    ):
        if marker not in result.stdout:
            ERRORS.append(f"build size report missing marker: {marker}")


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
        "docs/art/BUILD-SIZE-BUDGET.md",
        "LGO_BUILD_SIZE_BUDGET_READY",
        "runtime payload",
        "reference art",
        "mobile",
    )
    require(
        "docs/tasks/LGO-BUILD-SIZE-BUDGET-AND-CLEANUP-PASS-v1.0.md",
        "LGO_BUILD_SIZE_BUDGET_AND_CLEANUP_READY",
        "No dependency-bearing deletion",
    )
    require("tools/lgo_playable_closure_check.sh", "validate_lgo_build_size_budget.py")
    check_report_runs()
    check_limits()
    check_frozen()
    if ERRORS:
        print("LGO BUILD SIZE BUDGET VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_BUILD_SIZE_BUDGET_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
