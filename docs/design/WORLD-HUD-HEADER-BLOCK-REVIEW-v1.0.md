# World HUD Header Block Review v1.0

Marker: `LGO_WORLD_HUD_HEADER_BLOCK_READY`

## Decision

The title plus ornamental divider pattern is now a reusable `RuntimeUiFactory.NewSectionHeaderBlock` primitive. This keeps repeated title/divider composition out of screen controllers while avoiding a large stateful screen split.

## Implemented

- `RuntimeUiFactory.NewSectionHeaderBlock` composes `NewSectionTitle` and `NewOrnamentRule`.
- Character Hall and World HUD now use named header blocks.
- `NewOrnamentRule` remains available for future layout variants that need only the divider.

## Boundary

- Header blocks are stateless visual composition only.
- Runtime copy, screen transitions, selected character state, world state, and evidence state remain controller-owned.
- Do not make a full screen component until multiple screens share stateful behavior, not merely similar style.

## Non-Claims

- No gameplay change.
- No visual asset payload change.
- No protocol, GameData, ADR, or design-token change.
- No `VISUAL_RUNTIME_PASS` claim.
