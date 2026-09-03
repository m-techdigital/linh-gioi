# 08 — Delivery Cadence and Sandbox Flow

This process keeps solo/parallel development moving while avoiding drift.

## Default cadence

| Rhythm | Output |
|---|---|
| Daily when active | one clear current state update in `PROJECT-STATE.md` or handoff report |
| Per task | source delta, evidence, handoff, SHA |
| Per milestone | acceptance record, full source successor, rollback package |
| Before next milestone | gate checklist reviewed and status updated |

## Standard task lifecycle

```text
TASK -> DEEP_POST -> HANDOFF -> OWNER_REVIEW -> ACCEPTED_BASELINE
```

- `TASK`: implementation or documentation change.
- `DEEP_POST`: independent revalidation against the task goal and forbidden scope.
- `HANDOFF`: final packaging only; no new functionality unless verification fails.
- `OWNER_REVIEW`: owner/control tower accepts, rejects, or asks for fix.
- `ACCEPTED_BASELINE`: exact source package becomes next input.

## Anti-loop rules

- Do not rerun evidence already PASS on identical source/runtime/provenance.
- Rerun only missing/invalidated slices.
- Source change invalidates only impacted evidence.
- Environment limitation is not a PASS.
- `executed=0` is not a PASS.
- No `|| true` masking in evidence commands.

## Branch/source packaging rule

For ZIP handoffs:

- delta ZIP contains only changed files relative to baseline;
- full-source ZIP contains the whole successor source tree;
- no parent wrapper folder unless explicitly requested;
- include SHA256 next to every ZIP;
- include changed/deleted file inventory;
- include evidence or limitation report.

## Parallel work rule

Parallel work is allowed only when all lanes share one exact baseline and their hot files do not conflict.

Default merge sequence:

1. S0 contract/foundation/gate docs
2. S5 tooling/validation
3. S2 server/runtime
4. S1 client/runtime
5. S3 UI/UX
6. S4 content
7. S5 final CI/evidence closure

## When to stop

Stop and return `BLOCKED` if any condition is true:

- task requires a frozen contract change without S0 approval;
- runtime/tool is unavailable and no accepted evidence route exists;
- source baseline is ambiguous;
- generated code would need manual patching;
- requested work would start a locked milestone.

## Minimal owner review checklist

1. Is the source baseline correct?
2. Are changed files inside allowed scope?
3. Is the evidence real and non-masked?
4. Are runtime limitations honest?
5. Does the handoff include rollback instructions?
6. Does the next step respect the milestone gate?
