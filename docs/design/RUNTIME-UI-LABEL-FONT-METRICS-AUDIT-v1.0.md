# Runtime UI Label Font Metrics Audit v1.0

Status: `LGO_RUNTIME_UI_LABEL_FONT_METRICS_READY`

## Purpose

Runtime label typography needs shared metric ownership so login, Character Hall, world HUD, and dialogue text stay coherent across desktop, tablet, and mobile profiles.

## Ownership

- `RuntimeUiTypography` owns reusable runtime UI label font sizes.
- `RuntimeUiLayoutProfile` continues to own profile-specific layout dimensions and panel spacing.
- `RuntimeUiSkin.ApplyText` remains the shared text color/alignment/style helper.
- `M4PlayableClientController` keeps Vietnamese copy, visibility, flow state, and callbacks.

## Result

- Login hero, server, API, and account status font sizes use named typography constants.
- Character Hall intro/empty-state font sizes use named constants.
- Character selected-name font size uses a named typography constant.
- World HUD title/objective/interaction/meta compact font sizes use named typography constants.
- Dialogue speaker/line/progress font sizes use named typography constants.

## Non-Claims

- No visual runtime PASS claim.
- No Vietnamese copy or flow semantic change.
- No gameplay, auth, protocol, GameData schema, ADR, or design-token change.
