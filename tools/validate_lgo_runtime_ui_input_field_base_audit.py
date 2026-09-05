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
    spacing = require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSpacing.cs",
        "BaseInputMaxWidth",
        "BaseInputMinHeight",
        "BaseInputMarginTop",
        "BaseInputPaddingHorizontal",
        "BaseInputPaddingVertical",
    )
    skin = require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs",
        "ApplyInputMetrics(TextField field, float maxWidth = 0f, float minHeight = 0f, float marginTop = 0f)",
        "if (maxWidth > 0f) field.style.maxWidth = maxWidth;",
        "if (minHeight > 0f) field.style.minHeight = minHeight;",
        "if (marginTop > 0f) field.style.marginTop = marginTop;",
        "field.style.color = RuntimeArtCatalog.Text;",
    )
    factory = require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "RuntimeUiSkin.ApplyInputMetrics(field, RuntimeUiSpacing.BaseInputMaxWidth, marginTop: RuntimeUiSpacing.BaseInputMarginTop);",
        "RuntimeUiSkin.ApplyInputMetrics(field, minHeight: RuntimeUiSpacing.BaseInputMinHeight);",
        "RuntimeUiSkin.ApplyPadding(field, RuntimeUiSpacing.BaseInputPaddingHorizontal, RuntimeUiSpacing.BaseInputPaddingVertical);",
    )
    if factory.count("RuntimeUiSkin.ApplyInputMetrics(") < 2:
        ERRORS.append("RuntimeUiFactory should use ApplyInputMetrics for field construction and lobby input styling")
    require(
        "docs/design/RUNTIME-UI-INPUT-FIELD-BASE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_INPUT_FIELD_BASE_READY",
        "RuntimeUiSkin.ApplyInputMetrics",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-INPUT-FIELD-BASE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_INPUT_FIELD_BASE_READY",
        "LGO-RUNTIME-UI-INPUT-FIELD-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_input_field_base_audit",
        "validate_lgo_runtime_ui_input_field_base_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-INPUT-FIELD-EVIDENCE-REFRESH-v1.0",
        "LGO_RUNTIME_UI_INPUT_FIELD_BASE_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-INPUT-FIELD-BASE-AUDIT v1.0",
        "LGO_RUNTIME_UI_INPUT_FIELD_BASE_READY",
    )
    check_frozen()
    _ = spacing, skin
    if ERRORS:
        print("LGO RUNTIME UI INPUT FIELD BASE AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_INPUT_FIELD_BASE_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
