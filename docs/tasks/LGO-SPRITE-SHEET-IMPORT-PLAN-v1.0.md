# LGO Task 043 - Sprite Sheet Import Plan v1.0

Marker: `LGO_SPRITE_IMPORT_PLAN_READY`

## Scope

Define how sprite sheets and separated sprites may enter the Unity project without turning reference boards or AI composite sheets into runtime art.

Allowed:

- docs/tooling only;
- import role classification;
- size and compression budget references;
- validator coverage for sprite import boundaries.

Not allowed:

- No production art claim.
- No auto-slicing composite sheets into final runtime sprites.
- No import of reference boards, mockups, contact sheets, or posters as runtime assets.
- No gameplay, combat, protocol, GameData schema, auth, DB, economy, social, or live ops expansion.

## Closure

This task closes when:

- sprite import rules are documented;
- runtime candidate and experimental sheet boundaries are explicit;
- validator checks the boundary docs and frozen surfaces;
- closure gates run `validate_lgo_sprite_import_plan.py`;
- task ledger records `LGO_SPRITE_IMPORT_PLAN_READY`.
