# Runtime UI Status Chip Base Audit v1.0

Status: `LGO_RUNTIME_UI_STATUS_CHIP_BASE_READY`

## Purpose

Status chips and state labels appear across login, lobby, HUD, session menu, and combat prototype panels. This pass moves shared chip measurements into named spacing constants and routes dynamic accent changes through `RuntimeUiSkin`, keeping factory/controller responsibilities narrow.

## Ownership

- `RuntimeUiSpacing` owns status-chip max-width and padding constants.
- `RuntimeUiSkin.ApplyStatusAccent` owns dynamic status accent color application.
- `RuntimeUiFactory.ApplyStatusChip` owns status-chip composition and frame application.
- `M4PlayableClientController` continues to choose status text and status state.

## Result

- Status-chip width and padding use named constants.
- Dynamic combat/status accent changes delegate through skin.
- HUD/lobby/session text and behavior remain unchanged.

## Non-Claims

- No visual runtime PASS claim.
- No production art claim.
- No gameplay, auth, protocol, GameData schema, ADR, or design-token change.
