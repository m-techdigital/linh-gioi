# LGO Runtime UI Toggle Base Audit v1.0

Status: `LGO_RUNTIME_UI_TOGGLE_BASE_READY`

## Scope

This task reduces repeated settings-toggle sizing and state-pill measurements in runtime UI skin/factory paths.

## Changed

- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiSpacing.cs`
- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs`
- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs`
- `tools/validate_lgo_runtime_ui_toggle_base_audit.py`

## Result

- Added `RuntimeUiSpacing.SettingToggleMinHeight`.
- Added `RuntimeUiSpacing.SettingToggleMarginTop`.
- Added `RuntimeUiSpacing.SettingTogglePaddingHorizontal`.
- Added `RuntimeUiSpacing.SettingTogglePaddingVertical`.
- Added `RuntimeUiSpacing.SettingToggleFontSize`.
- Added `RuntimeUiSpacing.SettingTogglePillMinWidth`.
- Added `RuntimeUiSpacing.SettingTogglePillMarginLeft`.
- Added `RuntimeUiSpacing.SettingTogglePillPaddingHorizontal`.
- Added `RuntimeUiSpacing.SettingTogglePillPaddingTop`.
- Added `RuntimeUiSpacing.SettingTogglePillPaddingBottom`.
- Added `RuntimeUiSpacing.SettingTogglePillFontSize`.
- Added `RuntimeUiSpacing.SettingTogglePillRadius`.
- Routed setting-toggle row and pill metrics through the shared spacing constants.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No production art claim.
- No gameplay behavior change.
- No design-token JSON change.
- No protocol, GameData, or ADR change.

## Follow-Up

Continue with `LGO-RUNTIME-UI-TOGGLE-EVIDENCE-REFRESH-v1.0`: refresh session-menu screenshots and review local settings toggle readability after the helper consolidation.
