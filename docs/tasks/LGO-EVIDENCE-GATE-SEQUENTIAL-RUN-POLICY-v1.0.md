# LGO Evidence Gate Sequential Run Policy v1.0

Status: `LGO_EVIDENCE_GATE_SEQUENTIAL_RUN_POLICY_READY`

## Scope

This task records and validates that evidence-producing gates must not run in parallel with validators reading the same evidence output.

## Implemented

- `AGENTS.md` now warns against parallel gates sharing mutable evidence outputs.
- `docs/execution/CODEX-CONTINUOUS-WORKFLOW.md` records the sequential run policy.
- `docs/execution/LGO-EVIDENCE-GATE-SEQUENTIAL-RUN-POLICY-v1.0.md` defines affected commands.
- `tools/validate_lgo_evidence_gate_sequential_run_policy.py` protects the rule.

## Validation

- `python3.12 tools/validate_lgo_evidence_gate_sequential_run_policy.py`
- `git --no-pager diff --check`
- `./tools/lgo_playable_closure_check.sh --source-only`

## Follow-Up

Continue with `LGO-POST-LOGIN-RUNTIME-UI-REUSE-CLEANUP-v1.0`: continue reducing repeated post-login UI composition now that evidence and asset governance gates are stronger.
