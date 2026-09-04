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


def check_frozen() -> None:
    result = subprocess.run(
        ["git", "--no-pager", "diff", "--name-only", "--", "protocol", "gamedata/schemas", "docs/adr", "client/Unity/Assets/Game/UI/design-tokens.json"],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if result.returncode != 0:
        ERRORS.append(result.stderr.strip() or "git frozen diff failed")
    elif result.stdout.strip():
        ERRORS.append("frozen contract/design-token surface changed")


def main() -> int:
    require(
        "docs/tasks/LGO-ANIMATION-DIRECTION-PLAN-v1.0.md",
        "LGO_ANIMATION_DIRECTION_READY",
        "No animation implementation",
        "No production art or production animation claim",
    )
    require(
        "docs/art/ANIMATION-DIRECTION-PLAN.md",
        "LGO_ANIMATION_DIRECTION_READY",
        "Readable first",
        "Training dummy",
        "Button press feedback",
        "Composite/reference boards are not runtime animation sources",
        "Sẵn sàng",
        "does not implement animation",
    )
    require("tools/lgo_playable_closure_check.sh", "validate_lgo_animation_direction.py")
    check_executable("tools/validate_lgo_animation_direction.py")
    check_frozen()
    if ERRORS:
        print("LGO ANIMATION DIRECTION VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_ANIMATION_DIRECTION_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
