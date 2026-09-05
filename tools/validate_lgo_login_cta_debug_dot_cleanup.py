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
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "LGO Login CTA Lightweight Top Ornament v1",
        "LGO Login CTA Lightweight Bottom Ornament v1",
        "NewLoginOrnamentRule",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "row.style.width = Length.Percent(86);",
        "row.Add(NewLoginOrnamentLine(RuntimeArtCatalog.Gold));",
    )
    source = read("client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs")
    if "Diamond" in source:
        ERRORS.append("login ornament still contains debug-like Diamond marker")
    require(
        "docs/tasks/LGO-LOGIN-CTA-DEBUG-DOT-CLEANUP-PASS-v1.0.md",
        "LGO_LOGIN_CTA_DEBUG_DOT_CLEANUP_READY",
        "No VISUAL_RUNTIME_PASS claim",
        "No new runtime image import",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "login_cta_debug_dot_cleanup",
        "validate_lgo_login_cta_debug_dot_cleanup.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-LOGIN-CTA-DEBUG-DOT-CLEANUP-PASS-v1.0",
        "LGO_LOGIN_CTA_DEBUG_DOT_CLEANUP_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-LOGIN-CTA-DEBUG-DOT-CLEANUP-PASS v1.0",
        "LGO_LOGIN_CTA_DEBUG_DOT_CLEANUP_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO LOGIN CTA DEBUG DOT CLEANUP VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_LOGIN_CTA_DEBUG_DOT_CLEANUP_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
