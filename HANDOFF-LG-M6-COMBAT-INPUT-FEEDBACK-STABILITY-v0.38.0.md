# Handoff - M6 Combat Input Feedback Stability v0.38.0

Status: `M6_COMBAT_INPUT_FEEDBACK_STABILITY_RUNTIME_CLOSED_LOCAL_v0.38.0`

Source marker: `M6_COMBAT_INPUT_FEEDBACK_STABILITY_SOURCE_READY_v0.38.0`

## Summary

v0.38.0 makes the existing local-only target dummy input feedback deterministic under repeated input and cooldown recovery.

## Runtime Ownership

`PlayableWorldController` remains the canonical owner of local cooldown/readiness state. UI displays the state and invokes the action only.

## Frozen Surface Audit

- `protocol/**`: unchanged.
- `gamedata/schemas/**`: unchanged.
- `docs/adr/**`: unchanged.
- `client/Unity/Assets/Game/UI/design-tokens.json`: unchanged.

## Code Quality / Duplication / Ownership Audit

No duplicate cooldown logic was added to UI. The smoke runner asserts repeated input blocked state and recovered state through world-owned APIs. The recovery helper is smoke-only and does not add a player-facing mechanic.

## Validation Evidence

- Source gates: PASS.
- Package-ready gates: PASS.
- Runtime gates: PASS with `M6_MINIMAL_LOCAL_COMBAT_RUNTIME_SMOKE_PASS` and `LGO_PLAYABLE_CLOSURE_RUNTIME_GATES_PASS`.
- Repeated-input smoke evidence observed `cooldownBlockedAfterRepeatedInput=true`, `cooldownBlockedFeedbackText="Chưa thể tấn công: Đang hồi chiêu mô phỏng cục bộ."`, and `attackAfterCooldownRecovered=true`.
- Visual evidence: READY with captured screenshots; human visual acceptance remains pending by design.
