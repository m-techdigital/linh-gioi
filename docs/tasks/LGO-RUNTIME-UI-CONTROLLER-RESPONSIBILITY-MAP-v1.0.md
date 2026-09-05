# LGO Runtime UI Controller Responsibility Map v1.0

Status: `LGO_RUNTIME_UI_CONTROLLER_RESPONSIBILITY_MAP_READY`

## Scope

This pass maps the remaining responsibilities in `M4PlayableClientController` after primitive and button factory adoption.

## Result

- `RuntimeUiSkin` owns reusable role styling.
- `RuntimeUiFactory` owns stateless leaf widget and compact UI primitive creation.
- `M4PlayableClientController` remains the stateful playable shell coordinator.
- Broad screen-level controller splitting is deferred until state ownership is safer to separate.

## Follow-Up

Continue with `LGO-RUNTIME-UI-RESPONSIVE-LAYOUT-HELPER-REVIEW-v1.0`.

## Non-Claims

- No gameplay change.
- No runtime image payload change.
- No production art claim.
- No visual runtime PASS claim.
