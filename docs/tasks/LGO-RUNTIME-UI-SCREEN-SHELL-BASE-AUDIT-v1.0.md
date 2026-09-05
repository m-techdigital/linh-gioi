# LGO Runtime UI Screen Shell Base Audit v1.0

Status: `LGO_RUNTIME_UI_SCREEN_SHELL_BASE_READY`

## Scope

This task extracts the Character Hall screen shell setup into a reusable runtime UI factory helper.

## Changed

- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs`
- `client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs`
- `tools/validate_lgo_character_hall_panel_density.py`
- `tools/validate_lgo_character_hall_style_adoption.py`
- `tools/validate_lgo_runtime_ui_screen_shell_base_audit.py`

## Result

- Added `RuntimeUiFactory.NewCharacterHallPanel(RuntimeUiLayoutProfile layout)`.
- Replaced the controller-local Character Hall shell setup with `NewCharacterHallPanel(layout)`.
- Kept responsive runtime layout updates in the controller.
- Updated stale validators to verify the factory-owned shell path.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No production art claim.
- No gameplay behavior change.
- No design-token JSON change.
- No protocol, GameData, or ADR change.

## Follow-Up

Continue with `LGO-RUNTIME-UI-SCREEN-SHELL-EVIDENCE-REFRESH-v1.0`: refresh runtime screenshots and review Character Hall plus adjacent flow checkpoints after the shell extraction.
