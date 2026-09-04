#!/usr/bin/env python3
from __future__ import annotations

import csv
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
MANIFEST = ROOT / "docs/reference-art/v3b/metadata/runtime-candidates-v3b-manifest.csv"
FINAL_LOGIN_DIR = ROOT / "client/Unity/Assets/Game/Art/Runtime/FinalLogin/Resources/LGOFinalLogin"
ERRORS: list[str] = []

ROLE_LIMITS = {
    "login_background": 512 * 1024,
    "login_panel": 200 * 1024,
    "enter_world_button": 120 * 1024,
    "gate_keeper_npc_login": 220 * 1024,
    "world_spirit_gate": 320 * 1024,
    "world_training_stone": 180 * 1024,
    "vfx_wind_slash_frame_01": 90 * 1024,
    "vfx_impact_spark": 90 * 1024,
    "combat_cooldown_ready": 45 * 1024,
    "combat_cooldown_active": 45 * 1024,
    "combat_target_dummy_idle": 180 * 1024,
}

FINAL_LOGIN_LIMITS = {
    "login_background_spirit_gate_final_1920x1080.jpg": 520 * 1024,
    "logo_linh_gioi_online_final_420.png": 220 * 1024,
    "button_enter_world_final_384.png": 130 * 1024,
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


def check_manifest() -> None:
    if not MANIFEST.is_file():
        ERRORS.append("missing V3B runtime manifest")
        return
    with MANIFEST.open("r", encoding="utf-8", newline="") as handle:
        rows = list(csv.DictReader(handle))
    for role, limit in ROLE_LIMITS.items():
        matches = [row for row in rows if row.get("role") == role]
        if not matches:
            ERRORS.append(f"manifest missing role: {role}")
            continue
        row = matches[0]
        unity_path = ROOT / row["unity_path"]
        if not unity_path.is_file():
            ERRORS.append(f"missing runtime asset: {row['unity_path']}")
            continue
        size = unity_path.stat().st_size
        if size > limit:
            ERRORS.append(f"{row['unity_path']} exceeds role budget: {size} > {limit}")
        if role == "login_background" and unity_path.suffix.lower() not in {".jpg", ".jpeg"}:
            ERRORS.append("opaque login background should use JPEG runtime copy")
        if role != "login_background" and unity_path.suffix.lower() != ".png":
            ERRORS.append(f"transparent runtime sprite should remain PNG: {row['unity_path']}")


def check_no_large_root_full_source_zips() -> None:
    for path in ROOT.glob("*full-source*.zip"):
        if path.is_file() and path.stat().st_size > 1024 * 1024:
            ERRORS.append(f"large full-source ZIP should be outside repo root: {path.name}")


def check_final_login_assets() -> None:
    if not FINAL_LOGIN_DIR.is_dir():
        ERRORS.append("missing FinalLogin runtime directory")
        return
    for path in FINAL_LOGIN_DIR.iterdir():
        if not path.is_file() or path.suffix == ".meta":
            continue
        if path.name not in FINAL_LOGIN_LIMITS:
            ERRORS.append(f"unexpected FinalLogin runtime asset: {path.relative_to(ROOT)}")
            continue
        limit = FINAL_LOGIN_LIMITS[path.name]
        size = path.stat().st_size
        if size > limit:
            ERRORS.append(f"{path.relative_to(ROOT)} exceeds FinalLogin budget: {size} > {limit}")
        if not path.with_name(path.name + ".meta").is_file():
            ERRORS.append(f"missing Unity meta: {path.relative_to(ROOT)}.meta")
    for name in FINAL_LOGIN_LIMITS:
        if not (FINAL_LOGIN_DIR / name).is_file():
            ERRORS.append(f"missing FinalLogin runtime asset: {(FINAL_LOGIN_DIR / name).relative_to(ROOT)}")


def check_frozen() -> None:
    result = subprocess.run(
        ["git", "--no-pager", "diff", "--name-only", "--", "protocol", "gamedata/schemas", "docs/adr", "client/Unity/Assets/Game/UI/design-tokens.json"],
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
    require(
        "docs/tasks/LGO-RUNTIME-ASSET-WEIGHT-HYGIENE-v1.0.md",
        "LGO_RUNTIME_ASSET_WEIGHT_HYGIENE_READY",
        "No production art claim",
        "No composite sheet slicing",
    )
    require(
        "docs/art/RUNTIME-ASSET-WEIGHT-HYGIENE.md",
        "LGO_RUNTIME_ASSET_WEIGHT_HYGIENE_READY",
        "Opaque fullscreen/login backgrounds may use JPEG",
        "few KB to a few dozen KB",
        "does not claim production art quality",
    )
    require("tools/lgo_playable_closure_check.sh", "validate_lgo_runtime_asset_weight.py")
    check_manifest()
    check_final_login_assets()
    check_no_large_root_full_source_zips()
    check_frozen()
    if ERRORS:
        print("LGO RUNTIME ASSET WEIGHT VALIDATION FAILED", file=sys.stderr)
        for error in ERRORS:
            print(" - " + error, file=sys.stderr)
        return 1
    print("LGO_RUNTIME_ASSET_WEIGHT_VALIDATION_PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
