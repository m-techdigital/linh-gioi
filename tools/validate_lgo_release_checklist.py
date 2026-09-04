#!/usr/bin/env python3
from __future__ import annotations

import os
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


def main() -> int:
    require(
        "docs/tasks/LGO-ALPHA-BETA-LIVE-CHECKLIST-v1.0.md",
        "LGO_RELEASE_CHECKLIST_READY",
        "No implementation",
        "No production release",
    )
    require(
        "docs/execution/LGO-ALPHA-BETA-LIVE-CHECKLIST-v1.0.md",
        "LGO_RELEASE_CHECKLIST_READY",
        "Alpha Entry",
        "Beta Entry",
        "Live Entry",
        "pre-alpha development",
        "Do not claim production art",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "validate_lgo_release_checklist.py",
    )
    check_executable("tools/validate_lgo_release_checklist.py")
    if ERRORS:
        print("LGO RELEASE CHECKLIST VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RELEASE_CHECKLIST_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
