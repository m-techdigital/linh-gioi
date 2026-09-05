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
            ERRORS.append(f"{rel} still contains marker: {marker}")


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
        "internal static VisualElement NewBadgeStrip",
        "params (string title, string value)[] badges",
        "foreach (var badge in badges) strip.Add(NewBadge(badge.title, badge.value));",
        "internal static VisualElement NewBadge",
        "RuntimeUiSkin.ApplyBadgeFrame(badge)",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        '_worldDebugStrip = NewBadgeStrip(',
        '"LGO World Debug Badge Strip"',
        '("Tài khoản", "đã kết nối")',
        '("Tương tác", "F hoặc Space")',
        "_worldDebugStrip.style.display = DisplayStyle.None;",
    )
    reject(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "private static VisualElement NewBadge",
        "RuntimeUiSkin.ApplyBadgeFrame(badge);",
        "_worldDebugStrip.Add(NewBadge(",
        "_worldDebugStrip.style.flexWrap = Wrap.Wrap;",
    )
    require(
        "docs/design/WORLD-HUD-ROW-HELPER-COVERAGE-AUDIT-v1.0.md",
        "LGO_WORLD_HUD_ROW_HELPER_COVERAGE_READY",
        "RuntimeUiFactory.NewBadgeStrip",
        "Badge helpers are stateless visual composition only",
    )
    require(
        "docs/tasks/LGO-WORLD-HUD-ROW-HELPER-COVERAGE-AUDIT-v1.0.md",
        "LGO_WORLD_HUD_ROW_HELPER_COVERAGE_READY",
        "No gameplay change",
        "No runtime visual pass claim",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "world_hud_row_helper_coverage_audit",
        "validate_lgo_world_hud_row_helper_coverage_audit.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-WORLD-HUD-ROW-HELPER-COVERAGE-AUDIT-v1.0",
        "LGO_WORLD_HUD_ROW_HELPER_COVERAGE_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-WORLD-HUD-ROW-HELPER-COVERAGE-AUDIT v1.0",
        "LGO_WORLD_HUD_ROW_HELPER_COVERAGE_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO WORLD HUD ROW HELPER COVERAGE AUDIT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_WORLD_HUD_ROW_HELPER_COVERAGE_AUDIT_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
