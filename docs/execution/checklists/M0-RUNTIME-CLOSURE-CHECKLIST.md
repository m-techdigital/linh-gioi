# M0 Runtime Closure Checklist

## Source and provenance

- [ ] Source ZIP SHA verified.
- [ ] Accepted delta SHA verified.
- [ ] Source extracted without parent wrapper.
- [ ] Required docs read.
- [ ] Frozen contracts identified before changes.

## Source validation

- [ ] `./tools/validate_m0_source.sh` PASS.
- [ ] Protocol descriptor compile PASS.
- [ ] Protocol C# generation PASS.
- [ ] Protocol Java generation PASS.
- [ ] Protocol tests executed >0 and PASS.
- [ ] GameData tests executed >0 and PASS.
- [ ] Negative/adversarial GameData/protocol tests PASS.
- [ ] Unity static source validation PASS.
- [ ] Server source validation PASS.

## Server runtime

- [ ] Java 25 runtime PASS.
- [ ] Javac 25 PASS.
- [ ] Maven 3.9.16 PASS.
- [ ] `./server/scripts/require-java-25.sh` PASS.
- [ ] `server/build.sh` PASS.
- [ ] `server/test.sh` PASS.
- [ ] Server test count >0.
- [ ] Spring Boot API process boots.
- [ ] `/health` returns valid JSON.
- [ ] Netty realtime process binds TCP.
- [ ] Real TCP `ClientHello -> ServerHello` PASS.
- [ ] Unsupported protocol version rejected.
- [ ] Malformed payload rejected.
- [ ] Valid client still accepted after bad clients.
- [ ] Graceful shutdown PASS.
- [ ] No orphan process.

## Unity runtime

- [ ] Unity 6000.3.2f1 Editor available.
- [ ] `UNITY_EDITOR -version` PASS.
- [ ] Unity project import PASS.
- [ ] Unity package restore PASS.
- [ ] C# compile PASS.
- [ ] asmdef compile PASS.
- [ ] generated C# protobuf consumed by Unity compiler.
- [ ] EditMode tests executed >0 and PASS.
- [ ] Bootstrap scene validation PASS.
- [ ] UI foundation validation PASS.
- [ ] Unity-to-Java handshake PASS or explicitly `DEFERRED_BY_TOOLING`.

## Handoff

- [ ] Frozen contract audit PASS.
- [ ] Package hygiene PASS.
- [ ] No generated/cache/toolchain/log/secret in source delta.
- [ ] Report created.
- [ ] Handoff created.
- [ ] Artifact SHA files created.
- [ ] Final decision is exactly one allowed token.

## Lightweight Unity player evidence alternative

Use only when Unity Editor cannot be mounted in sandbox under reasonable size limits.

- [ ] External Unity Editor 6000.3.2f1 version evidence collected.
- [ ] External Unity import/generator evidence collected.
- [ ] External EditMode results XML collected with test cases > 0.
- [ ] External Linux player smoke build evidence collected.
- [ ] Player/evidence SHA256 files verified in sandbox.
- [ ] Unity-built player runs in sandbox.
- [ ] Unity-built player handshakes with real Java realtime server in sandbox.
- [ ] Report explicitly states whether Editor evidence is external or in-sandbox.
