# 04 — Integration Checklist

Use this after every phase.

## Provenance

- [ ] Input SHA256 recorded.
- [ ] Previous accepted overlays listed.
- [ ] Output SHA256 recorded.
- [ ] Sandbox started from current integration source, not stale original source.

## Scope

- [ ] Only allowed paths changed.
- [ ] Frozen contracts unchanged unless S0-approved.
- [ ] No unrelated refactor.
- [ ] No manually edited generated source.
- [ ] Deletions explicitly declared.

## Verification

- [ ] Sandbox commands captured.
- [ ] Relevant tests actually executed.
- [ ] Failures are not masked.
- [ ] Environmental limitation not represented as PASS.
- [ ] Static foundation validation rerun if appropriate.

## Integration

- [ ] Changes applied to a clean copy of current integration baseline.
- [ ] Impacted build/tests pass after application.
- [ ] Current baseline updated only after verification.
- [ ] Integration ledger updated.

## Decision

- [ ] ACCEPTED — next phase may start.
- [ ] REJECTED — sandbox must fix handoff.
- [ ] BLOCKED — S0 decision required.
