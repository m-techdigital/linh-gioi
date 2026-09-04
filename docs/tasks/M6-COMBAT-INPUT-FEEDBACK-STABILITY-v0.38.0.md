# M6 Combat Input / Feedback Stability v0.38.0

Status: `M6_COMBAT_INPUT_FEEDBACK_STABILITY_SOURCE_READY_v0.38.0`

## Decision

v0.38.0 stabilizes the existing local-only target dummy input and feedback loop. Combat remains a client-side prototype for readability and smoke testing only.

## Canonical Owner

`PlayableWorldController` owns the local dummy combat truth:

- target range;
- readiness counter;
- cooldown timing;
- hit acknowledgement;
- combat feedback text.

The UI only displays state and invokes the existing local action. It does not own cooldown or readiness truth.

## Stability Behavior

- First valid `Tấn công thử` near the dummy emits `Trúng mục tiêu`.
- Repeated input during cooldown is blocked deterministically with `Chưa thể tấn công` and `Đang hồi chiêu`.
- Cooldown recovery restores the `Sẵn sàng` state.
- A later valid attack after recovery emits local hit feedback again.
- All copy remains Vietnamese and labels the feature as `Mô phỏng cục bộ`.

## Boundaries

- No real combat balancing.
- No server combat.
- No projectile system.
- No AI enemy.
- No HP/death progression beyond the existing local dummy prototype.
- No loot, reward, XP, inventory, or economy.
- No protocol or GameData schema change.
- No production art.

## Runtime Evidence Requirement

Runtime evidence must include:

- repeated attack input does not corrupt local state;
- cooldown blocked state observed;
- cooldown recovery state observed;
- `M6_MINIMAL_LOCAL_COMBAT_RUNTIME_SMOKE_PASS`;
- `LGO_PLAYABLE_CLOSURE_RUNTIME_GATES_PASS`.

## Code Quality / Duplication / Ownership Audit

Cooldown/readiness logic is not duplicated in the UI. The smoke runner uses the world controller API to assert deterministic local behavior. The smoke-only cooldown recovery helper is explicitly named for smoke usage and does not create a player-facing mechanic.
