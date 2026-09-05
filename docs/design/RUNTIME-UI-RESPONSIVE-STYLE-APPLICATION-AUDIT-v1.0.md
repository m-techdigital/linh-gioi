# Runtime UI Responsive Style Application Audit v1.0

Status: `LGO_RUNTIME_UI_RESPONSIVE_STYLE_APPLICATION_AUDIT_READY`

## Decision

Viewport-specific numeric styling should be centralized when it is pure profile math. `M4PlayableClientController` should apply layout decisions, while `RuntimeUiLayoutProfile` should own reusable desktop/tablet/mobile metrics.

## Moved Into RuntimeUiLayoutProfile

- Root padding metrics.
- Header/auth panel height and spacing metrics.
- Login stage, Gate Keeper, and grounding shadow metrics.
- Login control column width and margin metrics.
- Login logo/card/server row/button metrics.
- Login card and grounding background color/opacity metrics.

## Controller Boundary

`M4PlayableClientController` still owns:

- which UI tree is visible;
- account/character/world/session state;
- player-facing Vietnamese copy;
- callbacks and async flow;
- whether a given screen is in auth/lobby/world mode.

## Non-Goals

- No screen-level controller split in this pass.
- No new art, gameplay, protocol, GameData, ADR, or design-token change.
- No claim that visual quality is final from source inspection.

## Follow-Up

Refresh desktop/tablet/mobile login and post-login evidence after this helper extraction, then continue with another narrow reusable UI or runtime maintainability pass.
