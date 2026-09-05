# LGO Runtime UI Toggle Evidence Refresh v1.0

Status: `LGO_RUNTIME_UI_TOGGLE_EVIDENCE_REFRESH_READY`

## Scope

This evidence refresh records runtime screenshots after routing local setting toggle row and state-pill metrics through shared runtime UI spacing constants.

## Evidence

- `build/visual-evidence/latest/session-menu.png`
- `build/visual-evidence/latest/world-hub.png`
- `build/visual-evidence/latest/visual-runtime-evidence-review.md`
- `build/visual-evidence/latest/visual-runtime-evidence-heuristics.json`

## Review Notes

- Session Menu setting rows remain readable after `SettingToggle*` metric extraction.
- `Bật` / `Tắt` state pills remain aligned to the row end and keep enough touch target space.
- Toggle labels remain Vietnamese and readable against the dark V3B-style panel.
- World HUD behind the menu remains visually subdued and does not interfere with the settings shell.
- The change is a UI ownership cleanup only; no setting semantics, save behavior, gameplay, or runtime image payload changed.
- Screenshot evidence was captured and reviewed, but `VISUAL_RUNTIME_PASS` is not claimed from capture/build alone.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No runtime image payload change.
- No gameplay, auth, protocol, GameData, ADR, or design-token change.
- No production auth, DB, economy, social, or liveops work.

## Follow-Up

Continue with `LGO-RUNTIME-UI-ICON-STATUS-ROW-BASE-AUDIT-v1.0`: audit icon/status row spacing so rows combining icons, cooldown rings, and compact labels use named layout constants instead of local literals.
