# LGO Runtime Asset Weight Actionable Budget v1.0

Status: `LGO_RUNTIME_ASSET_WEIGHT_ACTIONABLE_BUDGET_READY`

## Scope

This task turns the runtime asset size inventory into an operational budget that tells Codex what to do when assets approach budget limits.

## Implemented

- `tools/report_lgo_runtime_asset_size_inventory.py` now prints an `Action` column.
- Watch/over-budget states have explicit next actions.
- `docs/art/RUNTIME-ASSET-ACTIONABLE-BUDGET.md` defines OK/WATCH/OVER_BUDGET rules.
- `tools/validate_lgo_runtime_asset_weight_actionable_budget.py` protects this policy.

## Validation

- `python3.12 tools/validate_lgo_runtime_asset_weight_actionable_budget.py`
- `python3.12 tools/report_lgo_runtime_asset_size_inventory.py`

## Follow-Up

Continue with `LGO-VISUAL-EVIDENCE-REVIEW-SUMMARY-VI-EVIDENCE-v1.0`: regenerate visual evidence analysis so the new Vietnamese summary file exists in current runtime evidence output.
