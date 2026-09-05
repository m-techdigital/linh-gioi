# LGO Character Hall Style Adoption Pass v1.0

Status: `LGO_CHARACTER_HALL_STYLE_ADOPTION_READY`

## Scope

This pass moves Character Hall skin rules into reusable `RuntimeUiSkin` role helpers while leaving layout, responsive behavior, account flow, and character semantics intact.

## Changes

- Added shared skin helpers for the Character Hall main panel, character list, selected preview card, create panel, portrait frame, and lobby input frame.
- Updated `M4PlayableClientController` so Character Hall factories keep structure and sizing while `RuntimeUiSkin` owns color, border, and glass-frame rules.
- Updated validators so historical Character Hall checks assert the new shared-skin ownership instead of requiring repeated literal style assignments.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No new runtime image payload.
- No account/character semantics change.
- No gameplay, protocol, GameData, ADR, or design-token change.

## Follow-Up

Continue with `LGO-WORLD-HUD-STYLE-ADOPTION-PASS-v1.0`: migrate world HUD/dialogue/session panel style rules into shared helpers where it does not change gameplay behavior.
