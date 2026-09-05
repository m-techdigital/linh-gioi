# Runtime UI Combat HUD Spacing Audit v1.0

Status: `LGO_RUNTIME_UI_COMBAT_HUD_SPACING_READY`

## Purpose

The compact combat HUD needs stable, reusable measurements for target status, range status, feedback text, and its local action row. This pass moves those remaining controller-local combat UI spacing and font choices into `RuntimeUiSpacing`.

## Ownership

- `RuntimeUiSpacing` owns combat status font sizes and combat action row margins.
- `RuntimeUiFactory` owns reusable compact status labels and action rows.
- `M4PlayableClientController` continues to own local combat state, Vietnamese copy, target selection, cooldown state, and callbacks.

## Result

- Combat target status uses `CombatStatusFontSize`.
- Combat range status uses `CombatRangeStatusFontSize`.
- Combat feedback uses `CombatStatusFontSize`.
- Local combat action row uses `CombatActionRowMarginTop` and `CombatActionRowMarginBottom`.

## Non-Claims

- No visual runtime PASS claim.
- No production art claim.
- No gameplay, cooldown, damage, server-authoritative combat, protocol, GameData schema, ADR, or design-token change.
