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


def require(rel: str, *markers: str) -> str:
    text = read(rel)
    for marker in markers:
        if marker not in text:
            ERRORS.append(f"{rel} missing marker: {marker}")
    return text


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
        "LGO World Hub Visual Readability Cleanup v1",
        "WorldHubPoint",
        "WorldHubScale",
        "WorldHubShadowScale",
        "edge props scale down on narrow profiles",
        "WorldHubScale(1.02f, 0.88f, 0.70f)",
        "WorldHubScale(0.36f, 0.32f, 0.24f)",
    )
    require(
        "docs/tasks/LGO-WORLD-HUB-VISUAL-READABILITY-CLEANUP-PASS-v1.0.md",
        "LGO_WORLD_HUB_VISUAL_READABILITY_CLEANUP_READY",
        "No new runtime art import",
        "No gameplay mechanic change",
        "No VISUAL_RUNTIME_PASS claim",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "world_hub_visual_readability_cleanup",
        "validate_lgo_world_hub_visual_readability_cleanup.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO_WORLD_HUB_VISUAL_READABILITY_CLEANUP_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-WORLD-HUB-VISUAL-READABILITY-CLEANUP-PASS v1.0",
        "LGO_WORLD_HUB_VISUAL_READABILITY_CLEANUP_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO WORLD HUB VISUAL READABILITY CLEANUP VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_WORLD_HUB_VISUAL_READABILITY_CLEANUP_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
