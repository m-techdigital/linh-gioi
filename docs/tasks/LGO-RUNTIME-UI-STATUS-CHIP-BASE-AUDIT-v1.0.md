# LGO Runtime UI Status Chip Base Audit v1.0

Status: `LGO_RUNTIME_UI_STATUS_CHIP_BASE_READY`

## Scope

This task reduces repeated status-chip sizing and accent mutation in runtime UI factory paths.

## Changed

- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiSpacing.cs`
- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs`
- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs`
- `tools/validate_lgo_runtime_ui_status_chip_base_audit.py`

## Result

- Added `RuntimeUiSpacing.StatusChipMaxWidth`.
- Added `RuntimeUiSpacing.StatusChipPaddingHorizontal`.
- Added `RuntimeUiSpacing.StatusChipPaddingVertical`.
- Added `RuntimeUiSkin.ApplyStatusAccent`.
- Kept `RuntimeUiFactory.ApplyStatusAccent` as the controller-facing API while delegating style mutation to skin.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No production art claim.
- No gameplay behavior change.
- No design-token JSON change.
- No protocol, GameData, or ADR change.

## Follow-Up

Continue with `LGO-RUNTIME-UI-STATUS-CHIP-EVIDENCE-REFRESH-v1.0`: refresh screenshots and review top status, Character Hall state rows, World HUD state rows, and combat feedback labels after the helper consolidation.
