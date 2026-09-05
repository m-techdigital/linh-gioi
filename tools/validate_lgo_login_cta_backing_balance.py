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
    source = read("client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs")
    for marker in (
        "LGO Login CTA Backing Balance v1",
        "_loginCard.style.backgroundColor = new Color(0.005f, 0.018f, 0.040f, 0.18f);",
        "_loginCard.style.borderTopLeftRadius = 18;",
        "new Color(0.005f, 0.018f, 0.040f, 0.10f)",
        "new Color(0.005f, 0.018f, 0.040f, 0.14f)",
        "new Color(0.005f, 0.018f, 0.040f, 0.16f)",
        "Mathf.RoundToInt(100f * mobileScale)",
        "tablet ? 128 : 136",
    ):
        if marker not in source:
            ERRORS.append(f"login CTA backing source missing marker: {marker}")
    require(
        "docs/tasks/LGO-LOGIN-CTA-BACKING-BALANCE-PASS-v1.0.md",
        "LGO_LOGIN_CTA_BACKING_BALANCE_READY",
        "No new runtime image import",
        "No VISUAL_RUNTIME_PASS claim",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "login_cta_backing_balance",
        "validate_lgo_login_cta_backing_balance.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-LOGIN-CTA-BACKING-BALANCE-PASS-v1.0",
        "LGO_LOGIN_CTA_BACKING_BALANCE_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-LOGIN-CTA-BACKING-BALANCE-PASS v1.0",
        "LGO_LOGIN_CTA_BACKING_BALANCE_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO LOGIN CTA BACKING BALANCE VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_LOGIN_CTA_BACKING_BALANCE_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
