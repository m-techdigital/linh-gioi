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
        "AGENTS.md",
        "Do not run validation gates in parallel when they share mutable outputs",
        "build/visual-evidence/**",
    )
    require(
        "docs/execution/CODEX-CONTINUOUS-WORKFLOW.md",
        "Do not parallelize gates that clean or rewrite the same output roots",
        "./tools/lgo_visual_runtime_review.sh",
        "./tools/lgo_playable_closure_check.sh --source-only",
    )
    require(
        "docs/execution/LGO-EVIDENCE-GATE-SEQUENTIAL-RUN-POLICY-v1.0.md",
        "LGO_EVIDENCE_GATE_SEQUENTIAL_RUN_POLICY_READY",
        "Run these commands sequentially",
        "transient missing files",
    )
    require(
        "docs/tasks/LGO-EVIDENCE-GATE-SEQUENTIAL-RUN-POLICY-v1.0.md",
        "LGO_EVIDENCE_GATE_SEQUENTIAL_RUN_POLICY_READY",
        "LGO-POST-LOGIN-RUNTIME-UI-REUSE-CLEANUP-v1.0",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-POST-LOGIN-RUNTIME-UI-REUSE-CLEANUP-v1.0",
        "LGO_EVIDENCE_GATE_SEQUENTIAL_RUN_POLICY_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-EVIDENCE-GATE-SEQUENTIAL-RUN-POLICY v1.0",
        "LGO_EVIDENCE_GATE_SEQUENTIAL_RUN_POLICY_READY",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "evidence_gate_sequential_run_policy",
        "validate_lgo_evidence_gate_sequential_run_policy.py",
    )
    check_frozen()
    if ERRORS:
        print("LGO EVIDENCE GATE SEQUENTIAL RUN POLICY VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_EVIDENCE_GATE_SEQUENTIAL_RUN_POLICY_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
