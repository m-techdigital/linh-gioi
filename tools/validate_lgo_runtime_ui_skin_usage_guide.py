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
        "docs/design/RUNTIME-UI-SKIN-USAGE-GUIDE-v1.0.md",
        "LGO_RUNTIME_UI_SKIN_USAGE_GUIDE_READY",
        "Primitive helpers:",
        "Generic runtime roles:",
        "Login roles:",
        "Character Hall roles:",
        "World/HUD roles:",
        "Rules For New UI",
        "Do not touch frozen surfaces",
        "Source validators cannot claim `VISUAL_RUNTIME_PASS`",
    )
    require(
        "docs/tasks/LGO-RUNTIME-UI-SKIN-USAGE-GUIDE-PASS-v1.0.md",
        "LGO_RUNTIME_UI_SKIN_USAGE_GUIDE_READY",
        "No gameplay change",
        "No new runtime image payload",
    )
    require(
        "client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs",
        "ApplyLoginCtaBacking",
        "ApplyCharacterHallPanelFrame",
        "ApplyWorldHudGroupFrame",
        "WorldHudBackground",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_ui_skin_usage_guide",
        "validate_lgo_runtime_ui_skin_usage_guide.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-UI-SKIN-USAGE-GUIDE-PASS-v1.0",
        "LGO_RUNTIME_UI_SKIN_USAGE_GUIDE_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-UI-SKIN-USAGE-GUIDE-PASS v1.0",
        "LGO_RUNTIME_UI_SKIN_USAGE_GUIDE_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME UI SKIN USAGE GUIDE VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_UI_SKIN_USAGE_GUIDE_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
