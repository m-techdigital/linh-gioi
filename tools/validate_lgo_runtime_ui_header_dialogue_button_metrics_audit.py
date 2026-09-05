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
            ERRORS.append(f"{rel} still contains forbidden marker: {marker}")


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
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSpacing.cs",
        "HeaderActionsMobileViewportInset",
        "HeaderActionsMobileMaxWidthFloor",
        "HeaderActionsTabletMaxWidth",
        "HeaderActionsDesktopMaxWidth",
        "TopStatusWorldMobileMinHeight",
        "TopStatusWorldMobileMaxWidthRatioPercent",
        "HeaderQuitWorldMobileMinWidth",
        "HeaderQuitDefaultMinWidth",
        "DialogueButtonMobileMinHeight",
        "DialogueButtonDesktopMinHeight",
        "DialogueContinueMobileMinWidth",
        "DialogueContinueDesktopMinWidth",
        "DialogueCloseMobileMinWidth",
        "DialogueCloseDesktopMinWidth",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiTypography.cs",
        "TopStatusWorldMobileFontSize",
        "TopStatusTabletFontSize",
        "TopStatusDefaultFontSize",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "RuntimeUiSpacing.HeaderActionsMobileViewportInset",
        "RuntimeUiTypography.TopStatusWorldMobileFontSize",
        "RuntimeUiSpacing.TopStatusWorldMobileMaxWidthRatioPercent",
        "RuntimeUiSpacing.HeaderQuitWorldMobileMinWidth",
        "RuntimeUiSpacing.DialogueContinueMobileMinWidth",
        "RuntimeUiSpacing.DialogueCloseDesktopMinWidth",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "_dialogueContinueButton.style.minHeight = mobile ? 38 : 42;",
        "_dialogueContinueButton.style.minWidth = mobile ? 116 : 132;",
        "_dialogueCloseButton.style.minHeight = mobile ? 38 : 42;",
        "_dialogueCloseButton.style.minWidth = mobile ? 90 : 104;",
        "_headerActions.style.maxWidth = worldVisible && mobile ? Mathf.Max(320f, viewportWidth - 24f) : tablet ? 430 : 520;",
        "_status.style.fontSize = worldVisible && mobile ? 13 : tablet ? 13 : 14;",
        "_status.style.minHeight = worldVisible && mobile ? 34 : 32;",
        "_status.style.maxWidth = worldVisible && mobile ? Mathf.Clamp(viewportWidth * 0.28f, 180f, 260f) : tablet ? 270 : 360;",
        "_quitButton.style.minHeight = worldVisible && mobile ? 34 : 36;",
        "_quitButton.style.minWidth = worldVisible && mobile ? 78 : 88;",
        "_quitButton.style.fontSize = worldVisible && mobile ? 13 : 14;",
    )
    require(
        "docs/design/RUNTIME-UI-HEADER-DIALOGUE-BUTTON-METRICS-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_HEADER_DIALOGUE_BUTTON_METRICS_READY",
        "RuntimeUiSkin.ApplyButtonMetrics",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-HEADER-DIALOGUE-BUTTON-METRICS-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_HEADER_DIALOGUE_BUTTON_METRICS_READY",
        "LGO-RUNTIME-UI-HEADER-DIALOGUE-BUTTON-METRICS-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "tools/validate_lgo_world_hud_dialogue_panel_viewport_polish.py",
        "RuntimeUiSpacing.DialogueContinueMobileMinWidth",
        "RuntimeUiSpacing.DialogueButtonMobileMinHeight",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_header_dialogue_button_metrics_audit",
        "validate_lgo_runtime_ui_header_dialogue_button_metrics_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-HEADER-DIALOGUE-BUTTON-METRICS-EVIDENCE-REFRESH-v1.0",
        "LGO_RUNTIME_UI_HEADER_DIALOGUE_BUTTON_METRICS_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-HEADER-DIALOGUE-BUTTON-METRICS-AUDIT v1.0",
        "LGO_RUNTIME_UI_HEADER_DIALOGUE_BUTTON_METRICS_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI HEADER DIALOGUE BUTTON METRICS AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_HEADER_DIALOGUE_BUTTON_METRICS_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
