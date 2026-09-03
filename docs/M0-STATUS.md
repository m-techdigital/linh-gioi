# M0 Status / Exit Checklist

Current state: `M0_RUNTIME_CLOSED`

M0 is closed on `2026-09-03` after final server + Unity player runtime verification.

## Closed foundation/source gates

- [x] Product Vision / Constitution / GDD / TDD foundation.
- [x] Network Contract v1 and six frozen `.proto` sources.
- [x] Canonical pinned Protobuf tooling.
- [x] Real descriptor compile and C#/Java generation.
- [x] Deterministic protocol generation and negative compiler coverage.
- [x] Java 25 multi-module server source foundation.
- [x] Spring Boot API source + `/health` contract.
- [x] Netty realtime source foundation.
- [x] Canonical Java generated-Protobuf build wiring.
- [x] Server-side `ClientHello -> ServerHello` handshake source.
- [x] Version/malformed/bad-client-survival regression source.
- [x] GameData schema + deterministic compiler/validator.
- [x] GameData negative tests: duplicate ID, bounds, class/map references.
- [x] Unity `6000.3.2f1` / URP `17.3.0` source foundation.
- [x] Unity asmdef/module boundaries.
- [x] Canonical C# Protobuf preparation path.
- [x] Unity networking/config/bootstrap source.
- [x] UI foundation primitives and design-token consumption.
- [x] Reproducible generated Unity foundation assets strategy.
- [x] CI definitions for server/source and self-hosted Unity.
- [x] Runtime bootstrap/probe tooling.

## Closed runtime gates

- [x] Java 25 and Javac 25: PASS.
- [x] Maven `3.9.16`: PASS.
- [x] Server build: PASS.
- [x] Server tests: PASS, `25 executed / 0 skipped`.
- [x] Spring Boot `/health`: PASS.
- [x] Netty realtime bind: PASS.
- [x] Real Java TCP `ClientHello -> ServerHello`: PASS.
- [x] Unsupported protocol / malformed payload survival: PASS.
- [x] Graceful server shutdown / no orphan Java process: PASS.
- [x] Unity Editor evidence: PASS, `6000.3.2f1`.
- [x] Unity project import/generate: PASS.
- [x] Unity EditMode tests: PASS, `5 total / 5 passed / 0 failed / 0 skipped`.
- [x] Unity Linux player build: PASS.
- [x] Unity-built Linux player runs in sandbox: PASS.
- [x] Unity Player -> Java Netty handshake: PASS.

## Final evidence

See:

- `M0-RUNTIME-CLOSURE-FINAL-REPORT-v0.4.1.md`
- `HANDOFF-LG-M0-RUNTIME-CLOSED-v0.4.1.md`
- `linh-gioi-m0-runtime-closed-evidence-v0.4.1.zip`

## Next allowed milestone

`M1 Offline Combat Prototype`

M1 must preserve frozen contracts unless a formal contract change request is accepted.
