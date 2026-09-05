# LGO Runtime UI Icon Status Row Evidence Refresh v1.0

Status: `LGO_RUNTIME_UI_ICON_STATUS_ROW_EVIDENCE_REFRESH_READY`

## Scope

This evidence refresh records runtime screenshots after routing shared icon/status row margin and padding through named runtime UI spacing constants.

## Evidence

- `build/visual-evidence/latest/world-hub.png`
- `build/visual-evidence/latest/target-dummy-state.png`
- `build/visual-evidence/latest/visual-runtime-evidence-review.md`
- `build/visual-evidence/latest/visual-runtime-evidence-heuristics.json`

## Review Notes

- World HUD status rows remain compact and readable after `NewIconStatusRow` spacing extraction.
- Target dummy combat-readiness row keeps the cooldown icon, target status, and range status aligned.
- The icon/status column still wraps long Vietnamese copy without overlapping the action button.
- The change is a UI ownership cleanup only; no combat behavior, target state, or runtime image payload changed.
- Screenshot evidence was captured and reviewed, but `VISUAL_RUNTIME_PASS` is not claimed from capture/build alone.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No runtime image payload change.
- No gameplay, auth, protocol, GameData, ADR, or design-token change.
- No production auth, DB, economy, social, or liveops work.

## Follow-Up

Continue with `LGO-RUNTIME-UI-COMBAT-BUTTON-METRICS-AUDIT-v1.0`: audit combat button width, height, font, and padding metrics so cooldown/ready states use named spacing constants instead of local literals.
