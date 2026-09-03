# P6 PROMPT — S5-B QA / CI / M0 RUNTIME CLOSURE

You are S5-B. This is the closure half of S5 after S2/S1/S4/S3 have been accepted into the current integration baseline.

## Preconditions

Accepted artifacts present from:

- S5-A protocol/codegen tooling;
- S2 Java foundation;
- S1 Unity foundation;
- S4 GameData foundation;
- S3 UI foundation.

If any required artifact is absent or provenance is unknown, report BLOCKED.

## Required reading

- all M0 contract docs;
- `docs/M0-STATUS.md`;
- `docs/tasks/S5-QA-CI-FOUNDATION.md`;
- all accepted lane handoffs;
- `docs/execution/04-INTEGRATION-CHECKLIST.md`.

## Goal

Make M0 reproducible and produce final evidence for the M0 runtime exit gate without inventing PASS for unexecuted checks.

## Allowed paths

- `tools/**`
- `tests/**`
- CI configuration
- minimal build/codegen helper wiring needed to integrate already accepted lane implementations

Do not redesign feature code or contracts to make tests easier.

## Required work

1. Consolidate canonical root validation/build/test commands.
2. Run/validate protocol lint + C#/Java generation.
3. Run GameData positive and all required negative tests.
4. Run Java clean build/tests.
5. Start/target realtime server and run handshake smoke.
6. Run Unity batch-mode compile/edit/play tests where the environment truly supports the required Unity version/license/runtime.
7. If Unity cannot execute in this environment, preserve the exact local canonical command and mark runtime evidence UNVERIFIED, not PASS.
8. Create CI pipeline(s) that reproduce supported validations with non-zero failure propagation.
9. Capture logs/artifacts sufficient to diagnose failure.
10. Update M0 status only for gates with fresh valid evidence on this exact integrated source.

## Final M0 evidence table

Report each separately:

- Unity project open/bootstrap run: PASS / FAIL / UNVERIFIED
- Java API boot: PASS / FAIL
- Java realtime boot: PASS / FAIL
- C# protobuf generate+compile: PASS / FAIL
- Java protobuf generate+compile: PASS / FAIL
- Unity -> realtime accepted handshake: PASS / FAIL / UNVERIFIED
- valid GameData: PASS / FAIL
- invalid GameData rejection: PASS / FAIL
- CI/tool automation execution: PASS / FAIL / PARTIAL with exact unsupported slice

Do not declare `lg-m00-foundation` closed unless all mandatory runtime gates are actually satisfied.
