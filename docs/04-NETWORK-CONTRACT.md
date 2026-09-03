# 04 — Network Contract v1

## Versioning

All handshakes carry:
- `protocol_version`;
- `client_version`;
- `gamedata_version`.

The server may reject incompatible versions with a machine-readable reason.

## Intent model

Client sends intent such as:
- move input;
- attack request;
- skill request;
- interact request;
- chat request;
- party/friend request.

Client never declares authoritative combat/economy results.

## Sequence model

Realtime input messages that can arrive rapidly carry monotonically increasing client sequence values per active session.

## Time model

Server time is authoritative. Client presentation may estimate server time after synchronization.

## Compatibility rules

- Never reuse a released protobuf field number.
- Removed field numbers and names are `reserved`.
- Additive optional fields are preferred.
- Breaking semantics require protocol version change and migration plan.

## Initial v1 message families

- common / envelope;
- handshake;
- movement;
- combat;
- social;
- world event.

Durable business APIs may use HTTPS/JSON initially where appropriate, but shared canonical domain IDs must still match documented naming rules.
