# LGO Runtime UI Icon Status Row Base Audit v1.0

Status: `LGO_RUNTIME_UI_ICON_STATUS_ROW_BASE_READY`

## Scope

This task reduces local spacing literals in the shared icon/status row used by compact HUD and combat-readiness UI.

## Changed

- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiSpacing.cs`
- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs`
- `tools/validate_lgo_runtime_ui_icon_status_row_base_audit.py`

## Result

- Added `RuntimeUiSpacing.IconStatusRowMarginBottom`.
- Added `RuntimeUiSpacing.IconStatusRowPaddingHorizontal`.
- Added `RuntimeUiSpacing.IconStatusRowPaddingTop`.
- Added `RuntimeUiSpacing.IconStatusRowPaddingBottom`.
- Routed `NewIconStatusRow` row margin/padding through shared spacing constants.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No production art claim.
- No gameplay behavior change.
- No design-token JSON change.
- No protocol, GameData, or ADR change.

## Follow-Up

Continue with `LGO-RUNTIME-UI-ICON-STATUS-ROW-EVIDENCE-REFRESH-v1.0`: refresh world/combat screenshots and review icon/status row readability after the helper consolidation.
