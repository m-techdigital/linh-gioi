#!/usr/bin/env python3
from __future__ import annotations

import csv
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
MANIFEST = ROOT / "docs/reference-art/v3b/metadata/runtime-candidates-v3b-manifest.csv"
JSON_MANIFEST = ROOT / "docs/reference-art/v3b/metadata/runtime-candidates-v3b-manifest.json"
ERRORS: list[str] = []

ROLE_LIMITS = {
    "login_background": 512 * 1024,
    "login_logo": 300 * 1024,
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
    "combat_target_dummy_selected": 150 * 1024,
    "combat_target_dummy_hit": 150 * 1024,
    "combat_target_dummy_recover": 150 * 1024,
    "world_player_male_cultivator": 180 * 1024,
    "world_tree_cherry": 90 * 1024,
    "world_tree_pine": 90 * 1024,
    "world_lantern_prop": 60 * 1024,
    "world_rock_moss": 55 * 1024,
    "world_bridge_wood": 90 * 1024,
    "world_banner_cultivation": 55 * 1024,
    "world_shadow_slime": 45 * 1024,
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
    check_manifest_json_consistency(rows)
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


def check_manifest_json_consistency(csv_rows: list[dict[str, str]]) -> None:
    if not JSON_MANIFEST.is_file():
        ERRORS.append("missing V3B runtime JSON manifest")
        return
    try:
        import json

        json_rows = json.loads(JSON_MANIFEST.read_text(encoding="utf-8"))
    except Exception as exception:  # pragma: no cover - validation diagnostics only
        ERRORS.append(f"invalid V3B runtime JSON manifest: {exception}")
        return
    csv_by_role = {row.get("role", ""): row for row in csv_rows}
    json_by_role = {row.get("role", ""): row for row in json_rows if isinstance(row, dict)}
    for role in ROLE_LIMITS:
        if role not in json_by_role:
            ERRORS.append(f"JSON manifest missing role: {role}")
            continue
        csv_row = csv_by_role.get(role)
        json_row = json_by_role[role]
        if csv_row and csv_row.get("unity_path") != json_row.get("unity_path"):
            ERRORS.append(f"manifest path mismatch for {role}: CSV and JSON disagree")
        if csv_row and csv_row.get("unity_sha256") != json_row.get("unity_sha256"):
            ERRORS.append(f"manifest sha mismatch for {role}: CSV and JSON disagree")


def check_no_large_root_full_source_zips() -> None:
    for path in ROOT.glob("*full-source*.zip"):
        if path.is_file() and path.stat().st_size > 1024 * 1024:
            ERRORS.append(f"large full-source ZIP should be outside repo root: {path.name}")


def check_removed_login_candidates() -> None:
    forbidden_paths = [
        ROOT / "client/Unity/Assets/Game/Art/Runtime/V3BA",
        ROOT / "client/Unity/Assets/Game/Art/Runtime/FinalLogin",
        ROOT / "client/Unity/Assets/Game/Art/Runtime/LgoVisualAssetRegistryV3BA.cs",
        ROOT / "client/Unity/Assets/Game/Art/Runtime/LgoFinalLoginAssetRegistry.cs",
    ]
    for path in forbidden_paths:
        if path.exists():
            ERRORS.append(f"removed login candidate still present in Unity runtime assets: {path.relative_to(ROOT)}")


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
    check_removed_login_candidates()
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
