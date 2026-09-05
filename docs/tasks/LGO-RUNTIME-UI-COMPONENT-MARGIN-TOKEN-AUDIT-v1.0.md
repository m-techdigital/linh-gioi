# LGO Runtime UI Component Margin Token Audit v1.0

Status: `LGO_RUNTIME_UI_COMPONENT_MARGIN_TOKEN_READY`

## Scope

This pass extracts repeated component-local spacing and sizing values into a reusable runtime spacing token class.

## Changed Runtime Ownership

- Added `RuntimeUiSpacing` as a code-owned runtime UI spacing layer.
- `RuntimeUiFactory` now uses tokens for repeated panel, row, compact status, badge, toast, button, cooldown icon, and small icon sizes.
- `UIPrimitives` now uses tokens for base button padding, icon button padding, panel padding, and tab spacing.

## Preserved Local Values

- Screen-responsive spacing remains in `RuntimeUiLayoutProfile`.
- Component-specific heading, ornament, input, and toggle values stay local until they become shared component rules.

## Validation

- `python3.12 tools/validate_lgo_runtime_ui_component_margin_token_audit.py`
- `./tools/lgo_playable_closure_check.sh --source-only`

## Follow-Up

Continue with `LGO-RUNTIME-UI-COMPONENT-MARGIN-TOKEN-EVIDENCE-REFRESH-v1.0`.
