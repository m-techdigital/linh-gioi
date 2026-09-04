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


def check_list_output() -> None:
    result = subprocess.run(["python3.12", "tools/lgo_runtime_smoke_matrix.py", "--phase", "source", "--list"], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if result.returncode != 0:
        ERRORS.append(result.stderr.strip() or "matrix list failed")
        return
    for marker in ("package_hygiene", "continuous_mode", "playable_source", "playable_package_ready"):
        if marker not in result.stdout:
            ERRORS.append(f"matrix list missing gate: {marker}")


def main() -> int:
    require(
        "docs/tasks/LGO-RUNTIME-SMOKE-MATRIX-v1.0.md",
        "LGO_RUNTIME_SMOKE_MATRIX_READY",
        "No gameplay implementation",
        "validate_lgo_runtime_smoke_matrix.py",
    )
    require(
        "docs/execution/LGO-RUNTIME-SMOKE-MATRIX-v1.0.md",
        "LGO_RUNTIME_SMOKE_MATRIX_READY",
        "LGO_PLAYABLE_CLOSURE_RUNTIME_GATES_PASS",
        "UNVERIFIED_ENVIRONMENT",
        "Do not mask failures",
    )
    require(
        "tools/lgo_runtime_smoke_matrix.py",
        "SOURCE_GATES",
        "RUNTIME_GATES",
        "LGO_RUNTIME_SMOKE_MATRIX_RUN_PASS",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "validate_lgo_runtime_smoke_matrix.py",
    )
    check_executable("tools/lgo_runtime_smoke_matrix.py")
    check_executable("tools/validate_lgo_runtime_smoke_matrix.py")
    check_list_output()
    if ERRORS:
        print("LGO RUNTIME SMOKE MATRIX VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_SMOKE_MATRIX_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
