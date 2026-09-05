# LGO Runtime UI Primitive Factory Pass v1.0

Status: `LGO_RUNTIME_UI_PRIMITIVE_FACTORY_READY`

## Scope

This pass introduces `RuntimeUiFactory` for stateless repeated UI construction and migrates safe leaf builders out of `M4PlayableClientController`.

## Added

- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs`
- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs.meta`
- `tools/validate_lgo_runtime_ui_primitive_factory.py`

## Migrated Leaf Builders

- `NewPanel`
- `NewPreviewPanel`
- `NewReadabilityRow`
- `NewWorldHudGroup`
- `ApplyHudStatusCompact`
- `NewSectionTitle`
- `NewMutedLabel`
- `NewLoginOrnamentRule`
- `NewStatusLabel`
- `NewButtonRow`

## Boundaries

- `RuntimeUiFactory` constructs stateless leaf UI and calls `RuntimeUiSkin` for visual styling.
- `M4PlayableClientController` still owns account flow, character flow, world state, dialogue, session state, responsive decisions, and local combat preview semantics.
- `RuntimeUiSkin` remains the visual style owner.

## Non-Claims

- No gameplay change.
- No protocol, GameData, ADR, or design-token change.
- No runtime image payload change.
- No `VISUAL_RUNTIME_PASS` claim.

## Follow-Up

Continue with `LGO-RUNTIME-UI-BUTTON-FACTORY-ADOPTION-PASS-v1.0`: review button helper consolidation only if it stays stateless and avoids behavior coupling.
