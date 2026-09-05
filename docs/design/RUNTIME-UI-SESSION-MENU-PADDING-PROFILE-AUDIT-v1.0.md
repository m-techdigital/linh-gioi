# Runtime UI Session Menu Padding Profile Audit v1.0

Status: `LGO_RUNTIME_UI_SESSION_MENU_PADDING_PROFILE_AUDIT_READY`

## Purpose

The session menu is a recurring shell for pause/settings and should use the same profile-owned layout vocabulary as login, Character Hall, World HUD, and dialogue panels.

## Change

Session menu padding is now exposed through `RuntimeUiLayoutProfile`:

- `SessionMenuPaddingHorizontal`
- `SessionMenuPaddingTop`
- `SessionMenuPaddingBottom`

`M4PlayableClientController` consumes those values through `RuntimeUiSkin.ApplyPadding` instead of carrying a nested mobile/tablet/desktop ternary at the callsite.

## Non-Claims

- No gameplay change.
- No settings behavior change.
- No visual asset payload change.
- No protocol, GameData schema, ADR, or design-token change.
- No `VISUAL_RUNTIME_PASS` claim from this source audit.

## Follow-Up

Refresh runtime screenshots for the session menu and adjacent World HUD states before continuing to broader component cleanup.
