# LGO Login CTA Ornament Lightweight Pass v1.0

Marker: `LGO_LOGIN_CTA_ORNAMENT_LIGHTWEIGHT_READY`

## Goal

Polish the login CTA cluster using lightweight UI-native ornamentation so the first screen feels more intentional without adding image weight.

## Changes

- Added a small gold/cyan ornament rule above the server selector.
- Added a matching ornament rule below the main CTA button.
- Kept the existing V3B logo, background, button texture, and server copy.
- Avoided V3BA assets, composite slicing, and new PNG import.

## Evidence Targets

- `build/visual-evidence/latest/login.png`
- `build/visual-evidence/profiles/mobile/login.png`
- `build/visual-evidence/profiles/tablet/login.png`

## Non-Claims

- No production art claim.
- No auth/backend/DB change.
- No protocol, GameData schema, ADR, or design-token change.
- No `VISUAL_RUNTIME_PASS` claim from capture alone.

## Decision

`LGO_LOGIN_CTA_ORNAMENT_LIGHTWEIGHT_READY`
