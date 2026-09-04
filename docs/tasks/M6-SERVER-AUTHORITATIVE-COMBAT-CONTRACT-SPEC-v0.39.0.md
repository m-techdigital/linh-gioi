# M6 Server-Authoritative Combat Contract Spec v0.39.0

Status: `M6_SERVER_COMBAT_CONTRACT_SPEC_ACCEPTED_v0.39.0`

## Decision

v0.39.0 is a contract/spec task only. It defines the future server-authoritative combat boundary and required contract changes before implementation.

No protocol or GameData schema changes are made by this v0.39.0 task.

## Authority Model

- Client sends combat intent/input.
- Server validates identity, target, range, cooldown, idempotency, and skill legality.
- Server computes accepted outcome later.
- Client displays predicted/local feedback separately from authoritative combat result.
- Server emits authoritative accepted, rejected, result, and state snapshot events.

## Required Future Protocol Messages

- `CombatIntent`
- `CombatAccepted`
- `CombatRejected`
- `CombatResult`
- `CombatStateSnapshot`

These messages must be proposed in a future protocol contract task before implementation.

## Required Future GameData Schema Areas

- skill activation;
- cooldown;
- targeting rule;
- effect rule;
- telegraph rule.

These schema areas must be proposed in a future GameData contract task before implementation.

## Non-Goals

- No production balancing yet.
- No PvP yet.
- No anti-cheat complete yet.
- No economy, inventory, or loot yet.
- No DB persistence yet.
- No production combat implementation.

## Rollout Plan

- v0.40 protocol proposal.
- v0.41 GameData combat schema proposal.
- v0.42 Java combat validation skeleton.
- v0.43 Unity client integration skeleton.
- v0.44 client-server combat smoke.

## Risk Map

- Client prediction vs authority: predicted feedback must not masquerade as accepted damage.
- Desync: snapshots must reconcile local presentation.
- Cooldown spoofing: server time and authoritative cooldown rules must own final acceptance.
- Duplicate command: intents need sequence/idempotency keys.
- Reconnect: latest state snapshot must restore combat state.
- Replay/idempotency: repeated packets must not double-apply outcomes.

## Code Governance

- No duplicate DTO.
- No parallel combat config.
- No bypassing Protobuf/GameData.
- Validator required before implementation.
- Future implementation must keep protocol, GameData, server, Unity, and tooling ownership separated.
