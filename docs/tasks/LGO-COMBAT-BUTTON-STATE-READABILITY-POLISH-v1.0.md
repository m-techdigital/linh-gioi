# LGO Combat Button State Readability Polish v1.0

Status: `LGO_COMBAT_BUTTON_STATE_READABILITY_POLISH_READY`

## Scope

This pass improves local combat button readability during cooldown without changing combat mechanics.

## Implementation Notes

- `ApplyCombatButtonSkin` is now a reusable `RuntimeUiFactory` visual helper; the controller still owns local-only combat state and copy.
- Added state-aware min width, padding, font size, and no-wrap styling to the combat action button.
- Shortened the visible cooldown button copy from `Đang hồi chiêu` to `Hồi chiêu` so it fits the compact HUD.
- Kept `Đang hồi chiêu` in tooltip/explanatory copy so existing Vietnamese cooldown semantics remain clear.

## Non-Claims

- No combat mechanic change.
- No cooldown timing, damage, targeting, server authority, protocol, GameData, ADR, or design-token change.
- No new runtime image payload.
- No `VISUAL_RUNTIME_PASS` claim.

## Follow-Up

Continue with `LGO-COMBAT-BUTTON-STATE-EVIDENCE-REFRESH-v1.0`: capture and review target dummy cooldown screenshots after button-state readability polish.
