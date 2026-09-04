# M6 Combat Contract Impact Review v0.48.0

Decision: `NO_CONTRACT_CHANGE_REQUIRED_FOR_M6_V0_49_LOCAL_PROTOTYPE`.

## Reviewed Surfaces

- `protocol/**`
- `gamedata/schemas/**`
- `docs/04-NETWORK-CONTRACT.md`
- `docs/05-GAMEDATA-CONTRACT.md`
- `CombatPlaceholderAssets.cs`
- `M4PlayableClientController.cs`
- `PlayableWorldController.cs`
- M6 validators and playable closure orchestration.

## Protocol Impact

Current protocol already contains:

- `CombatIntent`
- `CombatAccepted`
- `CombatRejected`
- `CombatResult`
- `CombatStateSnapshot`
- `BasicAttackIntent`
- `SkillIntent`

v0.49 can express a local prototype intent, accepted/rejected classification, cooldown remaining, target validity, and placeholder result using these messages. No protobuf mutation is required for the next implementation task.

Contract change becomes required if v0.49 or later needs status-effect stacks, multi-hit payloads, per-resource costs, equipment stat snapshots, threat, aggro, rewards, durable HP sync, or server-side combat session envelopes not representable by current messages.

## GameData Impact

Current skill schema already contains activation, cooldown, targeting, effect placeholder amount, telegraph, damage coefficient, range, and tags. Current monster schema already contains level, max HP, move speed, and archetype.

v0.49 can use existing schema fields for `skill.sword.wind_slash` and existing prototype monster/target values. No schema mutation is required.

Contract change becomes required for real formulas, resistances, resource costs, status effects, loot tables, combat AI profiles, equipment scaling, skill upgrades, or non-placeholder damage semantics.

## Client Runtime Impact

Current client world/UI already separates player-facing Vietnamese local prototype copy from placeholder visual feedback. v0.49 may refactor local prototype state into a small deterministic combat component, but must preserve current account/character/world flow and avoid broad UI redesign.

## Server Impact

Existing M6 server validation work can remain a regression reference. v0.49 should not open full server-authoritative combat unless explicitly scoped. If it stays local-only, server code changes are not required.

## Contract Change Request

No `CONTRACT_CHANGE_REQUEST-M6-COMBAT-v0.48.0.md` is created because the next allowed task can proceed within existing contracts.

## Final Impact Classification

`NO_CONTRACT_CHANGE_REQUIRED`
