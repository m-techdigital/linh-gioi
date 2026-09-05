# Runtime UI Primitive Size Token Audit v1.0

Status: `LGO_RUNTIME_UI_PRIMITIVE_SIZE_TOKEN_READY`

## Purpose

`UIPrimitives` should not scatter fixed component dimensions and radii across individual constructors. Base sizes need named ownership so later PC/tablet/mobile and asset-density passes can adjust them coherently.

## Ownership Boundary

- `RuntimeUiSizing` owns primitive component dimensions and radii.
- `RuntimeUiSpacing` owns component rhythm: padding, gaps, margins, and icon spacing.
- `ThemeTokens` owns named access to frozen design-token colors, spacing scale, and touch target.
- `RuntimeUiLayoutProfile` owns viewport-responsive placement and screen sizing.

## Moved Into RuntimeUiSizing

- Base button radius.
- Base panel radius.
- Modal max width.
- Progress bar height and radius.
- Skill button size.
- Avatar size and radius.

## Kept Out

- Login hero copy width remains screen composition, not a primitive size.
- Runtime HUD tuned widths remain in `RuntimeUiSpacing` or `RuntimeUiLayoutProfile` depending on whether they are component-owned or profile-owned.
- No asset import profile or image-size budget changed.

## Non-Goals

- No gameplay behavior change.
- No visual redesign.
- No design-token JSON change.
- No protocol, GameData, or ADR change.

## Follow-Up

Continue with `LGO-RUNTIME-UI-PRIMITIVE-SIZE-TOKEN-EVIDENCE-REFRESH-v1.0`: refresh runtime screenshots to confirm size token extraction did not change layout or readability.
