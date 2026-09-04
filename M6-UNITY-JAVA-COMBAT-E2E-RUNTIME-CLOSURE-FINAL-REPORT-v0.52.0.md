# M6 Unity-Java Combat E2E Runtime Closure Final Report v0.52.0

Final decision: `M6_UNITY_JAVA_COMBAT_E2E_CLOSED_LOCAL_v0.52.0`

## Summary

v0.52 adds a Unity runtime smoke that sends a server-path combat intent to the Java combat smoke server. The server uses the v0.51 pilot validation and Unity parses accepted, result, snapshot, and rejection evidence using existing protobuf classes.

## Required Markers

- `M6_UNITY_JAVA_COMBAT_E2E_PASS_v0.52.0`
- `M6_SERVER_AUTHORITATIVE_COMBAT_PILOT_PASS_v0.51.0`
- `M6_LOCAL_COMBAT_RUNTIME_CLOSURE_PASS_v0.50.0`
- `LGO_PLAYABLE_CLOSURE_RUNTIME_GATES_PASS`

## Frozen Surface Audit

- `protocol/**`: unchanged.
- `gamedata/schemas/**`: unchanged.
- `docs/adr/**`: unchanged.
- `client/Unity/Assets/Game/UI/design-tokens.json`: unchanged.

## Non-Claims

- No production combat.
- No production art.
- No enemy AI, inventory, loot, reward, economy, DB, auth, social, or liveops.
- No full MMO runtime closure.
