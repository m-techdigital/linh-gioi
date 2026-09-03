# M2 Runtime Closure Checklist

## Source

- [ ] Source SHA verified.
- [ ] Baseline is `M1_OFFLINE_COMBAT_RUNTIME_CLOSED` v0.5.3 or accepted successor.
- [ ] `./tools/validate_m2_source.sh` PASS.
- [ ] `protocol/**` unchanged.
- [ ] `gamedata/schemas/**` unchanged.
- [ ] ADR/design contracts unchanged.

## Java server

- [ ] Java 25 verified.
- [ ] Maven 3.9.16 verified.
- [ ] `./server/build.sh` PASS.
- [ ] `./server/test.sh` PASS with nonzero tests and zero failures/skips.
- [ ] Realtime server binds.
- [ ] Existing handshake smoke remains PASS.
- [ ] `server/scripts/online-session-smoke.py` PASS.
- [ ] Server smoke proves first move, duplicate idempotence, and second move snapshot.
- [ ] Server logs `realtime_session_opened`.
- [ ] Server logs `realtime_session_move_applied`.
- [ ] Reconnect/failure path PASS.
- [ ] Server survival after invalid movement PASS.
- [ ] reconnect/failure path proves server survival without process death.

## Unity client

- [ ] Unity `6000.3.2f1` verified.
- [ ] Unity import/compile PASS.
- [ ] Unity EditMode tests PASS with nonzero tests and zero failures/skips.
- [ ] M2 protocol serialization tests present.
- [ ] Unity client rejects non-finite and diagonal-over-speed movement before send.
- [ ] Unity Linux player build PASS.
- [ ] Unity-to-Java session smoke PASS.
- [ ] `--lgo-m2-online-session-smoke` result JSON status PASS.
- [ ] Snapshot acknowledges sequence 1.
- [ ] Snapshot entity id is 1001.
- [ ] Snapshot position is deterministic within tolerance.
- [ ] Duplicate sequence does not move the player again.
- [ ] Second movement snapshot acknowledges sequence 2.
- [ ] Local runner prints `M2_LOCAL_RUNTIME_CANDIDATE_READY` only after required evidence exists.

## Handoff

- [ ] Final decision is one of the approved exact M2 statuses.
- [ ] Evidence ZIP includes command logs, result JSON, server logs, Unity logs, and SHA files.
- [ ] No generated/cache/temp/build outputs in source delta.
- [ ] Next allowed step documented.

Target final decision when all required gates pass:

```text
M2_ONLINE_SESSION_RUNTIME_CLOSED
```
