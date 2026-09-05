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


def check_responsive_padding_blocks() -> None:
    controller = read("client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs")
    responsive_start = controller.find("private void ApplyResponsiveLayoutProfile")
    if responsive_start < 0:
        ERRORS.append("missing ApplyResponsiveLayoutProfile")
        return
    responsive = controller[responsive_start:]
    required = [
        "RuntimeUiSkin.ApplyPadding(_root, layout.RootPaddingHorizontal",
        "RuntimeUiSkin.ApplyPadding(_loginCard, loginCardPadding",
        "RuntimeUiSkin.ApplyPadding(_loginServerRow, layout.LoginServerRowPaddingHorizontal",
        "RuntimeUiSkin.ApplyPadding(_lobbyPanel, layout.LobbyPanelPaddingHorizontal",
        "RuntimeUiSkin.ApplyPadding(_emptyCharacterCard, layout.EmptyCharacterCardPaddingHorizontal",
        "RuntimeUiSkin.ApplyPadding(_createPanel, layout.CreatePanelPaddingHorizontal",
        "RuntimeUiSkin.ApplyPadding(_worldHud, layout.WorldHudPaddingHorizontal",
        "RuntimeUiSkin.ApplyPadding(_sessionMenuPanel, layout.SessionMenuPaddingHorizontal",
        "RuntimeUiSkin.ApplyPadding(_dialoguePanel, layout.DialoguePanelPaddingHorizontal",
    ]
    for marker in required:
        if marker not in responsive:
            ERRORS.append(f"responsive layout missing shared padding call: {marker}")
    forbidden_pairs = [
        "_root.style.paddingLeft = layout.RootPaddingHorizontal;",
        "_loginCard.style.paddingLeft = loginCardPadding;",
        "_loginServerRow.style.paddingLeft = layout.LoginServerRowPaddingHorizontal;",
        "_lobbyPanel.style.paddingLeft = mobile ? 12 : 18;",
        "_createPanel.style.paddingLeft = mobile ? 12 : 16;",
        "_worldHud.style.paddingLeft = mobile ? 8 : 12;",
        "_sessionMenuPanel.style.paddingLeft = mobile ? 12 : tablet ? 16 : 22;",
        "_dialoguePanel.style.paddingLeft = mobile ? 10 : 14;",
    ]
    for marker in forbidden_pairs:
        if marker in responsive:
            ERRORS.append(f"responsive layout still has direct padding drift marker: {marker}")


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
        "internal static void ApplyPadding(VisualElement element, float left, float right, float top, float bottom)",
        "element.style.paddingLeft = left;",
        "element.style.paddingBottom = bottom;",
    )
    check_responsive_padding_blocks()
    require(
        "docs/design/RUNTIME-UI-CONTROLLER-STYLE-CONSTANTS-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_CONTROLLER_STYLE_CONSTANTS_READY",
        "No layout value change intended",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-CONTROLLER-STYLE-CONSTANTS-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_CONTROLLER_STYLE_CONSTANTS_READY",
        "No gameplay change",
        "No `VISUAL_RUNTIME_PASS` claim",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_controller_style_constants_audit",
        "validate_lgo_runtime_ui_controller_style_constants_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-CONTROLLER-STYLE-CONSTANTS-AUDIT-v1.0",
        "LGO_RUNTIME_UI_CONTROLLER_STYLE_CONSTANTS_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-CONTROLLER-STYLE-CONSTANTS-AUDIT v1.0",
        "LGO_RUNTIME_UI_CONTROLLER_STYLE_CONSTANTS_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI CONTROLLER STYLE CONSTANTS AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_CONTROLLER_STYLE_CONSTANTS_AUDIT_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
