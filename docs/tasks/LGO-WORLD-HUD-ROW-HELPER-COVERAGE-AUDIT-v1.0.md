# LGO World HUD Row Helper Coverage Audit v1.0

Status: `LGO_WORLD_HUD_ROW_HELPER_COVERAGE_READY`

## Scope

This pass removes the remaining World HUD badge row styling from the controller and centralizes it in `RuntimeUiFactory`.

## Source Changes

- Added reusable `NewBadgeStrip`.
- Moved `NewBadge` into the factory.
- World HUD debug badges now declare only content and visibility at the controller callsite.

## Validation

- `python3.12 tools/validate_lgo_world_hud_row_helper_coverage_audit.py`
- `python3.12 tools/validate_lgo_world_hud_header_block_review.py`
- `python3.12 tools/validate_lgo_runtime_ui_primitive_factory.py`
- `./tools/lgo_playable_closure_check.sh --source-only`

## Non-Claims

- No gameplay change.
- No runtime visual pass claim.
- No protocol, GameData, ADR, or design-token change.
