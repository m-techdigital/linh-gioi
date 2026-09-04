#!/usr/bin/env python3
from __future__ import annotations

import os
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


def check_executable(rel: str) -> None:
    path = ROOT / rel
    if not path.is_file():
        ERRORS.append(f"missing executable: {rel}")
    elif not os.access(path, os.X_OK):
        ERRORS.append(f"not executable: {rel}")


def check_frozen_design_tokens() -> None:
    result = subprocess.run(["git", "--no-pager", "diff", "--name-only", "--", "client/Unity/Assets/Game/UI/design-tokens.json"], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if result.returncode != 0:
        ERRORS.append(result.stderr.strip() or "git design-token diff failed")
    elif result.stdout.strip():
        ERRORS.append("design tokens changed")


def main() -> int:
    require(
        "docs/tasks/LGO-UI-ATLAS-IMPORT-SETTINGS-v1.0.md",
        "LGO_UI_ATLAS_PLAN_READY",
        "No design-token change",
        "No image generation",
    )
    require(
        "docs/art/UI-ATLAS-IMPORT-SETTINGS.md",
        "LGO_UI_ATLAS_PLAN_READY",
        "Read/Write Enabled: false",
        "Generate Mip Maps: false",
        "Max Size",
        "Do not bake Vietnamese UI copy",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "validate_lgo_ui_atlas_plan.py",
    )
    check_executable("tools/validate_lgo_ui_atlas_plan.py")
    check_frozen_design_tokens()
    if ERRORS:
        print("LGO UI ATLAS PLAN VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_UI_ATLAS_PLAN_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
