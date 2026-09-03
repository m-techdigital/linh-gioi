# 10 — Owner Review Playbook

Use this playbook when reviewing any sandbox handoff.

## Review outcomes

| Outcome | Meaning | Next action |
|---|---|---|
| ACCEPT | Handoff becomes next accepted baseline. | Package full source successor and update ledgers. |
| FIX_REQUIRED | Scope is valid, but defects or missing evidence remain. | Return focused fix prompt; preserve lifecycle phase. |
| BLOCKED | Missing baseline/tool/contract decision prevents safe progress. | Resolve blocker or record owner override. |
| REJECT | Handoff violates scope or unsafe assumptions. | Discard or restart from clean baseline. |

## Review sequence

1. Confirm source baseline SHA and overlay order.
2. Check changed/deleted files against allowed paths.
3. Inspect runtime claims and evidence class.
4. Validate no frozen contract drift unless explicitly approved.
5. Confirm no generated/cache/temp/build artifacts in delta.
6. Review known limitations and next step.
7. Accept only if next step does not bypass milestone gate.

## Owner override

An override is allowed only if the owner knowingly accepts risk. Record it using `docs/execution/templates/OWNER-OVERRIDE-RECORD.md`.

Override must include:

- exact blocked gate;
- reason for proceeding;
- risk accepted;
- mitigation;
- rollback point;
- date and owner statement.

## Fast review commands

For a source package:

```bash
sha256sum <artifact.zip>
unzip -t <artifact.zip>
./tools/validate_m0_source.sh
```

For a delta package:

```bash
unzip -l <delta.zip>
sha256sum <delta.zip>
```

Never accept a milestone from SHA/inventory alone if its exit gate requires runtime evidence.
