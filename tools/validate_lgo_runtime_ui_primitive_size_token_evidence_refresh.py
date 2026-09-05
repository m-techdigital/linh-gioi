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


def require_evidence(rel: str) -> None:
    path = ROOT / rel
    if not path.is_file():
        ERRORS.append(f"missing evidence: {rel}")
    elif path.stat().st_size <= 1024:
        ERRORS.append(f"evidence too small: {rel}")


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
        "docs/tasks/LGO-RUNTIME-UI-PRIMITIVE-SIZE-TOKEN-EVIDENCE-REFRESH-v1.0.md",
        "LGO_RUNTIME_UI_PRIMITIVE_SIZE_TOKEN_EVIDENCE_REFRESH_READY",
        "No `VISUAL_RUNTIME_PASS` claim",
        "LGO-RUNTIME-UI-PRIMITIVE-STYLE-BOUNDARY-GUIDE-v1.0",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-PRIMITIVE-STYLE-BOUNDARY-GUIDE-v1.0",
        "LGO_RUNTIME_UI_PRIMITIVE_SIZE_TOKEN_EVIDENCE_REFRESH_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-PRIMITIVE-SIZE-TOKEN-EVIDENCE-REFRESH v1.0",
        "LGO_RUNTIME_UI_PRIMITIVE_SIZE_TOKEN_EVIDENCE_REFRESH_READY",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_primitive_size_token_evidence_refresh",
        "validate_lgo_runtime_ui_primitive_size_token_evidence_refresh.py",
    )
    for rel in (
        "build/visual-evidence/latest/login.png",
        "build/visual-evidence/latest/character-select.png",
        "build/visual-evidence/latest/world-hub.png",
        "build/visual-evidence/latest/session-menu.png",
        "build/visual-evidence/latest/target-dummy-state.png",
    ):
        require_evidence(rel)
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI PRIMITIVE SIZE TOKEN EVIDENCE REFRESH VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_PRIMITIVE_SIZE_TOKEN_EVIDENCE_REFRESH_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
