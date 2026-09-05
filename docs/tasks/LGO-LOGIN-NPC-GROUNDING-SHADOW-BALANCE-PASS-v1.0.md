# LGO Login NPC Grounding Shadow Balance Pass v1.0

Status: `LGO_LOGIN_NPC_GROUNDING_SHADOW_BALANCE_READY`

## Scope

This pass softens the Gate Keeper grounding shadow on the login screen so the NPC feels staged into the V3B background rather than placed on a dark UI smear.

## Changes

- Added a retained `_loginNpcGrounding` reference for profile-aware styling.
- Reduced shadow width, height, alpha, and opacity on desktop and tablet.
- Hid the shadow on mobile because the NPC stage is not shown there.
- Reused `RuntimeUiSkin.ApplyRadius` instead of duplicating radius assignments.

## Non-Claims

- No VISUAL_RUNTIME_PASS claim.
- No new runtime image payload.
- No gameplay, auth, account-flow, protocol, GameData schema, ADR, or design-token change.

## Follow-Up

Refresh desktop/tablet/mobile login screenshots and inspect whether the Gate Keeper grounding feels scene-integrated.
