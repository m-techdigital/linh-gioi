# M6 Combat Readiness Spec Final Report v0.32.0

Decision: M6_COMBAT_READINESS_SPEC_CLOSED_v0.32.0.

Changed surfaces:

- `docs/tasks/M6-COMBAT-READINESS-SPEC-v0.32.0.md`
- `docs/execution/prompts/M6-COMBAT-FOUNDATION-LONG-TASK.md`
- `tools/validate_m6_combat_readiness_spec.py`
- `tools/lgo_playable_closure_check.sh`
- `HANDOFF-LG-M6-COMBAT-READINESS-SPEC-v0.32.0.md`
- `M6-COMBAT-READINESS-SPEC-FINAL-REPORT-v0.32.0.md`

Frozen surfaces unchanged:

- `protocol/**`
- `gamedata/schemas/**`
- `docs/adr/**`
- `client/Unity/Assets/Game/UI/design-tokens.json`

Validation:

- `git --no-pager diff --check`: PASS
- `python3.12 -m py_compile tools/validate_m6_combat_readiness_spec.py`: PASS
- `python3.12 tools/validate_m6_combat_readiness_spec.py`: PASS
- `./tools/lgo_playable_closure_check.sh --source-only`: PASS
- `./tools/lgo_playable_closure_check.sh --package-ready`: PASS

Runtime note: v0.32.0 is docs-only. No new combat runtime PASS is claimed.

Non-goals preserved: no combat implementation, HP, damage, cooldowns, projectiles, loot, inventory, production auth, protocol changes, GameData schema changes, ADR changes, or design-token changes.
