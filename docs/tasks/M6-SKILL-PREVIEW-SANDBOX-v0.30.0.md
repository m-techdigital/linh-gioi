# M6 Skill Preview Sandbox v0.30.0

Decision marker: M6_SKILL_PREVIEW_SANDBOX_RUNTIME_CLOSED_LOCAL_v0.30.0.

Scope:

- Adds a preview-only skill rehearsal strip to the playable HUD.
- Reuses existing local VFX placeholder states for Wind Slash, Shadow Bind, and Spirit Guard readability.
- Keeps gameplay behavior, account flow, protocol, GameData schemas, ADRs, and UI design tokens unchanged.

Non-goals:

- No combat system.
- No timing rules.
- No stats, loot, inventory, target resolution, or backend persistence.

Validation:

- `python3.12 tools/validate_m6_skill_preview_sandbox.py`
- `./tools/lgo_playable_closure_check.sh --source-only`
- `./tools/lgo_playable_closure_check.sh --package-ready`
- `./tools/lgo_playable_closure_check.sh --runtime`

Runtime result: PASS locally through inherited playable closure smokes.
