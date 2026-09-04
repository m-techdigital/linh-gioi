# Linh Gioi Crash/Error Reporting Plan v1.0

Marker: `LGO_CRASH_REPORTING_PLAN_READY`

## Purpose

Crash and error evidence should be useful during local development before any production reporting service exists.

## Ownership

| Area | Owner | Evidence |
|---|---|---|
| Unity player smoke failure | Unity runtime scripts | player log, smoke JSON, closure summary |
| Java API failure | server/api tests and logs | Maven result, Spring Boot log, smoke output |
| Java realtime failure | server/realtime tests and logs | Maven result, realtime bind/handshake log |
| Protocol generation failure | protocol tooling | compiler diagnostics and deterministic manifest |
| GameData validation failure | gamedata tooling | positive/negative validator output |
| Package hygiene failure | package hygiene validator | forbidden file report |

## Failure Classification

- `FIX_REQUIRED`: deterministic source, test, config, validator, or runtime failure.
- `UNVERIFIED_ENVIRONMENT`: missing Unity, missing Java/Maven, unavailable local tool, or unsupported host.
- `CONTRACT_CHANGE_REQUIRED`: fix requires frozen protocol/schema/ADR/design-token change.
- `HUMAN_REVIEW_REQUIRED`: visual quality or production art acceptance cannot be decided by automation.

## Local Evidence Bundle Rules

- Keep evidence outside runtime source when possible.
- Do not include Unity `Library`, `Temp`, `Logs`, Maven `target`, Python cache, or ZIPs inside source.
- Include failure command, return code, marker expected, marker observed, and log path.
- Prefer JSON summaries next to raw logs.

## Future Production Service Criteria

Before adding a production crash/error service, create a scoped task that covers:

- privacy and PII boundaries;
- sampling and rate limits;
- release channel tags;
- player/session correlation rules;
- retention policy;
- offline buffering behavior;
- security review;
- rollback plan.
