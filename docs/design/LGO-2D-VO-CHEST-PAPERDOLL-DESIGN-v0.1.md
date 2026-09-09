# Linh Gioi Online - Vo Chest And Paper Doll Design v0.1

Date: 2026-09-09. Scope: design/demo contract for class Vo equipment review only. This connects the existing inventory try-on spine to a clearer chest layout with a character preview. It is not inventory persistence, loot, economy, marketplace, or server authority work.

## Purpose

The chest exists to test equipment fit, not to simulate a final MMO bag. It must let the owner place Vo items in a grid, select one item, preview it on the character, apply or cancel the preview, and compare cross-level mixes on the same male/female base.

## Screen Layout

Use one working screen with four stable regions:

- Left: chest grid, compact square cells, filtered to Vo demo items first.
- Center: selected item inspect panel with slot, level, gender fit, ownership notes, and visual thumbnail.
- Right: paper doll character preview with the side-view base and ten equipped slots around it.
- Bottom: motion preview strip with idle, walk, run, jump, basic attack, and skill preview states.

The character preview is the main read. The chest grid should be dense and calm so it does not compete with the paper doll. Empty cells show a quiet placeholder. Equipped items get a small equipped marker. Previewed-but-not-applied items use a separate preview marker so cancel remains obvious.

## Data Model For Demo

Use a local design-facing model with no persistence claim:

- `ChestCell`: index, item id or empty.
- `VoEquipmentItem`: item id, slot id, level, gender fit, icon reference, preview reference, anchor profile, ownership warnings.
- `PaperDollLoadout`: ten slot ids mapped to item ids plus fallback base state for empty slots.
- `TryOnState`: selected cell, selected item, preview loadout, applied loadout, active motion state.

The UI can seed `lv001` and `lv050` items first. Full `lv001` and full `lv050` rows must fit the same base. Mixed rows must prove that clothes from one level and weapon/guard/belt/accessory from another level still line up.

## Interaction Contract

Baseline keyboard/controller behavior can follow the existing inventory input spine:

- Open/close chest.
- Move selection across grid cells.
- Inspect selected item.
- Try item on the paper doll without mutating applied loadout.
- Apply preview to the matching slot.
- Cancel preview and restore applied loadout.
- Toggle male/female base when reviewing art.
- Cycle motion state through idle, walk, run, jump, basic attack, and skill preview.
- Remove one equipped slot and show fallback base clothing/hair.

Invalid actions should be visible but quiet: wrong gender, wrong slot, missing preview art, or item failing anchor check. Do not add final item stats, rarity economy, currency, sell, dismantle, trade, loot, or storage limits in this design pass.

## Fit And Animation Review

Every selected item must be viewable in three contexts:

1. Detached item inspect.
2. Equipped on neutral side-view base.
3. Equipped during the active motion state.

The chest must make ownership bugs easy to see: fingers inside gauntlets, skin inside arm guards, head attached to hair, belt merged into pants, shoulder armor merged into outer top, boots containing feet, and VFX baked into equipment.

Motion preview is a visual compatibility test only. `skill_preview` means pose plus separated VFX layer. It does not define combat damage, cooldown, target selection, unlock level, or backend state.

## Draft Visual Direction

Use restrained dark ink, charcoal panels, muted red selection accents, old-gold slot outlines, and warm ivory paper-doll background. Avoid blue energy in item UI for Vo. The first mockup can be a flat design image under `build/class-2d-review/generated-drafts/` and must remain reference-only until runtime UI work is explicitly opened.

## Acceptance Gate

For class Vo, the next useful demo is a static chest mockup plus a small local fixture list containing `lv001` and `lv050` male/female items. The mockup passes only if a reviewer can see the chest grid, selected item, ten paper-doll slots, active loadout, preview/applied distinction, and motion state selector without reading gameplay docs.

Non-claims:

- no gameplay implementation
- no production art claim
- no runtime asset claim
- no protocol changes
- no GameData schema changes
