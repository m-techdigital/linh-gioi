# LINH GIỚI ONLINE — M6 COMBAT FOUNDATION LONG TASK

Do not start until `M6_COMBAT_READINESS_SPEC_CLOSED_v0.32.0` is accepted by the project owner.

Goal:

- Implement the first real combat foundation only after contract ownership is approved.
- Keep the existing account, character, enter-world, save-position, guided training, dialogue, local settings, API error handling, skill preview sandbox, and target dummy readability behavior intact.

Before coding:

- Confirm whether protocol changes are approved and list exact `.proto` ownership.
- Confirm whether GameData schema changes are approved and list exact schema ownership.
- Define runtime smoke markers for client intent, server result, rejection handling, and no-regression playable flow.
- Define a rollback plan if Unity build or smokes fail.

Frozen unless explicitly approved in the accepted contract review:

- `protocol/**`
- `gamedata/schemas/**`
- `docs/adr/**`
- `client/Unity/Assets/Game/UI/design-tokens.json`

Minimum implementation boundary after approval:

- One local player combat intent path.
- One server-validated result path.
- One rejection path.
- One visual feedback path that remains readable in the safe yard.
- No inventory, loot, economy, guild, chat, market, party, live ops, production auth, or database persistence.

Required validation:

- Source validators for all existing M4/M5/M6 gates.
- Python compile for changed tools.
- Unity build.
- Runtime smoke with explicit combat foundation markers and inherited playable markers.

Final report must state:

- Exact contracts changed.
- Exact files changed.
- Runtime markers observed.
- Frozen surfaces unchanged or explicitly approved.
- Whether real combat foundation is runtime closed.
