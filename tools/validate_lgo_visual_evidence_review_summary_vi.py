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
        "tools/analyze_lgo_visual_runtime_evidence.py",
        "write_vietnamese_markdown",
        "visual-runtime-evidence-review-vi.md",
        "LGO_VISUAL_EVIDENCE_REVIEW_SUMMARY_VI_READY",
        "Tóm Tắt Evidence Runtime Hình Ảnh",
        "Các mục vẫn phải tự review bằng mắt",
    )
    require(
        "docs/execution/LGO-VISUAL-EVIDENCE-REVIEW-SUMMARY-VI-v1.0.md",
        "LGO_VISUAL_EVIDENCE_REVIEW_SUMMARY_VI_READY",
        "visual-runtime-evidence-review-vi.md",
        "does not claim `VISUAL_RUNTIME_PASS`",
    )
    require(
        "docs/tasks/LGO-VISUAL-EVIDENCE-REVIEW-SUMMARY-VI-v1.0.md",
        "LGO_VISUAL_EVIDENCE_REVIEW_SUMMARY_VI_READY",
        "LGO-RUNTIME-ASSET-WEIGHT-ACTIONABLE-BUDGET-v1.0",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-ASSET-WEIGHT-ACTIONABLE-BUDGET-v1.0",
        "LGO_VISUAL_EVIDENCE_REVIEW_SUMMARY_VI_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-VISUAL-EVIDENCE-REVIEW-SUMMARY-VI v1.0",
        "LGO_VISUAL_EVIDENCE_REVIEW_SUMMARY_VI_READY",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "visual_evidence_review_summary_vi",
        "validate_lgo_visual_evidence_review_summary_vi.py",
    )
    check_frozen()
    if ERRORS:
        print("LGO VISUAL EVIDENCE REVIEW SUMMARY VI VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_VISUAL_EVIDENCE_REVIEW_SUMMARY_VI_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
