# P3 PROMPT — S1 UNITY FOUNDATION

You are S1. Implement the real Unity client foundation against accepted S5-A tooling and accepted S2 realtime server.

## Preconditions

- P0 accepted.
- P1 S5-A accepted.
- P2 S2 accepted; realtime handshake target and canonical commands are known.

If required upstream artifacts are missing, report BLOCKED instead of inventing replacements.

## Required reading

Read completely:

- `README.md`
- `docs/01-PRODUCT-CONSTITUTION.md`
- `docs/03-TDD.md`
- `docs/04-NETWORK-CONTRACT.md`
- `docs/06-UI-DESIGN-SYSTEM.md`
- `docs/07-ART-BIBLE.md`
- `docs/09-DEFINITION-OF-DONE.md`
- `docs/10-INTEGRATION-RULES.md`
- `docs/11-PERFORMANCE-BUDGET.md`
- `docs/tasks/S1-UNITY-FOUNDATION.md`
- accepted S5-A codegen instructions
- accepted S2 runtime/start instructions
- `docs/execution/03-HANDOFF-CONTRACT.md`

## Goal

Create a clean Unity 6.3 LTS URP project foundation with explicit module boundaries and a minimal realtime handshake client.

## Allowed paths

- `client/Unity/**`

Exception: do not implement S3-owned UI component library under `Assets/Game/UI/**`; only consume interfaces/placeholders already permitted by the contract.

## Forbidden paths

- `protocol/**`
- `gamedata/**`
- `server/**`
- `docs/adr/**`
- S3 component implementation

## Required implementation

1. Initialize/open a valid Unity 6.3 LTS project at `client/Unity`.
2. Configure URP appropriate for mobile/PC stylized 3D without doing art/content production.
3. Create one bootstrap scene owned by S1.
4. Create `GameBootstrap` or equivalent composition root with explicit initialization ordering.
5. Add sensible Assembly Definitions for Foundation, GameData, Networking, Character, Combat, World, Social, Inventory, Tests; avoid circular dependencies and do not create empty complexity purely for architecture theater.
6. Consume generated C# protobuf output from accepted S5-A tooling; never hand-write equivalent protocol DTOs.
7. Implement a minimal realtime client abstraction that can connect to accepted S2, send `ClientHello`, parse `ServerHello`, surface accepted/rejected/disconnected states.
8. Expose connection state via interface/event; do not couple Networking directly to S3 UI implementation.
9. Add serialization/bootstrap tests and an actual handshake integration verification against S2 where environment permits.
10. No combat, production character controller, world event, guild, market, housing, custom UDP.

## Acceptance

- Unity project opens cleanly in the required version;
- bootstrap scene enters play with no exception;
- asmdefs compile with no cycle;
- C# protobuf generation/consumption compiles;
- `ClientHello` serialization round trip passes;
- client connects to accepted S2 and receives accepted `ServerHello` in a real integration run;
- controlled rejection is surfaced as a connection state, not an unhandled exception;
- no S3 production scene/prefab ownership violation.

If Unity runtime cannot execute in the sandbox, do not call those gates PASS. Provide source/build evidence and exact local canonical commands; runtime gate remains pending.
