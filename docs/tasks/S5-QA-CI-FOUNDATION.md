# LG-M0-S5 — QA / Tooling / CI Foundation

## Goal
Make M0 reproducible so PASS evidence is not dependent on one sandbox machine.

## Allowed paths
- `tools/**`
- `tests/**`
- CI configuration at repo root once created
- build/codegen helper configuration agreed with S1/S2

## Forbidden paths
- changing protocol semantics;
- changing GameData schemas;
- gameplay feature implementation.

## Required work
1. Pin/document protobuf compiler/tooling version and create reproducible C# + Java codegen commands/build integration.
2. Add contract lint/compile check for `.proto`.
3. Add GameData validation command entrypoint by integrating S4's validator once available.
4. Add server test/build lane.
5. Add Unity batch-mode compile/test lane when Unity license/environment supports it; otherwise make the limitation explicit and provide local canonical command, never mark unexecuted tests PASS.
6. Add a smoke/integration handshake test that can start or target realtime server and verify `ClientHello -> ServerHello`.
7. Ensure scripts fail on real errors; no `|| true` masking.

## Acceptance
- one documented root validation command or small set of canonical commands;
- failures propagate non-zero exit;
- protocol generation is deterministic/reproducible;
- positive and negative contract tests exist;
- CI artifacts/logs are sufficient to diagnose failure.
