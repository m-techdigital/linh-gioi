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


def git_lines(*args: str) -> list[str]:
    result = subprocess.run(["git", "--no-pager", *args], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if result.returncode != 0:
        ERRORS.append("git command failed: git --no-pager " + " ".join(args) + " " + result.stderr.strip())
        return []
    return result.stdout.splitlines()


def main() -> int:
    require(
        "docs/art/RUNTIME-ASSET-SIZE-INVENTORY.md",
        "LGO_RUNTIME_ASSET_SIZE_INVENTORY_READY",
        "runtime candidate, not production final art",
        "Do not crop composite/reference boards",
        "mobile/tablet/desktop",
        "login_background",
        "world_spirit_gate",
        "login_logo",
        "world_player_male_cultivator",
    )
    require(
        "docs/tasks/LGO-RUNTIME-ASSET-SIZE-INVENTORY-PASS-v1.0.md",
        "LGO_RUNTIME_ASSET_SIZE_INVENTORY_READY",
        "No production art claim",
        "No gameplay change",
    )
    require("tools/report_lgo_runtime_asset_size_inventory.py", "runtime-candidates-v3b-manifest.csv")
    require("tools/lgo_playable_closure_check.sh", "validate_lgo_runtime_asset_size_inventory.py")

    for path in git_lines("diff", "--name-only"):
        if path == "client/Unity/Assets/Game/UI/design-tokens.json":
            ERRORS.append(f"frozen surface modified: {path}")
        for prefix in ("protocol/", "gamedata/schemas/", "docs/adr/"):
            if path.startswith(prefix):
                ERRORS.append(f"frozen surface modified: {path}")

    if ERRORS:
        print("LGO RUNTIME ASSET SIZE INVENTORY VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_ASSET_SIZE_INVENTORY_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
