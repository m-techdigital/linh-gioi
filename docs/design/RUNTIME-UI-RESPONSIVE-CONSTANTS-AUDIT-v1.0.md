# Runtime UI Responsive Constants Audit v1.0

Status: `LGO_RUNTIME_UI_RESPONSIVE_CONSTANTS_AUDIT_READY`

## Scope

This pass audits responsive constants after `RuntimeUiLayoutProfile` extraction.

## Result

The base profile helper now uses named constants for:

- fallback viewport dimensions;
- mobile and tablet breakpoints;
- mobile short-side scale bounds;
- login logo width ratios;
- login card width ratios;
- login logo aspect ratio.

The larger world HUD, Character Hall, and session menu viewport values remain in `M4PlayableClientController` because they still depend on live screen state and visible panel state. They should be extracted only after a later pass proves repeated use or repeated drift.

## Next Safe Target

`LGO-RUNTIME-UI-RESPONSIVE-SESSION-SHELL-HELPER-REVIEW-v1.0`

Review session menu and world HUD responsive shell calculations for a small pure helper. Do not move dialogue, combat, account, or character state.

## Non-Claims

- No gameplay change.
- No account/character flow semantics change.
- No combat mechanic change.
- No runtime image payload change.
- No visual runtime PASS claim.
