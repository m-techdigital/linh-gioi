#!/usr/bin/env python3
from __future__ import annotations

import csv
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
MANIFEST = ROOT / "docs/reference-art/v3b/metadata/runtime-candidates-v3b-manifest.csv"

ROLE_LIMITS = {
    "login_background": 512 * 1024,
    "world_spirit_gate": 320 * 1024,
    "world_player_male_cultivator": 180 * 1024,
    "world_tree_pine": 90 * 1024,
    "world_tree_cherry": 90 * 1024,
    "world_bridge_wood": 90 * 1024,
    "world_rock_moss": 55 * 1024,
}


def role_mobile_max(role: str, default_max: int) -> int:
    if role == "login_background":
        return min(default_max, 1024)
    if role.startswith("vfx_"):
        return min(default_max, 256)
    if role.startswith("combat_cooldown_"):
        return min(default_max, 128)
    return min(default_max, 512)


def role_ios_max(role: str, default_max: int) -> int:
    if role == "login_background":
        return min(default_max, 1536)
    if role.startswith("vfx_"):
        return min(default_max, 256)
    if role.startswith("combat_cooldown_"):
        return min(default_max, 128)
    return min(default_max, 768)


def fmt(size: int) -> str:
    return f"{size / 1024:.1f} KB"


def main() -> int:
    if not MANIFEST.is_file():
        print(f"missing manifest: {MANIFEST.relative_to(ROOT)}")
        return 1
    with MANIFEST.open("r", encoding="utf-8", newline="") as handle:
        rows = [row for row in csv.DictReader(handle) if row.get("role") in ROLE_LIMITS]

    print("# LGO Runtime Asset Watch Queue")
    print()
    print("Marker: `LGO_RUNTIME_ASSET_WATCH_QUEUE_IMPORT_PROFILE_READY`")
    print()
    print("| Priority | Role | Size | Budget | Margin | Source Max | Standalone | Android | iPhone | Action |")
    print("|---:|---|---:|---:|---:|---:|---:|---:|---:|---|")
    prioritized_rows = sorted(rows, key=lambda item: ROLE_LIMITS[item["role"]] - (ROOT / item["unity_path"]).stat().st_size)
    for priority, row in enumerate(prioritized_rows, start=1):
        path = ROOT / row["unity_path"]
        size = path.stat().st_size
        budget = ROLE_LIMITS[row["role"]]
        default_max = int(row["runtime_max_texture_size"])
        margin = budget - size
        android_max = role_mobile_max(row["role"], default_max)
        iphone_max = role_ios_max(row["role"], default_max)
        if margin < 12 * 1024:
            action = "next optimization candidate"
        elif row["role"] == "login_background":
            action = "keep JPEG source; rely on 1024/1536 mobile/tablet max texture"
        else:
            action = "keep current source; enforce platform max texture"
        print(
            f"| {priority} | `{row['role']}` | {fmt(size)} | {fmt(budget)} | {fmt(margin)} | "
            f"{default_max} | {default_max} | {android_max} | {iphone_max} | {action} |"
        )
    print()
    print("## Policy")
    print()
    print("- Do not recompress transparent PNGs blindly; compare runtime screenshots before replacement.")
    print("- No runtime art replacement is performed by this report.")
    print("- Do not add animation frames for WATCH character/prop roles without a per-frame budget.")
    print("- Prefer platform import profiles before adding duplicate mobile image folders.")
    print("- V3B remains runtime candidate art, not production final art.")
    print("- Priority is sorted by smallest budget margin first, not visual importance.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
