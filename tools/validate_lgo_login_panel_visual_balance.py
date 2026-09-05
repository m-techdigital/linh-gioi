#!/usr/bin/env python3
from __future__ import annotations

import subprocess
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
ERRORS: list[str] = []


def require(path: str, *markers: str) -> None:
    file_path = ROOT / path
    if not file_path.is_file():
        ERRORS.append(f"missing file: {path}")
        return
    text = file_path.read_text(encoding="utf-8", errors="replace")
    for marker in markers:
        if marker not in text:
            ERRORS.append(f"{path} missing marker: {marker}")


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
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "UseLoginOrnatePanelTexture = false",
        "LGO Login Gate Entry Bottom CTA v3 Final Panel V3B",
        "LGO Login CTA Backing Balance v1",
        "RuntimeUiSkin.ApplyLoginCtaBacking(_loginCard);",
        "LGO Login Gate Keeper Soft Grounding Glow V3B",
        "style.opacity = 0.93f",
        "LgoVisualAssetRegistryV3B.ButtonEnterWorldGoldTexture",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiLayoutProfile.cs",
        "Mathf.Clamp(width * 0.26f",
        "Mathf.Clamp(width * 0.46f",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs",
        "LGO Runtime UI Skin Foundation v1",
        "SoftLoginGlass = new Color(0.005f, 0.018f, 0.040f, 0.18f)",
        "LightGoldBorder = new Color(0.93f, 0.73f, 0.36f, 0.20f)",
    )
    require(
        "docs/tasks/LGO-LOGIN-PANEL-VISUAL-BALANCE-PASS-v1.0.md",
        "LGO_LOGIN_PANEL_VISUAL_BALANCE_READY",
        "No new runtime image",
        "VISUAL_RUNTIME_PASS",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "login_panel_visual_balance",
        "validate_lgo_login_panel_visual_balance.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-LOGIN-PANEL-VISUAL-BALANCE-PASS-v1.0",
        "LGO_LOGIN_PANEL_VISUAL_BALANCE_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-LOGIN-PANEL-VISUAL-BALANCE-PASS v1.0",
        "LGO_LOGIN_PANEL_VISUAL_BALANCE_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO LOGIN PANEL VISUAL BALANCE VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(f" - {error}", file=sys.stderr)
        return 1
    print("LGO_LOGIN_PANEL_VISUAL_BALANCE_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
