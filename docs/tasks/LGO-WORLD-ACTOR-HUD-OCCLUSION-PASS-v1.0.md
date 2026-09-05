# LGO World Actor HUD Occlusion Pass v1.0

Status: `LGO_WORLD_ACTOR_HUD_OCCLUSION_READY`

Date: `2026-09-05`

## Scope

This pass reduces actor/HUD occlusion on tablet/mobile world-hub profiles by separating Gate Keeper visual staging from the gameplay interaction target. The Gate Keeper interaction position, smoke positioning, guided objective, and dialogue behavior remain unchanged.

## Runtime Presentation Changes

- Desktop keeps the original Gate Keeper visual position and scale.
- Tablet nudges the Gate Keeper runtime sprite rightward and slightly reduces scale to avoid the left HUD edge.
- Mobile uses a smaller rightward presentation offset so the actor remains visible while the camera stays close enough for readability.
- The Gate Keeper world label follows the visual sprite, while interaction range still evaluates against `GateKeeperPosition`.

## Non-Claims

- No gameplay change.
- No production art claim.
- No protocol or GameData schema change.
- No VISUAL_RUNTIME_PASS claim.
- No frozen-surface change.

## Validation

```bash
git --no-pager diff --check
python3.12 tools/validate_lgo_world_actor_hud_occlusion.py
./tools/lgo_playable_closure_check.sh --source-only
./tools/lgo_visual_runtime_review_profiles.sh
```
