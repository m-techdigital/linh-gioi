# LGO Task 044 - Animation Direction Plan v1.0

Marker: `LGO_ANIMATION_DIRECTION_READY`

## Scope

Define animation direction for readable 2D/2.5D gameplay presentation without importing new animation assets or implementing gameplay behavior.

Allowed:

- docs/tooling only;
- state taxonomy for player, NPC, monster, combat feedback, UI, and VFX;
- timing/readability guidance;
- future import and runtime smoke criteria.

Not allowed:

- No animation implementation.
- No new gameplay mechanic.
- No production art or production animation claim.
- No auto-slicing composite sheets.
- No protocol, GameData schema, ADR, design-token, auth, DB, economy, social, or live ops expansion.

## Closure

This task closes when:

- animation direction and runtime boundaries are documented;
- validator blocks frozen surface drift;
- closure gates include `validate_lgo_animation_direction.py`;
- task ledger records `LGO_ANIMATION_DIRECTION_READY`.
