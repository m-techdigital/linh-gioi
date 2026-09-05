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


def forbid(rel: str, *markers: str) -> None:
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
        "tools/lgo_m4_closure_check.sh",
        "\"build/generated\"",
        "\"build/package\"",
        "\"build/packages\"",
        "\"build/full-source\"",
        "\"build/delta\"",
        "client/Unity/Library",
        "client/Unity/Temp",
        "client/Unity/Logs",
    )
    forbid("tools/lgo_m4_closure_check.sh", '"build",')
    require(
        "docs/tasks/LGO-SOURCE-GATE-EVIDENCE-PRESERVATION-PASS-v1.0.md",
        "LGO_SOURCE_GATE_EVIDENCE_PRESERVATION_READY",
        "build/visual-evidence",
        "No package hygiene weakening",
        "LGO-VISUAL-EVIDENCE-PROFILE-INDEX-PASS-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "source_gate_evidence_preservation",
        "validate_lgo_source_gate_evidence_preservation.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-VISUAL-EVIDENCE-PROFILE-INDEX-PASS-v1.0",
        "LGO_SOURCE_GATE_EVIDENCE_PRESERVATION_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-SOURCE-GATE-EVIDENCE-PRESERVATION-PASS v1.0",
        "LGO_SOURCE_GATE_EVIDENCE_PRESERVATION_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO SOURCE GATE EVIDENCE PRESERVATION VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_SOURCE_GATE_EVIDENCE_PRESERVATION_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
