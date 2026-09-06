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
    require(
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "RuntimeLoginResponsiveLayout.Apply(",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeLoginResponsiveLayout.cs",
        "LGO Login Responsive Scale Cleanup v1",
        "loginStage.style.width = layout.LoginStageWidth",
        "loginGateKeeper.style.width = layout.LoginGateKeeperWidth",
        "LoginNpcGroundingBloomWidth",
        "loginControlColumn.style.width = layout.LoginControlColumnWidth",
        "loginControlColumn.style.justifyContent = layout.IsMobile ? Justify.FlexStart : Justify.Center;",
        "loginControlColumn.style.position = layout.IsMobile ? Position.Absolute : Position.Relative;",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiLayoutProfile.cs",
        "MobileLoginLogoWidthRatio = 0.58f",
        "DesktopLoginLogoWidthRatio = 0.54f",
        "MobileLoginCardWidthRatio = 0.70f",
        "TabletLoginCardWidthRatio = 0.74f",
        "Mathf.Clamp(Mathf.Min(width * MobileLoginLogoWidthRatio, height * 0.65f), 115f, 180f)",
        "Mathf.Clamp(Height - 24f, 320f, 560f)",
        "LoginStageWidth => IsTablet ? 262 : 304",
        "LoginGateKeeperWidth => IsTablet ? 240 : 282",
        "LoginNpcGroundingBloomWidth => IsTablet ? 166 : 196",
        "LoginControlColumnWidth => IsMobile ? Length.Percent(100) : IsTablet ? Length.Percent(56) : Length.Percent(54)",
    )
    require(
        "docs/tasks/LGO-LOGIN-RESPONSIVE-SCALE-CLEANUP-PASS-v1.0.md",
        "LGO_LOGIN_RESPONSIVE_SCALE_CLEANUP_READY",
        "desktop",
        "tablet",
        "mobile",
        "No new runtime art import",
        "No VISUAL_RUNTIME_PASS claim",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "login_responsive_scale_cleanup",
        "validate_lgo_login_responsive_scale_cleanup.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-WORLD-HUB-VISUAL-READABILITY-CLEANUP-PASS-v1.0",
        "LGO_LOGIN_RESPONSIVE_SCALE_CLEANUP_READY",
        "LGO_VISUAL_EVIDENCE_PROFILE_INDEX_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-LOGIN-RESPONSIVE-SCALE-CLEANUP-PASS v1.0",
        "LGO_LOGIN_RESPONSIVE_SCALE_CLEANUP_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO LOGIN RESPONSIVE SCALE CLEANUP VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_LOGIN_RESPONSIVE_SCALE_CLEANUP_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
