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
            ERRORS.append(f"{rel} contains removed/repeated marker: {marker}")


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
        "internal static Label NewHiddenStatusLabel(string text, Color color)",
        "var label = NewStatusLabel(text, color);",
        "label.style.display = DisplayStyle.None;",
        "return label;",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        '_worldPoseState = NewHiddenStatusLabel("Tư thế:',
        '_worldVfxState = NewHiddenStatusLabel("Hiệu ứng:',
        '_skinSource = NewHiddenStatusLabel("Nguồn giao diện:',
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "_worldPoseState = NewStatusLabel(",
        "_worldVfxState = NewStatusLabel(",
        "_skinSource = NewStatusLabel(",
        "_worldPoseState.style.display = DisplayStyle.None;",
        "_worldVfxState.style.display = DisplayStyle.None;",
        "_skinSource.style.display = DisplayStyle.None;",
    )
    require(
        "docs/tasks/LGO-WORLD-HUD-RUNTIME-UI-REUSE-AUDIT-v1.0.md",
        "LGO_WORLD_HUD_RUNTIME_UI_REUSE_AUDIT_READY",
        "No gameplay",
        "No `VISUAL_RUNTIME_PASS` claim",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "world_hud_runtime_ui_reuse_audit",
        "validate_lgo_world_hud_runtime_ui_reuse_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-WORLD-HUD-RUNTIME-UI-REUSE-AUDIT-v1.0",
        "LGO_WORLD_HUD_RUNTIME_UI_REUSE_AUDIT_READY",
        "LGO-WORLD-HUD-RUNTIME-UI-REUSE-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-WORLD-HUD-RUNTIME-UI-REUSE-AUDIT v1.0",
        "LGO_WORLD_HUD_RUNTIME_UI_REUSE_AUDIT_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO WORLD HUD RUNTIME UI REUSE AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_WORLD_HUD_RUNTIME_UI_REUSE_AUDIT_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
