#!/usr/bin/env python3
from __future__ import annotations

import os
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


def main() -> int:
    require(
        "docs/tasks/LGO-ASSET-PROVENANCE-RULES-v1.0.md",
        "LGO_ASSET_PROVENANCE_READY",
        "No runtime art replacement",
        "No composite-sheet slicing",
    )
    require(
        "docs/art/ASSET-PROVENANCE-RULES.md",
        "LGO_ASSET_PROVENANCE_READY",
        "Required Metadata",
        "PRODUCTION_FINAL_ACCEPTED",
        "RUNTIME-ASSET-SIZE-BUDGET.md",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "validate_lgo_asset_provenance.py",
    )
    check_executable("tools/validate_lgo_asset_provenance.py")
    if ERRORS:
        print("LGO ASSET PROVENANCE VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_ASSET_PROVENANCE_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
