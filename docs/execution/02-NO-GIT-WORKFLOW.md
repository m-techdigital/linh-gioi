# 02 — Sequential Workflow Before Git Exists

Git is strongly recommended later, but M0 Batch 01 can proceed safely without it if provenance is explicit.

## Directory convention

Keep three local roots:

```text
linh-gioi-baseline-original/
linh-gioi-integration-current/
linh-gioi-sandbox-work/
```

Never edit `linh-gioi-baseline-original/`.

## Initialize

1. Verify SHA256 of the original M0 ZIP.
2. Extract original ZIP into `linh-gioi-baseline-original/`.
3. Copy it to `linh-gioi-integration-current/`.
4. Apply this execution overlay to `linh-gioi-integration-current/`.
5. Run `./tools/validate_foundation.sh`.
6. Record a manifest/hash before starting P1.

## Starting each sandbox

For phase Pn:

1. Create a fresh ZIP of `linh-gioi-integration-current/` with **no parent wrapper directory**.
2. Record its SHA256.
3. Give that ZIP and only the current phase prompt to the sandbox.
4. Sandbox must not assume files not present in that ZIP.
5. Sandbox returns:
   - `HANDOFF.md`;
   - a delta ZIP or full clean source ZIP as requested;
   - exact changed/add/delete file list;
   - exact tests/results;
   - output SHA256.

## Accepting a handoff

Before moving to next phase:

1. inspect allowed/forbidden path compliance;
2. reject any silent frozen-contract mutation;
3. apply delta to a copy of current integration source;
4. run impacted validation/tests;
5. run static foundation validation again if shared foundation/tooling changed;
6. if PASS, replace `linh-gioi-integration-current/` with the verified copy;
7. record overlay name + SHA256 in `docs/execution/INTEGRATION-LEDGER.md`.

## Deletions

A delta handoff that deletes files must include a machine-readable deletion list, for example:

`HANDOFF-DELETIONS.txt`

Never infer deletions from absence in a partial ZIP.

## Do not do

- do not let the next sandbox start from the original ZIP after accepted overlays already exist;
- do not merge two independently evolved full source trees by visually comparing folders;
- do not accept generated code manually edited by a sandbox;
- do not call skipped/unavailable runtime checks PASS;
- do not overwrite frozen contract files with feature-lane copies.
