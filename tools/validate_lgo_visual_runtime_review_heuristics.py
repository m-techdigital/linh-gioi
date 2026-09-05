#!/usr/bin/env python3
from __future__ import annotations

import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
ERRORS: list[str] = []


def require_file(path: str, markers: list[str]) -> None:
    file_path = ROOT / path
    if not file_path.is_file():
        ERRORS.append(f"missing file: {path}")
        return
    text = file_path.read_text(encoding="utf-8")
    for marker in markers:
        if marker not in text:
            ERRORS.append(f"{path} missing marker: {marker}")


def require_absent(path: str) -> None:
    if (ROOT / path).exists():
        ERRORS.append(f"forbidden path changed/created: {path}")


def main() -> int:
    require_file(
        "tools/analyze_lgo_visual_runtime_evidence.py",
        [
            "LGO_VISUAL_RUNTIME_REVIEW_HEURISTICS_READY",
            "VISUAL_RUNTIME_PASS",
            "EXPECTED_SCREENSHOTS",
            "sample_png_pixels",
            "duplicate screenshot bytes",
        ],
    )
    require_file(
        "tools/lgo_visual_runtime_review.sh",
        [
            "analyze_lgo_visual_runtime_evidence.py",
            "visual-runtime-evidence-heuristics.json",
            "LGO_VISUAL_RUNTIME_PASS_NOT_CLAIMED",
        ],
    )
    require_file(
        "tools/lgo_playable_closure_check.sh",
        [
            "visual_runtime_review_heuristics",
            "validate_lgo_visual_runtime_review_heuristics.py",
            "analyze_lgo_visual_runtime_evidence.py",
        ],
    )
    require_file(
        "docs/tasks/LGO-VISUAL-RUNTIME-REVIEW-HEURISTICS-PASS-v1.0.md",
        [
            "LGO_VISUAL_RUNTIME_REVIEW_HEURISTICS_READY",
            "FIX_REQUIRED",
            "VISUAL_RUNTIME_PASS",
        ],
    )
    require_file(
        "docs/execution/NEXT-ACTION.md",
        [
            "LGO-VISUAL-RUNTIME-REVIEW-HEURISTICS-PASS-v1.0",
            "LGO_VISUAL_RUNTIME_REVIEW_HEURISTICS_READY",
        ],
    )
    require_file(
        "docs/execution/TASK-LEDGER.md",
        [
            "LGO-VISUAL-RUNTIME-REVIEW-HEURISTICS-PASS v1.0",
            "LGO_VISUAL_RUNTIME_REVIEW_HEURISTICS_READY",
        ],
    )
    require_absent("protocol/__pycache__")
    require_absent("gamedata/schemas/__pycache__")

    if ERRORS:
        print("LGO VISUAL RUNTIME REVIEW HEURISTICS VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(f" - {error}", file=sys.stderr)
        return 1
    print("LGO_VISUAL_RUNTIME_REVIEW_HEURISTICS_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
