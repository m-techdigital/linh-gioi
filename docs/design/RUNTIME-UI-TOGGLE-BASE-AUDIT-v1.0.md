# Runtime UI Toggle Base Audit v1.0

Status: `LGO_RUNTIME_UI_TOGGLE_BASE_READY`

## Purpose

Settings toggles are a reusable runtime UI primitive for session menu display options and future client-side preferences. This pass moves local toggle row and state-pill measurements into `RuntimeUiSpacing` so rows can stay visually consistent across desktop, tablet, and mobile without copy-written sizes in controllers.

## Ownership

- `RuntimeUiSpacing` owns setting-toggle row and state-pill measurements.
- `RuntimeUiSkin.ApplySettingToggleFrame` owns toggle row frame, typography, and base visual treatment.
- `RuntimeUiSkin.ApplySettingToggleStatePill` owns the `Bật` / `Tắt` state pill metrics and frame.
- `RuntimeUiFactory.NewLocalSettingToggle` owns construction, callback wiring, and state refresh.
- `M4PlayableClientController` continues to choose the setting labels and apply session-display behavior.

## Result

- Toggle row min-height, margin-top, padding, and font-size use named constants.
- Toggle state-pill min-width, margin, padding, font-size, and radius use named constants.
- Existing Vietnamese labels, setting defaults, callback flow, and local-session semantics remain unchanged.

## Non-Claims

- No visual runtime PASS claim.
- No production art claim.
- No gameplay, auth, protocol, GameData schema, ADR, or design-token change.
