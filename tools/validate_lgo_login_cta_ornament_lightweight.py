#!/usr/bin/env python3
from __future__ import annotations

import subprocess
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
ERRORS: list[str] = []


def read(path: str) -> str:
    file_path = ROOT / path
    if not file_path.is_file():
        ERRORS.append(f"missing file: {path}")
        return ""
    return file_path.read_text(encoding="utf-8", errors="replace")


def require(path: str, *markers: str) -> None:
    text = read(path)
    for marker in markers:
        if marker not in text:
            ERRORS.append(f"{path} missing marker: {marker}")


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
        ERRORS.append("frozen contract/design-token surface changed")


def main() -> int:
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "LGO Login CTA Lightweight Top Ornament v1",
        "LGO Login CTA Lightweight Bottom Ornament v1",
        "NewLoginOrnamentRule",
        "RuntimeArtCatalog.Spirit",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "NewLoginOrnamentLine",
        "RuntimeArtCatalog.Gold",
    )
    require(
        "docs/tasks/LGO-LOGIN-CTA-ORNAMENT-LIGHTWEIGHT-PASS-v1.0.md",
        "LGO_LOGIN_CTA_ORNAMENT_LIGHTWEIGHT_READY",
        "Avoided V3BA assets",
        "new PNG import",
        "No production art claim",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "login_cta_ornament_lightweight",
        "validate_lgo_login_cta_ornament_lightweight.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-LOGIN-CTA-ORNAMENT-LIGHTWEIGHT-PASS-v1.0",
        "LGO_LOGIN_CTA_ORNAMENT_LIGHTWEIGHT_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-LOGIN-CTA-ORNAMENT-LIGHTWEIGHT-PASS v1.0",
        "LGO_LOGIN_CTA_ORNAMENT_LIGHTWEIGHT_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO LOGIN CTA ORNAMENT LIGHTWEIGHT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(f" - {error}", file=sys.stderr)
        return 1
    print("LGO_LOGIN_CTA_ORNAMENT_LIGHTWEIGHT_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
