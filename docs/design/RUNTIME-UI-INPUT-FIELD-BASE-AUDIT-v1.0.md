# Runtime UI Input Field Base Audit v1.0

Status: `LGO_RUNTIME_UI_INPUT_FIELD_BASE_READY`

## Purpose

Runtime login and character-management forms should not hand-write input dimensions and padding in separate places. This pass moves reusable `TextField` metrics into shared runtime UI primitives while keeping field labels, values, visibility, and account/character semantics unchanged.

## Ownership

- `RuntimeUiSpacing` owns named input size, margin, and padding constants.
- `RuntimeUiSkin.ApplyInputMetrics` owns shared `TextField` max-width, min-height, margin, and text color application.
- `RuntimeUiFactory` owns field construction and lobby input skinning.
- `M4PlayableClientController` continues to own which fields appear for login, character creation, and runtime flow state.

## Result

- `NewTextField` now uses helper-owned input max-width, margin, and text color.
- `ApplyLobbyInputStyle` now uses helper-owned input min-height plus named padding constants.
- Existing Vietnamese labels and default values are unchanged.

## Non-Claims

- No visual runtime PASS claim.
- No production art claim.
- No gameplay, auth, protocol, GameData schema, ADR, or design-token change.
