# Runtime UI Header Dialogue Button Metrics Audit v1.0

Status: `LGO_RUNTIME_UI_HEADER_DIALOGUE_BUTTON_METRICS_READY`

## Purpose

Top header actions and dialogue buttons need shared metric ownership so responsive UI tuning stays consistent across login, lobby, world, dialogue, and session flows.

## Ownership

- `RuntimeUiSpacing` owns top-status, header quit, and dialogue button dimensions/font metrics.
- `RuntimeUiSkin.ApplyButtonMetrics` owns button metric application.
- `RuntimeUiLayoutProfile` still owns profile-level panel spacing and padding.
- `M4PlayableClientController` keeps visibility, Vietnamese copy, world/dialogue state, and callbacks.

## Result

- Dialogue continue/close button dimensions use named constants.
- Header quit button dimensions and font size use named constants through `RuntimeUiSkin.ApplyButtonMetrics`.
- Top status chip font/height/max-width breakpoints use named constants.
- Header actions max-width breakpoints use named constants.

## Non-Claims

- No visual runtime PASS claim.
- No gameplay behavior change.
- No dialogue semantic change.
- No auth, protocol, GameData schema, ADR, or design-token change.
