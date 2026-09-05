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
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSizing.cs",
        "internal static class RuntimeUiSizing",
        "internal const int BaseButtonRadius = 10;",
        "internal const int BasePanelRadius = 14;",
        "internal const int ModalMaxWidth = 560;",
        "internal const int ProgressBarHeight = 22;",
        "internal const int SkillButtonSize = 56;",
        "internal const int AvatarRadius = 28;",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/UIPrimitives.cs",
        "RuntimeUiSizing.BaseButtonRadius",
        "RuntimeUiSizing.BasePanelRadius",
        "RuntimeUiSizing.ModalMaxWidth",
        "RuntimeUiSizing.ProgressBarHeight",
        "RuntimeUiSizing.ProgressBarRadius",
        "RuntimeUiSizing.SkillButtonSize",
        "RuntimeUiSizing.AvatarSize",
        "RuntimeUiSizing.AvatarRadius",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/UIPrimitives.cs",
        "style.maxWidth = 560;",
        "style.height = 22;",
        "style.minWidth = 56;",
        "style.width = 56;",
        "style.borderTopLeftRadius = 10;",
        "style.borderTopLeftRadius = 14;",
        "style.borderTopLeftRadius = 11;",
        "style.borderTopLeftRadius = 28;",
    )
    require(
        "docs/design/RUNTIME-UI-PRIMITIVE-SIZE-TOKEN-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_PRIMITIVE_SIZE_TOKEN_READY",
        "`RuntimeUiSizing` owns primitive component dimensions and radii",
        "No design-token JSON change",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-PRIMITIVE-SIZE-TOKEN-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_PRIMITIVE_SIZE_TOKEN_READY",
        "LGO-RUNTIME-UI-PRIMITIVE-SIZE-TOKEN-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_primitive_size_token_audit",
        "validate_lgo_runtime_ui_primitive_size_token_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-PRIMITIVE-SIZE-TOKEN-EVIDENCE-REFRESH-v1.0",
        "LGO_RUNTIME_UI_PRIMITIVE_SIZE_TOKEN_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-PRIMITIVE-SIZE-TOKEN-AUDIT v1.0",
        "LGO_RUNTIME_UI_PRIMITIVE_SIZE_TOKEN_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI PRIMITIVE SIZE TOKEN AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_PRIMITIVE_SIZE_TOKEN_AUDIT_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
