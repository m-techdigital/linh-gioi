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
        "client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs",
        "LGO Procedural Cultivation Platform Material v1",
        "LGO Procedural Cultivation Platform Texture v1",
        "HashNoise",
        "SmoothBand",
        "platformGlow",
        "innerRing",
        "midRing",
        "outerRing",
        "DistanceToSegment",
    )
    require(
        "docs/tasks/LGO-WORLD-GROUND-VISUAL-QUALITY-PASS-v1.0.md",
        "LGO_WORLD_GROUND_VISUAL_QUALITY_READY",
        "No large image import",
        "No gameplay change",
        "No production art claim",
    )
    require("tools/lgo_playable_closure_check.sh", "validate_lgo_world_ground_visual_quality.py")
    check_frozen()
    if ERRORS:
        print("LGO WORLD GROUND VISUAL QUALITY VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_WORLD_GROUND_VISUAL_QUALITY_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
