# LGO Runtime UI Panel Hierarchy Evidence Refresh v1.0

Status: `LGO_RUNTIME_UI_PANEL_HIERARCHY_EVIDENCE_REFRESH_READY`

## Scope

This evidence refresh records real Unity Player screenshots after login CTA backing and Character Hall nested panels were softened through shared runtime UI skin helpers.

## Evidence

- `build/visual-evidence/latest/login.png`
- `build/visual-evidence/latest/character-lobby.png`
- `build/visual-evidence/latest/character-select.png`
- `build/visual-evidence/latest/world-hub.png`
- `build/visual-evidence/latest/npc-dialogue.png`
- `build/visual-evidence/latest/session-menu.png`
- `build/visual-evidence/latest/visual-runtime-evidence-review.md`
- `build/visual-evidence/latest/visual-runtime-evidence-heuristics.json`

## Review Notes

- Login CTA backing is calmer and no longer competes as strongly with the V3B logo or gold enter-world button.
- Character Hall nested list/profile/create frames are less high-contrast and read more like one composed shell.
- Text remains readable in login, Character Hall, world, NPC dialogue, and session menu screenshots.
- The world hub still needs future visual/background depth work; that is outside this panel hierarchy pass.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No gameplay, account/character semantics, auth, protocol, GameData, ADR, design-token, or asset payload change.

## Follow-Up

Continue with `LGO-RUNTIME-UI-CHARACTER-HALL-CONTENT-DENSITY-POLISH-v1.0`: simplify Character Hall text density and repeated status rows while preserving all existing character creation/selection behavior.
