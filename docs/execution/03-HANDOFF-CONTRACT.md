# 03 — Mandatory Sandbox Handoff Contract

Every sandbox must return a `HANDOFF.md` with the exact structure below.

```text
# HANDOFF

TASK_ID:
PHASE:
STATUS: VERIFY | BLOCKED
SOURCE_BASELINE_ID: lg-m00-spec-v01
SOURCE_BASELINE_SHA256:
PRIOR_ACCEPTED_OVERLAYS:

## Goal completed

## Changed files
### Added
### Modified
### Deleted

## Allowed-path compliance

## Contracts consumed

## Contract changes
NONE
or
CONTRACT_CHANGE_REQUEST present

## Build/tool versions actually used

## Commands executed

## Test/evidence results
- command
- executed count where meaningful
- PASS/FAIL

## Runtime/manual verification

## Known limitations

## Integration steps

## Rollback notes

## Output artifact SHA256
```

## Status rules

`VERIFY` means implementation is ready for control-tower review. It does **not** mean milestone DONE.

`BLOCKED` is required when:

- frozen contract must change;
- required runtime/tool cannot be installed or invoked;
- authoritative baseline/provenance is unclear;
- task would require editing a forbidden path;
- upstream accepted artifact is missing.

## Evidence rules

- No `|| true` masking.
- No `executed=0` as PASS.
- Do not claim a UI/runtime test PASS from source inspection alone.
- Generated code must be regenerated, not hand-patched.
- If a command cannot run, state the exact limitation and leave that gate unverified.
