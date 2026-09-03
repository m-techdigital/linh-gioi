# Linh Giới Online — Project State

Last updated: `2026-09-03`

## Current milestone

`M3-B Unity Account / Character Integration`

## Current decision

`M3B_UNITY_ACCOUNT_CHARACTER_SOURCE_READY_FOR_VERIFY`

M0 final decision: `M0_RUNTIME_CLOSED`.

M1 final decision: `M1_OFFLINE_COMBAT_RUNTIME_CLOSED` (`M1 Offline Combat Prototype` closed).

M2 has source-level implementation for the first online session scaffold. Runtime closure is still pending because this sandbox does not run Unity Editor and Java 25/Maven runtime together for M2 evidence.

## Authoritative source baseline

`linh-gioi-m1-offline-combat-runtime-closed-v0.5.3-full-source.zip`

SHA256:

```text
9260c75e4f17259c1252f820629e32412e95a0a448f431a554ae27a820848234
```

## Current source successor

`linh-gioi-m2-runtime-candidate-v0.6.2-full-source.zip`

This source includes:

- accepted M0 runtime closure v0.4.1;
- accepted M1 offline combat runtime closure v0.5.3;
- Java realtime `OnlineSession` movement scaffold;
- Java Netty `OnlineSessionHandler` installed after accepted `ClientHello`;
- server-side authoritative `MoveIntent -> PlayerTransformSnapshot` loop;
- sequence acknowledgement with duplicate/late sequence idempotence;
- invalid movement failure path that closes only the offending session;
- reconnect/survival integration tests;
- Python TCP `online-session-smoke.py` for runtime server smoke;
- Unity `TcpRealtimeClient.SendMoveIntentAsync(...)`;
- Unity `OnlineSessionSmokeRunner` with `--lgo-m2-online-session-smoke`;
- M2 source validator, runtime evidence plan, checklist, prompt, and manifest.

## Closed M0 gates

- Source validation: PASS.
- Protocol codegen and tests: PASS.
- GameData tests and compiled manifest: PASS.
- Java 25 runtime: PASS.
- Maven 3.9.16 runtime: PASS.
- Server build/test: PASS, 25 executed / 0 skipped.
- Spring Boot `/health`: PASS.
- Netty TCP bind: PASS.
- Real TCP `ClientHello -> ServerHello`: PASS.
- Unsupported protocol / malformed payload survival: PASS.
- Unity Editor evidence: PASS, 6000.3.2f1.
- Unity project import/generate: PASS.
- Unity EditMode tests: PASS.
- Unity Linux player build: PASS.
- Unity Player -> Java Netty handshake: PASS.
- Graceful shutdown / no orphan server process: PASS.
- Frozen contract audit: PASS.

## Closed M1 gates

- M1 source implementation: PASS.
- M1 GameData-driven catalog mapping: PASS.
- M1 stricter catalog validation source: PASS.
- M1 invalid request rejection source: PASS.
- M1 deterministic combat test source: PASS.
- M1 HUD prototype source: PASS.
- M1 default camera/light scene generation: PASS.
- M1 offline smoke command: PASS.
- M1 runtime evidence scripts: PASS.
- Unity `6000.3.2f1` M1 EditMode runtime: PASS.
- Unity M1 player build: PASS.
- Sandbox M1 offline combat Linux player replay: PASS.

## Current M2 source gates

- Existing protocol consumed without mutation: PASS source audit.
- Java online session source: PASS static validation.
- Java online session unit/integration tests added: PASS static validation.
- Unity online session client source: PASS static validation.
- Unity online smoke command source: PASS static validation.
- M2 runtime evidence tooling/docs: PASS static validation.

## M2 runtime gates still pending

- Java 25/Maven server build and tests with new M2 session classes: pending runtime verification.
- Server `online-session-smoke.py` against live Java Netty: pending runtime verification.
- Unity EditMode tests with M2 client serialization: pending runtime verification.
- Unity-built Linux player `--lgo-m2-online-session-smoke` against live Java Netty: pending runtime verification.
- Reconnect/failure path in runtime smoke evidence: pending runtime verification.

## Next allowed step

Run targeted VERIFY for `LG-M3B-UNITY-ACCOUNT-CHARACTER-INTEGRATION-v0.8.0`. M2 runtime closure remains pending local Unity evidence. M4 remains forbidden until M3-B is accepted.

## Forbidden next step

Do not start M4 social/guild, payment, marketplace, PvP ranking, production auth, production DB infra, or broad content expansion. Do not mutate `protocol/**`, `gamedata/schemas/**`, ADR, or design tokens without an S0 contract-change task.


## M3 owner override note

`OWNER_OVERRIDE_FROM_M2_RUNTIME_CANDIDATE` is recorded because M3 source work was explicitly opened by the project owner before `M2_ONLINE_SESSION_RUNTIME_CLOSED`. This does not claim the pending M2 Unity local evidence gates as PASS.

## Current M3 source gates

- Dev account login API source: PASS static/source validation.
- Dev key SHA-256 persistence hygiene: PASS static/source validation.
- Character create/list/load API source: PASS static/source validation.
- Character position save/load API source: PASS static/source validation.
- Local JSON persistence schema v1: PASS static/source validation.
- Unsupported future schema guard: PASS static/source validation.
- API persistence smoke tooling: PASS static/source validation.

## M3 runtime gates pending until execution

- Java 25 + Maven server build/test on M3 source.
- Spring Boot API runtime with `LG_API_PERSISTENCE_DIR`.
- Dev login/create/load/save HTTP smoke.
- API restart reload smoke.
- No raw dev key persisted.
- No orphan API process after smoke.

## M2 v0.6.2 hardening note

`M3_ACCOUNT_CHARACTER_PERSISTENCE_SOURCE_READY` supersedes v0.6.1 while keeping M2 locked to the same protocol and scope. v0.6.2 retains the one-command local runner and dependency-free GameData fallback, then adds server/client movement validation parity, multi-snapshot Unity smoke assertions, and no-false-ready local evidence classification.


## Previous M2 state marker for inherited M2 validators

`M2 Online Session Prototype` remains at `M2_RUNTIME_CANDIDATE_HARDENED_READY_FOR_LOCAL_EVIDENCE`; M2 runtime evidence remains pending local Unity execution.


## Closed M3 server/API gates

- M3 final decision: `M3_ACCOUNT_CHARACTER_PERSISTENCE_RUNTIME_SMOKE_CLOSED`.
- Dev login API runtime smoke: PASS.
- Character create/list/load runtime smoke: PASS.
- Character position save/load runtime smoke: PASS.
- API restart/reload persistence smoke: PASS.
- Raw dev key persistence hygiene: PASS.

## Current M3-B source gates

- Unity `ClientRuntimeConfig` API endpoint configuration: PASS static/source validation.
- Unity `LinhGioi.Account` asmdef and dependency graph: PASS static/source validation.
- Unity `AccountApiClient` real HTTP implementation with `UnityWebRequest`: PASS static/source validation.
- Unity login/list/create/load/save-position model surface: PASS static/source validation.
- Unity command-line smoke path `--lgo-m3b-account-character-smoke`: PASS static/source validation.
- Restart-aware smoke path `--lgo-m3b-expect-existing`: PASS static/source validation.

## M3-B runtime gates pending until current Unity player execution

- Current Unity Linux player built from M3-B source.
- Unity player first-pass API smoke.
- API restart.
- Unity player restart-pass API smoke with persisted character reuse.
- Raw `m3b-unity-dev-key` absence in `players-v1.json`.

## M3-B owner override note

`OWNER_OVERRIDE_FROM_M3_SERVER_API_CLOSED` is recorded because the owner explicitly opened Unity client integration after the M3 server/API persistence closure while M2 Unity local evidence remains separately pending. This does not claim `M2_ONLINE_SESSION_RUNTIME_CLOSED`.


Historical milestone reference: `M3 Account / Character Persistence` is closed at server/API persistence level before M3-B.
