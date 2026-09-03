# M4 Playable Slice Stabilization v0.13.0

Status: `M4_PLAYABLE_SLICE_STABILIZATION_SOURCE_READY`

This task stabilizes the current M4 playable slice for repeatable closure. It preserves the v0.12.0 playable UI/art quality source and adds closure automation, source-state validation, and macOS protobuf tooling for inherited source gates.

Changed:

- Added `tools/lgo_m4_closure_check.sh` with source-only, runtime, and package-ready modes.
- Added `tools/validate_m4_stabilization.py`.
- Added checksum-pinned macOS `protoc` `3.13.0` under `tools/protobuf/darwin-arm64/`.
- Updated current-state docs and version markers to v0.13.0.
- Added machine-readable placeholder asset coverage in `m4-visual-manifest.json`.

Did not change:

- no gameplay expansion
- no UI redesign beyond the accepted M4-2 source
- no art redesign beyond the accepted M4-3 source
- no protocol changes
- no GameData schema changes
- no ADR changes
- no design token changes
- no production authentication or database persistence changes

Validation:

```bash
git --no-pager diff --check
python3.12 -m py_compile tools/validate_project_state.py tools/validate_m4_playable_source.py tools/validate_m4_visual_foundation.py tools/validate_m4_2_playable_ui.py tools/validate_m4_stabilization.py tools/m4_playable_vertical_slice_runtime.py tools/m4_visual_foundation_runtime.py
python3.12 tools/validate_project_state.py
python3.12 tools/validate_m4_playable_source.py
python3.12 tools/validate_m4_visual_foundation.py
python3.12 tools/validate_m4_2_playable_ui.py
python3.12 tools/validate_m4_stabilization.py
./tools/validate_m4_source.sh
./tools/lgo_m4_closure_check.sh --source-only
./tools/lgo_m4_closure_check.sh --package-ready
```

Runtime closure command:

```bash
./tools/lgo_m4_closure_check.sh --runtime
```

Runtime closure requires Unity `6000.3.2f1`, Java `25`, Maven `3.9.16`, a current macOS Unity player build, and observed M3-B/M4 smoke markers on the current source.
