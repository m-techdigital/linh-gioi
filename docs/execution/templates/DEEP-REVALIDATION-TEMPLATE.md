# Deep Post-Implementation Revalidation

TASK_ID:
BASELINE:
CANDIDATE_ARTIFACT:
PHASE: DEEP_POST
DECISION: PASS_FOR_FINAL_HANDOFF | FIX_REQUIRED | BLOCKED_CONTRACT | BLOCKED_ENVIRONMENT

## Revalidation rules

- Do not add new functionality.
- Do not redesign architecture.
- Do not touch frozen contracts unless the task explicitly approved it.
- Reuse valid evidence only when source, runtime, toolchain, and provenance are unchanged.
- Do not mask failures.

## Scope audit

| Item | Result | Notes |
|---|---|---|
| Allowed paths only | | |
| Frozen contracts unchanged | | |
| No cache/build artifacts | | |
| Generated code produced by generator | | |
| Known limitations honest | | |

## Evidence audit

| Evidence | Expected | Actual | Result |
|---|---|---|---|
| Source validation | PASS | | |
| Direct task tests | PASS | | |
| Regression tests | PASS or not impacted | | |
| Runtime evidence | PASS or documented limitation | | |

## Findings

### Critical

### Major

### Minor

## Final decision
