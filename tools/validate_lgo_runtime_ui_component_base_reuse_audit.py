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
    skin = require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs",
        "ApplyText(Label label, Color color, float fontSize = 0f, bool bold = false, TextAnchor alignment = TextAnchor.UpperLeft)",
        "if (fontSize > 0f) label.style.fontSize = fontSize;",
        "if (bold) label.style.unityFontStyleAndWeight = FontStyle.Bold;",
    )
    factory = require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "RuntimeUiSkin.ApplyText(sigil, RuntimeArtCatalog.Spirit, 11, true);",
        "RuntimeUiSkin.ApplyText(heading, RuntimeArtCatalog.Text, 15, true);",
        "RuntimeUiSkin.ApplyText(label, RuntimeArtCatalog.Text, 20, true, TextAnchor.MiddleCenter);",
        "RuntimeUiSkin.ApplyText(titleLabel, RuntimeArtCatalog.Gold, 11);",
        "RuntimeUiSkin.ApplyText(label, RuntimeArtCatalog.Muted);",
    )
    controller = require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "RuntimeUiSkin.ApplyText(_loginHeroTitle, RuntimeArtCatalog.Text, RuntimeUiTypography.LoginHeroTitleFontSize, true, TextAnchor.MiddleCenter);",
        "RuntimeUiSkin.ApplyText(serverText, RuntimeArtCatalog.Text, RuntimeUiTypography.LoginServerTextInitialFontSize, true, TextAnchor.MiddleCenter);",
        "RuntimeUiSkin.ApplyText(_selectedName, RuntimeArtCatalog.Gold, RuntimeUiTypography.SelectedCharacterNameFontSize, true);",
        "RuntimeUiSkin.ApplyText(_worldName, RuntimeArtCatalog.Gold, RuntimeUiTypography.WorldNameInitialFontSize, true);",
        "RuntimeUiSkin.ApplyText(_dialogueSpeaker, RuntimeArtCatalog.Gold, RuntimeUiTypography.DialogueSpeakerInitialFontSize, true);",
    )
    if factory.count("RuntimeUiSkin.ApplyText(") < 8:
        ERRORS.append("RuntimeUiFactory should reuse ApplyText in at least eight component label paths")
    if controller.count("RuntimeUiSkin.ApplyText(") < 5:
        ERRORS.append("M4PlayableClientController should reuse ApplyText in key screen labels")
    require(
        "docs/design/RUNTIME-UI-COMPONENT-BASE-REUSE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_COMPONENT_BASE_REUSE_READY",
        "RuntimeUiSkin.ApplyText",
        "No visual runtime PASS claim",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-COMPONENT-BASE-REUSE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_COMPONENT_BASE_REUSE_READY",
        "LGO-RUNTIME-UI-COMPONENT-BASE-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_component_base_reuse_audit",
        "validate_lgo_runtime_ui_component_base_reuse_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-COMPONENT-BASE-EVIDENCE-REFRESH-v1.0",
        "LGO_RUNTIME_UI_COMPONENT_BASE_REUSE_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-COMPONENT-BASE-REUSE-AUDIT v1.0",
        "LGO_RUNTIME_UI_COMPONENT_BASE_REUSE_READY",
    )
    check_frozen()
    _ = skin
    if ERRORS:
        print("LGO RUNTIME UI COMPONENT BASE REUSE AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_COMPONENT_BASE_REUSE_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
