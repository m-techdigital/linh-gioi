# Handoff - M6 Server Authoritative Combat Contract Spec v0.39.0

Status: `M6_SERVER_COMBAT_CONTRACT_SPEC_ACCEPTED_v0.39.0`

## Summary

v0.39.0 defines the future server-authoritative combat contract boundary. It is docs/tooling only and does not implement combat.

## Frozen Surface Audit

- `protocol/**`: unchanged.
- `gamedata/schemas/**`: unchanged.
- `docs/adr/**`: unchanged.
- `client/Unity/Assets/Game/UI/design-tokens.json`: unchanged.
- `server/**`: no implementation change.

## Code Governance Audit

The spec requires no duplicate DTO, no parallel combat config, no bypassing Protobuf/GameData, and validator coverage before implementation. Unity prediction must remain separate from Java realtime authority.

## Contract Change Request Summary

`CONTRACT_CHANGE_REQUEST-M6-SERVER-COMBAT-v0.39.0.md` explains why current contracts are insufficient, lists `CombatIntent`, `CombatAccepted`, `CombatRejected`, `CombatResult`, and `CombatStateSnapshot`, names required GameData areas, affected consumers, migration plan, and backward compatibility notes.

## Non-Claims

- No protocol change.
- No GameData schema change.
- No server combat implementation.
- No production combat.
- No auth, DB, inventory, economy, social, or live ops.
