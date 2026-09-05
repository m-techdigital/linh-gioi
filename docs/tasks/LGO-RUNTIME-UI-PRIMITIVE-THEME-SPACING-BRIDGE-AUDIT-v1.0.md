# LGO Runtime UI Primitive Theme Spacing Bridge Audit v1.0

Status: `LGO_RUNTIME_UI_PRIMITIVE_THEME_SPACING_BRIDGE_READY`

## Scope

This pass connects reusable primitive UI spacing back to the named runtime representation of the frozen design-token spacing scale.

## Changed

- `ThemeTokens` now exposes named spacing accessors with fallbacks matching the existing token scale.
- `BaseButton`, `IconButton`, `BasePanel`, and `TabBar` use `ThemeTokens` spacing names for base design-system rhythm.
- `RuntimeUiSpacing` remains the owner for tuned runtime component measurements that are not general design-token scale values.

## Ownership Rules

- Use `ThemeTokens.Space*` for base primitive design rhythm.
- Use `RuntimeUiSpacing` for runtime component measurements, tuned widths/heights, and specialized HUD/login spacing.
- Use `RuntimeUiLayoutProfile` for desktop/tablet/mobile responsive values.
- Use `RuntimeUiSkin` to apply repeated style assignments.

## Validation

- `python3.12 tools/validate_lgo_runtime_ui_primitive_theme_spacing_bridge_audit.py`
- `./tools/lgo_playable_closure_check.sh --source-only`

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No gameplay behavior change.
- No design-token JSON change.
- No protocol, GameData, or ADR change.

## Follow-Up

Continue with `LGO-RUNTIME-UI-PRIMITIVE-THEME-SPACING-BRIDGE-EVIDENCE-REFRESH-v1.0`.
