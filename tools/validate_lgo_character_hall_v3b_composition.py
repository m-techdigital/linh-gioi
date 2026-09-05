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
        "client/Unity/Assets/Game/Art/Runtime/LgoVisualAssetRegistryV3B.cs",
        "PlayerMaleCultivatorTexture",
        "World/characters/player_male_cultivator_idle_v3b_candidate",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "LGO Character Hall Main Selection Grid V3B",
        "LGO Character Hall Selected Cultivator Card V3B",
        "LGO Character Hall V3B Cultivator Portrait",
        "_createPanel = NewCharacterCreatePanel(layout);",
        "LgoVisualAssetRegistryV3B.PlayerMaleCultivatorTexture",
        "Wrap.NoWrap",
        "mobile ? DisplayStyle.None : DisplayStyle.Flex",
        "Position.Absolute",
        "Chọn tu sĩ để bước qua Linh Môn",
        "Tạo tu sĩ đầu tiên",
        "Vào sân luyện",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "NewCharacterHallPanel(RuntimeUiLayoutProfile layout)",
        "NewCharacterCreatePanel(RuntimeUiLayoutProfile layout)",
        "LGO Character Hall V3B Composition Panel",
        "LGO Character Hall Create Cultivator Panel V3B",
    )
    require(
        "docs/tasks/LGO-CHARACTER-HALL-V3B-COMPOSITION-POLISH-v1.0.md",
        "LGO_CHARACTER_HALL_V3B_COMPOSITION_READY",
        "V3B cultivator portrait",
        "account/character semantics unchanged",
    )
    check_frozen()
    if ERRORS:
        print("LGO CHARACTER HALL V3B COMPOSITION VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_CHARACTER_HALL_V3B_COMPOSITION_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
