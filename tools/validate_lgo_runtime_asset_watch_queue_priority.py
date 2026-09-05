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


def run_report() -> str:
    result = subprocess.run(
        ["python3.12", "tools/report_lgo_runtime_asset_watch_queue.py"],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if result.returncode != 0:
        ERRORS.append(result.stderr.strip() or "runtime asset watch queue report failed")
    return result.stdout


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
        "tools/report_lgo_runtime_asset_watch_queue.py",
        "Priority",
        "prioritized_rows",
        "smallest budget margin first",
    )
    require(
        "docs/art/RUNTIME-ASSET-WATCH-QUEUE-PRIORITY.md",
        "LGO_RUNTIME_ASSET_WATCH_QUEUE_PRIORITY_READY",
        "`world_player_male_cultivator`",
        "No composite/reference sheet import or slicing",
    )
    require(
        "docs/tasks/LGO-RUNTIME-ASSET-WATCH-QUEUE-PRIORITIZATION-v1.0.md",
        "LGO_RUNTIME_ASSET_WATCH_QUEUE_PRIORITY_READY",
        "LGO-RUNTIME-ASSET-WATCH-QUEUE-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-ASSET-WATCH-QUEUE-EVIDENCE-REFRESH-v1.0",
        "LGO_RUNTIME_ASSET_WATCH_QUEUE_PRIORITY_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-ASSET-WATCH-QUEUE-PRIORITIZATION v1.0",
        "LGO_RUNTIME_ASSET_WATCH_QUEUE_PRIORITY_READY",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_asset_watch_queue_priority",
        "validate_lgo_runtime_asset_watch_queue_priority.py",
    )
    report = run_report()
    for marker in (
        "| Priority | Role | Size | Budget | Margin |",
        "| 1 | `world_player_male_cultivator`",
        "Priority is sorted by smallest budget margin first",
    ):
        if marker not in report:
            ERRORS.append(f"watch queue priority report missing marker: {marker}")
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME ASSET WATCH QUEUE PRIORITY VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_ASSET_WATCH_QUEUE_PRIORITY_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
