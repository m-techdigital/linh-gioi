# M6 Combat Input Feedback Stability Final Report v0.38.0

Final decision: `M6_COMBAT_INPUT_FEEDBACK_STABILITY_RUNTIME_CLOSED_LOCAL_v0.38.0`

## Scope Result

The local-only target dummy loop now has deterministic repeated input and cooldown recovery checks.

## Non-Claims

- No server combat.
- No protocol change.
- No GameData schema change.
- No production combat.
- No auth, DB, inventory, economy, social, or live ops.

## Validation

Runtime PASS is only valid after `./tools/lgo_playable_closure_check.sh --runtime` emits `M6_MINIMAL_LOCAL_COMBAT_RUNTIME_SMOKE_PASS` and `LGO_PLAYABLE_CLOSURE_RUNTIME_GATES_PASS`.

Observed runtime evidence:

- `M6_MINIMAL_LOCAL_COMBAT_RUNTIME_SMOKE_PASS`
- `LGO_PLAYABLE_CLOSURE_RUNTIME_GATES_PASS`
- `cooldownBlockedAfterRepeatedInput=true`
- `cooldownBlockedFeedbackText="Chưa thể tấn công: Đang hồi chiêu mô phỏng cục bộ."`
- `cooldownRecoveredText="Hồi chiêu: Sẵn sàng"`
- `attackAfterCooldownRecovered=true`

Visual evidence emitted `LGO_PLAYABLE_VISUAL_EVIDENCE_READY` with `screenshotStatus=CAPTURED` and `humanVisualAcceptancePending=true`.
