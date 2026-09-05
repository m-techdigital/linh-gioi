# LGO Runtime Asset Weight Budget Refresh v1.0

Status: `LGO_RUNTIME_ASSET_WEIGHT_BUDGET_REFRESH_READY`

## Scope

This pass refreshes the runtime image asset budget report after the latest V3B login/world/combat placeholder work. It improves visibility into which runtime candidates are safe, which are near budget, and where future optimization should focus.

## Current Result

- Runtime candidate image payload: 2600.3 KB.
- Configured role budget total: 3417.0 KB.
- Roles over budget: 0.
- Roles in watch band >=85% budget: 7.

## Watch Rows

- `login_background`: 444.6 KB / 512 KB.
- `world_spirit_gate`: 295.3 KB / 320 KB.
- `world_player_male_cultivator`: 173.7 KB / 180 KB.
- `world_tree_pine`: 82.4 KB / 90 KB.
- `world_tree_cherry`: 79.3 KB / 90 KB.
- `world_bridge_wood`: 78.1 KB / 90 KB.
- `world_rock_moss`: 48.0 KB / 55 KB.

## Changes

- `tools/report_lgo_runtime_asset_size_inventory.py` now prints budget, margin, status, and summary totals.
- `docs/art/RUNTIME-ASSET-SIZE-INVENTORY.md` now records the latest budget snapshot and optimization queue.
- Source validation now checks the refreshed budget marker.

## Non-Claims

- No production art claim.
- No asset recompression or visual replacement in this pass.
- No gameplay mechanic change.
- No protocol, GameData schema, ADR, or design-token change.

## Follow-Up

Before adding new animation frames, prop variants, or larger UI images, either reduce WATCH rows or document the intended platform import/profile impact.
