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
        "tools/report_lgo_visual_evidence_profile_index.py",
        "LGO_VISUAL_EVIDENCE_PROFILE_INDEX_READY",
        "EXPECTED_SCREENSHOTS",
        "desktop",
        "tablet",
        "mobile",
        "VISUAL_RUNTIME_PASS",
    )
    require(
        "tools/lgo_visual_runtime_review_profiles.sh",
        "report_lgo_visual_evidence_profile_index.py",
        "LGO_VISUAL_PROFILE_INDEX_PHASE",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "visual_evidence_profile_index",
        "validate_lgo_visual_evidence_profile_index.py",
        "report_lgo_visual_evidence_profile_index.py",
    )
    require(
        "docs/tasks/LGO-VISUAL-EVIDENCE-PROFILE-INDEX-PASS-v1.0.md",
        "LGO_VISUAL_EVIDENCE_PROFILE_INDEX_READY",
        "desktop",
        "tablet",
        "mobile",
        "No VISUAL_RUNTIME_PASS claim",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-VISUAL-EVIDENCE-PROFILE-INDEX-PASS-v1.0",
        "LGO_VISUAL_EVIDENCE_PROFILE_INDEX_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-VISUAL-EVIDENCE-PROFILE-INDEX-PASS v1.0",
        "LGO_VISUAL_EVIDENCE_PROFILE_INDEX_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO VISUAL EVIDENCE PROFILE INDEX VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_VISUAL_EVIDENCE_PROFILE_INDEX_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
