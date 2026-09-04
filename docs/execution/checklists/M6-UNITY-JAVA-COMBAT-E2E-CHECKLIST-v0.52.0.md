# M6 Unity-Java Combat E2E Checklist v0.52.0

- [x] v0.51 server-authoritative pilot baseline used.
- [x] Combat smoke server can emit accepted/result/snapshot for server-path intents.
- [x] Legacy Unity-Java smoke remains compatible with local preview intent.
- [x] Unity E2E runner sends server-path Wind Slash intent.
- [x] Unity parses `CombatAccepted`.
- [x] Unity parses `CombatResult`.
- [x] Unity parses `CombatStateSnapshot`.
- [x] Unity observes no target, out of range, cooldown, and invalid skill rejections.
- [x] Frozen protocol/schema/ADR/design-token surfaces unchanged.

Decision: `M6_UNITY_JAVA_COMBAT_E2E_CLOSED_LOCAL_v0.52.0` after local runtime gates pass.
