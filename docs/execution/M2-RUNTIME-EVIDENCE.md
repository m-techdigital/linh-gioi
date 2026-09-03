# M2 Runtime Evidence

M2 runtime evidence closes the gap between source-ready online session code and real client/server execution.

## Target final decision

`M2_ONLINE_SESSION_RUNTIME_CLOSED`

## Required inputs

- Source: `linh-gioi-m2-runtime-candidate-v0.6.2-full-source.zip`.
- Unity: `6000.3.2f1`.
- Java: `25`.
- Maven: `3.9.16`.
- Protobuf compiler/runtime: `3.13.0`.

## Required evidence

| Gate | Required evidence |
|---|---|
| Source validation | `./tools/validate_m2_source.sh` PASS. |
| Server build/test | Java 25 + Maven 3.9.16, nonzero tests, zero failures/skips. |
| Server runtime | Java realtime process binds and logs `realtime_started`. |
| Handshake | Real TCP `ClientHello -> ServerHello accepted`. |
| Session open | Server logs `realtime_session_opened`. |
| Movement | Client sends `MoveIntent(sequence=1, axis=(1,0), dt=0.1)`. |
| Snapshot | Client receives `PlayerTransformSnapshot(entityId=1001, acknowledgedSequence=1, x≈0.4)`. |
| Failure path | Invalid movement closes only that session; server accepts a later valid client. |
| Unity client | Unity-built Linux player runs `--lgo-m2-online-session-smoke`. |
| Hygiene | No generated output, Unity Library, Maven target, toolchain archive, or secrets in source delta. |

## Local Unity player command

The Unity player smoke must be invoked with:

```text
--lgo-m2-online-session-smoke --lgo-m2-host 127.0.0.1 --lgo-m2-port <port> --lgo-m2-result <result.json>
```

Expected JSON markers:

```text
status=PASS
handshakeAccepted=true
snapshot.entityId=1001
snapshot.acknowledgedSequence=1
snapshot.x≈0.4
exitCode=0
```

## Server smoke command

```bash
./server/scripts/online-session-smoke.py --host 127.0.0.1 --port <port>
```

Expected marker:

```text
M2_ONLINE_SESSION_SMOKE_PASS
```

## Non-claims

Do not claim any of the following from M2 evidence:

- durable account login;
- character persistence;
- Redis routing;
- multiple zones;
- MMO AOI scaling;
- live combat authority;
- production reconnect semantics.

These belong to later milestones.


Required summary marker: Unity-to-Java session smoke.


## v0.6.2 evidence guard

A complete local runtime candidate must emit `M2_LOCAL_RUNTIME_CANDIDATE_READY`. The runner must emit only `M2_LOCAL_RUNTIME_CANDIDATE_PARTIAL` when server smoke or Unity evidence is explicitly skipped. Runtime closure evidence should include the first snapshot, duplicate-sequence idempotence snapshot, and second movement snapshot.

## Preferred v0.6.2 local runner

Use one command from the repo root:

```bash
./tools/run_m2_local_runtime_once.sh
```

Expected marker:

```text
M2_LOCAL_RUNTIME_CANDIDATE_READY
```

The script writes `UPLOAD-THESE-FILES-M2-RUNTIME-CANDIDATE.txt` and avoids repeated manual upload cycles. The Linux player produced on macOS is not executed on macOS; upload it for Linux sandbox replay.
