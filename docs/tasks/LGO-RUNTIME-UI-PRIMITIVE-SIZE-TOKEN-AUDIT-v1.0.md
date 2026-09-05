# LGO Runtime UI Primitive Size Token Audit v1.0

Status: `LGO_RUNTIME_UI_PRIMITIVE_SIZE_TOKEN_READY`

## Scope

This pass moves primitive-local dimensions and radii out of `UIPrimitives` and into `RuntimeUiSizing`.

## Changed

- Added `RuntimeUiSizing`.
- Routed base button radius, base panel radius, modal max width, progress bar size/radius, skill button size, and avatar size/radius through named constants.
- Preserved current numeric values and visual behavior.

## Validation

- `python3.12 tools/validate_lgo_runtime_ui_primitive_size_token_audit.py`
- `./tools/lgo_playable_closure_check.sh --source-only`

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No gameplay behavior change.
- No visual redesign or runtime art import.
- No design-token JSON change.
- No protocol, GameData, or ADR change.

## Follow-Up

Continue with `LGO-RUNTIME-UI-PRIMITIVE-SIZE-TOKEN-EVIDENCE-REFRESH-v1.0`.
