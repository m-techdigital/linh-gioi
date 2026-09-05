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
        "client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs",
        "LGO World Label Safe Area v1",
        "GateKeeperWorldLabelText",
        '"Người Giữ\\nCổng"',
        "CurrentGateKeeperLabelOffset",
        "IsMobileWorldViewport",
        "IsNarrowWorldViewport",
        "new Vector3(0.52f, 1.78f, -0.02f)",
    )
    require(
        "docs/tasks/LGO-WORLD-LABEL-SAFE-AREA-PASS-v1.0.md",
        "LGO_WORLD_LABEL_SAFE_AREA_READY",
        "No gameplay change",
        "No VISUAL_RUNTIME_PASS claim",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "world_label_safe_area",
        "validate_lgo_world_label_safe_area.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-WORLD-LABEL-SAFE-AREA-PASS-v1.0",
        "LGO_WORLD_LABEL_SAFE_AREA_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-WORLD-LABEL-SAFE-AREA-PASS v1.0",
        "LGO_WORLD_LABEL_SAFE_AREA_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO WORLD LABEL SAFE AREA VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(f" - {error}", file=sys.stderr)
        return 1
    print("LGO_WORLD_LABEL_SAFE_AREA_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
