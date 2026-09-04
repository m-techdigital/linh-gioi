#!/usr/bin/env python3
from __future__ import annotations

import csv
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
MANIFEST = ROOT / "docs/reference-art/v3b/metadata/runtime-candidates-v3b-manifest.csv"
ERRORS: list[str] = []

PROFILE_LIMITS = {
    "mobile-light": {
        "default_sprite_max": 512,
        "background_max": 1024,
        "vfx_max": 256,
        "cooldown_max": 128,
    },
    "tablet-standard": {
        "default_sprite_max": 1024,
        "background_max": 1536,
        "vfx_max": 256,
        "cooldown_max": 128,
    },
    "pc-standard": {
        "default_sprite_max": 1024,
        "background_max": 2048,
        "vfx_max": 256,
        "cooldown_max": 128,
    },
}


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


def role_limit_key(role: str) -> str:
    if role == "login_background":
        return "background_max"
    if role.startswith("vfx_"):
        return "vfx_max"
    if role.startswith("combat_cooldown_"):
        return "cooldown_max"
    return "default_sprite_max"


def check_manifest_against_profiles() -> None:
    if not MANIFEST.is_file():
        ERRORS.append("missing V3B runtime manifest")
        return

    with MANIFEST.open("r", encoding="utf-8", newline="") as handle:
        rows = list(csv.DictReader(handle))

    required_columns = {"role", "unity_path", "runtime_width", "runtime_height", "runtime_max_texture_size"}
    if not rows:
        ERRORS.append("V3B runtime manifest is empty")
        return
    missing_columns = required_columns.difference(rows[0].keys())
    if missing_columns:
        ERRORS.append("V3B runtime manifest missing columns: " + ", ".join(sorted(missing_columns)))
        return

    for row in rows:
        role = row["role"]
        unity_path = ROOT / row["unity_path"]
        if not unity_path.is_file():
            ERRORS.append(f"missing runtime asset: {row['unity_path']}")
            continue
        try:
            runtime_width = int(row["runtime_width"])
            runtime_height = int(row["runtime_height"])
            runtime_max_texture_size = int(row["runtime_max_texture_size"])
        except ValueError:
            ERRORS.append(f"manifest has non-integer runtime dimensions for role: {role}")
            continue

        major_axis = max(runtime_width, runtime_height, runtime_max_texture_size)
        key = role_limit_key(role)
        pc_limit = PROFILE_LIMITS["pc-standard"][key]
        if major_axis > pc_limit:
            ERRORS.append(f"{row['unity_path']} exceeds pc-standard {key}: {major_axis} > {pc_limit}")


def check_docs_and_loop_are_wired() -> None:
    require(
        "docs/art/RUNTIME-ASSET-WEIGHT-HYGIENE.md",
        "Device Delivery Profiles",
        "mobile-light",
        "tablet-standard",
        "pc-standard",
        "Future platform builds should derive profile-specific bundles or import overrides",
    )
    require(
        "docs/tasks/LGO-MOBILE-TABLET-UI-PROFILE-HARDENING-v1.0.md",
        "LGO_MOBILE_TABLET_UI_PROFILE_HARDENING_READY",
        "mobile-light",
        "tablet-standard",
        "pc-standard",
        "No duplicate ad hoc runtime asset folders",
    )
    require("tools/lgo_continue_dev_loop.sh", "validate_lgo_device_profile_ui_budgets.py")
    require("tools/lgo_playable_closure_check.sh", "validate_lgo_device_profile_ui_budgets.py")


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
        ERRORS.append("frozen contract/design-token surface changed")


def main() -> int:
    check_manifest_against_profiles()
    check_docs_and_loop_are_wired()
    check_frozen()
    if ERRORS:
        print("LGO DEVICE PROFILE UI BUDGET VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_DEVICE_PROFILE_UI_BUDGET_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
