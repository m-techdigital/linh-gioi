# LGO World HUD Mobile Hierarchy Polish v1.0

Status: `LGO_WORLD_HUD_MOBILE_HIERARCHY_POLISH_READY`

Date: `2026-09-05`

## Scope

This pass refines the normal mobile World Hub HUD so the scene remains the primary focus while objective and interaction text stay readable.

## Runtime Presentation Changes

- Mobile normal world mode hides the extra direction row because objective and interaction text already carry the next action.
- Mobile HUD width is reduced by viewport ratio in the final responsive pass.
- Mobile HUD background opacity is softened in normal world mode and kept stronger only while dialogue is open.
- Guidance-card spacing is tightened on mobile without changing the underlying objective state.

## Follow-Up

Continue with `LGO-WORLD-HUD-MOBILE-HIERARCHY-EVIDENCE-REFRESH-v1.0` to capture and review mobile/tablet `world-hub.png` plus `npc-dialogue.png` after this polish.

## Non-Claims

- No gameplay change.
- No production art claim.
- No VISUAL_RUNTIME_PASS claim.
- No frozen-surface change.

## Validation

```bash
git --no-pager diff --check
python3.12 tools/validate_lgo_world_hud_mobile_hierarchy_polish.py
./tools/lgo_playable_closure_check.sh --source-only
./tools/lgo_visual_runtime_review_profiles.sh
```
