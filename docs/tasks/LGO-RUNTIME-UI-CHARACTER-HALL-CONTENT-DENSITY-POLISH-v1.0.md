# LGO Runtime UI Character Hall Content Density Polish v1.0

Status: `LGO_RUNTIME_UI_CHARACTER_HALL_CONTENT_DENSITY_READY`

## Scope

This task reduces visual density in the Character Hall selected profile area without changing account or character flow behavior.

## Changes

- Shortened empty and selected character summary text.
- Moved cultivation/class summary into the meta line.
- Kept `_selectedClassSummary` as an internal source element, but hid it from the current visual card to avoid a third repeated status box.
- Preserved selected status/objective rows and enter-world CTA behavior.

## Validation

- `validate_lgo_runtime_ui_character_hall_content_density_polish.py`
- `git --no-pager diff --check`
- `./tools/lgo_playable_closure_check.sh --source-only`

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No gameplay or account/character semantic change.
- No new runtime asset payload.

## Follow-Up

Continue with `LGO-RUNTIME-UI-CHARACTER-HALL-CONTENT-DENSITY-EVIDENCE-REFRESH-v1.0`: capture/review Character Hall screenshots after the density polish.
