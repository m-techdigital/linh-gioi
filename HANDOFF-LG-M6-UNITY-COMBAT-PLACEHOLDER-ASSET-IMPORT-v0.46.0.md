# Handoff: LG M6 Unity Combat Placeholder Asset Import v0.46.0

Decision: `M6_UNITY_COMBAT_PLACEHOLDER_ASSET_IMPORT_SOURCE_CLOSED_v0.46.0`

Unity import path:
`client/Unity/Assets/Game/Art/Combat/Placeholders/Resources/CombatPlaceholders/`

Main runtime files:
- `client/Unity/Assets/Game/Art/Runtime/CombatPlaceholderAssets.cs`
- `client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs`
- `client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs`

Validation gates:
- `python3.12 -m py_compile tools/validate_m6_unity_combat_placeholder_asset_import.py`
- `git --no-pager diff --check`
- `python3.12 tools/validate_m6_unity_combat_placeholder_asset_import.py`
- `./tools/lgo_playable_closure_check.sh --source-only`
- Unity player build and runtime smokes when local Unity environment is available.
- `./tools/lgo_playable_closure_check.sh --runtime`
- `./tools/lgo_playable_closure_check.sh --visual-evidence`

Notes:
- Player-facing combat labels remain Vietnamese in runtime code.
- v0.45 PNGs are used as placeholder sprites/textures only.
- Runtime visual evidence may still require macOS screen recording permission for screenshot capture.
- v0.46 local run captured visual evidence screenshots successfully; human visual acceptance remains a manual review gate.
