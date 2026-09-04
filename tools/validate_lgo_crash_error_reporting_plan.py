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


def check_summary_tool() -> None:
    result = subprocess.run(["python3.12", "tools/lgo_error_report_summary.py"], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if result.returncode != 0:
        ERRORS.append(result.stderr.strip() or "error summary tool failed")
        return
    for marker in ("classification", "status"):
        if marker not in result.stdout:
            ERRORS.append(f"error summary output missing {marker}")


def main() -> int:
    require(
        "docs/tasks/LGO-CRASH-ERROR-REPORTING-PLAN-v1.0.md",
        "LGO_CRASH_REPORTING_PLAN_READY",
        "No production crash-reporting service",
        "No telemetry backend",
    )
    require(
        "docs/execution/LGO-CRASH-ERROR-REPORTING-PLAN-v1.0.md",
        "LGO_CRASH_REPORTING_PLAN_READY",
        "FIX_REQUIRED",
        "UNVERIFIED_ENVIRONMENT",
        "CONTRACT_CHANGE_REQUIRED",
        "HUMAN_REVIEW_REQUIRED",
    )
    require(
        "tools/lgo_error_report_summary.py",
        "FIX_REQUIRED",
        "UNVERIFIED_ENVIRONMENT",
        "CONTRACT_CHANGE_REQUIRED",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "validate_lgo_crash_error_reporting_plan.py",
    )
    check_executable("tools/lgo_error_report_summary.py")
    check_executable("tools/validate_lgo_crash_error_reporting_plan.py")
    check_summary_tool()
    if ERRORS:
        print("LGO CRASH ERROR REPORTING PLAN VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_CRASH_ERROR_REPORTING_PLAN_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
