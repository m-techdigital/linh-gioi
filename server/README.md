# Linh Giới Java Server — M0 Unified Foundation

Target: **Java 25**.

Modules:

- `shared` — dependency-light runtime/config primitives.
- `api` — Spring Boot API foundation and `GET /health`.
- `realtime` — Netty TCP realtime foundation and M0 protobuf handshake.

## Canonical protocol integration

`protocol/*.proto` remains the only wire-contract source. Before Maven compilation, the canonical S5 tooling generates Java source into:

`build/generated/protocol/java`

`server/realtime/pom.xml` consumes that directory through build-helper. Generated Java is disposable and is never hand-edited.

Canonical commands:

```bash
./server/build.sh
./server/test.sh
./server/scripts/runtime-smoke.sh
```

`build.sh` and `test.sh` both invoke `server/scripts/prepare-protocol.sh` first.

## M0 realtime framing

M0 uses raw TCP with a bounded 4-byte unsigned big-endian length prefix followed by exactly one protobuf payload per frame. Maximum frame size is 64 KiB.

Handshake state:

1. client sends framed `ClientHello`;
2. server validates protocol/GameData versions and required semantic fields;
3. server sends framed `ServerHello`;
4. rejected or malformed handshakes are closed without killing the realtime process;
5. gameplay message routing is deferred until later milestones.

The 4-byte framing is a transport implementation detail; `protocol/*.proto` remains the message contract.

## Runtime smoke

The runtime smoke proves, when Java 25 + Maven are genuinely available:

- API process boots;
- real `GET /health` succeeds;
- realtime process binds;
- valid `ClientHello -> ServerHello` succeeds over a real TCP socket;
- unsupported version is rejected;
- malformed ClientHello is closed;
- a subsequent valid client still succeeds;
- both Java processes shut down without orphaned tracked children.

The smoke script fails non-zero when the required runtime environment is unavailable. Source inspection is never reported as runtime PASS.
