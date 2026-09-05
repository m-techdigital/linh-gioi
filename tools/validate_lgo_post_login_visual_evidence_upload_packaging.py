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


def require(rel: str, *markers: str) -> None:
    text = read(rel)
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
    require(
        "tools/package_lgo_visual_evidence_upload.py",
        "LGO_POST_LOGIN_VISUAL_EVIDENCE_UPLOAD_READY",
        "SCREENSHOTS",
        "near-gatekeeper-prompt.png",
        "near-training-stone-prompt.png",
        "No VISUAL_RUNTIME_PASS claim",
        "No Unity Library/Temp/cache/player binary included",
        "--verify-only",
    )
    require(
        "docs/tasks/LGO-POST-LOGIN-VISUAL-EVIDENCE-UPLOAD-PACKAGING-v1.0.md",
        "LGO_POST_LOGIN_VISUAL_EVIDENCE_UPLOAD_READY",
        "No VISUAL_RUNTIME_PASS claim",
        "build/chatgpt-handoff/",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "post_login_visual_evidence_upload_packaging",
        "validate_lgo_post_login_visual_evidence_upload_packaging.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-POST-LOGIN-VISUAL-EVIDENCE-UPLOAD-PACKAGING-v1.0",
        "LGO_POST_LOGIN_VISUAL_EVIDENCE_UPLOAD_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-POST-LOGIN-VISUAL-EVIDENCE-UPLOAD-PACKAGING v1.0",
        "LGO_POST_LOGIN_VISUAL_EVIDENCE_UPLOAD_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO POST LOGIN VISUAL EVIDENCE UPLOAD PACKAGING VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_POST_LOGIN_VISUAL_EVIDENCE_UPLOAD_PACKAGING_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
