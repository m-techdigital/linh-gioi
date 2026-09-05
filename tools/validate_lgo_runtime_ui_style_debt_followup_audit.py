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
            ERRORS.append(f"{rel} still contains rejected duplicate marker: {marker}")


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
        "internal static Label NewCompactStatusLabel",
        "var label = NewStatusLabel(text, color);",
        "ApplyHudStatusCompact(label, fontSize);",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        '_worldArea = NewCompactStatusLabel("Khu vực: xem trước tại sảnh"',
        '_worldStep = NewCompactStatusLabel("Tiến trình: Bước 1 Người Giữ Cổng / Bước 2 Đá Luyện."',
        '_worldDirection = NewCompactStatusLabel("Chỉ dẫn: vào sân để hiện mốc gần nhất."',
        '_worldObjective = NewCompactStatusLabel("Mục tiêu: gặp Người Giữ Cổng."',
        '_interactionHint = NewCompactStatusLabel("Di chuyển tới gần Người Giữ Cổng."',
        '_combatTargetStatus = NewCompactStatusLabel("Bia luyện: chưa vào sân."',
        '_combatRangeStatus = NewCompactStatusLabel("Tầm: chưa vào sân."',
        '_combatFeedback = NewCompactStatusLabel("Chưa phải chiến đấu thật."',
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "ApplyHudStatusCompact(_worldArea,",
        "ApplyHudStatusCompact(_worldStep,",
        "ApplyHudStatusCompact(_worldDirection,",
        "ApplyHudStatusCompact(_worldObjective,",
        "ApplyHudStatusCompact(_interactionHint,",
        "ApplyHudStatusCompact(_combatTargetStatus,",
        "ApplyHudStatusCompact(_combatRangeStatus,",
        "ApplyHudStatusCompact(_combatFeedback,",
    )
    require(
        "docs/design/RUNTIME-UI-STYLE-DEBT-FOLLOWUP-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_STYLE_DEBT_FOLLOWUP_AUDIT_READY",
        "NewCompactStatusLabel",
        "stateful screen composition",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-STYLE-DEBT-FOLLOWUP-AUDIT-v1.0.md",
        "LGO_RUNTIME_UI_STYLE_DEBT_FOLLOWUP_AUDIT_READY",
        "No gameplay",
        "LGO-RUNTIME-UI-COMPACT-STATUS-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_style_debt_followup_audit",
        "validate_lgo_runtime_ui_style_debt_followup_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-STYLE-DEBT-FOLLOWUP-AUDIT-v1.0",
        "LGO_RUNTIME_UI_STYLE_DEBT_FOLLOWUP_AUDIT_READY",
        "LGO-RUNTIME-UI-COMPACT-STATUS-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-STYLE-DEBT-FOLLOWUP-AUDIT v1.0",
        "LGO_RUNTIME_UI_STYLE_DEBT_FOLLOWUP_AUDIT_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI STYLE DEBT FOLLOWUP AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_STYLE_DEBT_FOLLOWUP_AUDIT_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
