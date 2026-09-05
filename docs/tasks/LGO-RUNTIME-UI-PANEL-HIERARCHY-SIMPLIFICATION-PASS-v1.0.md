# LGO Runtime UI Panel Hierarchy Simplification Pass v1.0

Status: `LGO_RUNTIME_UI_PANEL_HIERARCHY_SIMPLIFICATION_READY`

## Scope

This task reduces visible nested-frame noise in login and Character Hall UI by reusing a shared subtle nested frame helper.

## Changes

- Added `RuntimeUiSkin.ApplySubtleNestedFrame`.
- Character list, selected profile, create panel, and empty character card now use that helper.
- Login CTA backing border emphasis was softened.
- Existing V3B assets, account flow, character flow, world HUD, dialogue, and session behavior were preserved.

## Validation

- `validate_lgo_runtime_ui_panel_hierarchy_simplification.py`
- `git --no-pager diff --check`
- `./tools/lgo_playable_closure_check.sh --source-only`

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No new gameplay.
- No new runtime asset payload.
- No production art claim.

## Follow-Up

Continue with `LGO-RUNTIME-UI-PANEL-HIERARCHY-EVIDENCE-REFRESH-v1.0`: capture/review login and Character Hall screenshots after the frame simplification.
