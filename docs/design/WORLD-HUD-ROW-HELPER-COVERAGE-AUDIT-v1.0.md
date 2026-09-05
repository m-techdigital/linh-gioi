# World HUD Row Helper Coverage Audit v1.0

Marker: `LGO_WORLD_HUD_ROW_HELPER_COVERAGE_READY`

## Decision

The World HUD debug badge strip now uses reusable `RuntimeUiFactory.NewBadgeStrip` and `RuntimeUiFactory.NewBadge` primitives. The controller keeps the player-facing Vietnamese badge copy, while row layout, wrapping, padding, and badge frame styling live in the factory.

## Implemented

- Added `RuntimeUiFactory.NewBadgeStrip`.
- Moved `NewBadge` out of `M4PlayableClientController`.
- Reused existing `RuntimeUiSkin.ApplyBadgeFrame` and `RuntimeUiSkin.ApplyPadding`.
- Kept debug badge visibility controller-owned with `DisplayStyle.None`.

## Boundary

- Badge helpers are stateless visual composition only.
- Gameplay state, input bindings, runtime labels, evidence flow, and HUD visibility remain controller-owned.
- Do not create a screen component for static badge rows until multiple stateful screens share the same behavior.

## Non-Claims

- No gameplay change.
- No visual asset payload change.
- No protocol, GameData, ADR, or design-token change.
- No `VISUAL_RUNTIME_PASS` claim.
