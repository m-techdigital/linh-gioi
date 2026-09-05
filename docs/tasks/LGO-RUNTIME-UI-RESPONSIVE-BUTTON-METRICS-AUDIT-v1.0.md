# LGO Runtime UI Responsive Button Metrics Audit v1.0

Status: `LGO_RUNTIME_UI_RESPONSIVE_BUTTON_METRICS_READY`

## Scope

This task reduces controller-local responsive button numeric styling in the Character Hall action row.

## Changed

- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiSpacing.cs`
- `client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs`
- `tools/validate_lgo_runtime_ui_responsive_button_metrics_audit.py`

## Result

- Added named Character Hall action button metrics in `RuntimeUiSpacing`.
- Routed create and enter-world button metric application through `RuntimeUiSkin.ApplyButtonMetrics`.
- Kept selected-state ordering and Vietnamese button copy in `M4PlayableClientController`.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No gameplay behavior change.
- No account/character-flow semantic change.
- No protocol, GameData, ADR, or design-token change.

## Follow-Up

Continue with `LGO-RUNTIME-UI-RESPONSIVE-BUTTON-METRICS-EVIDENCE-REFRESH-v1.0`: refresh Character Hall/login/world runtime screenshots and review responsive CTA readability after metric extraction.
