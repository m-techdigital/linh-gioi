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
        "client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs",
        "_loginNpcGrounding",
        "LGO Login NPC Grounding Shadow Balance v1",
        "npcGrounding.style.width = 232;",
        "npcGrounding.style.height = 20;",
        "new Color(0.005f, 0.018f, 0.035f, 0.26f)",
        "RuntimeUiSkin.ApplyRadius(npcGrounding, 110);",
        "_loginNpcGrounding.style.display = layout.LoginNpcGroundingDisplay;",
        "_loginNpcGrounding.style.width = layout.LoginNpcGroundingWidth;",
        "_loginNpcGrounding.style.opacity = layout.LoginNpcGroundingOpacity;",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiLayoutProfile.cs",
        "LoginNpcGroundingDisplay => IsMobile ? DisplayStyle.None : DisplayStyle.Flex",
        "LoginNpcGroundingWidth => IsTablet ? 202 : 232",
        "LoginNpcGroundingOpacity => IsTablet ? 0.70f : 0.76f",
        "LoginNpcGroundingColor => IsTablet",
    )
    require(
        "docs/tasks/LGO-LOGIN-NPC-GROUNDING-SHADOW-BALANCE-PASS-v1.0.md",
        "LGO_LOGIN_NPC_GROUNDING_SHADOW_BALANCE_READY",
        "No new runtime image payload",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "login_npc_grounding_shadow_balance",
        "validate_lgo_login_npc_grounding_shadow_balance.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-LOGIN-NPC-GROUNDING-SHADOW-BALANCE-PASS-v1.0",
        "LGO_LOGIN_NPC_GROUNDING_SHADOW_BALANCE_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-LOGIN-NPC-GROUNDING-SHADOW-BALANCE-PASS v1.0",
        "LGO_LOGIN_NPC_GROUNDING_SHADOW_BALANCE_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO LOGIN NPC GROUNDING SHADOW BALANCE VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_LOGIN_NPC_GROUNDING_SHADOW_BALANCE_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
