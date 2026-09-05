# LGO Runtime UI Input Field Base Audit v1.0

Status: `LGO_RUNTIME_UI_INPUT_FIELD_BASE_READY`

## Scope

This task reduces repeated `TextField` sizing and padding setup in runtime UI factory paths.

## Changed

- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiSpacing.cs`
- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs`
- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs`
- `tools/validate_lgo_runtime_ui_input_field_base_audit.py`

## Result

- Added reusable input constants for base max-width, min-height, margin, and padding.
- Added `RuntimeUiSkin.ApplyInputMetrics`.
- Routed `NewTextField` and `ApplyLobbyInputStyle` through the shared helper/token path.
- Kept login/account and character-create field semantics unchanged.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No production art claim.
- No gameplay behavior change.
- No design-token JSON change.
- No protocol, GameData, or ADR change.

## Follow-Up

Continue with `LGO-RUNTIME-UI-INPUT-FIELD-EVIDENCE-REFRESH-v1.0`: refresh runtime screenshots and review Login hidden-dev field safety, Character Hall create form fit, and responsive form readability after the input helper consolidation.
