# LGO Runtime UI Button Factory Adoption Pass v1.0

Status: `LGO_RUNTIME_UI_BUTTON_FACTORY_ADOPTION_READY`

## Scope

This pass extends `RuntimeUiFactory` to own stateless button, icon, toggle, input, and combat-preview panel builders.

## Migrated Builders

- `NewTextField`
- `ApplyLobbyInputStyle`
- `NewPrimaryButton`
- `NewCompactPrimaryButton`
- `NewQuietButton`
- `NewSecondaryButton`
- `NewCompactSecondaryButton`
- `NewIconButton`
- `NewLocalSettingToggle`
- `NewListButton`
- `NewRuntimeIcon`
- `NewCombatCooldownIcon`
- `ApplyCombatPanelSkin`
- `ApplyV2PanelSkin`

## Boundaries

- Button actions are still passed in explicitly by the controller.
- Async account and character operations remain in `M4PlayableClientController`.
- World state and combat preview semantics remain in `M4PlayableClientController`.
- Visual styling remains in `RuntimeUiSkin`.

## Non-Claims

- No gameplay change.
- No runtime image payload change.
- No protocol, GameData, ADR, or design-token change.
- No `VISUAL_RUNTIME_PASS` claim.

## Follow-Up

Continue with `LGO-RUNTIME-UI-CONTROLLER-RESPONSIBILITY-MAP-v1.0`: map remaining controller regions and identify the next safe code-health target.
