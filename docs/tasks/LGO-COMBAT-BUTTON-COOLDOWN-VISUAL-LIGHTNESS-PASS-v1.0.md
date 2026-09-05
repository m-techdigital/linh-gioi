# LGO Combat Button Cooldown Visual Lightness Pass v1.0

Status: `LGO_COMBAT_BUTTON_COOLDOWN_VISUAL_LIGHTNESS_READY`

## Changed

- Adjusted `RuntimeUiFactory.ApplyCombatButtonSkin` so the local combat cooldown button uses a lightweight code-styled glass state instead of the visually heavy dark cooldown texture.
- Kept the ready/pressed button texture behavior intact.
- Kept cooldown ring/icon state unchanged so combat readability remains explicit.

## Boundaries

- No combat mechanic, damage, cooldown timing, target selection, protocol, or server-authoritative behavior change.
- No visual asset import, deletion, or image generation.
- No frozen contract surface change.
- No `VISUAL_RUNTIME_PASS` claim until runtime screenshots are captured and reviewed.

## Next

`LGO-COMBAT-BUTTON-COOLDOWN-VISUAL-EVIDENCE-REFRESH-v1.0`
