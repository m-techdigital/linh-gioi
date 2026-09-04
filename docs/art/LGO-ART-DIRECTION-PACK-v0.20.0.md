# LGO Art Direction Pack v0.20.0

Marker: LGO_ART_DIRECTION_PACK_ACCEPTED_v0.20.0

This pack records owner-provided visual direction for Linh Gioi Online. The images are reference-only and must be translated into simplified runtime-safe placeholder assets. They are not final production art, licensed production assets, final animation sheets, or final UI skins.

## Image Roles

- `lgo-art-direction-overview-v0200.png`: global art language, palette, motifs, silhouettes, environment mood, UI frame language, icon style, and VFX color grammar.
- `lgo-playable-hero-pose-sheet-v0200.png`: runtime character pose and animation placeholder direction for idle, walk, interact, and spirit channel states.
- `lgo-npc-direction-sheet-v0200.png`: Gate Keeper and future NPC placeholder direction, with robe silhouettes, cyan spirit props, gold trim, and readable portrait-scale motifs.
- `lgo-monster-direction-sheet-v0200.png`: Shadow Slime readability and non-combat warning direction. This file is an alias of the owner-provided `lgo-shadow-slime-monster-direction-v0200.png`.
- `lgo-ui-component-skin-sheet-v0200.png`: runtime UI frame, button, list, status chip, input, and scroll skinning direction.
- `lgo-window-popup-sheet-v0200.png`: dialog, confirm, reward-style popup, toast, and window frame pattern direction.
- `lgo-item-skill-vfx-sheet-v0200.png`: skill and VFX placeholder feedback direction for spirit pulse, wind slash preview, shadow bind warning, portal rune, and toast effects.
- `lgo-environment-prop-sheet-v0200.png`: Spirit Gate, Training Stone, lantern, ground, shrine, safe circle, and hub prop placeholder direction.

## Implementation Stage Mapping

- Overview board feeds global art language for all runtime placeholder work.
- Hero pose sheet feeds v0.21 runtime character pose and animation placeholders.
- NPC sheet feeds v0.21 Gate Keeper and later NPC placeholders.
- Monster sheet feeds v0.21 Shadow Slime readability and non-combat warning states.
- UI component sheet feeds v0.22 runtime UI frame, button, list, and status chip skinning.
- Window and popup sheet feeds v0.22 dialog, confirm, toast, and popup patterns.
- Item, skill, and VFX sheet feeds v0.23 non-damaging local skill and VFX feedback placeholders.
- Environment prop sheet feeds v0.19-v0.23 Spirit Gate, Training Stone, lantern, ground, safe circle, and shrine prop placeholders.

## Translation Rules

- Simplify for runtime: use primitives, UI Toolkit styles, lightweight generated materials, and small deterministic markers.
- Preserve readability at 1280x720 and mobile scale before ornament density.
- Do not copy images 1:1 into runtime UI, world props, or animation frames.
- Use the sheets as direction for silhouette, color, spacing, hierarchy, glow language, and motif selection.
- Do not claim production art, final UI, final animation, or final VFX.

## Palette And Motif Rules

- Dark navy surfaces are the default UI and world backing tone.
- Spirit cyan is the primary active, portal, training, and guidance signal.
- Warm gold is the trim, title, frame, and guidance accent.
- Jade and teal are secondary calm/support accents.
- Purple shadow is reserved for Shadow Slime and non-combat threat/warning language.
- Small red/orange alert appears only for warning accents, never as a dominant palette.
- Motifs: spirit gate, lotus seal, yin-yang chain, tiger rune, dragon spirit, cloud scroll, lantern charm, shrine structure, circular training ground, and thin ornamented frame corners.

## Non-Claims

- Reference art only.
- Not final production art.
- Not licensed production asset.
- Not final animation sheet.
- Not final UI skin.
- Not a protocol, GameData, production authentication, database persistence, combat, inventory, economy, guild, chat, market, party, or live ops change.
