# LGO Runtime Asset Optimization Pass v1.0

Status: `LGO_RUNTIME_ASSET_OPTIMIZATION_READY`

## Scope

This pass optimizes runtime delivery weight without changing gameplay, UI semantics, protocol, GameData schemas, ADRs, or design tokens.

## Changes

- Added platform-specific Unity import settings for all V3B runtime candidate assets.
- Kept one source runtime asset per role instead of adding duplicate mobile/tablet folders.
- Preserved transparent PNG sprites and avoided risky recompression without a dedicated visual comparison tool.
- Documented the build-target profile policy for Standalone, Android, and iPhone.

## Non-Claims

- No production art claim.
- No final visual quality claim.
- No gameplay change.
- No composite sheet slicing.

## Validation

- `python3.12 tools/validate_lgo_runtime_asset_import_profiles.py`
- `python3.12 tools/validate_lgo_runtime_asset_weight.py`
- `python3.12 tools/validate_lgo_device_profile_ui_budgets.py`
- `./tools/lgo_playable_closure_check.sh --source-only`
