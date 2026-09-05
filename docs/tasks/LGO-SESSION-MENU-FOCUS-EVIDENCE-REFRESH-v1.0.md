# LGO Session Menu Focus Evidence Refresh v1.0

Status: `LGO_SESSION_MENU_FOCUS_EVIDENCE_REFRESH_READY`

## Scope

This pass refreshes desktop/tablet/mobile runtime screenshots after the session-menu focus cleanup and records the visual review result.

## Evidence

- `build/visual-evidence/profiles/desktop/session-menu.png`
- `build/visual-evidence/profiles/tablet/session-menu.png`
- `build/visual-evidence/profiles/mobile/session-menu.png`
- `build/visual-evidence/profiles/index.md`
- `build/visual-evidence/profiles/index.json`

## Review Notes

- Mobile session menu now owns the focus area without the dialogue panel or header action chips competing with it.
- Tablet session menu uses a wide focus sheet with the world HUD hidden while the menu is open.
- Desktop keeps the floating pause-panel behavior.
- World context remains visible outside the compact pause sheet, but gameplay/HUD controls no longer overlap the active menu.

## Non-Claims

- No VISUAL_RUNTIME_PASS claim.
- No gameplay change.
- No new runtime image import.
- No protocol, GameData schema, ADR, or design-token change.

## Follow-Up

Continue with `LGO-CHARACTER-HALL-MOBILE-COPY-DENSITY-PASS-v1.0` to reduce dense mobile lobby copy and preserve the same responsive/evidence loop.
