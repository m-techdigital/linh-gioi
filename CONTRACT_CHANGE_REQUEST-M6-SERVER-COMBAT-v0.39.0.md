# Contract Change Request - M6 Server Combat v0.39.0

Status: `M6_SERVER_COMBAT_CONTRACT_SPEC_ACCEPTED_v0.39.0`

No protocol/GameData changes are made by this v0.39.0 task.

## Why Current Contracts Are Insufficient

why current contracts are insufficient:

The current contracts support foundation movement/session surfaces and placeholder local combat readability. They do not define combat intent identity, authoritative acceptance/rejection, authoritative result delivery, state snapshots, cooldown validation, targeting rules, effect rules, telegraph rules, or idempotency.

## Proposed Protocol Areas

- `CombatIntent`
- `CombatAccepted`
- `CombatRejected`
- `CombatResult`
- `CombatStateSnapshot`

Each message must be reviewed for versioning, sequence handling, actor/target identity, rejection reason shape, and snapshot reconciliation.

## Proposed GameData Areas

- skill activation;
- cooldown;
- targeting rule;
- effect rule;
- telegraph rule.

## Consumers Affected

- Unity: intent submission, prediction display, rejection/result rendering, state reconciliation.
- Java realtime: validation, authority, result computation, idempotency, snapshot emission.
- tools/codegen: Protobuf generation and manifest determinism.
- validators: contract, schema, source, and non-claim gates.
- smoke tests: accepted intent, rejected intent, duplicate intent, cooldown, reconnect/snapshot.

## Migration Plan

1. v0.40 proposes protocol messages without implementation.
2. v0.41 proposes GameData combat schema areas without balancing production data.
3. v0.42 adds Java validation skeleton behind tests.
4. v0.43 adds Unity integration skeleton that separates prediction from authoritative result.
5. v0.44 adds client-server combat smoke.

## Backward Compatibility Note

backward compatibility:

Existing M4/M5/M6 local runtime smokes must remain valid while future combat messages are introduced. The local dummy prototype should remain clearly marked as local-only until it is replaced or bridged by authoritative smoke coverage.

## Explicit Statement

No protocol/GameData changes are made by this v0.39.0 task.
