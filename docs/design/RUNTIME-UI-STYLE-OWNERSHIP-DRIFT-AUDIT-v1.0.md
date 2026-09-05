# Runtime UI Style Ownership Drift Audit v1.0

Marker: `LGO_RUNTIME_UI_STYLE_OWNERSHIP_DRIFT_READY`

## Decision

Repeated style-only helpers for toast messages, status chips, status accent updates, and local combat button skinning now live in `RuntimeUiFactory`. The playable controller keeps screen state, data binding, player-facing Vietnamese copy, and event flow.

## Implemented

- Moved `NewToast` to `RuntimeUiFactory`.
- Moved `ApplyStatusChip` to `RuntimeUiFactory`.
- Moved `ApplyStatusAccent` to `RuntimeUiFactory`.
- Moved `ApplyCombatButtonSkin` to `RuntimeUiFactory`.
- Updated stale validators/docs that assumed controller-owned visual helper implementations.

## Boundary

- `RuntimeUiSkin` owns low-level frame/color recipes.
- `RuntimeUiFactory` owns stateless UI element composition and reusable visual helpers.
- `M4PlayableClientController` owns gameplay/session/account state, runtime copy, event handlers, and evidence trigger flow.
- Do not move methods that depend on selected character, server response, world state, or input semantics into the factory.

## Non-Claims

- No gameplay change.
- No combat mechanic change.
- No runtime asset payload change.
- No protocol, GameData, ADR, or design-token change.
- No `VISUAL_RUNTIME_PASS` claim.
