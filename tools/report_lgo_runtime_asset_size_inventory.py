#!/usr/bin/env python3
from __future__ import annotations

import csv
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
MANIFEST = ROOT / "docs/reference-art/v3b/metadata/runtime-candidates-v3b-manifest.csv"

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


def fmt(size: int) -> str:
    return f"{size / 1024:.1f} KB"


def main() -> int:
    if not MANIFEST.is_file():
        print("missing V3B runtime manifest")
        return 1

    with MANIFEST.open("r", encoding="utf-8", newline="") as handle:
        rows = list(csv.DictReader(handle))

    total_size = 0
    total_budget = 0
    over_budget = 0
    near_budget = 0
    print("# Runtime Asset Size Inventory Snapshot")
    print()
    print("Marker: `LGO_RUNTIME_ASSET_WEIGHT_BUDGET_REFRESH_READY`")
    print()
    print("| Role | Runtime Asset | Dimensions | File Size | Budget | Margin | Status | Classification |")
    print("|---|---:|---:|---:|---:|---:|---|---|")
    for row in sorted(rows, key=lambda item: (ROOT / item["unity_path"]).stat().st_size if (ROOT / item["unity_path"]).is_file() else 0, reverse=True):
        path = ROOT / row["unity_path"]
        size = path.stat().st_size if path.is_file() else 0
        budget = ROLE_LIMITS.get(row["role"], 0)
        total_size += size
        total_budget += budget
        margin = budget - size if budget else 0
        if budget and margin < 0:
            over_budget += 1
            status = "OVER_BUDGET"
        elif budget and size >= budget * 0.85:
            near_budget += 1
            status = "WATCH"
        elif budget:
            status = "OK"
        else:
            status = "NO_BUDGET"
        dims = f"{row.get('runtime_width', '?')}x{row.get('runtime_height', '?')}"
        budget_text = fmt(budget) if budget else "-"
        margin_text = fmt(margin) if budget else "-"
        print(f"| `{row['role']}` | `{row['unity_path']}` | {dims} | {fmt(size)} | {budget_text} | {margin_text} | `{status}` | `{row.get('classification', '')}` |")
    print()
    print("## Summary")
    print()
    print(f"- runtime candidate image payload: {fmt(total_size)}")
    print(f"- configured role budget total: {fmt(total_budget)}")
    print(f"- roles over budget: {over_budget}")
    print(f"- roles in watch band >=85% budget: {near_budget}")
    print("- V3B assets remain runtime candidates, not production final art.")
    print("- Use Unity platform import profiles for mobile/tablet/desktop delivery rather than duplicating ad hoc asset folders.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
