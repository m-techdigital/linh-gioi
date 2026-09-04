# LGO Crash/Error Reporting Plan v1.0

Marker: `LGO_CRASH_REPORTING_PLAN_READY`

## Scope

Define local crash/error reporting ownership and evidence rules for future alpha readiness. This task does not integrate a production service.

## Non-Claims

- No production crash-reporting service.
- No telemetry backend.
- No analytics SDK.
- No auth, DB, economy, social, or live-ops integration.
- No protocol or GameData schema changes.

## Exit Gate

`tools/validate_lgo_crash_error_reporting_plan.py` prints `LGO_CRASH_ERROR_REPORTING_PLAN_VALIDATION_PASS`.
