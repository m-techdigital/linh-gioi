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


def reject(rel: str, *markers: str) -> None:
    text = read(rel)
    for marker in markers:
        if marker in text:
            ERRORS.append(f"{rel} still contains rejected marker: {marker}")


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
        "LGO Player Pose Pulse Placeholder Sprite V3B",
        "LgoVisualAssetRegistryV3B.CooldownReady ?? CombatPlaceholderAssets.CooldownReady",
        "new Vector3(0.62f, 0.62f, 1f)",
        "spriteRenderer.color = new Color(color.r, color.g, color.b, 0.78f);",
        "new Vector3(0.34f, 0.02f, 0.34f)",
    )
    reject(
        "client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs",
        "new Vector3(1.4f, 0.08f, 1.4f)).transform",
    )
    require(
        "docs/tasks/LGO-WORLD-POSE-PULSE-VISUAL-CLEANUP-v1.0.md",
        "LGO_WORLD_POSE_PULSE_VISUAL_CLEANUP_READY",
        "square-looking player pose pulse",
        "No gameplay",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "world_pose_pulse_visual_cleanup",
        "validate_lgo_world_pose_pulse_visual_cleanup.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-WORLD-POSE-PULSE-VISUAL-CLEANUP-v1.0",
        "LGO_WORLD_POSE_PULSE_VISUAL_CLEANUP_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-WORLD-POSE-PULSE-VISUAL-CLEANUP v1.0",
        "LGO_WORLD_POSE_PULSE_VISUAL_CLEANUP_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO WORLD POSE PULSE VISUAL CLEANUP VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_WORLD_POSE_PULSE_VISUAL_CLEANUP_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
