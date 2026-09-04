# Linh Gioi Telemetry Event Schema Plan v1.0

Marker: `LGO_TELEMETRY_SCHEMA_PLAN_READY`

## Purpose

Linh Gioi needs consistent event names for QA logs and runtime evidence before any production analytics exists. This plan is documentation-only and keeps telemetry separate from gameplay contracts.

## Event Classes

| Class | Purpose | Production status |
|---|---|---|
| `qa.smoke.*` | automated source/runtime smoke evidence | local only |
| `qa.visual.*` | screenshot/video review milestones | local only |
| `runtime.ui.*` | screen and state visibility checks | planned only |
| `runtime.world.*` | zone, actor, and interaction visibility checks | planned only |
| `runtime.combat.*` | accepted combat feedback/result evidence | planned only |
| `runtime.error.*` | recoverable client/server error classification | planned only |

## Naming Rules

```text
<scope>.<surface>.<event>
```

Examples:

- `qa.smoke.login.visible`
- `qa.visual.hud.combat_ready`
- `runtime.ui.character_selected`
- `runtime.world.zone_loaded`
- `runtime.combat.intent_sent`
- `runtime.error.api_unavailable`

## Payload Planning

Planning-only event payloads should include:

- event id;
- source surface;
- timestamp;
- build/version label;
- session-local correlation id;
- screen or zone id when applicable;
- result status;
- error classification when applicable;
- privacy note.

## Privacy And Boundary Rules

- Do not log passwords, tokens, API keys, private chat, payment data, or raw personal data.
- Do not add external analytics SDKs in this task.
- Do not create production dashboards or live ops tools in this task.
- Keep player-facing UI copy Vietnamese; event ids may remain stable ASCII identifiers.
- Treat network protocol messages and GameData schemas as separate frozen contracts.

## Local Evidence Uses

Allowed local uses:

- summarize closure smoke runs;
- classify runtime screenshot/video coverage;
- diagnose UI state visibility;
- record source-only validator outcomes;
- correlate Unity/player logs with smoke steps.

## Future Entry Criteria

Before implementing production telemetry:

- approve privacy model and data retention rules;
- define event schema ownership and versioning;
- decide whether any server protocol or persistence changes are required;
- add opt-in/consent requirements if needed;
- add negative tests for forbidden sensitive fields;
- create a contract-change request for any frozen surface change.

## Non-Claims

This plan does not implement analytics, live ops, player tracking, dashboards, or production telemetry storage.
