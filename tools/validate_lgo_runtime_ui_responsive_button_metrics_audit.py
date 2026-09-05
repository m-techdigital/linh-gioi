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
        "CharacterActionButtonMinWidth",
        "CharacterActionButtonMinHeight",
        "CharacterCreateButtonFontSize",
        "CharacterEnterWorldButtonFontSize",
        "CharacterSelectedPrimaryMobileMinWidth",
        "CharacterSelectedPrimaryMobileMinHeight",
        "CharacterSelectedPrimaryMobileFontSize",
        "CharacterSelectedPrimaryMobileMarginTop",
        "CharacterSelectedSecondaryMobileMinWidth",
        "CharacterSelectedSecondaryMobileMinHeight",
        "CharacterSelectedSecondaryMobileFontSize",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "RuntimeUiSkin.ApplyButtonMetrics(",
        "RuntimeUiSpacing.CharacterSelectedPrimaryMobileMinWidth",
        "RuntimeUiSpacing.CharacterSelectedPrimaryMobileMinHeight",
        "RuntimeUiSpacing.CharacterSelectedPrimaryMobileFontSize",
        "RuntimeUiSpacing.CharacterSelectedPrimaryMobileMarginTop",
        "RuntimeUiSpacing.CharacterSelectedSecondaryMobileMinWidth",
        "RuntimeUiSpacing.CharacterSelectedSecondaryMobileMinHeight",
        "RuntimeUiSpacing.CharacterSelectedSecondaryMobileFontSize",
        "RuntimeUiSpacing.CharacterActionButtonMinWidth",
        "RuntimeUiSpacing.CharacterActionButtonMinHeight",
        "RuntimeUiSpacing.CharacterCreateButtonFontSize",
        "RuntimeUiSpacing.CharacterEnterWorldButtonFontSize",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "_enterWorldButton.style.minHeight = 48;",
        "_enterWorldButton.style.minWidth = 154;",
        "_enterWorldButton.style.fontSize = 17;",
        "_enterWorldButton.style.marginTop = 8;",
        "_createButton.style.minHeight = 38;",
        "_createButton.style.minWidth = 124;",
        "_createButton.style.fontSize = 14;",
        "_createButton.style.minHeight = 44;",
        "_createButton.style.minWidth = 132;",
        "_enterWorldButton.style.minHeight = 44;",
        "_enterWorldButton.style.minWidth = 132;",
        "_enterWorldButton.style.fontSize = 15;",
    )
    require(
        "docs/design/RUNTIME-UI-RESPONSIVE-BUTTON-METRICS-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_RESPONSIVE_BUTTON_METRICS_READY",
        "RuntimeUiSkin.ApplyButtonMetrics",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-RESPONSIVE-BUTTON-METRICS-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_RESPONSIVE_BUTTON_METRICS_READY",
        "LGO-RUNTIME-UI-RESPONSIVE-BUTTON-METRICS-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_responsive_button_metrics_audit",
        "validate_lgo_runtime_ui_responsive_button_metrics_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-RESPONSIVE-BUTTON-METRICS-EVIDENCE-REFRESH-v1.0",
        "LGO_RUNTIME_UI_RESPONSIVE_BUTTON_METRICS_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-RESPONSIVE-BUTTON-METRICS-AUDIT v1.0",
        "LGO_RUNTIME_UI_RESPONSIVE_BUTTON_METRICS_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI RESPONSIVE BUTTON METRICS AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_RESPONSIVE_BUTTON_METRICS_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
