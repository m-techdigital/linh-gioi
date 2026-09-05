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
            ERRORS.append(f"{rel} contains forbidden output isolation marker: {marker}")


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
        "tools/lgo_visual_runtime_review.sh",
        'OUT_DIR="${LGO_VISUAL_RUNTIME_OUT_DIR:-$ROOT/build/visual-evidence/latest}"',
        "cleanup_outputs()",
        "shutil.rmtree(out)",
        "out.mkdir(parents=True, exist_ok=True)",
    )
    require(
        "tools/lgo_visual_runtime_review_profiles.sh",
        'LOG_DIR="$ROOT/build/visual-evidence/profiles"',
        'LGO_VISUAL_RUNTIME_OUT_DIR="$out_dir"',
        "run_profile desktop 1920 1080 build fast",
        "run_profile tablet 1366 1024 skip skip",
        "run_profile mobile 960 540 skip skip",
    )
    require(
        "tools/run_m5_visual_evidence_review.sh",
        'OUT_DIR="$ROOT/build/visual-evidence/m5-latest"',
        "shutil.rmtree(out)",
        "visual-evidence-summary.json",
    )
    reject(
        "tools/run_m5_visual_evidence_review.sh",
        'OUT_DIR="$ROOT/build/visual-evidence"',
    )
    require(
        "docs/execution/LGO-VISUAL-EVIDENCE-OUTPUT-ISOLATION-v1.0.md",
        "LGO_VISUAL_EVIDENCE_OUTPUT_ISOLATION_READY",
        "build/visual-evidence/latest",
        "build/visual-evidence/profiles/<profile>",
        "build/visual-evidence/m5-latest",
        "No visual evidence script should delete the whole `build/visual-evidence` tree",
    )
    require(
        "docs/tasks/LGO-VISUAL-EVIDENCE-OUTPUT-ISOLATION-AUDIT-v1.0.md",
        "LGO_VISUAL_EVIDENCE_OUTPUT_ISOLATION_READY",
        "LGO-QUICK-FULL-GATE-STRATEGY-v1.0",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-QUICK-FULL-GATE-STRATEGY-v1.0",
        "LGO_VISUAL_EVIDENCE_OUTPUT_ISOLATION_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-VISUAL-EVIDENCE-OUTPUT-ISOLATION-AUDIT v1.0",
        "LGO_VISUAL_EVIDENCE_OUTPUT_ISOLATION_READY",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "visual_evidence_output_isolation",
        "validate_lgo_visual_evidence_output_isolation.py",
    )
    check_frozen()
    if ERRORS:
        print("LGO VISUAL EVIDENCE OUTPUT ISOLATION VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_VISUAL_EVIDENCE_OUTPUT_ISOLATION_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
