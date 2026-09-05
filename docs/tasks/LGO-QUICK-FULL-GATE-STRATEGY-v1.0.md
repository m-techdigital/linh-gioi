# LGO Quick / Full Gate Strategy v1.0

Status: `LGO_QUICK_FULL_GATE_STRATEGY_READY`

## Scope

This task adds an explicit fast-iteration versus full-validation policy for continuous Linh Giới Online development.

## Implemented

- `tools/lgo_continue_dev_loop.sh` now supports `LGO_DEV_LOOP_GATE_PROFILE=quick|full`.
- `quick` keeps the targeted validator set for daily iteration.
- `full` runs `git --no-pager diff --check` plus `./tools/lgo_playable_closure_check.sh --source-only`.
- Quick/full policy is documented in `docs/execution/LGO-QUICK-FULL-GATE-STRATEGY-v1.0.md`.
- Closure validation includes `tools/validate_lgo_quick_full_gate_strategy.py`.

## Validation

- `python3.12 tools/validate_lgo_quick_full_gate_strategy.py`
- `./tools/lgo_continue_dev_loop.sh` may run in quick mode by default.
- `LGO_DEV_LOOP_GATE_PROFILE=full ./tools/lgo_continue_dev_loop.sh` is reserved for feature-sized checkpoints and handoff/release gates.

## Non-Claims

- Quick gate success is not package readiness.
- Quick visual capture is not human visual acceptance.
- No gameplay, protocol, GameData, ADR, design-token, auth, DB, economy, social, or liveops change.

## Follow-Up

Continue with `LGO-VISUAL-EVIDENCE-BLANK-SCREEN-DETECTION-v1.0`: add an evidence sanity check so blank/gray screenshots cannot satisfy evidence validators by filename alone.
