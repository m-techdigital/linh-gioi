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
            ERRORS.append(f"{rel} still contains marker: {marker}")


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
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "internal static VisualElement NewSectionHeaderBlock",
        "block.Add(NewSectionTitle(title));",
        "block.Add(NewOrnamentRule(ornamentColor));",
        "internal static VisualElement NewOrnamentRule",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        'NewSectionHeaderBlock("Điện Nhân Vật", RuntimeArtCatalog.Gold, "LGO Character Hall Header Block")',
        'NewSectionHeaderBlock("Sân Luyện An Toàn", RuntimeArtCatalog.Spirit, "LGO World HUD Header Block")',
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        '_lobbyPanel.Add(NewSectionTitle("Điện Nhân Vật"));',
        '_lobbyPanel.Add(NewOrnamentRule(RuntimeArtCatalog.Gold));',
        '_worldHud.Add(NewSectionTitle("Sân Luyện An Toàn"));',
        '_worldHud.Add(NewOrnamentRule(RuntimeArtCatalog.Spirit));',
    )
    require(
        "docs/design/WORLD-HUD-HEADER-BLOCK-REVIEW-v1.0.md",
        "LGO_WORLD_HUD_HEADER_BLOCK_READY",
        "RuntimeUiFactory.NewSectionHeaderBlock",
        "Header blocks are stateless visual composition only",
    )
    require(
        "docs/tasks/LGO-WORLD-HUD-HEADER-BLOCK-REVIEW-v1.0.md",
        "LGO_WORLD_HUD_HEADER_BLOCK_READY",
        "No gameplay change",
        "No `VISUAL_RUNTIME_PASS` claim",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "world_hud_header_block_review",
        "validate_lgo_world_hud_header_block_review.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-WORLD-HUD-HEADER-BLOCK-REVIEW-v1.0",
        "LGO_WORLD_HUD_HEADER_BLOCK_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-WORLD-HUD-HEADER-BLOCK-REVIEW v1.0",
        "LGO_WORLD_HUD_HEADER_BLOCK_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO WORLD HUD HEADER BLOCK REVIEW VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_WORLD_HUD_HEADER_BLOCK_REVIEW_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
