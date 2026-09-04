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


def check_matrix_output() -> None:
    result = subprocess.run(["python3.12", "tools/lgo_visual_evidence_matrix.py"], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if result.returncode != 0:
        ERRORS.append(result.stderr.strip() or "visual matrix command failed")
        return
    for marker in ("login_gate_entry", "world_hud", "combat_readiness_hud", "combat_placeholder_assets"):
        if marker not in result.stdout:
            ERRORS.append(f"visual matrix output missing {marker}")


def main() -> int:
    require(
        "docs/tasks/LGO-VISUAL-EVIDENCE-MATRIX-v1.0.md",
        "LGO_VISUAL_EVIDENCE_MATRIX_READY",
        "No production art claim",
        "No new image generation",
    )
    require(
        "docs/execution/LGO-VISUAL-EVIDENCE-MATRIX-v1.0.md",
        "LGO_VISUAL_EVIDENCE_MATRIX_READY",
        "LGO_PLAYABLE_VISUAL_EVIDENCE_READY",
        "reference-only mockups",
        "Do not crop/slice boards",
    )
    require(
        "tools/lgo_visual_evidence_matrix.py",
        "LGO_VISUAL_EVIDENCE_MATRIX_READY",
        "combat_readiness_hud",
        "not production art",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "validate_lgo_visual_evidence_matrix.py",
    )
    check_executable("tools/lgo_visual_evidence_matrix.py")
    check_executable("tools/validate_lgo_visual_evidence_matrix.py")
    check_matrix_output()
    if ERRORS:
        print("LGO VISUAL EVIDENCE MATRIX VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_VISUAL_EVIDENCE_MATRIX_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
