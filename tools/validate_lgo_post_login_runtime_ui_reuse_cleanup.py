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
            ERRORS.append(f"{rel} still contains direct post-login composition marker: {marker}")


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
    factory = require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "NewCharacterHallContentRow(RuntimeUiLayoutProfile layout)",
        "ApplyCharacterHallContentResponsive(VisualElement row, RuntimeUiLayoutProfile layout)",
        "NewCharacterListPanel(RuntimeUiLayoutProfile layout)",
        "ApplyCharacterListResponsive(VisualElement list, RuntimeUiLayoutProfile layout, int viewportWidth, bool hasSelectedCharacter = false)",
        "NewSelectedCharacterPreviewPanel()",
        "ApplySelectedCharacterPreviewResponsive(VisualElement preview, Label selectedName, RuntimeUiLayoutProfile layout, int viewportWidth, bool hasSelectedCharacter = true)",
        "NewCharacterProfileHero(RuntimeUiLayoutProfile layout, VisualElement portrait, VisualElement copy)",
        "NewCharacterPortraitFrame(RuntimeUiLayoutProfile layout, Texture2D portraitTexture, Texture2D fallbackTexture)",
        "NewFlexibleColumn(string name = null)",
    )
    if factory.count("NewCharacter") < 6:
        ERRORS.append("RuntimeUiFactory should own the reusable Character Hall construction helpers")
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "_lobbyContent = NewCharacterHallContentRow(layout);",
        "_characterList = NewCharacterListPanel(layout);",
        "_selectedPreview = NewSelectedCharacterPreviewPanel();",
        "var portrait = NewCharacterPortraitFrame(layout, portraitTexture, null);",
        "var profileCopy = NewFlexibleColumn(\"LGO Character Hall Selected Profile Copy V3B\");",
        "var profileHero = NewCharacterProfileHero(layout, portrait, profileCopy);",
        "RuntimeCharacterHallResponsiveLayout.Apply(",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeCharacterHallResponsiveLayout.cs",
        "RuntimeUiFactory.ApplyCharacterListResponsive(characterList, layout, width, hasSelectedCharacter);",
        "RuntimeUiFactory.ApplyCharacterHallContentResponsive(lobbyContent, layout);",
        "RuntimeUiFactory.ApplySelectedCharacterPreviewResponsive(selectedPreview, selectedName, layout, width, hasSelectedCharacter);",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "_lobbyContent = new VisualElement();",
        "_characterList = new VisualElement();",
        "_selectedPreview = NewPreviewPanel(\"TU SĨ\", \"Hồ sơ đang chọn\");",
        "var profileHero = new VisualElement();",
        "var profileCopy = new VisualElement();",
        "portrait.style.width = RuntimeUiSizing.CharacterPortraitWidth;",
    )
    require(
        "docs/design/POST-LOGIN-RUNTIME-UI-REUSE-CLEANUP-v1.0.md",
        "LGO_POST_LOGIN_RUNTIME_UI_REUSE_CLEANUP_READY",
        "RuntimeUiFactory",
        "No `VISUAL_RUNTIME_PASS` claim",
    )
    require(
        "docs/tasks/LGO-POST-LOGIN-RUNTIME-UI-REUSE-CLEANUP-v1.0.md",
        "LGO_POST_LOGIN_RUNTIME_UI_REUSE_CLEANUP_READY",
        "LGO-POST-LOGIN-RUNTIME-UI-REUSE-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "post_login_runtime_ui_reuse_cleanup",
        "validate_lgo_post_login_runtime_ui_reuse_cleanup.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-POST-LOGIN-RUNTIME-UI-REUSE-CLEANUP-v1.0",
        "LGO_POST_LOGIN_RUNTIME_UI_REUSE_CLEANUP_READY",
        "LGO-POST-LOGIN-RUNTIME-UI-REUSE-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-POST-LOGIN-RUNTIME-UI-REUSE-CLEANUP v1.0",
        "LGO_POST_LOGIN_RUNTIME_UI_REUSE_CLEANUP_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO POST LOGIN RUNTIME UI REUSE CLEANUP VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_POST_LOGIN_RUNTIME_UI_REUSE_CLEANUP_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
