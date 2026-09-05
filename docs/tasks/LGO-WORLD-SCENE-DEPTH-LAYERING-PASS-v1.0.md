# LGO World Scene Depth Layering Pass v1.0

Marker: `LGO_WORLD_SCENE_DEPTH_LAYERING_READY`

## Goal

Improve the World Hub readable depth without importing heavy art or changing gameplay. This pass focuses on grounding, scale hierarchy, and visual anchors so the hub feels less like flat placeholder sprites on a debug plane.

## Changes

- Added a lightweight procedural transparent ground-shadow sprite generated at runtime.
- Added grounding shadows for the player, Gate Keeper, Spirit Gate, Training Stone, target dummy, shadow slime, and readable set-dressing props.
- Preserved current positions, interaction ranges, combat-preview behavior, protocol contracts, and GameData contracts.
- Kept the pass asset-light: no new PNG import, no composite slicing, no production art claim.

## Runtime Evidence

Review target:

- `build/visual-evidence/latest/world-hub.png`
- `build/visual-evidence/latest/enter-world.png`
- `build/visual-evidence/latest/npc-dialogue.png`

The expected visual result is clearer object grounding and a stronger world-depth read while retaining the existing M4/M5/M6 playable behavior.

## Validation

```bash
python3.12 tools/validate_lgo_world_scene_depth_layering.py
git --no-pager diff --check
./tools/lgo_playable_closure_check.sh --source-only
./tools/lgo_visual_runtime_review.sh
```

## Non-Claims

- No new gameplay mechanic.
- No protocol, GameData schema, ADR, or design-token change.
- No production/final art claim.
- No `VISUAL_RUNTIME_PASS` claim from screenshot capture alone.

## Decision

`LGO_WORLD_SCENE_DEPTH_LAYERING_READY`
