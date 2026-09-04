# M6 Target Dummy Readability v0.31.0

Decision marker: M6_TARGET_DUMMY_READABILITY_RUNTIME_CLOSED_LOCAL_v0.31.0.

Scope:

- Adds a safe-yard target dummy readability marker as a visual landmark.
- Updates HUD landmark text so players can distinguish the dummy from the far-east shadow warning.
- Keeps the marker non-interactive and non-combat.

Non-goals:

- No combat system.
- No stats, HP, timing rules, hit checks, loot, inventory, or backend persistence.
- No protocol, GameData schema, ADR, or design-token changes.

Validation:

- `python3.12 tools/validate_m6_target_dummy_readability.py`
- `./tools/lgo_playable_closure_check.sh --source-only`
- `./tools/lgo_playable_closure_check.sh --package-ready`
- `./tools/lgo_playable_closure_check.sh --runtime`

Runtime result: PASS locally through inherited playable closure smokes.
