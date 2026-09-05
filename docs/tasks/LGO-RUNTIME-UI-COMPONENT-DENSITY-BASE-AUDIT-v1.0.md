# LGO Runtime UI Component Density Base Audit v1.0

Status: `LGO_RUNTIME_UI_COMPONENT_DENSITY_BASE_READY`

## Scope

This task adds a reusable density-profile base for repeated runtime UI component padding and margin. It avoids growing the controller with one-off values while preserving the existing runtime flow.

## Changes

- Added `RuntimeUiDensityProfile` with marker `LGO Runtime UI Component Density Base v1`.
- Moved Character Hall list, empty-card, and status-row density through the new profile.
- Kept viewport placement in `RuntimeUiLayoutProfile`.
- Kept raw numeric spacing constants in `RuntimeUiSpacing`.
- Added factory helpers for applying list/card density.

## Validation

- `validate_lgo_runtime_ui_component_density_base_audit.py`
- `./tools/lgo_playable_closure_check.sh --source-only`

## Non-Claims

- No gameplay or semantic flow changes.
- No frozen contract edits.
- No `VISUAL_RUNTIME_PASS` claim.

## Follow-Up

Continue with `LGO-RUNTIME-UI-COMPONENT-DENSITY-EVIDENCE-REFRESH-v1.0`.
