# Runtime Asset Watch Queue Priority

Marker: `LGO_RUNTIME_ASSET_WATCH_QUEUE_PRIORITY_READY`

## Purpose

The watch queue must tell Codex what to optimize first before adding new assets. Priority is sorted by smallest remaining budget margin, not by visual importance.

## Current Priority Rule

1. Optimize or cap assets closest to their role budget first.
2. Do not add animation frames for a WATCH character/prop role until a per-frame budget exists.
3. Prefer Unity platform import profiles before adding duplicate mobile/tablet source folders.
4. Recompress transparent PNGs only after visual comparison confirms alpha/glow edges stay clean.

## Current High-Risk Roles

- `world_player_male_cultivator`: closest to budget; block animation frame expansion until per-frame budget is documented.
- `world_rock_moss`, `world_tree_pine`, `world_tree_cherry`, `world_bridge_wood`: avoid multiplying prop variants before profile evidence.
- `world_spirit_gate`: visually important but large; prefer maxTextureSize tuning or audited optimization before adding variants.
- `login_background`: keep JPEG source and rely on platform profiles for mobile/tablet delivery.

## Non-Claims

- No asset replacement in this task.
- No production-final art claim.
- No composite/reference sheet import or slicing.
