# LGO Login CTA Debug Dot Cleanup Pass v1.0

Status: `LGO_LOGIN_CTA_DEBUG_DOT_CLEANUP_READY`

## Scope

This pass removes debug-looking cyan square ornaments around the login CTA stack and keeps the login first screen lightweight.

## Changes

- Replaced the tiny cyan ornament diamond with a quiet gold rule.
- Kept the V3B logo, background, server selector, and CTA button behavior unchanged.
- Added no new runtime image payload.

## Non-Claims

- No VISUAL_RUNTIME_PASS claim.
- No gameplay, auth, or account-flow change.
- No new runtime image import.
- No protocol, GameData schema, ADR, or design-token change.

## Follow-Up

Refresh desktop/tablet/mobile login screenshots and inspect `login.png` for hierarchy, spacing, and debug-looking artifacts.
