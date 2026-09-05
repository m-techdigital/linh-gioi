# Runtime UI Combat Button Metrics Audit v1.0

Status: `LGO_RUNTIME_UI_COMBAT_BUTTON_METRICS_READY`

## Purpose

The local combat button has different ready and cooldown widths/font sizes so Vietnamese labels fit in the compact World HUD. This pass moves those metrics into `RuntimeUiSpacing` to keep combat button styling reusable and prevent future button states from reintroducing local numeric literals.

## Ownership

- `RuntimeUiSpacing` owns combat button ready/cooldown width, height, font, and inset padding metrics.
- `RuntimeUiFactory.ApplyCombatButtonSkin` owns texture selection application and state-aware metrics.
- `M4PlayableClientController` continues to own local combat state, label copy, cooldown timing, and callbacks.

## Result

- Ready and cooldown widths use named spacing constants.
- Ready and cooldown font sizes use named spacing constants.
- Combat button height and padding use named spacing constants.
- Existing local-only combat behavior and Vietnamese copy remain unchanged.

## Non-Claims

- No visual runtime PASS claim.
- No production art claim.
- No gameplay, cooldown, damage, server-authoritative combat, protocol, GameData schema, ADR, or design-token change.
