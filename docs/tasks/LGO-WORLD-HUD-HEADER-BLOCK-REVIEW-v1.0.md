# LGO World HUD Header Block Review v1.0

Status: `LGO_WORLD_HUD_HEADER_BLOCK_READY`

## Scope

This pass moves repeated title plus ornament composition into `RuntimeUiFactory.NewSectionHeaderBlock`.

## Source Changes

- Added `NewSectionHeaderBlock`.
- Character Hall and World HUD now use header blocks.
- Updated stale validators to check the shared factory primitive instead of direct controller ornament calls.

## Validation

- `python3.12 tools/validate_lgo_world_hud_header_block_review.py`
- `python3.12 tools/validate_m5_ui_skinning.py`
- `python3.12 tools/validate_lgo_world_hud_component_boundary_audit.py`
- `./tools/lgo_playable_closure_check.sh --source-only`

## Non-Claims

- No gameplay change.
- No visual asset payload change.
- No protocol, GameData, ADR, or design-token change.
- No `VISUAL_RUNTIME_PASS` claim.
