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
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "_worldGuidanceCard",
        "RuntimeWorldHudResponsiveLayout.ApplyLocalVisibility(",
        "RuntimeWorldHudResponsiveLayout.ApplyHudPanel(",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeWorldHudResponsiveLayout.cs",
        "LGO Runtime World HUD Responsive Layout Helper v1",
        "LGO World HUD Dialogue Viewport Polish v1",
        "dialogueVisible && compactViewport",
        "layout.WorldHudMaxWidth(dialogueVisible)",
        "ApplyMobileHudChildConstraint(layout, worldGuidanceCard)",
        "ApplyMobileHudChildConstraint(layout, skillPreviewPanel)",
        "ApplyMobileHudChildConstraint(layout, localCombatPanel)",
        "ApplyMobileHudChildConstraint(layout, dialoguePanel)",
        "element.style.minWidth = 0",
        "element.style.width = Length.Percent(100)",
        "element.style.maxWidth = Length.Percent(100)",
        "RuntimeUiSpacing.DialogueContinueMobileMinWidth",
        "RuntimeUiSpacing.DialogueButtonMobileMinHeight",
        "RuntimeUiSpacing.DialogueCloseMobileMinWidth",
        "RuntimeUiSpacing.DialogueCloseDesktopMinWidth",
        "LGO Dialogue Action Sizing Contract v1",
        "worldHud.style.minWidth = layout.WorldHudMinWidthFor(dialogueVisible);",
        "dialogueActionRow.style.flexWrap = Wrap.NoWrap",
        "RuntimeUiOverflowGuard.ApplyResponsiveColumns(dialogueActionRow, mobile ? 1 : 2, mobile ? 4 : 6, dialogueContinueButton, dialogueCloseButton)",
        "RuntimeUiOverflowGuard.ApplyBoundedScroll(dialogueLineScroll, layout.DialogueLineScrollMaxHeight)",
        "dialogueLine.style.whiteSpace = WhiteSpace.Normal",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiOverflowGuard.cs",
        "LGO Runtime UI Overflow Guard v1",
        "ApplyBoundedActionRow(VisualElement row)",
        "ApplyBoundedScroll(ScrollView scroll, float maxHeight)",
        "ApplyResponsiveColumns(VisualElement row, int columns, float gap, params Button[] buttons)",
        "row.style.flexDirection = columns == 1 ? FlexDirection.Column : FlexDirection.Row",
        "button.style.minWidth = 0",
        "button.style.flexBasis = 0",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSpacing.cs",
        "DialogueContinueMobileMinWidth = 108",
        "DialogueCloseMobileMinWidth = 82",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiLayoutProfile.cs",
        "Mathf.Clamp(Width * 0.68f, 260f, 320f)",
        "DialogueLineScrollMaxHeight",
    )
    require(
        "docs/tasks/LGO-WORLD-HUD-DIALOGUE-PANEL-VIEWPORT-POLISH-v1.0.md",
        "LGO_WORLD_HUD_DIALOGUE_PANEL_VIEWPORT_POLISH_READY",
        "No gameplay change",
        "No VISUAL_RUNTIME_PASS claim",
        "LGO-WORLD-HUD-DIALOGUE-PANEL-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "world_hud_dialogue_panel_viewport_polish",
        "validate_lgo_world_hud_dialogue_panel_viewport_polish.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-WORLD-HUD-DIALOGUE-PANEL-EVIDENCE-REFRESH-v1.0",
        "LGO_WORLD_HUD_DIALOGUE_PANEL_VIEWPORT_POLISH_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-WORLD-HUD-DIALOGUE-PANEL-VIEWPORT-POLISH v1.0",
        "LGO_WORLD_HUD_DIALOGUE_PANEL_VIEWPORT_POLISH_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO WORLD HUD DIALOGUE PANEL VIEWPORT POLISH VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_WORLD_HUD_DIALOGUE_PANEL_VIEWPORT_POLISH_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
