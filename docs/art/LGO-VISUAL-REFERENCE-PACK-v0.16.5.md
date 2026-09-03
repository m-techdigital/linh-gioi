# Linh Gioi Online Visual Reference Pack v0.16.5

Status: `LGO_VISUAL_REFERENCE_PACK_ACCEPTED_v0.16.5`

This pack is the accepted visual reference input for the next playable UX passes. It is reference-only material. Runtime implementation must simplify it for readability and must not copy any image one-to-one into UI, world props, character art, icons, or production assets.

## Reference Images

| Image | Role | Guides |
|---|---|---|
| `docs/reference-art/v0.16.5/lgo-visual-reference-overview-v0165.png` | Overview board combining key visual, world hub, HUD, silhouettes, icon language, and motif samples. | Global art direction, palette balance, documentation summaries, consistency checks. |
| `docs/reference-art/v0.16.5/lgo-key-visual-moodboard-v0165.png` | Key mood image for the spiritual fantasy world. | Spirit gate atmosphere, lantern warmth, cyan energy focal point, purple shadow-realm contrast. |
| `docs/reference-art/v0.16.5/lgo-world-hub-2d5-v0165.png` | 2.5D world hub composition. | Compact safe center, readable interactable labels, gate/training/market style zoning, hub navigation readability. |
| `docs/reference-art/v0.16.5/lgo-gate-character-ui-v0165.png` | Gate entry and character hall UI reference. | Auth/lobby hierarchy, large character silhouette, side panels, primary Enter World call-to-action, ornate readable frames. |
| `docs/reference-art/v0.16.5/lgo-playable-hud-mockup-v0165.png` | Playable HUD density reference. | Status strip, objective area, minimap-like orientation language, action cluster placement, readable RPG feedback. |
| `docs/reference-art/v0.16.5/lgo-character-npc-monster-style-v0165.png` | Character, NPC, companion, and shadow monster silhouette board. | Hero and Gate Keeper silhouette distinction, friendly NPC language, non-combat Shadow Slime marker shape, mobile-scale readability. |
| `docs/reference-art/v0.16.5/lgo-item-skill-vfx-icons-v0165.png` | Item, skill, VFX, rune button, and status icon board. | Training Stone/spirit pulse motifs, circular icon frames, cyan skill energy, purple shadow bind contrast, API/status symbol direction. |

## Extracted Palette

- dark navy surface: deep blue-black foundation for world, panels, and HUD areas.
- cyan spirit energy: primary focal glow for spirit gates, interactables, skill/VFX cues, and selected/ready states.
- warm gold guidance: safe guidance, frame trims, important labels, confirmation, and primary readable accents.
- purple shadow threat: shadow realm edge, Shadow Slime markers, non-combat danger contrast, and world tension.
- small red/orange alert/accent: limited use for warnings, notification dots, maintenance/crowded realm status, or failed actions.

These colors align with the existing runtime catalog and design tokens. Do not change `client/Unity/Assets/Game/UI/design-tokens.json` for this pack.

## Motifs

- Spirit gate: central transition/focal motif for login, enter-world, and world hub direction.
- talisman: vertical slips, charms, and small hanging details for spiritual flavor.
- rune trim: thin gold/cyan linework for panels, buttons, labels, and icon frames.
- Lantern warmth: orange-gold safe-zone lighting and social hub warmth.
- cultivation circle: ground orientation, training marker, objective focus, and ritual UI framing.
- Shadow realm edge: purple forms around threat markers and background boundary, not the dominant UI color.

## Runtime Translation Rules

- Reference images are not production assets and must not be copied one-to-one into runtime.
- Implementation should reduce detail into readable placeholders, panels, labels, colors, and silhouettes.
- UI must remain usable at `1280x720` and at mobile scale; keep primary actions at least 44 px and avoid long button text.
- The current Unity implementation may use simplified placeholders, procedural primitives, existing SVG placeholders, and `RuntimeArtCatalog` colors.
- Ornate frames should be suggested through thin borders, corner details, spacing, and accent color, not dense decoration that harms readability.
- HUD density from the mockup should translate into clear zones, not full MMO systems such as inventory, chat, market, party, or live ops.
- Shadow Slime is a non-combat readability marker until a later combat task explicitly opens combat.

## Explicit Non-Claims

- Reference art only.
- not final production art.
- Not a licensed production asset package.
- Not final UI.
- No protocol change.
- No GameData schema change.
- No production auth, DB persistence, inventory, economy, guild, chat, market, party, live ops, full combat, or full MMO gameplay.
