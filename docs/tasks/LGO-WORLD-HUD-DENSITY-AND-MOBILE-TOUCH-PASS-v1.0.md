# LGO World HUD Density and Mobile Touch Pass v1.0

Status: `LGO_WORLD_HUD_DENSITY_AND_MOBILE_TOUCH_READY`

## Scope

This pass improves the in-world HUD presentation without adding gameplay, changing combat semantics, or touching frozen contract surfaces.

## Changes

- Compacted the in-world HUD panel so the world scene has more breathing room.
- Prioritized objective and interaction hints for touch/mobile readability.
- Shortened player-facing combat copy from technical intent wording to clear practice wording.
- Kept local combat prototype semantics unchanged.
- Hid layout/profile diagnostic copy from normal runtime HUD.

## Non-Claims

- No gameplay change.
- No production art claim.
- No final visual pass claim.
- No protocol, GameData schema, ADR, or design-token change.

## Evidence

- Source validation: `python3.12 tools/validate_lgo_world_hud_density_mobile_touch.py`
- Runtime evidence should be captured with `./tools/lgo_visual_runtime_review.sh` before human visual acceptance.
