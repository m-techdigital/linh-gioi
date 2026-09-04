# M6 Unity Java Combat Smoke v0.43.0

Decision marker: M6_UNITY_JAVA_COMBAT_SMOKE_SOURCE_READY_FOR_RUNTIME_v0.43.0.

v0.43.0 adds a local loopback smoke harness where Unity sends canonical protobuf CombatIntent payloads to a Java realtime-module combat smoke server and receives canonical CombatAccepted or CombatRejected responses.

Smoke matrix:

- accepted dummy intent
- rejected invalid target intent
- malformed payload survival

No protocol/GameData changes, no new mechanics, no production networking/auth/DB, and no production combat claim are included.
