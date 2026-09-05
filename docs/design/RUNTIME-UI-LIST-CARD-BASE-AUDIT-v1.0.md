# Runtime UI List Card Base Audit v1.0

Status: `LGO_RUNTIME_UI_LIST_CARD_BASE_READY`

## Purpose

Selectable rows and empty-state cards are core UI pieces that will recur across character, inventory, quest, mail, and social screens. This pass moves the Character Hall empty-card shell into the runtime UI factory and names list-button measurements, reducing controller-local style duplication.

## Ownership

- `RuntimeUiSpacing` owns list-button and empty-card hint spacing constants.
- `RuntimeUiFactory.NewEmptyCharacterCard` owns the empty character card shell.
- `RuntimeUiFactory.NewListButton` owns selectable list-button composition and now uses its `classId` argument.
- `M4PlayableClientController` continues to own character data, selection state, callbacks, and player-facing copy.

## Result

- Empty character card padding/frame setup moved out of the controller.
- List-button dimensions now use named spacing constants.
- List-button secondary text now comes from the supplied `classId` parameter.
- Account/character behavior and Vietnamese UI copy remain unchanged.

## Non-Claims

- No visual runtime PASS claim.
- No production art claim.
- No gameplay, auth, protocol, GameData schema, ADR, or design-token change.
