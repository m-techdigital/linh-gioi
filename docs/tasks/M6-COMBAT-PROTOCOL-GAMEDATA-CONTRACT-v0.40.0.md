# M6 Combat Protocol + GameData Contract v0.40.0

Decision marker: M6_COMBAT_PROTOCOL_GAMEDATA_CONTRACT_ACCEPTED_v0.40.0.

This task implements the minimal canonical wire and content contract approved by
`CONTRACT_CHANGE_REQUEST-M6-SERVER-COMBAT-v0.39.0.md`.

## Scope

- Add protobuf messages: CombatIntent, CombatAccepted, CombatRejected, CombatResult, CombatStateSnapshot.
- Keep existing BasicAttackIntent, SkillIntent, and CombatResultEvent fields unchanged for compatibility.
- Add GameData skill contract fields for skill activation, cooldown, targeting rule, effect rule placeholder, and telegraph/readability rule placeholder.
- Add invalid GameData regression coverage for combat-specific nested rules.
- Keep protocol_version = 1 and gamedata_version = 1.

## Semantics

- The client sends CombatIntent as intent/input only.
- The server validates later and responds with CombatAccepted or CombatRejected.
- CombatResult and CombatStateSnapshot are server-authored result/snapshot messages.
- Local client preview remains separate from accepted server result.

## Non-Claims

- No Java server combat validation implementation is added by v0.40.0.
- No Unity combat intent integration is added by v0.40.0.
- No production auth, DB persistence, inventory, loot, economy, guild, chat, market, party, live ops, or production art is added.
- No full MMO combat readiness is claimed.

## Ownership

Protocol and GameData changes are allowed only for this v0.40.0 contract task because v0.39.0 produced the explicit contract change request.
