# P2 PROMPT — S2 JAVA BACKEND + REALTIME FOUNDATION

You are S2. Implement the Java server foundation against the already accepted S5-A protocol tooling.

## Preconditions

- P0 accepted.
- P1 S5-A accepted and present in your source.
- Source provenance and prior overlays are known.

If any precondition is false, report BLOCKED.

## Required reading

Read completely:

- `README.md`
- `docs/01-PRODUCT-CONSTITUTION.md`
- `docs/03-TDD.md`
- `docs/04-NETWORK-CONTRACT.md`
- `docs/09-DEFINITION-OF-DONE.md`
- `docs/10-INTEGRATION-RULES.md`
- `docs/tasks/S2-JAVA-FOUNDATION.md`
- `protocol/*.proto`
- accepted S5-A tooling docs/scripts
- `docs/execution/03-HANDOFF-CONTRACT.md`

## Goal

Create a reproducible Java 25 server foundation with distinct API and realtime runtime boundaries, using generated Java protobuf types from the canonical protocol tooling.

## Allowed paths

- `server/**`

Build wiring may consume root/tooling files delivered by accepted S5-A, but do not rewrite their semantics without a change request.

## Forbidden paths

- `protocol/**`
- `gamedata/**`
- `client/**`
- `docs/adr/**`

## Required implementation

1. Select exactly one build system at `server/` root (Maven or Gradle) and document the reason briefly; no competing second build.
2. Target Java 25.
3. Keep API and realtime as separate process/module boundaries while sharing only necessary server-common code.
4. API: Spring Boot 4.x and a real runtime health endpoint.
5. Realtime: Netty WebSocket/TCP endpoint per M0 network contract.
6. Consume generated Java protobuf messages through accepted S5-A codegen/build wiring.
7. Implement minimal `ClientHello` validation and accepted `ServerHello` response including server time.
8. Implement controlled rejection for malformed/unsupported handshake; never crash the process.
9. Add structured logs with connection/session correlation where available.
10. Add automated tests for handshake encode/decode, accepted handshake, rejected handshake.
11. No DB, Redis, combat, inventory, account product flow, guild, market, housing.

## Acceptance

- clean server build from one documented command;
- API boots and health endpoint returns success;
- realtime process boots;
- generated Java protobuf types compile without manual DTO duplication;
- accepted handshake integration test passes;
- malformed/unsupported handshake rejection test passes;
- process remains alive after rejected handshake;
- no frozen contract changed.

## Handoff

Return `HANDOFF.md`, full changed-file list, exact start/build/test commands, ports/config defaults, actual Java/build-tool versions used, and known limitations.
