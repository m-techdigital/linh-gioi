# M6 Java Server Combat Validation v0.41.0

Decision marker: M6_JAVA_SERVER_COMBAT_VALIDATION_SOURCE_READY_v0.41.0.

v0.41.0 adds a minimal Java server combat validation skeleton using generated Java protocol classes from the canonical v0.40.0 protobuf contract.

## Scope

- Validate CombatIntent protocol version, sequence, intent id, actor ownership placeholder, target dummy id, skill id, and cooldown.
- Return canonical CombatAccepted or CombatRejected protobuf messages.
- Use deterministic clock injection for cooldown tests.

## Non-Claims

- No protocol/GameData changes.
- No Unity runtime combat intent integration.
- No production combat balancing, damage application, rollback, auth, DB persistence, inventory, loot, economy, social, live ops, or production art.
