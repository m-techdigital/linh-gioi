# M5 Session Menu Final Report v0.27.0

Decision: M5_SESSION_MENU_RUNTIME_CLOSED_LOCAL_v0.27.0.

Changed surfaces:

- `client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs`
- `tools/validate_m5_session_menu.py`
- `tools/lgo_playable_closure_check.sh`
- `docs/tasks/M5-SESSION-MENU-v0.27.0.md`

Frozen surfaces unchanged:

- `protocol/**`
- `gamedata/schemas/**`
- `docs/adr/**`
- `client/Unity/Assets/Game/UI/design-tokens.json`

Non-goals preserved: no gameplay expansion, combat, inventory, production auth, protocol changes, GameData schema changes, ADR changes, or design-token changes.

Validation PASS:

- `git --no-pager diff --check`
- `python3.12 -m py_compile tools/validate_m5_session_menu.py`
- `python3.12 tools/validate_m5_session_menu.py`
- `./tools/lgo_playable_closure_check.sh --source-only`
- `./tools/lgo_playable_closure_check.sh --package-ready`
- `./tools/lgo_playable_closure_check.sh --runtime`

Runtime markers observed:

- `M3B_UNITY_ACCOUNT_CHARACTER_RUNTIME_SMOKE_PASS`
- `M4_PLAYABLE_VERTICAL_SLICE_RUNTIME_SMOKE_PASS`
- `M4_VISUAL_PLACEHOLDER_FOUNDATION_SMOKE_PASS`
- `M5_FIRST_PLAYABLE_LOOP_RUNTIME_SMOKE_PASS`
- `M5_GUIDED_TRAINING_LOOP_RUNTIME_SMOKE_PASS`
- `M5_LIGHTWEIGHT_NPC_DIALOGUE_RUNTIME_SMOKE_PASS`
- `LGO_PLAYABLE_CLOSURE_RUNTIME_GATES_PASS`
