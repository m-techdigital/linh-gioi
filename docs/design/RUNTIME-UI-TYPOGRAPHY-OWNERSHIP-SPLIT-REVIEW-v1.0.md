# Runtime UI Typography Ownership Split Review v1.0

Status: `LGO_RUNTIME_UI_TYPOGRAPHY_OWNERSHIP_SPLIT_READY`

## Decision

Move reusable label/status font-size constants into `RuntimeUiTypography`.

## Rationale

`RuntimeUiSpacing` should own layout gaps, panel padding, element dimensions, and component spacing. Label typography has a different change cadence: it often changes by language, device profile, readability, and hierarchy. A dedicated owner keeps future UI tuning easier to review and reduces accidental growth in the spacing file.

## Ownership

- `RuntimeUiTypography` owns label/status font sizes.
- `RuntimeUiSpacing` keeps layout, dimensions, padding, margins, and button/toggle component metrics.
- `RuntimeUiSkin.ApplyText` keeps text style application.
- `M4PlayableClientController` keeps Vietnamese copy, flow state, and callbacks.

## Non-Claims

- No visual runtime PASS claim.
- No visual redesign claim.
- No gameplay, auth, protocol, GameData schema, ADR, or design-token change.
