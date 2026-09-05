# LGO Runtime UI Header Dialogue Button Metrics Audit v1.0

Status: `LGO_RUNTIME_UI_HEADER_DIALOGUE_BUTTON_METRICS_READY`

## Scope

This task reduces controller-local numeric styling in top-header status/quit actions and dialogue action buttons.

## Changed

- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiSpacing.cs`
- `client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs`
- `tools/validate_lgo_world_hud_dialogue_panel_viewport_polish.py`
- `tools/validate_lgo_runtime_ui_header_dialogue_button_metrics_audit.py`

## Result

- Added named header action/status metrics in `RuntimeUiSpacing`.
- Added named dialogue action button metrics in `RuntimeUiSpacing`.
- Routed dialogue and quit button metrics through `RuntimeUiSkin.ApplyButtonMetrics`.
- Updated the stale dialogue viewport validator to check named metrics instead of old literals.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No gameplay behavior change.
- No dialogue flow semantic change.
- No auth, protocol, GameData, ADR, or design-token change.

## Follow-Up

Continue with `LGO-RUNTIME-UI-HEADER-DIALOGUE-BUTTON-METRICS-EVIDENCE-REFRESH-v1.0`: refresh dialogue/session/world runtime screenshots and review header/dialogue readability after metric extraction.
