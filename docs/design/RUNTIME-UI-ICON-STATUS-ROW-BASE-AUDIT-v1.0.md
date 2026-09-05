# Runtime UI Icon Status Row Base Audit v1.0

Status: `LGO_RUNTIME_UI_ICON_STATUS_ROW_BASE_READY`

## Purpose

Icon/status rows appear in the world HUD and combat-readiness shell where an icon, cooldown ring, or marker sits beside compact status labels. This pass moves the row bottom margin and row inset padding into `RuntimeUiSpacing` so future HUD rows do not reintroduce local spacing literals.

## Ownership

- `RuntimeUiSpacing` owns icon/status row bottom margin and row inset padding.
- `RuntimeUiFactory.NewIconStatusRow` owns the shared row structure and status-column layout.
- `RuntimeUiSkin.ApplyPadding` remains the shared style mutation helper.
- `M4PlayableClientController` continues to choose the icon, labels, and combat/local-feedback state.

## Result

- Icon/status row bottom margin uses a named spacing constant.
- Icon/status row horizontal/top/bottom padding uses named spacing constants.
- Existing combat-readiness label text, cooldown icon, and state behavior remain unchanged.

## Non-Claims

- No visual runtime PASS claim.
- No production art claim.
- No gameplay, auth, protocol, GameData schema, ADR, or design-token change.
