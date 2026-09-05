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
        "client/Unity/Assets/Game/UI/Runtime/ThemeTokens.cs",
        "public int SpaceXs => SpacingAt(0, 4);",
        "public int SpaceS => SpacingAt(1, 8);",
        "public int SpaceM => SpacingAt(2, 12);",
        "public int SpaceL => SpacingAt(3, 16);",
        "public int Space4Xl => SpacingAt(7, 64);",
        "private int SpacingAt(int index, int fallback)",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/UIPrimitives.cs",
        "style.paddingLeft = theme.SpaceL;",
        "style.paddingRight = theme.SpaceL;",
        "style.paddingLeft = theme.SpaceS;",
        "RuntimeUiSkin.ApplyPadding(this, theme.SpaceL, theme.SpaceL);",
        "button.style.marginRight = _theme.SpaceS;",
        "button.style.marginBottom = _theme.SpaceS;",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/UIPrimitives.cs",
        "style.paddingLeft = RuntimeUiSpacing.PanelPaddingHorizontal;",
        "style.paddingRight = RuntimeUiSpacing.PanelPaddingHorizontal;",
        "RuntimeUiSkin.ApplyPadding(this, RuntimeUiSpacing.PanelPaddingHorizontal, RuntimeUiSpacing.PanelPaddingHorizontal)",
        "button.style.marginRight = RuntimeUiSpacing.RowGap;",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSpacing.cs",
        "internal static class RuntimeUiSpacing",
        "internal const int BaseButtonMinWidth = 132;",
        "internal const int CooldownIconSize = 52;",
    )
    require(
        "docs/design/RUNTIME-UI-PRIMITIVE-THEME-SPACING-BRIDGE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_PRIMITIVE_THEME_SPACING_BRIDGE_READY",
        "`ThemeTokens` owns named access",
        "`RuntimeUiSpacing` owns code-level component measurements",
        "No change to `client/Unity/Assets/Game/UI/design-tokens.json`",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-PRIMITIVE-THEME-SPACING-BRIDGE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_PRIMITIVE_THEME_SPACING_BRIDGE_READY",
        "Use `ThemeTokens.Space*`",
        "Use `RuntimeUiSpacing`",
        "LGO-RUNTIME-UI-PRIMITIVE-THEME-SPACING-BRIDGE-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_primitive_theme_spacing_bridge_audit",
        "validate_lgo_runtime_ui_primitive_theme_spacing_bridge_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-PRIMITIVE-THEME-SPACING-BRIDGE-EVIDENCE-REFRESH-v1.0",
        "LGO_RUNTIME_UI_PRIMITIVE_THEME_SPACING_BRIDGE_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-PRIMITIVE-THEME-SPACING-BRIDGE-AUDIT v1.0",
        "LGO_RUNTIME_UI_PRIMITIVE_THEME_SPACING_BRIDGE_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI PRIMITIVE THEME SPACING BRIDGE AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_PRIMITIVE_THEME_SPACING_BRIDGE_AUDIT_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
