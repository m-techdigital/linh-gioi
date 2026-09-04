# M6 Server Combat Contract Implementation Prompt v0.40.0

Use only after `M6_SERVER_COMBAT_CONTRACT_SPEC_ACCEPTED_v0.39.0` is accepted.

Goal: propose protocol and GameData contract changes for future server-authoritative combat.

Hard rules:

- Do not implement server combat until protocol and GameData changes are reviewed.
- Do not bypass Protobuf/GameData.
- Do not create duplicate DTO/config formats.
- Keep Unity prediction separate from server authority.
- No production balancing, PvP, DB persistence, economy, inventory, loot, auth, guild, chat, market, party, live ops, or production admin.

Required contract areas:

- `CombatIntent`
- `CombatAccepted`
- `CombatRejected`
- `CombatResult`
- `CombatStateSnapshot`
- skill activation
- cooldown
- targeting rule
- effect rule
- telegraph rule

Required gates:

- protocol validator;
- GameData schema validator;
- code governance validator;
- no implementation claim until runtime smoke exists.
