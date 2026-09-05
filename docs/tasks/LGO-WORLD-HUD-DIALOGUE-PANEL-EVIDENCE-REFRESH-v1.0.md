# LGO World HUD Dialogue Panel Evidence Refresh v1.0

Status: `LGO_WORLD_HUD_DIALOGUE_PANEL_EVIDENCE_REFRESH_READY`

Date: `2026-09-05`

## Scope

This pass refreshes desktop/tablet/mobile runtime screenshots after the dialogue panel viewport polish.

## Evidence Captured

- desktop: `build/visual-evidence/profiles/desktop/npc-dialogue.png`
- tablet: `build/visual-evidence/profiles/tablet/npc-dialogue.png`
- mobile: `build/visual-evidence/profiles/mobile/npc-dialogue.png`
- mobile world context: `build/visual-evidence/profiles/mobile/world-hub.png`

## Visual Review Notes

- desktop: dialogue remains readable in the left HUD column and the world scene stays unobstructed.
- tablet: dialogue panel retains full action buttons and no longer feels heavier than the current scene focus.
- mobile: guidance content is hidden while dialogue is active, so `Tiếp tục` and `Đóng` remain visible inside the viewport.
- remaining polish: normal mobile world HUD still benefits from future hierarchy tuning so the panel feels less dominant beside the scene.

## Follow-Up

Continue with `LGO-WORLD-HUD-MOBILE-HIERARCHY-POLISH-v1.0` to refine normal mobile world HUD hierarchy, panel opacity, and world/CTA balance without changing gameplay.

## Non-Claims

- No VISUAL_RUNTIME_PASS claim.
- No production art claim.
- No gameplay change.
- No frozen-surface change.

## Validation

```bash
git --no-pager diff --check
python3.12 tools/validate_lgo_world_hud_dialogue_panel_viewport_polish.py
python3.12 tools/validate_lgo_world_hud_dialogue_panel_evidence_refresh.py
./tools/lgo_playable_closure_check.sh --source-only
./tools/lgo_visual_runtime_review_profiles.sh
```
