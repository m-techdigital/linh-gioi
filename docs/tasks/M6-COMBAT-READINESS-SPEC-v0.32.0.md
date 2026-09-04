# M6 Combat Readiness Spec v0.32.0

Decision marker: M6_COMBAT_READINESS_SPEC_CLOSED_v0.32.0.

Purpose:

- Define the review boundary before any real combat foundation begins.
- Preserve the current playable loop as a non-combat safe-yard experience.
- Make the next implementation prompt explicit about ownership, validation, and frozen contracts.

Current runtime baseline:

- Account, character, enter-world, save-position, guided training, dialogue, local settings, API error handling, skill preview sandbox, and target dummy readability are present.
- Skill preview is visual rehearsal only.
- Target dummy readability is a landmark only.
- Real combat is not implemented.

Required contract review before real combat:

- Protocol ownership must define any client/server combat messages before implementation.
- GameData ownership must define combat-relevant data shape before implementation.
- Runtime ownership must define smoke evidence for player intent, server acceptance, rejection, and local presentation.
- UX ownership must define how combat readiness appears without hiding account, save-position, and guided training regressions.

Frozen surfaces for this spec:

- `protocol/**`
- `gamedata/schemas/**`
- `docs/adr/**`
- `client/Unity/Assets/Game/UI/design-tokens.json`

Explicit non-implementation:

- No combat code is added in v0.32.0.
- No protocol or GameData schema contract is changed in v0.32.0.
- No HP, damage, cooldown, projectile, loot, inventory, enemy AI, balancing, or production backend persistence is added in v0.32.0.

Recommended next step:

- Run a contract review and then execute `docs/execution/prompts/M6-COMBAT-FOUNDATION-LONG-TASK.md` only after the project owner accepts the protocol and GameData ownership plan.

Validation:

- `python3.12 tools/validate_m6_combat_readiness_spec.py`
- `./tools/lgo_playable_closure_check.sh --source-only`
- `./tools/lgo_playable_closure_check.sh --package-ready`

Runtime note: v0.32.0 is docs-only. It does not claim a new combat runtime PASS.
