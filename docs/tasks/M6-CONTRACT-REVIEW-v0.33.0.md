# M6 Contract Review v0.33.0

Decision: `M6_MINIMAL_LOCAL_COMBAT_ALLOWED_WITHOUT_CONTRACT_CHANGE_v0.33.0`

## Answers

1. Minimal local combat can proceed without protocol changes only as a client-local prototype against the existing target dummy. Existing `protocol/combat.proto` is not sufficient to claim server-authoritative combat because it is not wired through the realtime server/session result path in this milestone.
2. Target dummy hit feedback can proceed without GameData schema changes if it stays local, prototype-labeled, non-balancing, and does not create new item, XP, loot, monster, or skill schema semantics.
3. Ownership:
   - skill activation input: `client/Unity/Assets/Game/UI/Runtime/**`
   - target selection: `client/Unity/Assets/Game/World/Runtime/**`
   - hit feedback: `client/Unity/Assets/Game/World/Runtime/**`
   - target dummy state: `client/Unity/Assets/Game/World/Runtime/**`
   - damage placeholder: `client/Unity/Assets/Game/World/Runtime/**`, local/non-authoritative only
   - cooldown placeholder: `client/Unity/Assets/Game/World/Runtime/**`, local/non-authoritative only
   - UI combat feedback: `client/Unity/Assets/Game/UI/Runtime/**`
   - runtime smoke marker: Unity smoke runner plus `tools/lgo_playable_closure_check.sh`
4. Non-production boundaries:
   - no server-authoritative combat
   - no real damage balancing
   - no loot/reward
   - no inventory/equipment
   - no PvP
   - no anti-cheat
5. Future protocol/GameData changes likely needed before server combat. This is the explicit future protocol/GameData changes list:
   - authoritative combat intent/result lifecycle and rejection semantics
   - target entity identity and lifetime ownership
   - skill/effect schema ownership for timing, coefficients, tags, ranges, targeting, and cooldowns
   - health/resource state snapshots
   - server combat event stream and replay/idempotency rules
   - balancing data versioning and migration rules
6. Stage 2 may proceed under the local-only boundary above.

## Code Quality / Duplication / Ownership Audit

PASS: Stage 2 must reuse existing world interaction, HUD refresh, and smoke runner patterns. It must not create a parallel gameplay state machine, protocol DTO, GameData schema, inventory, loot, economy, or production server combat path.

## Frozen Surface Audit

PASS: This review does not modify `protocol/**`, `gamedata/schemas/**`, `docs/adr/**`, or `client/Unity/Assets/Game/UI/design-tokens.json`.
