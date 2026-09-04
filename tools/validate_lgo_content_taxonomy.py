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


def check_frozen_gamedata_schema() -> None:
    result = subprocess.run(["git", "--no-pager", "diff", "--name-only", "--", "gamedata/schemas"], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if result.returncode != 0:
        ERRORS.append(result.stderr.strip() or "git gamedata schema diff failed")
    elif result.stdout.strip():
        ERRORS.append("gamedata schemas changed")


def main() -> int:
    require(
        "docs/tasks/LGO-CONTENT-TAXONOMY-v1.0.md",
        "LGO_CONTENT_TAXONOMY_READY",
        "No gameplay implementation",
        "No GameData schema change",
    )
    require(
        "docs/design/LGO-CONTENT-TAXONOMY-v1.0.md",
        "LGO_CONTENT_TAXONOMY_READY",
        "Top-Level Domains",
        "world.hub.spirit_gate",
        "Taxonomy ids are planning identifiers",
        "GameData contract-change request",
    )
    require("tools/lgo_playable_closure_check.sh", "validate_lgo_content_taxonomy.py")
    check_executable("tools/validate_lgo_content_taxonomy.py")
    check_frozen_gamedata_schema()
    if ERRORS:
        print("LGO CONTENT TAXONOMY VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_CONTENT_TAXONOMY_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
