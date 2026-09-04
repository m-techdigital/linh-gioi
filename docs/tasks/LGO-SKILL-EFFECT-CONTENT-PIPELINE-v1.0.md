# LGO Task 034 - Skill/Effect Content Pipeline v1.0

Marker: `LGO_SKILL_EFFECT_PIPELINE_READY`

## Scope

Create a planning and validation lane for skill/effect content without implementing new combat behavior.

Allowed:

- skill/effect naming conventions;
- Vietnamese player-facing copy rules;
- visual feedback ownership notes;
- validator coverage for source-only governance;
- future entry criteria for GameData/protocol review.

Not allowed:

- No new combat mechanic implementation.
- No damage, HP, cooldown, targeting, or enemy AI changes.
- No protocol or GameData schema change.
- No production DB, auth, economy, social, or live ops expansion.
- No final production art claim.

## Closure

This task closes when:

- planning docs define skill/effect ownership and runtime copy boundaries;
- validator blocks frozen contract drift;
- source and package-ready gates include the validator;
- task ledger records `LGO_SKILL_EFFECT_PIPELINE_READY`.
