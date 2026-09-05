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
        ERRORS.append("frozen contract/design-token surface changed")


def main() -> int:
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "LGO World HUD Density Touch Shell v1",
        "LGO World Objective Touch Priority",
        "LGO World Interaction Touch Hint",
        "LGO World Touch Primary Combat Button",
        "Tu sĩ: ",
        "Bia luyện: ",
        "Tầm: ",
        "Tiến trình: ",
        "Bố cục: desktop / HUD tinh gọn.",
        "Mathf.Clamp(width * 0.28f, 238f, 272f)",
        "Mathf.Clamp(width * 0.31f, 360f, 420f)",
    )
    require(
        "docs/tasks/LGO-WORLD-HUD-DENSITY-AND-MOBILE-TOUCH-PASS-v1.0.md",
        "LGO_WORLD_HUD_DENSITY_AND_MOBILE_TOUCH_READY",
        "No gameplay change",
        "No production art claim",
    )
    require("tools/lgo_playable_closure_check.sh", "validate_lgo_world_hud_density_mobile_touch.py")
    check_frozen()
    if ERRORS:
        print("LGO WORLD HUD DENSITY MOBILE TOUCH VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_WORLD_HUD_DENSITY_MOBILE_TOUCH_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
