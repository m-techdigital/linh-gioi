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
    expected_report = run_report().strip()
    actual_report = read("docs/art/RUNTIME-ASSET-WATCH-QUEUE.md").strip()
    if expected_report and actual_report != expected_report:
        ERRORS.append("docs/art/RUNTIME-ASSET-WATCH-QUEUE.md is stale; regenerate from report_lgo_runtime_asset_watch_queue.py")
    require(
        "docs/art/RUNTIME-ASSET-WATCH-QUEUE.md",
        "LGO_RUNTIME_ASSET_WATCH_QUEUE_IMPORT_PROFILE_READY",
        "| Priority | Role | Size | Budget | Margin |",
        "| 1 | `world_player_male_cultivator`",
        "Priority is sorted by smallest budget margin first",
    )
    require(
        "docs/tasks/LGO-RUNTIME-ASSET-WATCH-QUEUE-EVIDENCE-REFRESH-v1.0.md",
        "LGO_RUNTIME_ASSET_WATCH_QUEUE_EVIDENCE_REFRESH_READY",
        "LGO-EVIDENCE-GATE-SEQUENTIAL-RUN-POLICY-v1.0",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-EVIDENCE-GATE-SEQUENTIAL-RUN-POLICY-v1.0",
        "LGO_RUNTIME_ASSET_WATCH_QUEUE_EVIDENCE_REFRESH_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-ASSET-WATCH-QUEUE-EVIDENCE-REFRESH v1.0",
        "LGO_RUNTIME_ASSET_WATCH_QUEUE_EVIDENCE_REFRESH_READY",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_asset_watch_queue_evidence_refresh",
        "validate_lgo_runtime_asset_watch_queue_evidence_refresh.py",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME ASSET WATCH QUEUE EVIDENCE REFRESH VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_ASSET_WATCH_QUEUE_EVIDENCE_REFRESH_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
