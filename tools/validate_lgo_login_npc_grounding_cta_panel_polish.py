#!/usr/bin/env python3
from __future__ import annotations

import subprocess
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
ERRORS: list[str] = []


def read(relative: str) -> str:
    path = ROOT / relative
    if not path.exists():
        ERRORS.append(f"Missing required file: {relative}")
        return ""
    return path.read_text(encoding="utf-8", errors="replace")


def require(text: str, needle: str, context: str) -> None:
    if needle not in text:
        ERRORS.append(f"{context} missing marker/text: {needle}")


def check_runtime_ui() -> None:
    controller = read("client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs")
    layout = read("client/Unity/Assets/Game/UI/Runtime/RuntimeUiLayoutProfile.cs")
    skin = read("client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs")
    require(controller, "_loginNpcGroundingBloom", "controller")
    require(controller, "LGO Login Gate Keeper Anchored Grounding Shadow V3B", "controller")
    require(controller, "LGO Login Gate Keeper Foot Bloom V3B", "controller")
    require(controller, "RuntimeUiSkin.ApplyLoginCtaSceneBlend(_loginCard);", "controller")
    require(layout, "LoginStageBottom => IsTablet ? -52 : -118", "layout profile")
    require(layout, "LoginGateKeeperWidth => IsTablet ? 240 : 282", "layout profile")
    require(layout, "LoginNpcGroundingWidth => IsTablet ? 218 : 256", "layout profile")
    require(layout, "LoginNpcGroundingBloomWidth => IsTablet ? 154 : 180", "layout profile")
    require(layout, "LoginNpcGroundingBloomOpacity => IsTablet ? 0.66f : 0.72f", "layout profile")
    require(skin, "ApplyLoginCtaSceneBlend(VisualElement element)", "runtime skin")
    require(skin, "element.style.borderLeftWidth = 0;", "runtime skin")
    require(skin, "element.style.borderRightWidth = 0;", "runtime skin")


def check_docs() -> None:
    task = read("docs/tasks/LGO-LOGIN-NPC-GROUNDING-AND-CTA-PANEL-POLISH-v1.0.md")
    next_action = read("docs/execution/NEXT-ACTION.md")
    ledger = read("docs/execution/TASK-LEDGER.md")
    require(task, "LGO_LOGIN_NPC_GROUNDING_CTA_PANEL_POLISH_READY", "task doc")
    require(task, "No new image import.", "task doc")
    require(next_action, "LGO_LOGIN_NPC_GROUNDING_CTA_PANEL_POLISH_READY", "NEXT-ACTION")
    require(next_action, "LGO-RUNTIME-UI-QUALITY-DEBT-FIRST-FIX-v1.0", "NEXT-ACTION")
    require(ledger, "LGO-LOGIN-NPC-GROUNDING-AND-CTA-PANEL-POLISH v1.0", "TASK-LEDGER")


def check_closure_hook() -> None:
    closure = read("tools/lgo_playable_closure_check.sh")
    require(closure, "tools/validate_lgo_login_npc_grounding_cta_panel_polish.py", "closure py_compile")
    require(closure, "login_npc_grounding_cta_panel_polish", "closure source-only phase")


def check_frozen_surfaces() -> None:
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
        check=False,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
    )
    if result.returncode != 0:
        ERRORS.append(f"git frozen-surface diff failed: {result.stderr.strip()}")
        return
    changed = [line for line in result.stdout.splitlines() if line.strip()]
    if changed:
        ERRORS.append("Frozen surfaces changed: " + ", ".join(changed))


def main() -> int:
    check_runtime_ui()
    check_docs()
    check_closure_hook()
    check_frozen_surfaces()
    if ERRORS:
        print("LGO LOGIN NPC GROUNDING CTA PANEL POLISH VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(f"- {error}", file=sys.stderr)
        return 1
    print("LGO_LOGIN_NPC_GROUNDING_CTA_PANEL_POLISH_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
