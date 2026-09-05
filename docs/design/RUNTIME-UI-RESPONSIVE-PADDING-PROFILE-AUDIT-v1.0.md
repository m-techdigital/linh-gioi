# Runtime UI Responsive Padding Profile Audit v1.0

Status: `LGO_RUNTIME_UI_RESPONSIVE_PADDING_PROFILE_AUDIT_READY`

## Purpose

The playable UI should not re-declare the same mobile/tablet/desktop padding decisions inside screen controller code. This pass moves repeated responsive padding values for Character Hall, World HUD, dialogue, and top status chips into `RuntimeUiLayoutProfile`.

## Ownership

- `RuntimeUiLayoutProfile` owns profile-specific numeric layout decisions for PC/tablet/mobile.
- `RuntimeUiSkin.ApplyPadding` owns edge assignment mechanics.
- `M4PlayableClientController` applies state and visibility while consuming named profile properties.

## Consolidated Surfaces

- Lobby panel padding.
- Character list and empty card padding.
- Create-character panel padding.
- World HUD base and dialogue-compressed padding.
- Guidance card and dialogue panel vertical padding.
- Top status horizontal padding.

## Non-Claims

- No gameplay change.
- No visual asset payload change.
- No protocol, GameData schema, ADR, or design-token change.
- No `VISUAL_RUNTIME_PASS` claim from this source audit.

## Follow-Up

Refresh runtime evidence after this source pass to confirm profile-owned padding does not regress login, Character Hall, World HUD, dialogue, session menu, or combat HUD screenshots.
