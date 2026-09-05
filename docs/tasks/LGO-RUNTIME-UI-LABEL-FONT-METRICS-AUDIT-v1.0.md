# LGO Runtime UI Label Font Metrics Audit v1.0

Status: `LGO_RUNTIME_UI_LABEL_FONT_METRICS_READY`

## Scope

This task reduces controller-local label font-size assignments in login, Character Hall, world HUD, and dialogue UI.

## Changed

- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiSpacing.cs`
- `client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs`
- `tools/validate_lgo_runtime_ui_label_font_metrics_audit.py`

## Result

- Added named label font-size constants to `RuntimeUiTypography`.
- Routed durable label font-size assignments and initial `RuntimeUiSkin.ApplyText` calls in `M4PlayableClientController` through those constants.
- Kept copy, state, and flow ownership in the controller.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No visual redesign claim.
- No gameplay, auth, protocol, GameData, ADR, or design-token change.

## Follow-Up

Continue with `LGO-RUNTIME-UI-LABEL-FONT-METRICS-EVIDENCE-REFRESH-v1.0`: refresh login/lobby/world/dialogue screenshots and review typographic readability after label metric extraction.
