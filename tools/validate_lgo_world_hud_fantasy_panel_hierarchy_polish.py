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
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs",
        "internal static void ApplyWorldHudRootFrame(VisualElement hud)",
        "hud.style.backgroundImage = StyleKeyword.None;",
        "new Color(0.004f, 0.020f, 0.048f, 0.68f)",
        "ApplyWorldHudGroupFrame(VisualElement group, Color accent)",
        "new Color(0.0f, 0.020f, 0.050f, 0.64f)",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "RuntimeUiSkin.ApplyWorldHudRootFrame(hud);",
        "RuntimeUiSkin.ApplyWorldHudGroupFrame(group, accent);",
    )
    require(
        "docs/tasks/LGO-WORLD-HUD-FANTASY-PANEL-HIERARCHY-POLISH-v1.0.md",
        "LGO_WORLD_HUD_FANTASY_PANEL_HIERARCHY_POLISH_READY",
        "No gameplay",
        "No new image assets",
        "No `VISUAL_RUNTIME_PASS` claim",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "world_hud_fantasy_panel_hierarchy_polish",
        "validate_lgo_world_hud_fantasy_panel_hierarchy_polish.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-WORLD-HUD-FANTASY-PANEL-HIERARCHY-POLISH-v1.0",
        "LGO_WORLD_HUD_FANTASY_PANEL_HIERARCHY_POLISH_READY",
        "LGO-WORLD-HUD-FANTASY-PANEL-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-WORLD-HUD-FANTASY-PANEL-HIERARCHY-POLISH v1.0",
        "LGO_WORLD_HUD_FANTASY_PANEL_HIERARCHY_POLISH_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO WORLD HUD FANTASY PANEL HIERARCHY POLISH VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_WORLD_HUD_FANTASY_PANEL_HIERARCHY_POLISH_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
