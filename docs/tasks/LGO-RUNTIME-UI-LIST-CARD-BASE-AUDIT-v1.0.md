# LGO Runtime UI List Card Base Audit v1.0

Status: `LGO_RUNTIME_UI_LIST_CARD_BASE_READY`

## Scope

This task reduces repeated list-card and empty-state card setup in the runtime UI layer.

## Changed

- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiSpacing.cs`
- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs`
- `client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs`
- `tools/validate_lgo_runtime_ui_list_card_base_audit.py`
- stale UI style validators updated to the factory-owned empty-card path.

## Result

- Added `RuntimeUiFactory.NewEmptyCharacterCard`.
- Added named list-button and empty-card hint spacing constants.
- Kept Character Hall empty/create/selected flow unchanged.
- Kept `NewListButton` behavior while honoring the supplied class text.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No production art claim.
- No gameplay behavior change.
- No design-token JSON change.
- No protocol, GameData, or ADR change.

## Follow-Up

Continue with `LGO-RUNTIME-UI-LIST-CARD-EVIDENCE-REFRESH-v1.0`: refresh screenshots and review Character Hall empty/selected list states after the list-card helper consolidation.
