#!/usr/bin/env python3
from __future__ import annotations

import subprocess
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
ERRORS: list[str] = []


def require(path: str, *markers: str) -> None:
    file_path = ROOT / path
    if not file_path.is_file():
        ERRORS.append(f"missing file: {path}")
        return
    text = file_path.read_text(encoding="utf-8", errors="replace")
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
        ERRORS.append("frozen surface changed")


def main() -> int:
    require(
        "client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs",
        "LGO Mobile World Camera Framing v1",
        "CurrentCameraOrthographicSize",
        "width <= 1000 || height <= 600",
        "return 5.45f",
        "return 6.15f",
        "return 7.0f",
    )
    require(
        "docs/tasks/LGO-WORLD-MOBILE-CAMERA-FRAMING-PASS-v1.0.md",
        "LGO_WORLD_MOBILE_CAMERA_FRAMING_READY",
        "No gameplay change",
        "No VISUAL_RUNTIME_PASS claim",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "world_mobile_camera_framing",
        "validate_lgo_world_mobile_camera_framing.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-WORLD-MOBILE-CAMERA-FRAMING-PASS-v1.0",
        "LGO_WORLD_MOBILE_CAMERA_FRAMING_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-WORLD-MOBILE-CAMERA-FRAMING-PASS v1.0",
        "LGO_WORLD_MOBILE_CAMERA_FRAMING_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO WORLD MOBILE CAMERA FRAMING VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(f" - {error}", file=sys.stderr)
        return 1
    print("LGO_WORLD_MOBILE_CAMERA_FRAMING_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
