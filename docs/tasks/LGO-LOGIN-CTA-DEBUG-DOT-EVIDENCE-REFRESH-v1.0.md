# LGO Login CTA Debug Dot Evidence Refresh v1.0

Status: `LGO_LOGIN_CTA_DEBUG_DOT_EVIDENCE_REFRESH_READY`

## Scope

This pass refreshes runtime profile screenshots after the login CTA debug-dot cleanup.

## Evidence

- `build/visual-evidence/profiles/desktop/login.png`
- `build/visual-evidence/profiles/tablet/login.png`
- `build/visual-evidence/profiles/mobile/login.png`
- `build/visual-evidence/profiles/index.md`

## Review Notes

- Login CTA stack no longer shows tiny cyan square ornaments that read like editor handles.
- The server online dot remains intentional status UI.
- No new runtime image payload was added.

## Non-Claims

- No VISUAL_RUNTIME_PASS claim.
- No gameplay, auth, or account-flow change.
- No protocol, GameData schema, ADR, or design-token change.

## Follow-Up

Continue with a small login panel balance pass if the translucent CTA backing still feels too rectangular against the V3B art direction.
