# Linh Gioi Online Design Direction Lock v0.11.0

Status: `LGO_DESIGN_DIRECTION_LOCK_READY_FOR_REVIEW_v0.11.0`

This document locks the next design direction for M4 follow-up work. It guides implementation but does not claim final production art.

## Game Feel Pillars

- Friendly online RPG: readable first, stylish second.
- Vietnamese spiritual fantasy: spirit gates, cultivation motifs, talismans, runes, lantern glow, village-hub warmth, and shadow-world contrast.
- Lightweight action clarity: silhouettes and UI states must be understandable at mobile scale.
- Social action flow: login, character lobby, world entry, HUD, and return actions should feel like one continuous game shell.
- Placeholder-safe: current source assets are technical placeholders and must not be mistaken for final art direction.

## Target Player Fantasy

The player is a new cultivator entering Linh Gioi through a spirit gate, choosing a readable archetype, meeting friendly keepers, and stepping into a compact online hub where supernatural threats are visible but not visually noisy.

## Camera And World Presentation

- Prefer 2D/2.5D presentation: slightly elevated camera, clear ground plane, readable character spacing.
- Keep the world hub compact and inspectable.
- Use spirit cyan for interactive energy, gold for safe guidance/reward, purple for shadow threat.
- Avoid dense realism while the project is still in placeholder art.

## Lobby To World UX

- Login should feel like entering a game gate, not an admin form.
- Character lobby should prioritize portrait/silhouette, name, class, and enter action.
- Create character should be compact and visible in empty state.
- World shell should immediately show who the player is, where they are, and whether API persistence is healthy.

## HUD Direction

- Compact top/status strip for account, character, API state.
- Bottom or side action cluster for save position and back-to-lobby.
- Position/debug values are acceptable in M4 but should be visually secondary.
- Control hints should be present but quiet: movement near lower edge or status panel, not as tutorial prose.

## Mobile And Desktop Readability

- Minimum interactive target: 44 px.
- Avoid long text inside buttons.
- Keep panels shallow; do not nest cards inside cards.
- Use high contrast for primary actions.
- Preserve space for mobile safe areas.
- Desktop can show more status text, but mobile should collapse debug fields first.

## Placeholder And Final Art Boundary

Allowed placeholders:

- Original simple SVG silhouettes and icon boards.
- Unity primitives colored from `RuntimeArtCatalog`.
- Low-detail UI frames/buttons used as layout references.
- Documentation-only mockups or concept prompts.

Not final art:

- Current SVGs under `client/Unity/Assets/Game/Art/**`.
- Procedural cube/capsule markers.
- M4 UI layout text and debug positioning.

Production art requires separate provenance, license notes, import settings, and review.

## Visual Anti-Patterns

- SaaS dashboard styling.
- Cyberpunk neon city language.
- Generic western medieval fantasy.
- Overly dark scenes where markers cannot be inspected.
- Purple-only or blue-only palette dominance.
- Decorative gradients/orbs that do not communicate gameplay.
- Large marketing hero pages in place of the playable game shell.
- Unlicensed or copied third-party characters, UI, icons, or marketplace assets.

## Explicit Note

The current SVG files are technical placeholders only. They are useful for source organization and runtime reference, but they are not production-quality art and should be replaced or upgraded in later accepted art tasks.
