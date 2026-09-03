# M4 Visible UI Usability Final Report v0.14.0

Final decision: `M4_VISIBLE_UI_USABILITY_REVIEW_READY_RUNTIME_UNVERIFIED_v0.14.0`.

Baseline tag: `lgo-m4-playable-slice-stabilization-source-closed-v0.13.0`.

Root cause of visible UI issue:

- Runtime smokes proved API/account/character/world behavior but did not inspect visible UI usability.
- The previous UI used large simultaneous panels and heavy dark/cyan surfaces, which could dominate a 1280x720 player window.
- Navigation/exit affordance was not obvious in manual visible review.

Exact UI fixes:

- Constrained the UI root/header/main shell to a readable 960 px max width.
- Reduced panel visual weight and kept panels state-focused.
- Hid the full auth panel after login so Character Hall becomes the main visible surface.
- Added explicit `API status` text in Gate Entry.
- Added visible Quit buttons and Escape key exit through `Application.Quit()`.
- Kept Save Position, Back to Lobby, movement hint, account/character flow, and smoke behavior intact.

Manual review harness summary:

- Added `tools/run_m4_visible_ui_review.sh` with `--rebuild`, `--open-existing`, and `--stop`.
- The harness detects Unity `6000.3.2f1` in common macOS paths, exports project-local pinned protobuf tooling, starts API on `127.0.0.1:18083`, builds the macOS player when requested, opens the player at 1280x720 windowed mode, and prints the manual checklist.

Validators added/updated:

- Added `tools/validate_m4_visible_ui.py`.
- Updated `tools/lgo_m4_closure_check.sh` to run the visible UI validator in source-only/package-ready flows.
- Updated `tools/validate_m4_stabilization.py` and `tools/validate_project_state.py` for v0.14.0 current truth.

Validation evidence:

```text
git --no-pager diff --check: PASS
python3.12 -m py_compile ...: PASS
python3.12 tools/validate_project_state.py: PASS
python3.12 tools/validate_m4_playable_source.py: PASS
python3.12 tools/validate_m4_visual_foundation.py: PASS
python3.12 tools/validate_m4_2_playable_ui.py: PASS
python3.12 tools/validate_m4_stabilization.py: PASS
python3.12 tools/validate_m4_visible_ui.py: PASS
./tools/validate_m4_source.sh: PASS
./tools/lgo_m4_closure_check.sh --source-only: PASS
./tools/lgo_m4_closure_check.sh --package-ready: PASS
./tools/lgo_m4_closure_check.sh --runtime: PASS
./tools/run_m4_visible_ui_review.sh --rebuild: PASS, opened review player and printed checklist
./tools/run_m4_visible_ui_review.sh --stop: PASS
```

Runtime markers observed:

```text
M3B_UNITY_ACCOUNT_CHARACTER_RUNTIME_SMOKE_PASS
M4_PLAYABLE_VERTICAL_SLICE_RUNTIME_SMOKE_PASS
M4_VISUAL_PLACEHOLDER_FOUNDATION_SMOKE_PASS
LGO_M4_CLOSURE_RUNTIME_GATES_PASS
```

Visible UI review status:

The visible review harness opened the macOS player in 1280x720 windowed mode. Codex could not capture or inspect screenshots because `screencapture` failed with `could not create image from display`, so final human visual acceptance remains pending.

Frozen surfaces confirmation:

No changes were made under `protocol/**`, `gamedata/schemas/**`, `docs/adr/**`, or `client/Unity/Assets/Game/UI/design-tokens.json`.

Known limitations and non-claims:

- not final production UI
- not final production art
- no production auth
- no DB persistence
- no full MMO gameplay
- no combat system
- full M0 runtime not newly claimed
