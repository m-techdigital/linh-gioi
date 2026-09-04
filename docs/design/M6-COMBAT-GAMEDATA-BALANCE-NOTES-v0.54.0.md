# M6 Combat GameData Balance Notes v0.54.0

Status: M6_COMBAT_GAMEDATA_BALANCE_READY_FOR_VERIFY_v0.54.0

## Current Accepted State

M6 local/server combat uses existing placeholder GameData only. `skill.sword.wind_slash` remains the readable single-target prototype action, and `monster.shadow.slime` remains the simple dummy-compatible monster data point.

## Development Bounds

- Skill cooldown must be readable and internally consistent: top-level `cooldown_ms` matches `cooldown.skill_ms`.
- Global cooldown must stay lower than or equal to the skill cooldown and remain visible enough for UI feedback.
- Single-target skills must require a target.
- `range_m` must match `targeting.max_range_m` so client readability and server validation do not drift.
- Placeholder effect amount and damage coefficient are constrained as development-only values, not production formulas.
- Monster HP/readiness values must be positive and stay inside current M6 dev bounds.

## Contract Decision

Decision: NO_SCHEMA_CHANGE_REQUIRED.

The existing GameData schemas can express the current M6 prototype values and reject the adversarial cases required for v0.54. Future production combat tuning may require schema expansion, but this task does not.

## Runtime Impact

No runtime values were changed in v0.54. Existing runtime smoke coverage remains the correct proof for gameplay behavior.

