# LGO Runtime UI Header Dialogue Button Metrics Evidence Refresh v1.0

Status: `LGO_RUNTIME_UI_HEADER_DIALOGUE_BUTTON_METRICS_EVIDENCE_REFRESH_READY`

## Scope

This evidence refresh records runtime screenshots after top-header status/quit metrics and dialogue button dimensions moved from controller-local numeric assignments to shared runtime UI spacing constants.

## Evidence

- `build/visual-evidence/latest/npc-dialogue.png`
- `build/visual-evidence/latest/session-menu.png`
- `build/visual-evidence/latest/world-hub.png`
- `build/visual-evidence/latest/character-select.png`
- `build/visual-evidence/latest/visual-runtime-evidence-review.md`
- `build/visual-evidence/latest/visual-runtime-evidence-heuristics.json`

## Review Notes

- NPC dialogue actions remain readable and correctly grouped after metric extraction.
- Session menu actions and top-header status/quit controls remain readable with no observed overlap.
- World HUD top status remains compact and does not regress the left HUD panel.
- Screenshot evidence was captured and reviewed, but `VISUAL_RUNTIME_PASS` is not claimed from capture/build alone.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No visual redesign claim.
- No gameplay, dialogue, session, auth, protocol, GameData, ADR, or design-token change.

## Follow-Up

Continue with `LGO-RUNTIME-UI-LABEL-FONT-METRICS-AUDIT-v1.0`: audit remaining controller-local responsive label font-size values and move stable values into shared runtime UI ownership.
