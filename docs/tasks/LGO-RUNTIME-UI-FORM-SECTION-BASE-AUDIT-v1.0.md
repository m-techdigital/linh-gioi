# LGO Runtime UI Form Section Base Audit v1.0

Status: `LGO_RUNTIME_UI_FORM_SECTION_BASE_READY`

## Scope

This task reduces repeated form-section setup around the Character Hall create panel.

## Changed

- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiSizing.cs`
- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs`
- `client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs`
- `tools/validate_lgo_runtime_ui_form_section_base_audit.py`
- stale Character Hall validators updated to the factory-owned shell path.

## Result

- Added `RuntimeUiSizing.CharacterCreatePanelMinHeight`.
- Added `RuntimeUiSizing.CharacterCreatePanelMaxHeight`.
- Added `RuntimeUiFactory.NewCharacterCreatePanel`.
- Replaced controller-local create-panel construction with the reusable factory helper.
- Kept form labels, default values, buttons, callbacks, and responsive mobile positioning unchanged.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No production art claim.
- No gameplay behavior change.
- No design-token JSON change.
- No protocol, GameData, or ADR change.

## Follow-Up

Continue with `LGO-RUNTIME-UI-FORM-SECTION-EVIDENCE-REFRESH-v1.0`: refresh screenshots and review Character Hall empty/create/selected states after the shell extraction.
