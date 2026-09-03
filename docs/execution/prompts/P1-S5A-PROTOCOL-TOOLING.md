# P1 PROMPT — S5-A PROTOCOL / CODEGEN TOOLING BOOTSTRAP

You are S5-A for Linh Giới Online M0 Batch 01.

This is deliberately a **partial S5 task**. Do not attempt to close all CI/runtime gates before S1/S2/S4 exist.

## Required reading

Read completely:

- `README.md`
- `docs/01-PRODUCT-CONSTITUTION.md`
- `docs/03-TDD.md`
- `docs/04-NETWORK-CONTRACT.md`
- `docs/09-DEFINITION-OF-DONE.md`
- `docs/10-INTEGRATION-RULES.md`
- `docs/tasks/S5-QA-CI-FOUNDATION.md`
- all `protocol/*.proto`
- `docs/execution/00-BASELINE-LOCK.md`
- `docs/execution/03-HANDOFF-CONTRACT.md`

## Goal

Create one reproducible protobuf validation/code-generation foundation that later S2 Java and S1 Unity consume without manually redefining DTOs.

## Allowed paths

- `tools/**`
- `tests/**`
- root CI/tool configuration only if required
- generated-output directories only if the M0 architecture expects checked-in generated code; otherwise prefer reproducible generation and document generated paths

## Forbidden paths

- `protocol/**` semantic edits
- `gamedata/schemas/**`
- `server/**` production implementation
- `client/**` production implementation
- `docs/adr/**`

## Required work

1. Pin/document the actual protobuf compiler/toolchain version used.
2. Define one canonical command or script for validating/compiling all current `.proto` files.
3. Define deterministic C# generation output path expected by S1.
4. Define deterministic Java generation output path expected by S2.
5. Generate/compile enough output to prove current protocol sources are syntactically valid.
6. Add a deterministic/reproducibility check where practical.
7. Ensure failure returns non-zero.
8. Do not manually patch generated files.
9. Document how S1/S2 invoke or consume the generator.
10. Keep full Unity/server/CI runtime checks explicitly pending.

## Acceptance

- current protocol sources validate;
- C# generation command is real and documented;
- Java generation command is real and documented;
- repeated generation from identical protocol source is reproducible in content/layout or a documented build-generated equivalent;
- invalid proto fixture or equivalent compile-negative test fails in a controlled way;
- no protocol semantic change.

## Handoff must include

- pinned versions;
- exact canonical commands;
- generated paths;
- files added/changed/deleted;
- tests executed/results;
- explicit list of gates still unverified until S1/S2/S5-B.
