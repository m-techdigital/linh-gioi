# LGO Runtime UI Component Metric Ownership Drift Scan v1.0

Status: `LGO_RUNTIME_UI_COMPONENT_METRIC_OWNERSHIP_DRIFT_SCAN_READY`

## Scope

This task scans runtime UI code for reusable metric drift and moves the safe, repeated subset into named UI owners. It is a maintainability pass, not a UI redesign.

## Changed Ownership

- `RuntimeUiSizing` now owns shared shell, header, login NPC, login CTA, Character Hall, and icon-button dimensions.
- `RuntimeUiSpacing` now owns ornament, hairline, primary/compact button, and icon-button gap metrics.
- `RuntimeUiTypography` now owns section/badge/primary-button font sizes.
- `M4PlayableClientController` keeps flow ownership and viewport-specific composition offsets.
- `RuntimeUiFactory` keeps component construction and applies the named metrics.

## Validation

- `validate_lgo_runtime_ui_component_metric_ownership_drift_scan.py`
- `git --no-pager diff --check`
- `./tools/lgo_playable_closure_check.sh --source-only`

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No gameplay, auth, protocol, GameData, ADR, design-token, or art asset change.
- No claim that world hub visuals are final-quality.

## Follow-Up

Continue with `LGO-RUNTIME-UI-COMPONENT-METRIC-OWNERSHIP-EVIDENCE-REFRESH-v1.0`: capture and review runtime screenshots after the metric owner cleanup.
