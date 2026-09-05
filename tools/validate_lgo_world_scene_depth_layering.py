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
        "client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs",
        "LGO World Scene Depth Layering",
        "LGO Procedural Soft Ground Shadow Texture v1",
        "LGO Procedural Soft Ground Shadow Sprite v1",
        "CreateGroundShadowSprite",
        "GetSoftGroundShadowSprite",
        "LGO Player Grounding Shadow V3B",
        "LGO Gate Keeper Grounding Shadow V3B",
        "LGO Spirit Gate Grounding Shadow V3B",
        "LGO Target Dummy Grounding Shadow V3B",
        "LGO World Cherry Tree Depth Shadow V3B",
        "renderer.color = new Color(0.0f, 0.012f, 0.028f, 0.42f)",
    )
    require(
        "docs/tasks/LGO-WORLD-SCENE-DEPTH-LAYERING-PASS-v1.0.md",
        "LGO_WORLD_SCENE_DEPTH_LAYERING_READY",
        "no new PNG import",
        "No new gameplay mechanic",
        "No production/final art claim",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "world_scene_depth_layering",
        "validate_lgo_world_scene_depth_layering.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-WORLD-SCENE-DEPTH-LAYERING-PASS-v1.0",
        "LGO_WORLD_SCENE_DEPTH_LAYERING_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-WORLD-SCENE-DEPTH-LAYERING-PASS v1.0",
        "LGO_WORLD_SCENE_DEPTH_LAYERING_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO WORLD SCENE DEPTH LAYERING VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(f" - {error}", file=sys.stderr)
        return 1
    print("LGO_WORLD_SCENE_DEPTH_LAYERING_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
