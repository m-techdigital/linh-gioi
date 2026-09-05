#!/usr/bin/env python3
from __future__ import annotations

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


def require(rel: str, *markers: str) -> str:
    text = read(rel)
    for marker in markers:
        if marker not in text:
            ERRORS.append(f"{rel} missing marker: {marker}")
    return text


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
        "tools/lgo_continue_dev_loop.sh",
        'GATE_PROFILE="${LGO_DEV_LOOP_GATE_PROFILE:-quick}"',
        "run_source_validation_profile()",
        "LGO_DEV_LOOP_GATE_PROFILE quick",
        "LGO_DEV_LOOP_GATE_PROFILE full",
        "run_logged playable_source_only ./tools/lgo_playable_closure_check.sh --source-only",
        "unsupported LGO_DEV_LOOP_GATE_PROFILE",
    )
    require(
        "tools/lgo_visual_runtime_review.sh",
        'SOURCE_GATE_MODE="${LGO_VISUAL_RUNTIME_SOURCE_GATES:-fast}"',
        'SERVER_BUILD_MODE="${LGO_VISUAL_RUNTIME_SERVER_BUILD:-fast}"',
        'PLAYER_BUILD_MODE="${LGO_VISUAL_RUNTIME_PLAYER_BUILD:-build}"',
        "LGO_VISUAL_RUNTIME_REVIEW_SOURCE_GATES fast",
        "LGO_VISUAL_RUNTIME_REVIEW_SOURCE_GATES full",
    )
    require(
        "docs/execution/LGO-QUICK-FULL-GATE-STRATEGY-v1.0.md",
        "LGO_QUICK_FULL_GATE_STRATEGY_READY",
        "LGO_DEV_LOOP_GATE_PROFILE=quick",
        "LGO_DEV_LOOP_GATE_PROFILE=full",
        "Quick gates may not be used to claim release readiness",
        "Commit only after a coherent feature/tooling/quality batch validates",
    )
    require(
        "docs/tasks/LGO-QUICK-FULL-GATE-STRATEGY-v1.0.md",
        "LGO_QUICK_FULL_GATE_STRATEGY_READY",
        "LGO-VISUAL-EVIDENCE-BLANK-SCREEN-DETECTION-v1.0",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-VISUAL-EVIDENCE-BLANK-SCREEN-DETECTION-v1.0",
        "LGO_QUICK_FULL_GATE_STRATEGY_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-QUICK-FULL-GATE-STRATEGY v1.0",
        "LGO_QUICK_FULL_GATE_STRATEGY_READY",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "quick_full_gate_strategy",
        "validate_lgo_quick_full_gate_strategy.py",
    )
    check_frozen()
    if ERRORS:
        print("LGO QUICK FULL GATE STRATEGY VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_QUICK_FULL_GATE_STRATEGY_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
