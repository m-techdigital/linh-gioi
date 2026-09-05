# LGO Combat Button Mobile Responsive Evidence v1.0

Status: `LGO_COMBAT_BUTTON_MOBILE_RESPONSIVE_EVIDENCE_READY`

## Scope

This pass tightens the visual evidence harness so the target-dummy checkpoint exposes the local combat action shell on compact profiles. Normal mobile HUD behavior remains compact; the forced display is only for screenshot evidence.

## Runtime Evidence

- `build/visual-evidence/profiles/desktop/target-dummy-state.png`
- `build/visual-evidence/profiles/tablet/target-dummy-state.png`
- `build/visual-evidence/profiles/mobile/target-dummy-state.png`
- `build/visual-evidence/profiles/index.md`

## Review Notes

- The target-dummy evidence checkpoint now maps directly to target dummy state clarity, cooldown ring, combat button fit, and local-only combat copy.
- Mobile/tablet evidence must show the compact combat button instead of silently hiding the action shell.
- The button keeps the short visible label `Hồi chiêu`; the fuller `Đang hồi chiêu` wording stays in tooltip/feedback context.
- Screenshot capture is evidence for human/runtime review; `VISUAL_RUNTIME_PASS` is not claimed from capture alone.

## Non-Claims

- No combat mechanic change.
- No cooldown timing, damage, target selection semantics, protocol, GameData, ADR, or design-token change.
- No new runtime image payload.
- No `VISUAL_RUNTIME_PASS` claim.
