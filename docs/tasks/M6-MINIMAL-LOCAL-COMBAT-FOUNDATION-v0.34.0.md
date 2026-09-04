# M6 Minimal Local Combat Foundation v0.34.0

Decision: `M6_MINIMAL_LOCAL_COMBAT_FOUNDATION_SOURCE_READY_v0.34.0`

Adds the smallest local-only target dummy combat foundation allowed by v0.33.0.

- Player-facing UI uses Vietnamese: "Tấn công thử", "Mục tiêu luyện tập", "Trúng mục tiêu", "Hồi chiêu", "Chỉ là mô phỏng cục bộ", and "Chưa phải chiến đấu thật".
- Target dummy feedback is local-only and non-authoritative.
- The readiness display is prototype/local only.
- The cooldown is local/non-authoritative only.
- no loot/reward
- no XP/level
- no inventory mutation
- no server-authoritative combat
- no protocol or GameData schema change

Runtime smoke marker: `M6_MINIMAL_LOCAL_COMBAT_RUNTIME_SMOKE_PASS`.

## Code Quality / Duplication / Ownership Audit

PASS: The implementation reuses `PlayableWorldController`, existing VFX feedback markers, HUD refresh, and bootstrap command-line smoke patterns. It does not add a parallel combat state machine.

## Frozen Surface Audit

PASS: Frozen surfaces remain unchanged.
