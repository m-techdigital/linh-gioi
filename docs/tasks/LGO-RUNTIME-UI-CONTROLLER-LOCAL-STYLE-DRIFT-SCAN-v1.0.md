# LGO Runtime UI Controller Local Style Drift Scan v1.0

Status: `LGO_RUNTIME_UI_CONTROLLER_LOCAL_STYLE_DRIFT_SCAN_READY`

## Scope

This task reduced controller-local style duplication without moving runtime flow ownership out of `M4PlayableClientController`.

## Changed

- `client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs`
- `docs/design/RUNTIME-UI-CONTROLLER-LOCAL-STYLE-DRIFT-SCAN-v1.0.md`
- `tools/validate_lgo_runtime_ui_controller_local_style_drift_scan.py`

## Result

- Screen mode visibility now uses `SetDisplayed`.
- Session menu and dialogue visibility now use `SetDisplayed`.
- Compact viewport overlay hiding now uses `SetElementVisibility`.
- Visibility checks now use `IsDisplayed`.

## Non-Claims

- No gameplay behavior change.
- No visual redesign.
- No production art claim.
- No `VISUAL_RUNTIME_PASS` claim.
- No design-token JSON change.
- No protocol, GameData, or ADR change.

## Follow-Up

Continue with `LGO-RUNTIME-UI-CONTROLLER-LOCAL-STYLE-EVIDENCE-REFRESH-v1.0`: refresh runtime screenshots after the controller visibility helper cleanup.
