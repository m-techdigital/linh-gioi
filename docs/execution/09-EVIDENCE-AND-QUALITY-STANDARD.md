# 09 — Evidence and Quality Standard

This standard defines what counts as proof in Linh Giới Online handoffs.

## Evidence classes

| Class | Valid proof | Not valid |
|---|---|---|
| Source/static | linters, validators, static contract checks, compile without runtime | claiming runtime PASS |
| Unit | test command, nonzero executed count, result log | dry-run/zero-test PASS |
| Integration | two or more real components interacting | mocked components unless task scope says mock |
| Runtime | actual tool/process boot and observed behavior | source inspection or planned command |
| Manual/player | screen/log/video/user-observed flow | statement without artifact or steps |
| Environment limitation | exact missing tool/version/network error | treating limitation as success |

## Required evidence fields

Every evidence record should include:

```text
command:
working_directory:
source_sha256_or_commit:
runtime_versions:
started_at:
finished_at:
exit_code:
executed_count_if_tests:
summary:
artifact_paths:
limitations:
```

## Runtime truth rules

- Java runtime acceptance requires Java 25 and Maven 3.9.x actually invoked.
- Unity runtime acceptance requires Unity `6000.3.2f1` logs from import/compile/test/build.
- Unity player evidence requires the player artifact to be built by Unity, SHA verified, and replayed against a real Java realtime server.
- Browser/UI acceptance requires browser/runtime execution, not screenshot-only source review.

## Generated-code rule

Generated code may be regenerated from canonical input. It must not be hand-edited unless the generator itself is the task and the generated output is explicitly an artifact.

## Handoff quality bar

A good handoff lets another sandbox reproduce the result without reading the whole conversation. It must include exact files, exact commands, exact limitations, and the next allowed step.

## Regression policy

For each task, record:

- direct evidence: tests for changed area;
- safety regression: minimum source validation;
- frozen surface audit: contract files unchanged unless approved;
- runtime regression: required only when runtime-affecting files changed.
