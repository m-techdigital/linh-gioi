#!/usr/bin/env python3
from __future__ import annotations

import struct
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


def png_size(rel: str) -> tuple[int, int] | None:
    path = ROOT / rel
    if not path.is_file():
        ERRORS.append(f"missing screenshot: {rel}")
        return None
    data = path.read_bytes()
    if len(data) < 24 or data[:8] != b"\x89PNG\r\n\x1a\n":
        ERRORS.append(f"not a PNG: {rel}")
        return None
    width, height = struct.unpack(">II", data[16:24])
    if width < 1280 or height < 720:
        ERRORS.append(f"{rel} too small for post-login evidence: {width}x{height}")
    if path.stat().st_size < 80_000:
        ERRORS.append(f"{rel} suspiciously small for captured gameplay evidence")
    return width, height


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
    for rel in (
        "build/visual-evidence/latest/character-lobby.png",
        "build/visual-evidence/latest/character-select.png",
        "build/visual-evidence/latest/world-hub.png",
    ):
        png_size(rel)
    require(
        "build/visual-evidence/latest/visual-runtime-evidence-review-vi.md",
        "LGO_VISUAL_EVIDENCE_REVIEW_SUMMARY_VI_READY",
        "character-lobby.png",
        "character-select.png",
        "world-hub.png",
        "Đã chụp đủ evidence",
    )
    require(
        "build/dev-loop/visual-runtime-post-login-runtime-ui-reuse-cleanup.log",
        "LGO_VISUAL_RUNTIME_EVIDENCE_READY",
        "LGO_VISUAL_RUNTIME_PASS_NOT_CLAIMED",
    )
    require(
        "docs/tasks/LGO-POST-LOGIN-RUNTIME-UI-REUSE-EVIDENCE-REFRESH-v1.0.md",
        "LGO_POST_LOGIN_RUNTIME_UI_REUSE_EVIDENCE_REFRESH_READY",
        "No `VISUAL_RUNTIME_PASS` claim",
        "LGO-CHARACTER-HALL-V3B-VISUAL-POLISH-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "post_login_runtime_ui_reuse_evidence_refresh",
        "validate_lgo_post_login_runtime_ui_reuse_evidence_refresh.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-POST-LOGIN-RUNTIME-UI-REUSE-EVIDENCE-REFRESH-v1.0",
        "LGO_POST_LOGIN_RUNTIME_UI_REUSE_EVIDENCE_REFRESH_READY",
        "LGO-CHARACTER-HALL-V3B-VISUAL-POLISH-v1.0",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-POST-LOGIN-RUNTIME-UI-REUSE-EVIDENCE-REFRESH v1.0",
        "LGO_POST_LOGIN_RUNTIME_UI_REUSE_EVIDENCE_REFRESH_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO POST LOGIN RUNTIME UI REUSE EVIDENCE REFRESH VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_POST_LOGIN_RUNTIME_UI_REUSE_EVIDENCE_REFRESH_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
