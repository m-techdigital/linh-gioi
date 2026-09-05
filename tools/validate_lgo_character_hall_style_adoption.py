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
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs",
        "ApplyCharacterHallPanelFrame(VisualElement panel)",
        "ApplyCharacterListFrame(VisualElement list)",
        "ApplyCharacterPreviewFrame(VisualElement preview)",
        "ApplyCharacterCreateFrame(VisualElement panel)",
        "ApplyCharacterPortraitFrame(VisualElement portrait)",
        "ApplyLobbyInputFrame(TextField field)",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "_lobbyPanel = NewCharacterHallPanel(layout);",
        "RuntimeUiSkin.ApplyCharacterListFrame(_characterList);",
        "RuntimeUiSkin.ApplyCharacterPreviewFrame(_selectedPreview);",
        "RuntimeUiSkin.ApplyCharacterPortraitFrame(portrait);",
        "RuntimeUiSkin.ApplyCharacterCreateFrame(_createPanel);",
        "LGO Character Hall Create Cultivator Panel V3B",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "NewCharacterHallPanel(RuntimeUiLayoutProfile layout)",
        "LGO Character Hall V3B Composition Panel",
        "RuntimeUiSkin.ApplyCharacterHallPanelFrame(panel);",
        "RuntimeUiSkin.ApplyLobbyInputFrame(field);",
    )
    require(
        "docs/tasks/LGO-CHARACTER-HALL-STYLE-ADOPTION-PASS-v1.0.md",
        "LGO_CHARACTER_HALL_STYLE_ADOPTION_READY",
        "No account/character semantics change",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "character_hall_style_adoption",
        "validate_lgo_character_hall_style_adoption.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-CHARACTER-HALL-STYLE-ADOPTION-PASS-v1.0",
        "LGO_CHARACTER_HALL_STYLE_ADOPTION_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-CHARACTER-HALL-STYLE-ADOPTION-PASS v1.0",
        "LGO_CHARACTER_HALL_STYLE_ADOPTION_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO CHARACTER HALL STYLE ADOPTION VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_CHARACTER_HALL_STYLE_ADOPTION_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
