# LGO NPC/Dialogue Content Pipeline v1.0

Marker: `LGO_DIALOGUE_PIPELINE_READY`

## Scope

Define a lightweight NPC/dialogue content pipeline for future content authoring without production DB or schema implementation.

## Non-Claims

- No production DB.
- No dialogue database.
- No new gameplay implementation.
- No protocol or GameData schema change.
- No social/live-ops implementation.

## Exit Gate

`tools/validate_lgo_dialogue_pipeline.py` prints `LGO_DIALOGUE_PIPELINE_VALIDATION_PASS`.
