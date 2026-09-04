# LGO Server-Authoritative Combat Design v0.39.0

Status: `M6_SERVER_COMBAT_CONTRACT_SPEC_ACCEPTED_v0.39.0`

## Design Boundary

Server-authoritative combat is not implemented in v0.39.0. This document defines the intended split between local readability feedback and future authoritative outcomes.

No protocol or GameData changes are made by this v0.39.0 task.

## Client Intent / Server Authority Separation

The Unity client may show predicted/local feedback for responsiveness. The Java realtime server must own combat acceptance, rejection, result computation, cooldown truth, and state snapshots.

Future client UI must clearly distinguish:

- local/predicted feedback;
- accepted server state;
- rejected intent explanation;
- authoritative combat result.

## Future Message Roles

- `CombatIntent`: client request with actor, target, skill, sequence, and idempotency key.
- `CombatAccepted`: server acknowledgement that the intent passed validation and is queued/applied.
- `CombatRejected`: server rejection with stable reason code and latest known state hint.
- `CombatResult`: authoritative outcome such as effect application, miss/block, or resolved damage once balancing exists.
- `CombatStateSnapshot`: authoritative state for reconnect, reconciliation, and desync recovery.

## Future GameData Needs

- Skill activation rules define input, cast/channel, and permitted context.
- Cooldown rules define global and skill-specific lockouts.
- Targeting rules define range, line/cone/area, faction, and target validity.
- Effect rules define outcome payloads after balancing opens.
- Telegraph rules define readable warning timing for client presentation.

## Review Notes

The existing local dummy remains a prototype and must not become a hidden authority path. Future implementation should migrate from local feedback toward explicit client prediction plus server result reconciliation.
