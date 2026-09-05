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
        "ApplyButtonMetrics(Button button, float minWidth = 0f, float minHeight = 0f, float fontSize = 0f, bool bold = false, WhiteSpace whiteSpace = WhiteSpace.NoWrap)",
        "if (minWidth > 0f) button.style.minWidth = minWidth;",
        "if (minHeight > 0f) button.style.minHeight = minHeight;",
        "if (fontSize > 0f) button.style.fontSize = fontSize;",
        "if (bold) button.style.unityFontStyleAndWeight = FontStyle.Bold;",
    )
    factory = require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "RuntimeUiSpacing.CombatButtonCooldownMinWidth",
        "RuntimeUiSpacing.CombatButtonReadyMinWidth",
        "RuntimeUiSpacing.CombatButtonMinHeight",
        "RuntimeUiSpacing.CombatButtonCooldownFontSize",
        "RuntimeUiSpacing.CombatButtonReadyFontSize",
        "RuntimeUiSkin.ApplyButtonMetrics(button, minHeight: RuntimeUiSpacing.PrimaryButtonMinHeight, fontSize: RuntimeUiTypography.PrimaryButtonFontSize, bold: true);",
        "RuntimeUiSkin.ApplyButtonMetrics(button, RuntimeUiSpacing.CompactPrimaryButtonMinWidth, RuntimeUiSpacing.BaseButtonMinHeight, RuntimeUiSpacing.CompactButtonFontSize, true);",
        "RuntimeUiSkin.ApplyButtonMetrics(button, RuntimeUiSpacing.BaseButtonMinWidth, RuntimeUiSpacing.CompactButtonMinHeight, RuntimeUiSpacing.CompactButtonFontSize);",
        "RuntimeUiSkin.ApplyButtonMetrics(button, RuntimeUiSpacing.BaseButtonMinWidth, RuntimeUiSpacing.BaseButtonMinHeight);",
    )
    if factory.count("RuntimeUiSkin.ApplyButtonMetrics(") < 7:
        ERRORS.append("RuntimeUiFactory should reuse ApplyButtonMetrics in at least seven button paths")
    require(
        "tools/validate_lgo_combat_button_state_readability_polish.py",
        "RuntimeUiSpacing.CombatButtonCooldownMinWidth",
    )
    require(
        "tools/validate_lgo_runtime_ui_style_ownership_drift_audit.py",
        "RuntimeUiSpacing.CombatButtonCooldownMinWidth",
    )
    require(
        "docs/design/RUNTIME-UI-ACTION-ROW-BASE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_ACTION_ROW_BASE_READY",
        "RuntimeUiSkin.ApplyButtonMetrics",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-ACTION-ROW-BASE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_ACTION_ROW_BASE_READY",
        "LGO-RUNTIME-UI-ACTION-ROW-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_action_row_base_audit",
        "validate_lgo_runtime_ui_action_row_base_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-ACTION-ROW-EVIDENCE-REFRESH-v1.0",
        "LGO_RUNTIME_UI_ACTION_ROW_BASE_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-ACTION-ROW-BASE-AUDIT v1.0",
        "LGO_RUNTIME_UI_ACTION_ROW_BASE_READY",
    )
    check_frozen()
    _ = skin
    if ERRORS:
        print("LGO RUNTIME UI ACTION ROW BASE AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_ACTION_ROW_BASE_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
