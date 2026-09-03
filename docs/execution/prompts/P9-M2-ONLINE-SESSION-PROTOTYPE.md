# P9 — M2 Online Session Prototype

You are implementing M2 for Linh Giới Online.

## Required lifecycle

TASK -> VERIFY -> HANDOFF -> DONE

Every response must end with:

```text
LGO_STATUS::1A062B88F82-C2C9FFBC::<STATUS>
```

## Entry condition

M2 may start only from a source where:

```text
M1_OFFLINE_COMBAT_RUNTIME_CLOSED
```

is accepted.

## Scope

Implement the smallest single-session online loop:

```text
ClientHello -> ServerHello accepted -> MoveIntent -> PlayerTransformSnapshot
```

The Java realtime server is authoritative for the snapshot. Unity may send input only; it must not authoritatively decide position.

## Allowed

- Java online session state object.
- Java Netty post-handshake session handler.
- Unity realtime client method to send movement and parse snapshot.
- One command-line Unity smoke: `--lgo-m2-online-session-smoke`.
- Runtime smoke scripts and evidence verifiers.
- Tests for sequence ack, invalid movement, reconnect/failure path, and server survival.

## Forbidden

- No protocol/schema changes.
- No account persistence.
- No character database.
- No Redis routing.
- No economy/guild/marketplace/PvP ranking.
- No broad content expansion.
- No source-inspection-as-runtime-PASS.
- No `|| true`, masked failures, skip-as-PASS, or zero-test PASS.

## Required verification

Source-ready can be claimed only after:

```bash
./tools/validate_m2_source.sh
```

passes.

Runtime closed can be claimed only after Java 25/Maven server tests and a Unity-built Linux player run against the real Java realtime server with:

```text
--lgo-m2-online-session-smoke
```

and the result proves handshake accepted plus snapshot ack/position.

## Final handoff requirements

Provide:

- report;
- handoff;
- changed files;
- evidence archive;
- source delta ZIP without parent wrapper;
- full source successor only if accepted;
- SHA256 for all artifacts;
- exact final decision.
