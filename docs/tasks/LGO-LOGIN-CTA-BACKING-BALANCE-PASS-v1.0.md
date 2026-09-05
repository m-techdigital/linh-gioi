# LGO Login CTA Backing Balance Pass v1.0

Status: `LGO_LOGIN_CTA_BACKING_BALANCE_READY`

## Scope

This pass softens the login CTA backing panel so the first screen reads closer to the V3B reference direction without adding image payload.

## Changes

- Reduced the login CTA backing opacity across desktop, tablet, and mobile profiles.
- Rounded and de-emphasized the panel border so the server selector and gold CTA carry the visual focus.
- Reduced CTA stack height and padding to preserve background breathing room on compact screens.

## Non-Claims

- No VISUAL_RUNTIME_PASS claim.
- No new runtime image import.
- No gameplay, auth, account-flow, protocol, GameData schema, ADR, or design-token change.

## Follow-Up

Refresh profile screenshots and inspect `desktop/login.png`, `tablet/login.png`, and `mobile/login.png` for backing weight, readability, and V3B visual hierarchy.
