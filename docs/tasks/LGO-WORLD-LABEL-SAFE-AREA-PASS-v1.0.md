# LGO World Label Safe Area Pass v1.0

Status: `LGO_WORLD_LABEL_SAFE_AREA_READY`

Date: `2026-09-05`

## Scope

This pass keeps the Gate Keeper world-space label readable on tablet/mobile profiles by shortening the rendered label width and nudging it away from the left HUD safe-area. The NPC position, interaction range, guided objective, and dialogue semantics remain unchanged.

## Runtime Presentation Changes

- Desktop keeps the full `Người Giữ Cổng` label and original offset.
- Tablet/mobile use a two-line `Người Giữ\nCổng` label to avoid long text extending under the HUD.
- Narrow profiles nudge the label slightly right/up while preserving the Gate Keeper actor staging.

## Non-Claims

- No gameplay change.
- No production art claim.
- No protocol or GameData schema change.
- No VISUAL_RUNTIME_PASS claim.
- No frozen-surface change.

## Validation

```bash
git --no-pager diff --check
python3.12 tools/validate_lgo_world_label_safe_area.py
./tools/lgo_playable_closure_check.sh --source-only
./tools/lgo_visual_runtime_review_profiles.sh
```
