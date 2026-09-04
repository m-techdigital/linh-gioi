# M6 Server-Authoritative Combat Pilot v0.51.0

Status: `M6_SERVER_AUTHORITATIVE_COMBAT_PILOT_SOURCE_READY_v0.51.0`

## Objective

Add a narrow Java realtime combat pilot on top of the existing combat protocol and GameData values. This task keeps Unity local preview behavior intact while proving that the server can validate a combat intent and emit deterministic protobuf evidence.

## Scope

- existing protocol only: `CombatIntent`, `CombatAccepted`, `CombatRejected`, `CombatResult`, and `CombatStateSnapshot`.
- Existing dev skill values only: Wind Slash, 6000 ms cooldown, 4.5 m range, placeholder effect amount 12.
- Server-side validation for accepted, no target, invalid target, unknown skill, out of range, and cooldown active paths.
- Runtime smoke marker: `M6_SERVER_AUTHORITATIVE_COMBAT_PILOT_PASS_v0.51.0`.

## Non-Claims

- No production combat claim.
- No full MMO runtime closure claim.
- No production art claim.
- No DB/auth/economy/social/liveops claim.
- No protocol/GameData schema changes.

## Validation Contract

The pilot is accepted only when Java tests execute non-zero combat cases, the source validator passes, the smoke script emits the v0.51 marker, and closure gates continue to pass.
