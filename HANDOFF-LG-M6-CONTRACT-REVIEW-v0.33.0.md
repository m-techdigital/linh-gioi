# Linh Gioi Online - M6 Contract Review Handoff v0.33.0

Decision: `M6_MINIMAL_LOCAL_COMBAT_ALLOWED_WITHOUT_CONTRACT_CHANGE_v0.33.0`

Stage 2 is approved only for the smallest local target dummy combat foundation. It must not claim production combat, server-authoritative combat, DB persistence, loot, inventory, economy, PvP, anti-cheat, or protocol/GameData readiness.

## Ownership

- UI input and visible Vietnamese combat feedback: `client/Unity/Assets/Game/UI/Runtime/**`
- Target selection, local dummy state, local hit feedback, local damage placeholder, cooldown placeholder: `client/Unity/Assets/Game/World/Runtime/**`
- Smoke marker and closure wiring: Unity smoke runner plus `tools/lgo_playable_closure_check.sh`

## Code Quality / Duplication / Ownership Audit

PASS: The next stage must extend existing playable world/HUD/smoke patterns and avoid duplicated interaction logic or one-off state machines.

## Frozen Surface Audit

PASS: No frozen surfaces are changed by this stage.
