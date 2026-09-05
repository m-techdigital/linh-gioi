# LGO Runtime UI Responsive Padding Profile Audit v1.0

Status: `LGO_RUNTIME_UI_RESPONSIVE_PADDING_PROFILE_AUDIT_READY`

## Scope

Move repeated responsive padding decisions out of `M4PlayableClientController` and into `RuntimeUiLayoutProfile`, while preserving current UI layout values and runtime semantics.

## Changes

- Added named profile padding properties for lobby, character list, empty character card, create panel, World HUD, dialogue, and top status chips.
- Updated `M4PlayableClientController` to consume those profile properties through `RuntimeUiSkin.ApplyPadding`.
- Kept controller ownership limited to runtime state, visibility, copy, and event flow.

## Validation

- `python3.12 tools/validate_lgo_runtime_ui_responsive_padding_profile_audit.py`
- `git --no-pager diff --check`
- `./tools/lgo_playable_closure_check.sh --source-only`

## Non-Claims

- No gameplay change.
- No new art or asset payload change.
- No `VISUAL_RUNTIME_PASS` claim.
