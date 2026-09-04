# LGO Combat Protocol + GameData Contract v0.40.0

Decision marker: M6_COMBAT_PROTOCOL_GAMEDATA_CONTRACT_ACCEPTED_v0.40.0.

## Protocol

`CombatIntent` is client input. It carries protocol version, sequence, intent id, actor entity id, target entity id, skill id, target position, client time, and a local_preview_only flag.

`CombatAccepted` means the authoritative server accepted an intent for processing after the server validates it, and may include cooldown/snapshot state.

`CombatRejected` means the authoritative server rejected an intent and includes ErrorInfo plus an optional CombatStateSnapshot.

`CombatResult` is the authoritative result event placeholder for later damage/effect application.

`CombatStateSnapshot` is the server-owned state snapshot used by rejected/accepted/result responses.

## GameData

Skill data now contains canonical nested rules:

- skill activation
- cooldown
- targeting rule
- effect rule placeholder
- telegraph/readability rule placeholder

These are schema-validated fields, not ad hoc client/server config.

## Separation

Local preview is never a server result. Vietnamese UI may show local prototype feedback, but server acceptance/rejection/result must use canonical protobuf messages.
