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
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSpacing.cs",
        "internal static class RuntimeUiSpacing",
        "internal const int PanelPaddingHorizontal = 16;",
        "internal const int PreviewPanelPaddingHorizontal = 14;",
        "internal const int RowGap = 8;",
        "internal const int CompactStatusPaddingTop = 5;",
        "internal const int BaseButtonMinWidth = 132;",
        "internal const int RuntimeIconSmall = 28;",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "RuntimeUiSpacing.PanelMinWidth",
        "RuntimeUiSpacing.PreviewPanelPaddingHorizontal",
        "RuntimeUiSpacing.ReadabilityRowPaddingHorizontal",
        "RuntimeUiSpacing.WorldHudRootPaddingHorizontal",
        "RuntimeUiSpacing.CompactStatusPaddingHorizontal",
        "RuntimeUiSpacing.BadgePaddingHorizontal",
        "RuntimeUiSpacing.BaseButtonMinWidth",
        "RuntimeUiSpacing.CooldownIconSize",
        "RuntimeUiSpacing.RuntimeIconSmall",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/UIPrimitives.cs",
        "RuntimeUiSpacing.PanelPaddingHorizontal",
        "RuntimeUiSpacing.WorldHudGroupPaddingHorizontal",
        "RuntimeUiSkin.ApplyPadding(this, RuntimeUiSpacing.PanelPaddingHorizontal, RuntimeUiSpacing.PanelPaddingHorizontal)",
        "RuntimeUiSpacing.RowGap",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "panel.style.minWidth = 300;",
        "preview.style.minWidth = 220;",
        "row.style.marginTop = 8;",
        "RuntimeUiSkin.ApplyPadding(row, 10, 7);",
        "icon.style.width = 52;",
        "button.style.minWidth = 132;\n            button.style.minHeight = 44;\n            button.style.marginTop = 8;",
    )
    require(
        "docs/design/RUNTIME-UI-COMPONENT-MARGIN-TOKEN-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_COMPONENT_MARGIN_TOKEN_READY",
        "RuntimeUiSpacing",
        "RuntimeUiLayoutProfile",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-COMPONENT-MARGIN-TOKEN-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_COMPONENT_MARGIN_TOKEN_READY",
        "LGO-RUNTIME-UI-COMPONENT-MARGIN-TOKEN-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-COMPONENT-MARGIN-TOKEN-EVIDENCE-REFRESH-v1.0",
        "LGO_RUNTIME_UI_COMPONENT_MARGIN_TOKEN_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-COMPONENT-MARGIN-TOKEN-AUDIT v1.0",
        "LGO_RUNTIME_UI_COMPONENT_MARGIN_TOKEN_READY",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_component_margin_token_audit",
        "validate_lgo_runtime_ui_component_margin_token_audit.py",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI COMPONENT MARGIN TOKEN AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_COMPONENT_MARGIN_TOKEN_AUDIT_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
