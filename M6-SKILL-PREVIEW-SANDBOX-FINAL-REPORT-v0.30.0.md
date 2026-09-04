# M6 Skill Preview Sandbox Final Report v0.30.0

Decision: M6_SKILL_PREVIEW_SANDBOX_RUNTIME_CLOSED_LOCAL_v0.30.0.

Changed surfaces:

- `client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs`
- `client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs`
- `tools/validate_m6_skill_preview_sandbox.py`
- `tools/lgo_playable_closure_check.sh`
- `docs/tasks/M6-SKILL-PREVIEW-SANDBOX-v0.30.0.md`
- `HANDOFF-LG-M6-SKILL-PREVIEW-SANDBOX-v0.30.0.md`
- `M6-SKILL-PREVIEW-SANDBOX-FINAL-REPORT-v0.30.0.md`

Frozen surfaces unchanged:

- `protocol/**`
- `gamedata/schemas/**`
- `docs/adr/**`
- `client/Unity/Assets/Game/UI/design-tokens.json`

Validation:

- `git --no-pager diff --check`: PASS
- `python3.12 -m py_compile tools/validate_m6_skill_preview_sandbox.py`: PASS
- `python3.12 tools/validate_m6_skill_preview_sandbox.py`: PASS
- `./tools/lgo_playable_closure_check.sh --source-only`: PASS
- `./tools/lgo_playable_closure_check.sh --package-ready`: PASS
- `./tools/lgo_playable_closure_check.sh --runtime`: PASS locally.

Runtime evidence observed locally:

- `M3B_UNITY_ACCOUNT_CHARACTER_RUNTIME_SMOKE_PASS`
- `M4_PLAYABLE_VERTICAL_SLICE_RUNTIME_SMOKE_PASS`
- `M4_VISUAL_PLACEHOLDER_FOUNDATION_SMOKE_PASS`
- `M5_FIRST_PLAYABLE_LOOP_RUNTIME_SMOKE_PASS`
- `M5_GUIDED_TRAINING_LOOP_RUNTIME_SMOKE_PASS`
- `M5_LIGHTWEIGHT_NPC_DIALOGUE_RUNTIME_SMOKE_PASS`
- `LGO_PLAYABLE_CLOSURE_RUNTIME_GATES_PASS`

Non-goals preserved: no combat system, stats, loot, inventory, target resolution, production auth, protocol changes, GameData schema changes, ADR changes, or design-token changes.
