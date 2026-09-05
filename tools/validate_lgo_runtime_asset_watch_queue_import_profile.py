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


def run_report() -> str:
    result = subprocess.run(
        ["python3.12", "tools/report_lgo_runtime_asset_watch_queue.py"],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if result.returncode != 0:
        ERRORS.append(result.stderr.strip() or "runtime asset watch queue report failed")
    return result.stdout


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
        "tools/report_lgo_runtime_asset_watch_queue.py",
        "LGO_RUNTIME_ASSET_WATCH_QUEUE_IMPORT_PROFILE_READY",
        "role_mobile_max",
        "role_ios_max",
        "Prefer platform import profiles",
    )
    require(
        "docs/art/RUNTIME-ASSET-WATCH-QUEUE.md",
        "LGO_RUNTIME_ASSET_WATCH_QUEUE_IMPORT_PROFILE_READY",
        "No runtime art replacement",
        "login_background",
        "world_player_male_cultivator",
    )
    require(
        "docs/tasks/LGO-RUNTIME-ASSET-WATCH-QUEUE-IMPORT-PROFILE-POLISH-v1.0.md",
        "LGO_RUNTIME_ASSET_WATCH_QUEUE_IMPORT_PROFILE_READY",
        "No production art claim",
        "No gameplay change",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_asset_watch_queue_import_profile",
        "validate_lgo_runtime_asset_watch_queue_import_profile.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-ASSET-WATCH-QUEUE-IMPORT-PROFILE-POLISH-v1.0",
        "LGO_RUNTIME_ASSET_WATCH_QUEUE_IMPORT_PROFILE_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-ASSET-WATCH-QUEUE-IMPORT-PROFILE-POLISH v1.0",
        "LGO_RUNTIME_ASSET_WATCH_QUEUE_IMPORT_PROFILE_READY",
    )
    report = run_report()
    for marker in (
        "`login_background`",
        "`world_spirit_gate`",
        "`world_player_male_cultivator`",
        "next optimization candidate",
        "Android",
        "iPhone",
    ):
        if marker not in report:
            ERRORS.append(f"watch queue report missing marker: {marker}")
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME ASSET WATCH QUEUE IMPORT PROFILE VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_ASSET_WATCH_QUEUE_IMPORT_PROFILE_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
