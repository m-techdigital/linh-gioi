# LGO World HUD Row Helper Evidence Refresh v1.0

Status: `LGO_WORLD_HUD_ROW_HELPER_EVIDENCE_REFRESH_READY`

## Scope

This pass refreshes visual runtime evidence after `RuntimeUiFactory.NewBadgeStrip`/`NewBadge` extraction.

## Evidence

- `build/visual-evidence/latest/world-hub.png`
- `build/visual-evidence/latest/npc-dialogue.png`
- `build/visual-evidence/latest/session-menu.png`
- `build/visual-evidence/latest/target-dummy-state.png`
- `build/visual-evidence/latest/visual-runtime-evidence-manifest.json`
- `build/visual-evidence/latest/visual-runtime-evidence-review.md`

## Review Notes

- World HUD title/header still renders correctly after header block and badge strip extraction.
- Session menu and dialogue overlays remain readable.
- Target dummy checkpoint keeps combat row visible with compact `Hồi chiêu` copy.
- Debug badge strip remains hidden in normal HUD screenshots.

## Validation

- `./tools/lgo_visual_runtime_review.sh`
- `python3.12 tools/validate_lgo_world_hud_row_helper_evidence_refresh.py`
- `./tools/lgo_playable_closure_check.sh --source-only`

## Non-Claims

- No gameplay change.
- No visual asset payload change.
- No `VISUAL_RUNTIME_PASS` claim.
