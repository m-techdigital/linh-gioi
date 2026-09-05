#!/usr/bin/env python3
from __future__ import annotations

import subprocess
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
ERRORS: list[str] = []


def require_file(rel: str) -> str:
    path = ROOT / rel
    if not path.is_file():
        ERRORS.append(f"missing file: {rel}")
        return ""
    return path.read_text(encoding="utf-8", errors="replace")


def require_binary(rel: str) -> None:
    path = ROOT / rel
    if not path.is_file():
        ERRORS.append(f"missing evidence file: {rel}")
    elif path.stat().st_size <= 0:
        ERRORS.append(f"empty evidence file: {rel}")


def require_markers(rel: str, *markers: str) -> None:
    text = require_file(rel)
    for marker in markers:
        if marker not in text:
            ERRORS.append(f"{rel} missing marker: {marker}")


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
    for profile in ("desktop", "tablet", "mobile"):
        require_binary(f"build/visual-evidence/profiles/{profile}/near-gatekeeper-prompt.png")
        require_binary(f"build/visual-evidence/profiles/{profile}/near-training-stone-prompt.png")
        require_binary(f"build/visual-evidence/profiles/{profile}/visual-runtime-evidence-manifest.json")
        require_binary(f"build/visual-evidence/profiles/{profile}/visual-runtime-evidence-heuristics.json")
    require_markers(
        "build/visual-evidence/profiles/index.md",
        "LGO_VISUAL_EVIDENCE_PROFILE_INDEX_READY",
        "near-gatekeeper-prompt.png",
        "near-training-stone-prompt.png",
        "This index maps evidence files only. It does not claim `VISUAL_RUNTIME_PASS`.",
    )
    require_markers(
        "docs/tasks/LGO-NEAR-INTERACTION-EVIDENCE-REFRESH-v1.0.md",
        "LGO_NEAR_INTERACTION_EVIDENCE_REFRESH_READY",
        "near-gatekeeper-prompt.png",
        "near-training-stone-prompt.png",
        "No VISUAL_RUNTIME_PASS claim",
    )
    require_markers(
        "tools/lgo_playable_closure_check.sh",
        "near_interaction_evidence_refresh",
        "validate_lgo_near_interaction_evidence_refresh.py",
    )
    require_markers(
        "docs/execution/NEXT-ACTION.md",
        "LGO-NEAR-INTERACTION-EVIDENCE-REFRESH-v1.0",
        "LGO_NEAR_INTERACTION_EVIDENCE_REFRESH_READY",
    )
    require_markers(
        "docs/execution/TASK-LEDGER.md",
        "LGO-NEAR-INTERACTION-EVIDENCE-REFRESH v1.0",
        "LGO_NEAR_INTERACTION_EVIDENCE_REFRESH_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO NEAR INTERACTION EVIDENCE REFRESH VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_NEAR_INTERACTION_EVIDENCE_REFRESH_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
