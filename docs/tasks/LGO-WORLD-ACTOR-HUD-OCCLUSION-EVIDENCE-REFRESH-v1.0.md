# LGO World Actor HUD Occlusion Evidence Refresh v1.0

Status: `LGO_WORLD_ACTOR_HUD_OCCLUSION_EVIDENCE_REFRESH_READY`

Date: `2026-09-05`

## Scope

This pass refreshes desktop/tablet/mobile runtime profile screenshots after the Gate Keeper actor/HUD occlusion polish.

## Evidence Captured

- desktop: `build/visual-evidence/profiles/desktop/world-hub.png`
- tablet: `build/visual-evidence/profiles/tablet/world-hub.png`
- mobile: `build/visual-evidence/profiles/mobile/world-hub.png`

## Visual Review Notes

- desktop: Gate Keeper keeps the original world staging and does not interfere with the larger left HUD.
- tablet: Gate Keeper sprite and label are shifted clear of the left HUD edge while remaining near the intended northwest guidance target.
- mobile: Gate Keeper is visible to the right of the compact HUD panel, with the two-line label readable and no direct HUD overlap.
- remaining polish: mobile world still benefits from future viewport/panel composition tuning, but the actor/HUD occlusion issue is resolved for this batch.

## Follow-Up

Continue with `LGO-WORLD-HUD-DIALOGUE-PANEL-VIEWPORT-POLISH-v1.0` to reduce mobile panel dominance and improve dialogue/action panel placement without changing gameplay semantics.

## Non-Claims

- No VISUAL_RUNTIME_PASS claim.
- No production art claim.
- No gameplay change.
- No frozen-surface change.

## Validation

```bash
git --no-pager diff --check
python3.12 tools/validate_lgo_world_actor_hud_occlusion.py
python3.12 tools/validate_lgo_world_actor_hud_occlusion_evidence_refresh.py
./tools/lgo_playable_closure_check.sh --source-only
./tools/lgo_visual_runtime_review_profiles.sh
```
