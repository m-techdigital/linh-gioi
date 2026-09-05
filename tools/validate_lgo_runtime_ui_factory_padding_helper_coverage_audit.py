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
            ERRORS.append(f"{rel} still contains rejected marker: {marker}")


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
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "RuntimeUiSkin.ApplyPadding(label, RuntimeUiSpacing.CompactStatusPaddingHorizontal, RuntimeUiSpacing.CompactStatusPaddingHorizontal, RuntimeUiSpacing.CompactStatusPaddingTop, RuntimeUiSpacing.CompactStatusPaddingBottom);",
        "RuntimeUiSkin.ApplyPadding(row, 4, 4, 0, 0);",
        "RuntimeUiSkin.ApplyPadding(button, 14, 14, 0, 0);",
        "button.style.paddingLeft = RuntimeUiSpacing.ListButtonPaddingLeft;",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "label.style.paddingLeft = 8;",
        "label.style.paddingRight = 8;",
        "label.style.paddingTop = 5;",
        "label.style.paddingBottom = 5;",
        "row.style.paddingLeft = 4;",
        "row.style.paddingRight = 4;",
        "button.style.paddingRight = 14;",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs",
        "ApplyPadding(pill, 9, 9, 3, 4);",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs",
        "pill.style.paddingLeft = 9;",
        "pill.style.paddingRight = 9;",
        "pill.style.paddingTop = 3;",
        "pill.style.paddingBottom = 4;",
    )
    require(
        "docs/design/RUNTIME-UI-FACTORY-PADDING-HELPER-COVERAGE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_FACTORY_PADDING_HELPER_COVERAGE_READY",
        "RuntimeUiSkin.ApplyPadding",
        "Single-edge semantic offsets remain local",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-FACTORY-PADDING-HELPER-COVERAGE-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_FACTORY_PADDING_HELPER_COVERAGE_READY",
        "No gameplay",
        "LGO-RUNTIME-UI-FACTORY-PADDING-HELPER-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_factory_padding_helper_coverage_audit",
        "validate_lgo_runtime_ui_factory_padding_helper_coverage_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-FACTORY-PADDING-HELPER-COVERAGE-AUDIT-v1.0",
        "LGO_RUNTIME_UI_FACTORY_PADDING_HELPER_COVERAGE_READY",
        "LGO-RUNTIME-UI-FACTORY-PADDING-HELPER-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-FACTORY-PADDING-HELPER-COVERAGE-AUDIT v1.0",
        "LGO_RUNTIME_UI_FACTORY_PADDING_HELPER_COVERAGE_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI FACTORY PADDING HELPER COVERAGE AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_FACTORY_PADDING_HELPER_COVERAGE_AUDIT_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
