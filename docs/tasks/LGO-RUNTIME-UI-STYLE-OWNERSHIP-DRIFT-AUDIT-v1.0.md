# LGO Runtime UI Style Ownership Drift Audit v1.0

Status: `LGO_RUNTIME_UI_STYLE_OWNERSHIP_DRIFT_READY`

## Scope

This pass continues reducing UI duplication by moving style-only controller helpers into `RuntimeUiFactory`.

## Source Changes

- `NewToast`, `ApplyStatusChip`, `ApplyStatusAccent`, and `ApplyCombatButtonSkin` are factory helpers.
- Controller callsites remain unchanged through `using static LinhGioi.UI.RuntimeUiFactory`.
- Stale validators now verify the new ownership boundary instead of direct controller helper bodies.

## Validation

- `python3.12 tools/validate_lgo_runtime_ui_style_ownership_drift_audit.py`
- `python3.12 tools/validate_lgo_combat_button_state_readability_polish.py`
- `python3.12 tools/validate_m5_ui_skinning.py`
- `python3.12 tools/validate_lgo_runtime_ui_skin_adoption_audit.py`
- `./tools/lgo_playable_closure_check.sh --source-only`

## Non-Claims

- No gameplay change.
- No combat mechanic change.
- No visual asset payload change.
- No `VISUAL_RUNTIME_PASS` claim.
