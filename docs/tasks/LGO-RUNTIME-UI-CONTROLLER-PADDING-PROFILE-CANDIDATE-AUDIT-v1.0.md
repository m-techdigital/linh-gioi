# LGO Runtime UI Controller Padding Profile Candidate Audit v1.0

Status: `LGO_RUNTIME_UI_CONTROLLER_PADDING_PROFILE_CANDIDATE_READY`

## Scope

This pass moves safe remaining rectangular padding assignments in `M4PlayableClientController` into `RuntimeUiLayoutProfile` and applies them through `RuntimeUiSkin.ApplyPadding`.

## Implementation Notes

- Added a controller-local `CurrentLayoutProfile()` helper.
- Added named profile values for auth bottom padding, character list horizontal padding, position chip padding, local combat panel padding, settings panel padding, world guidance horizontal padding, dialogue progress horizontal padding, and top status vertical padding.
- Kept visual values unchanged.
- Kept gameplay, server calls, and combat semantics unchanged.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No new art import.
- No gameplay, protocol, GameData, ADR, or design-token change.
- No production auth, DB, economy, social, or liveops work.

## Follow-Up

Continue with `LGO-RUNTIME-UI-CONTROLLER-PADDING-PROFILE-EVIDENCE-REFRESH-v1.0`: refresh login, Character Hall, World HUD, session menu, and target dummy screenshots after profile cleanup.
