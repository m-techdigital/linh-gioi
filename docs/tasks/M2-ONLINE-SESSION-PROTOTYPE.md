# M2 — Online Session Prototype

## Status

`M2_RUNTIME_CANDIDATE_HARDENED_READY_FOR_LOCAL_EVIDENCE`

## Entry requirement

- `M1_OFFLINE_COMBAT_RUNTIME_CLOSED` must be accepted.
- `M0_RUNTIME_CLOSED` remains the foundation baseline.
- Existing Java realtime handshake must remain compatible.

## Goal

Create the first online session loop without opening persistence or MMO-scale systems:

```text
Unity Client
  ClientHello
      ↓
Java Realtime Gateway
  ServerHello accepted
      ↓
Unity MoveIntent
      ↓
Java OnlineSession authoritative update
      ↓
Unity PlayerTransformSnapshot
```

## Allowed scope

- A single-session Java realtime state object.
- Post-handshake Netty handler for movement intent processing.
- Sequence acknowledgement and idempotent duplicate/late sequence handling.
- Failure path for malformed/invalid movement that closes only the offending connection.
- Unity client method for sending one movement intent and parsing one snapshot.
- Runtime smoke/evidence scripts.

## Forbidden scope

- No account/auth persistence.
- No character database.
- No Redis routing.
- No zone sharding/AOI.
- No economy, guild, marketplace, PvP ranking, or broad content expansion.
- No protocol/schema mutation without S0 contract-change request.


## v0.6.2 hardening scope

M2 remains the current milestone. v0.6.2 may harden the online-session candidate by improving movement validation parity, proving duplicate sequence idempotence in Unity smoke output, and preventing false READY markers in local runtime evidence scripts. It must not add M3 persistence or mutate protocol/schema contracts.

## Acceptance criteria

Source acceptance:

- `./tools/validate_m2_source.sh` PASS.
- M0 source validation still PASS.
- M1 offline combat static validation still PASS.
- M2 static validation PASS.
- Frozen contracts unchanged.

Runtime acceptance:

- Java 25/Maven server build/test PASS.
- Live Java Netty M2 online session smoke PASS.
- Unity EditMode includes M2 protocol/session serialization tests.
- Unity-built Linux player runs `--lgo-m2-online-session-smoke` against real Java Netty and returns JSON `status=PASS`.
- Reconnect/failure path evidence proves server survives invalid movement and accepts a later valid client.

## Non-goals

M2 does not produce a real MMO gameplay zone. It proves the smallest reliable client/server session lifecycle that later milestones can expand.
