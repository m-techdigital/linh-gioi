# LGO Execution Ledger Rollup View v1.0

Marker: `LGO_EXECUTION_LEDGER_ROLLUP_VIEW_READY`

## Purpose

Create a compact, regenerated rollup for the append-only task ledger so long continuous sessions can scan recent work quickly without deleting or rewriting historical rows.

## Scope

- Add `docs/execution/TASK-LEDGER-ROLLUP.md`.
- Add `tools/report_lgo_task_ledger_rollup.py` with a `--check` mode.
- Add a validator that prevents stale rollup output.
- Keep `docs/execution/TASK-LEDGER.md` as the append-only source of truth.

## Boundaries

- Documentation/tooling only.
- No gameplay, runtime UI behavior, asset replacement, protocol, GameData schema, ADR, or design-token changes.
- No runtime or visual PASS is claimed by this task.

## Validation

- `python3.12 tools/report_lgo_task_ledger_rollup.py --check`
- `python3.12 tools/validate_lgo_execution_ledger_rollup_view.py`
- `git --no-pager diff --check`
- `./tools/lgo_playable_closure_check.sh --source-only`

## Next

Continue with `LGO-COMMIT-CADENCE-POLICY-HARDENING-v1.0` so the continuous workflow follows the owner's preference to commit/push only after coherent feature or phase checkpoints.
