# LGO Login NPC Grounding and CTA Panel Polish v1.0

Marker: `LGO_LOGIN_NPC_GROUNDING_CTA_PANEL_POLISH_READY`

## Purpose

Improve the real login runtime composition without adding new image payload: the Gate Keeper should feel anchored in the scene, and the CTA panel should blend with the V3B fantasy background instead of reading like a heavy debug/tool panel.

## Scope

- Reposition and slightly resize the desktop/tablet Gate Keeper stage.
- Add a lightweight UI-only foot bloom under the NPC.
- Make the CTA backing less boxy while keeping the ornate button and server selector readable.
- Preserve mobile behavior by keeping the NPC stage hidden on mobile.

## Boundaries

- No new gameplay.
- No new image import.
- No composite/reference slicing.
- No protocol, GameData schema, ADR, or design-token changes.
- No `VISUAL_RUNTIME_PASS` claim from source changes alone.

## Validation

- `python3.12 tools/validate_lgo_login_npc_grounding_cta_panel_polish.py`
- `git --no-pager diff --check`
- `./tools/lgo_playable_closure_check.sh --source-only`
- `./tools/lgo_visual_runtime_review.sh` when runtime evidence is available.

## Next

Refresh visual evidence with `LGO-LOGIN-NPC-GROUNDING-EVIDENCE-REFRESH-v1.0` and review `build/visual-evidence/latest/login.png`.
