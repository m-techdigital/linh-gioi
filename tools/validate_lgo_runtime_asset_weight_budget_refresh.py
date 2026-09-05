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
        ["python3.12", "tools/report_lgo_runtime_asset_size_inventory.py"],
        cwd=ROOT,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if result.returncode != 0:
        ERRORS.append(result.stderr.strip() or "runtime asset size report failed")
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
        "tools/report_lgo_runtime_asset_size_inventory.py",
        "LGO_RUNTIME_ASSET_WEIGHT_BUDGET_REFRESH_READY",
        "ROLE_LIMITS",
        "roles over budget",
        "roles in watch band >=85% budget",
        "WATCH",
    )
    require(
        "docs/art/RUNTIME-ASSET-SIZE-INVENTORY.md",
        "LGO_RUNTIME_ASSET_WEIGHT_BUDGET_REFRESH_READY",
        "runtime candidate image payload: 2600.3 KB",
        "roles over budget: 0",
        "roles in watch band >=85% budget: 7",
        "Treat `WATCH` rows as the next optimization queue",
    )
    require(
        "docs/tasks/LGO-RUNTIME-ASSET-WEIGHT-BUDGET-REFRESH-v1.0.md",
        "LGO_RUNTIME_ASSET_WEIGHT_BUDGET_REFRESH_READY",
        "No production art claim",
        "No asset recompression or visual replacement in this pass",
    )
    require(
        "tools/lgo_playable_closure_check.sh",
        "runtime_asset_weight_budget_refresh",
        "validate_lgo_runtime_asset_weight_budget_refresh.py",
    )
    require(
        "docs/execution/NEXT-ACTION.md",
        "LGO-RUNTIME-ASSET-WEIGHT-BUDGET-REFRESH-v1.0",
        "LGO_RUNTIME_ASSET_WEIGHT_BUDGET_REFRESH_READY",
    )
    require(
        "docs/execution/TASK-LEDGER.md",
        "LGO-RUNTIME-ASSET-WEIGHT-BUDGET-REFRESH v1.0",
        "LGO_RUNTIME_ASSET_WEIGHT_BUDGET_REFRESH_READY",
    )
    report = run_report()
    for marker in (
        "roles over budget: 0",
        "roles in watch band >=85% budget: 7",
        "`login_background`",
        "`world_spirit_gate`",
        "`world_player_male_cultivator`",
    ):
        if marker not in report:
            ERRORS.append(f"runtime asset report missing marker: {marker}")
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME ASSET WEIGHT BUDGET REFRESH VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_ASSET_WEIGHT_BUDGET_REFRESH_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
