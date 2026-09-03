# Linh Gioi Online Visual Identity Guide v0.10.0

Status: `M4_VISUAL_PLACEHOLDER_FOUNDATION_SOURCE_READY`

## Direction

Linh Gioi should feel like a Vietnamese spiritual-fantasy action world: quiet dark surfaces, bright spirit energy, warm artifact gold, and controlled shadow magic. This is early runtime placeholder art, not final production art.

## Palette

Use the existing UI design tokens as the source of truth:

- background: `#0B1324`
- surface: `#111D32`
- raised surface: `#182741`
- spirit: `#28D7C7`
- shadow: `#9B5CFF`
- gold: `#E6B85C`
- danger: `#E35D6A`
- text: `#F5F2EA`
- muted: `#9BA7BC`

## Runtime Style

- Character placeholders use readable silhouettes and one strong class cue.
- NPC placeholders use warmer gold/spirit accents and non-threatening posture.
- Monster placeholders use shadow purple and squat shapes.
- Item and skill icons should read at 64-128 px.
- World placeholders should be simple enough to inspect during smoke runs.
- UI should be quiet, compact, and legible, using spirit/gold accents sparingly.

## Placeholder vs Production

Committed SVG files under `client/Unity/Assets/Game/Art/**` are source placeholders. They may be used in docs, future import steps, or as reference for runtime procedural visuals. Current Unity runtime uses procedural primitives and the shared catalog colors so headless smoke is not dependent on an SVG package.

## Do

- Keep placeholder files small and original.
- Add `.meta` files for committed Unity assets.
- Update the runtime art manifest whenever adding/removing assets.
- Preserve protocol, schema, ADR, and design-token contracts unless a formal change is approved.

## Don't

- Do not import random web art or unlicensed assets.
- Do not commit Unity `Library`, `Temp`, `Logs`, build output, or generated protocol output.
- Do not replace placeholders with final art without documenting provenance and license.
- Do not open economy, guild, production auth, or MMO-scale systems from art work.
