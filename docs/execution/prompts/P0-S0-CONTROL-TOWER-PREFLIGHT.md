# P0 PROMPT — S0 CONTROL TOWER PREFLIGHT

You are the control-tower/preflight lane for Linh Giới Online M0 Batch 01.

## Authority

Authoritative specification baseline is `linh-gioi-m0-foundation-v0.1.zip` with SHA256:

`2b8e30bb4e1206c5e6364615c17980d274bee97c065f04272b4e44779a347421`

Logical baseline: `lg-m00-spec-v01`.

Do not redesign product or technical architecture.

## Required reading

Read completely before acting:

- `START-HERE.md`
- `README.md`
- `docs/01-PRODUCT-CONSTITUTION.md`
- `docs/03-TDD.md`
- `docs/04-NETWORK-CONTRACT.md`
- `docs/05-GAMEDATA-CONTRACT.md`
- `docs/09-DEFINITION-OF-DONE.md`
- `docs/10-INTEGRATION-RULES.md`
- `docs/M0-STATUS.md`
- `docs/execution/00-BASELINE-LOCK.md`
- `docs/execution/01-SEQUENTIAL-ORCHESTRATION.md`
- `docs/execution/02-NO-GIT-WORKFLOW.md`

## Goal

Verify that the source package is clean and suitable to begin sequential M0 runtime implementation.

## Required work

1. Confirm package/source provenance against the authoritative SHA256 if the ZIP is available.
2. Inspect the baseline file structure and frozen-contract files.
3. Install only the documented Python validation dependencies.
4. Run `./tools/validate_foundation.sh` without masking errors.
5. Confirm current M0 status remains `FOUNDATION_SPEC_READY / RUNTIME_NOT_YET_VERIFIED`.
6. Identify any contradiction between execution overlay and authoritative M0 contracts. Do not silently resolve it; report it.
7. Produce `HANDOFF.md` using the mandatory execution handoff contract.

## Forbidden

- no Unity project implementation;
- no Java project implementation;
- no protocol semantic change;
- no GameData schema semantic change;
- no design-token change;
- no feature implementation.

## Exit gate

PASS/VERIFY only if static baseline validation passes and no unapproved frozen-contract drift exists.
