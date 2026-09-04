# M6 Server-Authoritative Combat Pilot Final Report v0.51.0

Final decision: `M6_SERVER_AUTHORITATIVE_COMBAT_PILOT_CLOSED_LOCAL_v0.51.0`

## Summary

v0.51 hardens the Java realtime combat pilot without changing protocol or GameData schemas. `CombatValidationService` now has a pilot path that validates Wind Slash intent server-side and returns existing protobuf messages for accepted, result, and snapshot evidence.

## Runtime Markers

- `M6_SERVER_AUTHORITATIVE_COMBAT_PILOT_PASS_v0.51.0`
- `M6_LOCAL_COMBAT_RUNTIME_CLOSURE_PASS_v0.50.0`

## Covered Cases

- Valid Wind Slash accepted.
- No target rejected as `combat_intent_rejected_no_target`.
- Invalid target rejected as `combat_intent_rejected_target_entity_id`.
- Unknown skill rejected as `combat_intent_rejected_skill_id`.
- Out of range rejected as `combat_intent_rejected_out_of_range`.
- Cooldown active rejected as `combat_intent_rejected_cooldown`.

## Frozen Surface Audit

- `protocol/**`: unchanged.
- `gamedata/schemas/**`: unchanged.
- `docs/adr/**`: unchanged.
- `client/Unity/Assets/Game/UI/design-tokens.json`: unchanged.

## Contract Change

No contract change is required for this narrow pilot. Existing `CombatIntent.target_position`, `CombatAccepted`, `CombatRejected`, `CombatResult`, and `CombatStateSnapshot` were sufficient.

## Non-Claims

- No production combat claim.
- No production art claim.
- No enemy AI, loot, reward, inventory, DB, auth, economy, social, or liveops implementation.
- No full MMO runtime closure claim.
