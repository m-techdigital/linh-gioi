# LGO Task 046 - Telemetry Event Schema Plan v1.0

Marker: `LGO_TELEMETRY_SCHEMA_PLAN_READY`

## Scope

Define a planning-only event taxonomy for local QA, smoke evidence, and future telemetry review.

Allowed:

- docs/tooling only;
- event naming and payload planning;
- local runtime evidence categories;
- validator coverage for frozen surfaces and non-production claims.

Not allowed:

- No production analytics code.
- No live ops implementation.
- No external tracking SDK.
- No player profiling, monetization, economy, social, auth, DB, protocol, GameData schema, ADR, or design-token change.

## Closure

This task closes when:

- telemetry/event planning docs define allowed local QA events;
- production analytics and live ops remain explicitly out of scope;
- validator checks docs and frozen surfaces;
- closure gates include `validate_lgo_telemetry_schema_plan.py`;
- task ledger records `LGO_TELEMETRY_SCHEMA_PLAN_READY`.
