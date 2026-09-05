# Runtime UI Controller Style Constants Audit v1.0

Marker: `LGO_RUNTIME_UI_CONTROLLER_STYLE_CONSTANTS_READY`

## Decision

Controller-local responsive padding assignments now use a shared four-edge `RuntimeUiSkin.ApplyPadding` helper instead of repeating direct `style.padding*` setters. This keeps existing layout values intact while reducing style drift in login, Character Hall, World HUD, session menu, and dialogue panels.

## Implemented

- Added `RuntimeUiSkin.ApplyPadding(VisualElement element, float left, float right, float top, float bottom)`.
- Replaced repeated responsive four-edge padding blocks in `M4PlayableClientController`.
- Kept layout decisions and responsive constants in `RuntimeUiLayoutProfile`/controller callsites.

## Boundary

- `RuntimeUiSkin` owns shared low-level style application.
- `RuntimeUiLayoutProfile` owns responsive metric values.
- The controller owns when a metric applies, because that depends on screen visibility and gameplay/evidence state.

## Non-Claims

- No gameplay change.
- No layout value change intended.
- No visual asset payload change.
- No protocol, GameData, ADR, or design-token change.
- No `VISUAL_RUNTIME_PASS` claim.
