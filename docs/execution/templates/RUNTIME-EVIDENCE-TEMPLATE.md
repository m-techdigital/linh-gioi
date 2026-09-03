# Runtime Evidence — <TASK_ID>

## Command evidence

| Field | Value |
|---|---|
| Command | `<command>` |
| CWD | `<cwd>` |
| Started at | `<timestamp>` |
| Ended at | `<timestamp>` |
| Exit code | `<code>` |
| Classification | `<PASS | FAIL | UNVERIFIED_ENVIRONMENT | DEFERRED_BY_TOOLING>` |
| Stdout/stderr file | `<path>` |
| Test count | `<count or n/a>` |
| Skipped count | `<count or n/a>` |

## Required assertions

- [ ] command actually executed.
- [ ] exit code captured.
- [ ] output preserved.
- [ ] no `|| true` masking.
- [ ] no zero-test PASS.
- [ ] source inspection not promoted to runtime PASS.
- [ ] version pins preserved.
