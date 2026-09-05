# LGO Runtime UI Combat HUD Spacing Audit v1.0

Status: `LGO_RUNTIME_UI_COMBAT_HUD_SPACING_READY`

## Scope

This task reduces remaining controller-local numeric styling in the compact combat HUD/action shell.

## Changed

- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiSpacing.cs`
- `client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs`
- `tools/validate_lgo_runtime_ui_combat_hud_spacing_audit.py`

## Result

- Added `RuntimeUiSpacing.CombatStatusFontSize`.
- Added `RuntimeUiSpacing.CombatRangeStatusFontSize`.
- Added `RuntimeUiSpacing.CombatActionRowMarginTop`.
- Added `RuntimeUiSpacing.CombatActionRowMarginBottom`.
- Routed target/range/feedback compact status label sizes and local combat action row margins through named constants.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No production art claim.
- No gameplay behavior change.
- No cooldown, damage, targeting, server authority, protocol, GameData, ADR, or design-token change.

## Follow-Up

Continue with `LGO-RUNTIME-UI-COMBAT-HUD-SPACING-EVIDENCE-REFRESH-v1.0`: refresh target-dummy and world screenshots and review compact combat HUD readability after spacing extraction.
