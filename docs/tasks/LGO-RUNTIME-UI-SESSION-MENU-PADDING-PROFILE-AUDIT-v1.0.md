# LGO Runtime UI Session Menu Padding Profile Audit v1.0

Status: `LGO_RUNTIME_UI_SESSION_MENU_PADDING_PROFILE_AUDIT_READY`

## Scope

Move session-menu responsive padding values into `RuntimeUiLayoutProfile`, preserving current desktop/tablet/mobile layout values.

## Changes

- Added named session menu padding profile properties.
- Updated `M4PlayableClientController` to apply session menu padding through profile properties and `RuntimeUiSkin.ApplyPadding`.
- Updated the older controller style constants validator to expect the profile-owned callsite.

## Validation

- `python3.12 tools/validate_lgo_runtime_ui_session_menu_padding_profile_audit.py`
- `git --no-pager diff --check`
- `./tools/lgo_playable_closure_check.sh --source-only`

## Non-Claims

- No gameplay change.
- No visual asset payload change.
- No `VISUAL_RUNTIME_PASS` claim.
