# LGO Commit Cadence Policy Hardening v1.0

Marker: `LGO_COMMIT_CADENCE_POLICY_HARDENING_READY`

## Purpose

Align continuous work and autopilot with the owner preference for fewer, cleaner commits: checkpoint after coherent feature/phase/tooling batches instead of committing after every small edit.

## Scope

- Set autopilot auto-commit default to off.
- Keep push opt-in only.
- Document when to commit versus continue accumulating a related validated batch.
- Add validation so the default does not drift back to commit-heavy behavior.

## Boundaries

- Documentation/tooling only.
- No gameplay, runtime UI behavior, asset replacement, protocol, GameData schema, ADR, or design-token changes.
- No runtime or visual PASS is claimed by this task.

## Validation

- `python3.12 tools/validate_lgo_commit_cadence_policy_hardening.py`
- `git --no-pager diff --check`
- `./tools/lgo_playable_closure_check.sh --source-only`

## Next

Continue with `LGO-RUNTIME-UI-QUALITY-DEBT-TRIAGE-v1.0` to choose the next high-value UI/runtime quality batch from current evidence and source debt.
