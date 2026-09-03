# Linh Giới Online — Milestone Roadmap

This roadmap prevents sandbox drift. Each sandbox must preserve milestone boundaries and handoff status honestly.

## M0 — Foundation Runtime Closure

Entry criteria:
- Authoritative M0 source is selected and SHA verified.
- Frozen contracts are locked.

Allowed scope:
- Protocol tooling, Java server foundation, Unity source foundation, GameData pipeline, UI foundation, CI/runtime validation, and minimal runtime-closure tooling fixes.

Forbidden scope:
- No gameplay expansion, map/class/skill/monster additions, economy, persistence, guild, marketplace, or M1 combat systems.

Required gates:
- Source validation, protocol generation, GameData tests, Java 25/Maven, server build/test, API `/health`, Netty TCP, real handshake, bad-client survival, Unity import/compile/EditMode/bootstrap/UI validation, and frozen contract audit.

Handoff artifacts:
- Runtime closure report, handoff, source delta, evidence archive, SHA files, and full source successor when needed.

## M1 — Offline Combat Prototype

Entry criteria:
- `M0_RUNTIME_CLOSED`, or explicit owner override from `M0_SERVER_RUNTIME_CLOSED_UNITY_ENV_LIMITED`.

Allowed scope:
- Offline combat loop prototype using existing protocol/GameData/UI foundations.

Forbidden scope:
- No online persistence, monetization, guild, marketplace, content expansion beyond M1 prototype fixtures, or protocol/schema changes without contract request.

Required gates:
- Unity runtime compile, deterministic local combat test cases, GameData-driven skills, UI HUD prototype, and no frozen contract drift.

Handoff artifacts:
- M1 report, playable/editor evidence, source delta/full source as applicable, and regression checklist.

## M2 — Online Session Prototype

Entry criteria:
- M1 offline combat closed.
- Existing Java handshake still PASS.

Allowed scope:
- Single-session online loop, client/server session lifecycle, latency-safe input/state scaffold.

Forbidden scope:
- No large-scale MMO systems, economy, social/guild production systems, or account monetization.

Required gates:
- Unity-to-Java session smoke, reconnect/failure path, deterministic protocol compatibility, server survival tests.

Handoff artifacts:
- Runtime evidence, compatibility report, source delta, handoff.

## M3 — Account / Character Persistence

Entry criteria:
- M2 online session prototype closed.

Allowed scope:
- Account, character creation, persistence schema, basic login/dev auth path, data migration discipline.

Forbidden scope:
- No payment, marketplace, public launch, or multi-region infra.

Required gates:
- Persistence tests, migration rollback checks, server runtime smoke, character load/save E2E.

Handoff artifacts:
- Migration report, persistence evidence, handoff.

## M4 — Core Social / Party / Guild Foundation

Entry criteria:
- M3 persistence closed.

Allowed scope:
- Party/guild/social foundation only.

Forbidden scope:
- No live-ops economy, cross-server architecture, or monetization.

Required gates:
- Permission/state tests, party/guild lifecycle smoke, server/runtime regression.

Handoff artifacts:
- Social foundation report, evidence, handoff.

## M5 — Content Expansion Pipeline

Entry criteria:
- M4 social foundation closed.

Allowed scope:
- Content authoring pipeline, validation, fixtures, deterministic build outputs.

Forbidden scope:
- No unvalidated content ingestion or direct code-side content definitions.

Required gates:
- Positive/negative GameData tests, deterministic hash, Unity/server consumption evidence.

Handoff artifacts:
- Content pipeline report, compiled manifest, evidence, handoff.

## M6 — Public Alpha Readiness

Entry criteria:
- M5 content expansion pipeline closed.

Allowed scope:
- Stability, packaging, release hygiene, operational smoke tests, minimum support docs.

Forbidden scope:
- No uncontrolled scope expansion or feature work that bypasses release gates.

Required gates:
- Release build, install smoke, client/server compatibility, security/config audit, rollback evidence.

Handoff artifacts:
- Alpha readiness report, release candidate package, evidence, handoff.
