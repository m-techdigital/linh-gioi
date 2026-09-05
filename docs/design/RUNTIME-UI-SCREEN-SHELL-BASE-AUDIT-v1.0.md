# Runtime UI Screen Shell Base Audit v1.0

Status: `LGO_RUNTIME_UI_SCREEN_SHELL_BASE_READY`

## Purpose

Character Hall shell setup was still assembled directly in `M4PlayableClientController`: base panel, frame, width, min-height, responsive padding, and alignment. This pass moves that repeatable shell composition into `RuntimeUiFactory.NewCharacterHallPanel` so future lobby/character work can reuse one base instead of rebuilding the same screen shell in controller code.

## Ownership

- `RuntimeUiFactory.NewCharacterHallPanel` owns Character Hall shell construction.
- `RuntimeUiSkin.ApplyCharacterHallPanelFrame` still owns the frame styling.
- `RuntimeUiLayoutProfile` still owns responsive padding values.
- `M4PlayableClientController` still owns flow, screen order, event wiring, account state, and character semantics.

## Result

- Character Hall panel construction now has a reusable factory entry point.
- Existing responsive layout refresh remains in the controller because it depends on current viewport state.
- Existing validators now check the new factory-owned shell path rather than stale controller-local construction.

## Non-Claims

- No visual runtime PASS claim.
- No production art claim.
- No gameplay, auth, protocol, GameData schema, ADR, or design-token change.
