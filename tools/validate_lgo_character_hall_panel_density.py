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
        ["git", "--no-pager", "diff", "--name-only", "--", "protocol", "gamedata/schemas", "docs/adr", "client/Unity/Assets/Game/UI/design-tokens.json"],
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
        "LGO Character Hall Main Selection Grid V3B",
        "LGO Character Hall Selected Cultivator Card V3B",
        "LGO Character Hall V3B Cultivator Portrait",
        "LGO Character Hall Create Cultivator Panel V3B",
        "_lobbyPanel = NewCharacterHallPanel(layout);",
        "RuntimeUiSkin.ApplyCharacterListFrame(_characterList);",
        "portrait.style.width = 92",
        "RuntimeUiSkin.ApplyCharacterPortraitFrame(portrait);",
        "Mathf.Min(width - 40f, 780f)",
        "tablet ? 790 : 800",
        "tablet ? 334 : 350",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "NewCharacterHallPanel(RuntimeUiLayoutProfile layout)",
        "LGO Character Hall V3B Composition Panel",
        "RuntimeUiSkin.ApplyCharacterHallPanelFrame(panel);",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs",
        "ApplyCharacterHallPanelFrame(VisualElement panel)",
        "new Color(0.005f, 0.025f, 0.055f, 0.82f)",
        "ApplyCharacterListFrame(VisualElement list)",
        "new Color(0.0f, 0.018f, 0.045f, 0.70f)",
    )
    require(
        "docs/tasks/LGO-CHARACTER-HALL-PANEL-DENSITY-PASS-v1.0.md",
        "LGO_CHARACTER_HALL_PANEL_DENSITY_READY",
        "No account",
        "VISUAL_RUNTIME_PASS",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "character_hall_panel_density",
        "validate_lgo_character_hall_panel_density.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-CHARACTER-HALL-PANEL-DENSITY-PASS-v1.0",
        "LGO_CHARACTER_HALL_PANEL_DENSITY_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-CHARACTER-HALL-PANEL-DENSITY-PASS v1.0",
        "LGO_CHARACTER_HALL_PANEL_DENSITY_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO CHARACTER HALL PANEL DENSITY VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(f" - {error}", file=sys.stderr)
        return 1
    print("LGO_CHARACTER_HALL_PANEL_DENSITY_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
