# M6 Server-Authoritative Combat Pilot Checklist v0.51.0

- [x] Existing v0.50 baseline used as authoritative source.
- [x] Protocol and GameData schemas left unchanged.
- [x] Server accepts valid Wind Slash intent.
- [x] Server rejects no target.
- [x] Server rejects invalid target.
- [x] Server rejects unknown skill.
- [x] Server rejects out of range using existing target position field.
- [x] Server rejects cooldown active without killing the session path.
- [x] Server emits accepted/result/snapshot evidence using existing protobuf messages.
- [x] Local Unity combat prototype remains separate and intact.
- [x] v0.51 source validator added.
- [x] v0.51 runtime smoke script added.

Decision: `M6_SERVER_AUTHORITATIVE_COMBAT_PILOT_CLOSED_LOCAL_v0.51.0` after required commands pass locally.
