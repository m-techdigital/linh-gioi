# M6 Unity-Java Combat E2E Runtime Closure v0.52.0

Status: `M6_UNITY_JAVA_COMBAT_E2E_SOURCE_READY_v0.52.0`

## Objective

Close the narrow Unity-to-Java combat evidence path using the existing protocol. Unity sends combat intent to the Java smoke server, Java validates it with the server-authoritative pilot, and Unity parses accepted/result/snapshot plus rejection evidence.

## Scope

- existing protocol only.
- No protocol/GameData schema changes.
- No new combat mechanics.
- No new images or production art.
- Existing local preview copy remains separate from server authority copy.

## Runtime Evidence

Required runtime marker: `M6_UNITY_JAVA_COMBAT_E2E_PASS_v0.52.0`.

The E2E runner checks:

- accepted Wind Slash response;
- server `CombatResult`;
- server `CombatStateSnapshot`;
- no target rejection;
- out of range rejection;
- cooldown rejection;
- invalid skill rejection.
