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
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs",
        "SettingToggleStatePillName",
        "ApplySettingToggleFrame(Toggle toggle, Color accent)",
        "toggle.style.justifyContent = Justify.SpaceBetween;",
        "ApplySettingToggleState(Toggle toggle, bool enabled)",
        "ApplySettingToggleStatePill(Label pill, bool enabled)",
        "pill.text = enabled ? \"Bật\" : \"Tắt\";",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs",
        "var statePill = new Label();",
        "RuntimeUiSkin.ApplySettingToggleStatePill(statePill, value);",
        "toggle.Add(statePill);",
        "RuntimeUiSkin.ApplySettingToggleState(toggle, evt.newValue);",
    )
    require(
        "docs/tasks/LGO-SESSION-MENU-SETTING-ROW-VISUAL-POLISH-v1.0.md",
        "LGO_SESSION_MENU_SETTING_ROW_VISUAL_POLISH_READY",
        "Bật",
        "Tắt",
        "No gameplay",
        "LGO-SESSION-MENU-SETTING-ROW-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "session_menu_setting_row_visual_polish",
        "validate_lgo_session_menu_setting_row_visual_polish.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-SESSION-MENU-SETTING-ROW-VISUAL-POLISH-v1.0",
        "LGO_SESSION_MENU_SETTING_ROW_VISUAL_POLISH_READY",
        "LGO-SESSION-MENU-SETTING-ROW-EVIDENCE-REFRESH-v1.0",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-SESSION-MENU-SETTING-ROW-VISUAL-POLISH v1.0",
        "LGO_SESSION_MENU_SETTING_ROW_VISUAL_POLISH_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO SESSION MENU SETTING ROW VISUAL POLISH VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_SESSION_MENU_SETTING_ROW_VISUAL_POLISH_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
