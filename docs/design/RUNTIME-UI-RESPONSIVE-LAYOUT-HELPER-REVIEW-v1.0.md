# Runtime UI Responsive Layout Helper Review v1.0

Status: `LGO_RUNTIME_UI_RESPONSIVE_LAYOUT_HELPER_REVIEW_READY`

## Scope

This pass reviews and extracts pure responsive layout calculations from `M4PlayableClientController` without moving screen state.

## Decision

`RuntimeUiLayoutProfile` now owns the reusable, side-effect-free profile calculation for:

- viewport fallback width and height;
- profile name selection: `desktop`, `tablet`, `mobile`;
- mobile short-side scale;
- login logo width/height;
- login CTA card width/padding;
- login primary button height/font size.

The controller still owns applying those values to live UI elements because that code depends on visible screen state, dialogue/session state, and existing runtime references.

## Why This Boundary

The extracted values are pure calculations and can be reused by future screen-specific polish without duplicating profile thresholds. The remaining layout application code is intentionally left in `M4PlayableClientController` because it still coordinates auth, lobby, world HUD, session menu, dialogue, and evidence states.

## Follow-Up

Continue with `LGO-RUNTIME-UI-RESPONSIVE-CONSTANTS-AUDIT-v1.0` to identify whether world HUD/session/Character Hall viewport clamps should become named profile constants. Do not extract them until there is a clear repeated-use benefit.

## Non-Claims

- No gameplay change.
- No account/character flow semantics change.
- No combat mechanic change.
- No runtime image payload change.
- No visual runtime PASS claim.
