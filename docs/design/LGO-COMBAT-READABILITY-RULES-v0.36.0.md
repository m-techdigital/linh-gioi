# LGO Combat Readability Rules v0.36.0

These rules apply to local-only combat readability polish after v0.35.0.

## Target Highlight Rules

The training dummy should remain easy to identify from the safe yard. Highlight, selected, hit, recover, and disabled states must be visually distinct without implying production enemy AI.

## Hit Feedback Rules

Hit feedback should be immediate, brief, high contrast, and readable from the default camera. It must remain local-only and prototype-labeled.

## Cooldown Feedback Rules

Cooldown feedback should clearly distinguish `Sẵn sàng` from `Đang hồi chiêu`. The cooldown indicator is local/non-authoritative until a future contract opens server combat.

## Telegraph Warning Rules

Enemy telegraph sheets are reference-only in v0.37. Do not implement enemy attacks, projectile warnings, PvE behavior, or server combat because a telegraph appears in an image.

## UI Contrast Rules

Combat text must keep strong contrast against the HUD surface and avoid competing with account, save-position, and guided-loop information.

## Accessibility And Noise

Use concise Vietnamese text, stable layout, and small high-contrast accents. Maximum visual noise rule: one active hit/cooldown emphasis at a time around the local target dummy.

## Mobile And Desktop

Mobile readability favors short labels and visible state words. Desktop readability can include one extra local-only note, but should not add mechanics.

## Local Prototype Label

Combat feedback must preserve `Mô phỏng cục bộ` or equivalent local-only/prototype wording.
