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


def reject(rel: str, *markers: str) -> None:
    text = read(rel)
    for marker in markers:
        if marker in text:
            ERRORS.append(f"{rel} contains forbidden marker: {marker}")


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
        "tools/analyze_lgo_visual_runtime_evidence.py",
        "expected_files: tuple[str, ...]",
        "parser.add_argument(\"--expected-file\"",
        "parser.add_argument(\"--expected-width\"",
        "parser.add_argument(\"--expected-height\"",
        "LIKELY_BLANK_OR_FLAT",
        "LIKELY_TRANSPARENT_OR_EMPTY",
        "suspiciously small file size",
    )
    require(
        "tools/run_m5_visual_evidence_review.sh",
        "python3.12 tools/analyze_lgo_visual_runtime_evidence.py",
        "--expected-width 1280",
        "--expected-height 720",
        "--expected-file gate-entry.png",
        "--expected-file first-playable-loop-feedback.png",
    )
    reject(
        "tools/run_m5_visual_evidence_review.sh",
        "'-batchmode'",
    )
    require(
        "docs/execution/LGO-VISUAL-EVIDENCE-BLANK-SCREEN-DETECTION-v1.0.md",
        "LGO_VISUAL_EVIDENCE_BLANK_SCREEN_DETECTION_READY",
        "no longer launches the player with `-batchmode`",
        "likely blank, flat, transparent",
    )
    require(
        "docs/tasks/LGO-VISUAL-EVIDENCE-BLANK-SCREEN-DETECTION-v1.0.md",
        "LGO_VISUAL_EVIDENCE_BLANK_SCREEN_DETECTION_READY",
        "LGO-VISUAL-EVIDENCE-REVIEW-SUMMARY-VI-v1.0",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-VISUAL-EVIDENCE-REVIEW-SUMMARY-VI-v1.0",
        "LGO_VISUAL_EVIDENCE_BLANK_SCREEN_DETECTION_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-VISUAL-EVIDENCE-BLANK-SCREEN-DETECTION v1.0",
        "LGO_VISUAL_EVIDENCE_BLANK_SCREEN_DETECTION_READY",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "visual_evidence_blank_screen_detection",
        "validate_lgo_visual_evidence_blank_screen_detection.py",
    )
    check_frozen()
    if ERRORS:
        print("LGO VISUAL EVIDENCE BLANK SCREEN DETECTION VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_VISUAL_EVIDENCE_BLANK_SCREEN_DETECTION_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
