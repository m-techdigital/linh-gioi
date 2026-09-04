# M6 Contract Review Final Report v0.33.0

Final decision: `M6_MINIMAL_LOCAL_COMBAT_ALLOWED_WITHOUT_CONTRACT_CHANGE_v0.33.0`

Minimal local combat is allowed only as a Unity client-local target dummy prototype. The current contracts are not sufficient for production or server-authoritative combat, and no protocol or GameData schema changes are made in this stage.

Stage 2 may proceed with Vietnamese UI feedback, explicit local/non-authoritative labels, no loot/reward, no XP/level, no inventory mutation, no economy, no PvP, and no server combat authority claim.

Frozen surfaces unchanged:

- `protocol/**`
- `gamedata/schemas/**`
- `docs/adr/**`
- `client/Unity/Assets/Game/UI/design-tokens.json`
