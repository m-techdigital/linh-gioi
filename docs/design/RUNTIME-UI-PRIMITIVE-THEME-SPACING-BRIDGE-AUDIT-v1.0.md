# Runtime UI Primitive Theme Spacing Bridge Audit v1.0

Status: `LGO_RUNTIME_UI_PRIMITIVE_THEME_SPACING_BRIDGE_READY`

## Purpose

`ThemeTokens.spacing` is the frozen design-token spacing scale loaded from `client/Unity/Assets/Game/UI/design-tokens.json`. Runtime UI should not treat it as an anonymous integer array at call sites.

## Ownership Boundary

- `ThemeTokens` owns named access to the frozen design-token spacing scale.
- `RuntimeUiSpacing` owns code-level component measurements that are not pure design-token scale values, such as preview min widths, compact HUD icon sizes, and tuned runtime button widths.
- `RuntimeUiLayoutProfile` owns viewport-responsive desktop/tablet/mobile layout decisions.
- `RuntimeUiSkin` owns repeated style application helpers, such as padding and margin setters.
- Screen controllers own state, player-facing Vietnamese copy, event wiring, and runtime flow.

## Implemented Bridge

- Added named spacing accessors on `ThemeTokens`: `SpaceXs`, `SpaceS`, `SpaceM`, `SpaceL`, `SpaceXl`, `Space2Xl`, `Space3Xl`, and `Space4Xl`.
- Added safe fallback values matching the frozen spacing scale so runtime UI remains stable if a test theme omits spacing data.
- Routed base primitive spacing in `UIPrimitives` through named `ThemeTokens` spacing accessors.
- Kept runtime-specific measurements in `RuntimeUiSpacing`.

## Non-Goals

- No change to `client/Unity/Assets/Game/UI/design-tokens.json`.
- No gameplay behavior change.
- No new runtime art import or production art claim.
- No protocol, GameData, or ADR change.

## Follow-Up

Continue with `LGO-RUNTIME-UI-PRIMITIVE-THEME-SPACING-BRIDGE-EVIDENCE-REFRESH-v1.0`: refresh runtime screenshots to confirm the theme-spacing bridge does not regress login, Character Hall, World HUD, or session menu readability.
