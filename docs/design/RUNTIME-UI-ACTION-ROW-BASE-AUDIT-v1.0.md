# Runtime UI Action Row Base Audit v1.0

Status: `LGO_RUNTIME_UI_ACTION_ROW_BASE_READY`

## Purpose

Runtime button sizing and text weight were repeated across primary, compact, quiet, icon, list, and local combat button paths. This pass moves reusable button metrics into `RuntimeUiSkin.ApplyButtonMetrics` while keeping each button role's color, texture, command callback, and gameplay semantics unchanged.

## Ownership

- `RuntimeUiSkin.ApplyButtonMetrics` owns shared min-width, min-height, font size, bold weight, and nowrap text behavior.
- `RuntimeUiFactory` owns button role construction and continues to decide primary, secondary, quiet, compact, icon, list, and combat button variants.
- `M4PlayableClientController` continues to own button actions and responsive overrides that depend on current screen mode.

## Result

- Shared button metrics have one reusable helper.
- Repeated button size/font assignments in factory paths were consolidated.
- Combat button cooldown/readiness semantics are unchanged; only its size/font styling path moved.

## Non-Claims

- No visual runtime PASS claim.
- No production art claim.
- No gameplay, combat mechanic, auth, protocol, GameData schema, ADR, or design-token change.
