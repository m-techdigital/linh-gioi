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
            ERRORS.append(f"{rel} still contains drift marker: {marker}")


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
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSizing.cs",
        "MainShellMaxWidth",
        "HeaderMinHeight",
        "LoginNpcStageWidth",
        "LoginGateKeeperHeight",
        "LoginButtonMaxWidth",
        "CharacterHallPanelMaxWidth",
        "CharacterPortraitHeight",
        "IconButtonMinWidth",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSpacing.cs",
        "PreviewPanelHeadingMarginTop",
        "PrimaryButtonMinHeight",
        "CompactPrimaryButtonMinWidth",
        "IconButtonTextGap",
        "SectionTitleMarginBottom",
        "LoginOrnamentWidthPercent",
        "HairlineHeight",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiTypography.cs",
        "SectionSigilFontSize",
        "SectionHeadingFontSize",
        "SectionTitleFontSize",
        "BadgeTitleFontSize",
        "PrimaryButtonFontSize",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "RuntimeUiSizing.MainShellMaxWidth",
        "RuntimeUiSizing.HeaderMinHeight",
        "RuntimeUiSizing.LoginNpcStageWidth",
        "RuntimeUiSizing.LoginGateKeeperHeight",
        "RuntimeUiSizing.LoginButtonMaxWidth",
        "RuntimeUiSizing.CharacterPortraitHeight",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "RuntimeUiTypography.SectionSigilFontSize",
        "RuntimeUiTypography.SectionTitleFontSize",
        "RuntimeUiSpacing.LoginOrnamentWidthPercent",
        "RuntimeUiSpacing.PrimaryButtonMinHeight",
        "RuntimeUiSizing.IconButtonMinWidth",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "_mainShell.style.maxWidth = 1180;",
        "header.style.minHeight = 76;",
        "brand.style.width = 300;",
        "_authPanel.style.minHeight = 560;",
        "_loginStage.style.width = 304;",
        "gateKeeper.style.height = 438;",
        "_loginButton.style.maxWidth = 436;",
        "portrait.style.height = 128;",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "RuntimeUiSkin.ApplyText(sigil, RuntimeArtCatalog.Spirit, 11, true);",
        "RuntimeUiSkin.ApplyText(heading, RuntimeArtCatalog.Text, 15, true);",
        "RuntimeUiSkin.ApplyText(label, RuntimeArtCatalog.Text, 20, true",
        "RuntimeUiSkin.ApplyButtonMetrics(button, minHeight: 58, fontSize: 16",
        "RuntimeUiSkin.ApplyButtonMetrics(button, 112, 48);",
        "row.style.width = Length.Percent(86);",
    )
    require(
        "docs/design/RUNTIME-UI-COMPONENT-METRIC-OWNERSHIP-DRIFT-SCAN-v1.0.md",
        "LGO_RUNTIME_UI_COMPONENT_METRIC_OWNERSHIP_DRIFT_SCAN_READY",
        "Controller-local values are allowed only when the value is viewport/composition specific",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-COMPONENT-METRIC-OWNERSHIP-DRIFT-SCAN-v1.0.md",
        "LGO_RUNTIME_UI_COMPONENT_METRIC_OWNERSHIP_DRIFT_SCAN_READY",
        "LGO-RUNTIME-UI-COMPONENT-METRIC-OWNERSHIP-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-COMPONENT-METRIC-OWNERSHIP-EVIDENCE-REFRESH-v1.0",
        "LGO_RUNTIME_UI_COMPONENT_METRIC_OWNERSHIP_DRIFT_SCAN_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-COMPONENT-METRIC-OWNERSHIP-DRIFT-SCAN v1.0",
        "LGO_RUNTIME_UI_COMPONENT_METRIC_OWNERSHIP_DRIFT_SCAN_READY",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_component_metric_ownership_drift_scan",
        "validate_lgo_runtime_ui_component_metric_ownership_drift_scan.py",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI COMPONENT METRIC OWNERSHIP DRIFT SCAN VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_COMPONENT_METRIC_OWNERSHIP_DRIFT_SCAN_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
