# Runtime UI Responsive Button Metrics Audit v1.0

Status: `LGO_RUNTIME_UI_RESPONSIVE_BUTTON_METRICS_READY`

## Purpose

Character Hall action buttons need reusable metric ownership so mobile/tablet/desktop layouts can evolve without rewriting the same width, height, and font rules inside screen flow code.

## Ownership

- `RuntimeUiSpacing` owns Character Hall action button dimensions, font sizes, and selected-mobile top spacing.
- `RuntimeUiSkin.ApplyButtonMetrics` owns the shared button metric application path.
- `M4PlayableClientController` keeps action ordering, Vietnamese copy, selection state, opacity, tooltips, and callbacks.

## Result

- Default create/enter-world button metrics use named constants.
- Mobile selected primary CTA metrics use named constants.
- Mobile selected secondary create CTA metrics use named constants.
- Character Hall action metric application now goes through `RuntimeUiSkin.ApplyButtonMetrics`.

## Non-Claims

- No visual runtime PASS claim.
- No gameplay behavior change.
- No account, character creation, character selection, enter-world flow, protocol, GameData schema, ADR, or design-token change.
