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


def reject(rel: str, *markers: str) -> None:
    text = read(rel)
    for marker in markers:
        if marker in text:
            ERRORS.append(f"{rel} contains rejected marker: {marker}")


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
        "ApplyCharacterHallListHeading(Label label)",
        "ApplyRadius(panel, 12);",
        "var texture = LgoVisualAssetRegistryV3B.PanelMainDarkGoldTexture;",
        "panel.style.backgroundImage = new StyleBackground(texture);",
        "panel.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;",
        "panel.style.borderTopWidth = 2;",
        "preview.style.backgroundColor = new Color(0.0f, 0.020f, 0.050f, 0.74f);",
        "panel.style.backgroundColor = new Color(0.0f, 0.018f, 0.042f, 0.60f);",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "NewCharacterHallListHeading(string text)",
        "LGO Character Hall V3B List Heading",
        "RuntimeUiSkin.ApplyCharacterHallListHeading(label);",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "_characterList.Add(NewCharacterHallListHeading(_characters.Length == 0 ? \"Chưa có nhân vật. Tạo tu sĩ đầu tiên.\" : \"Danh sách tu sĩ\"));",
        "await RefreshCharactersAsync();",
        "ShowLobbyMode();",
        "RunAsync(CreateCharacterAsync)",
        "RunAsync(EnterWorldAsync)",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "_characterList.Add(NewStatusLabel(_characters.Length == 0 ? \"Chưa có nhân vật. Tạo tu sĩ đầu tiên.\" : \"Danh sách tu sĩ\", RuntimeArtCatalog.Spirit));",
    )
    require(
        "docs/design/CHARACTER-HALL-V3B-VISUAL-POLISH-v1.0.md",
        "LGO_CHARACTER_HALL_V3B_VISUAL_POLISH_READY",
        "No gameplay or character-flow change",
    )
    require(
        "docs/tasks/LGO-CHARACTER-HALL-V3B-VISUAL-POLISH-v1.0.md",
        "LGO_CHARACTER_HALL_V3B_VISUAL_POLISH_READY",
        "LGO-CHARACTER-HALL-V3B-VISUAL-POLISH-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "character_hall_v3b_visual_polish",
        "validate_lgo_character_hall_v3b_visual_polish.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-CHARACTER-HALL-V3B-VISUAL-POLISH-v1.0",
        "LGO_CHARACTER_HALL_V3B_VISUAL_POLISH_READY",
        "LGO-CHARACTER-HALL-V3B-VISUAL-POLISH-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-CHARACTER-HALL-V3B-VISUAL-POLISH v1.0",
        "LGO_CHARACTER_HALL_V3B_VISUAL_POLISH_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO CHARACTER HALL V3B VISUAL POLISH VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_CHARACTER_HALL_V3B_VISUAL_POLISH_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
