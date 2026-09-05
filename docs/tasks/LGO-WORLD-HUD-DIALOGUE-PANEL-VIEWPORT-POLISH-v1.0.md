# LGO World HUD Dialogue Panel Viewport Polish v1.0

Status: `LGO_WORLD_HUD_DIALOGUE_PANEL_VIEWPORT_POLISH_READY`

Date: `2026-09-05`

## Scope

This pass reduces mobile/tablet world HUD dominance when dialogue is open. It keeps the current interaction flow, dialogue content, combat placeholder behavior, and save/back actions unchanged.

## Runtime Presentation Changes

- Mobile dialogue hides the guidance card while the dialogue panel is visible so the dialogue controls fit inside the viewport.
- Dialogue panel padding, font sizes, and button minimum sizes now scale by profile instead of using one desktop-like size everywhere.
- World HUD max width and max height are recalculated from current viewport dimensions for dialogue versus normal world mode.
- Tablet keeps more guidance visible than mobile, but narrows the dialogue HUD footprint slightly.

## Follow-Up

Continue with `LGO-WORLD-HUD-DIALOGUE-PANEL-EVIDENCE-REFRESH-v1.0` to capture and review mobile/tablet `npc-dialogue.png` plus world-hub screenshots after this polish.

## Non-Claims

- No gameplay change.
- No production art claim.
- No VISUAL_RUNTIME_PASS claim.
- No frozen-surface change.

## Validation

```bash
git --no-pager diff --check
python3.12 tools/validate_lgo_world_hud_dialogue_panel_viewport_polish.py
./tools/lgo_playable_closure_check.sh --source-only
./tools/lgo_visual_runtime_review_profiles.sh
```
