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
            ERRORS.append(f"{rel} still contains stale marker: {marker}")


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
        "internal static void ApplyMargin(VisualElement element, float left, float right, float top, float bottom)",
        "internal static void ApplyVerticalMargin(VisualElement element, float top, float bottom)",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiLayoutProfile.cs",
        "internal int LobbyIntroMarginBottom",
        "internal int LobbyContentMarginTop",
        "internal int LobbyContentMarginBottom",
        "internal int CharacterListMarginRight",
        "internal int EmptyCharacterCardMarginTop",
        "internal int CreatePanelMarginTop",
        "internal int WorldGuidanceCardMarginVertical",
        "internal int DialoguePanelMarginTop",
        "internal int SessionMenuStatusMarginBottom",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "RuntimeUiSkin.ApplyMargin(_characterList, 0, layout.CharacterListMarginRight, 0, layout.CharacterListMarginBottom);",
        "RuntimeUiSkin.ApplyVerticalMargin(_lobbyContent, layout.LobbyContentMarginTop, layout.LobbyContentMarginBottom);",
        "RuntimeUiSkin.ApplyVerticalMargin(_worldGuidanceCard, layout.WorldGuidanceCardMarginVertical, layout.WorldGuidanceCardMarginVertical);",
        "_dialoguePanel.style.marginTop = layout.DialoguePanelMarginTop;",
        "_sessionMenuStatus.style.marginBottom = layout.SessionMenuStatusMarginBottom;",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "_lobbyIntro.style.marginBottom = mobile ? 6 : 10;",
        "_characterList.style.marginRight = mobile ? 10 : 14;",
        "_lobbyContent.style.marginBottom = mobile ? 8 : 10;",
        "_createPanel.style.marginTop = mobile ? 0 : 10;",
        "_worldGuidanceCard.style.marginTop = mobile ? 6 : 8;",
        "_dialoguePanel.style.marginTop = mobile ? 6 : tablet ? 8 : 10;",
    )
    require(
        "docs/design/RUNTIME-UI-ONE-EDGE-LAYOUT-HELPER-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_ONE_EDGE_LAYOUT_HELPER_READY",
        "RuntimeUiSkin.ApplyMargin",
        "RuntimeUiLayoutProfile",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-ONE-EDGE-LAYOUT-HELPER-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_ONE_EDGE_LAYOUT_HELPER_READY",
        "LGO-RUNTIME-UI-ONE-EDGE-LAYOUT-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-ONE-EDGE-LAYOUT-EVIDENCE-REFRESH-v1.0",
        "LGO_RUNTIME_UI_ONE_EDGE_LAYOUT_HELPER_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-ONE-EDGE-LAYOUT-HELPER-AUDIT v1.0",
        "LGO_RUNTIME_UI_ONE_EDGE_LAYOUT_HELPER_READY",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_one_edge_layout_helper_audit",
        "validate_lgo_runtime_ui_one_edge_layout_helper_audit.py",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI ONE-EDGE LAYOUT HELPER AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_ONE_EDGE_LAYOUT_HELPER_AUDIT_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
