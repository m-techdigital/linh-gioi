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


def check_executable(path: str) -> None:
    file_path = ROOT / path
    if not file_path.is_file():
        ERRORS.append(f"missing file: {path}")
    elif not file_path.stat().st_mode & 0o111:
        ERRORS.append(f"{path} must be executable")


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
        "tools/lgo_visual_runtime_review_profiles.sh",
        "LGO_VISUAL_PROFILE_REVIEW_POLICY build_once_reuse_player",
        "run_profile desktop 1920 1080 build fast",
        "run_profile tablet 1366 1024 skip skip",
        "run_profile mobile 960 540 skip skip",
        "LGO_VISUAL_RUNTIME_PASS_NOT_CLAIMED",
    )
    check_executable("tools/lgo_visual_runtime_review_profiles.sh")
    require(
        ".vscode/tasks.json",
        "LGO: Visual Runtime Profiles",
        "./tools/lgo_visual_runtime_review_profiles.sh",
    )
    require(
        "docs/tasks/LGO-VISUAL-RUNTIME-FAST-PROFILE-REUSE-PASS-v1.0.md",
        "LGO_VISUAL_RUNTIME_FAST_PROFILE_REUSE_READY",
        "build_once_reuse_player",
        "No VISUAL_RUNTIME_PASS claim",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "visual_runtime_fast_profile_reuse",
        "validate_lgo_visual_runtime_fast_profile_reuse.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-VISUAL-RUNTIME-FAST-PROFILE-REUSE-PASS-v1.0",
        "LGO_VISUAL_RUNTIME_FAST_PROFILE_REUSE_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-VISUAL-RUNTIME-FAST-PROFILE-REUSE-PASS v1.0",
        "LGO_VISUAL_RUNTIME_FAST_PROFILE_REUSE_READY",
    )
    check_frozen()
    if ERRORS:
        print("LGO VISUAL RUNTIME FAST PROFILE REUSE VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(f" - {error}", file=sys.stderr)
        return 1
    print("LGO_VISUAL_RUNTIME_FAST_PROFILE_REUSE_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
