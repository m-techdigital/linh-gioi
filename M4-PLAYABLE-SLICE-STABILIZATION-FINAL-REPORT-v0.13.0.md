# M4 Playable Slice Stabilization Final Report v0.13.0

Final decision: `M4_PLAYABLE_SLICE_STABILIZATION_SOURCE_CLOSED_RUNTIME_UNVERIFIED_ENVIRONMENT_v0.13.0`.

Baseline used: `lgo-m4-playable-ui-art-quality-closed-local-v0.12.0`.

What changed:

- Added `tools/lgo_m4_closure_check.sh` one-command closure automation.
- Added `tools/validate_m4_stabilization.py`.
- Added checksum-pinned macOS `protoc` `3.13.0` at `tools/protobuf/darwin-arm64/protoc`.
- Updated protocol tooling resolution while preserving checksum/version verification.
- Updated M0 source validation cleanup so disposable Unity generated outputs do not poison package hygiene.
- Updated stale protocol tooling regression test for macOS support.
- Updated current-state docs and version identity to v0.13.0.
- Added machine-readable placeholder asset coverage to `m4-visual-manifest.json`.

What did not change:

- no gameplay expansion
- no UI redesign beyond accepted M4-2 source
- no art redesign beyond accepted M4-3 source
- no protocol changes
- no GameData schema changes
- no ADR changes
- no design token changes
- no production auth changes
- no database persistence changes

Validation evidence:

```text
git --no-pager diff --check: PASS
python3.12 -m py_compile ...: PASS
python3.12 tools/validate_project_state.py: PASS
python3.12 tools/validate_m4_playable_source.py: PASS
python3.12 tools/validate_m4_visual_foundation.py: PASS
python3.12 tools/validate_m4_2_playable_ui.py: PASS
python3.12 tools/validate_m4_stabilization.py: PASS
./tools/validate_m4_source.sh: PASS
./tools/lgo_m4_closure_check.sh --source-only: PASS
./tools/lgo_m4_closure_check.sh --package-ready: PASS
./tools/lgo_m4_closure_check.sh --runtime: UNVERIFIED_ENVIRONMENT
```

Runtime status:

`UNVERIFIED_ENVIRONMENT`: `.lgo-local-env` was not present and `UNITY_EDITOR` was not set in this sandbox. No Unity player build, M3-B runtime smoke, M4 playable runtime smoke, or M4 visual runtime smoke was claimed from this run.

Runtime markers observed in this run: none.

Required runtime markers for future closure:

```text
M3B_UNITY_ACCOUNT_CHARACTER_RUNTIME_SMOKE_PASS
M4_PLAYABLE_VERTICAL_SLICE_RUNTIME_SMOKE_PASS
M4_VISUAL_PLACEHOLDER_FOUNDATION_SMOKE_PASS
```

Restore/apply/verify commands:

```bash
git checkout main
git pull
./tools/lgo_m4_closure_check.sh --source-only
./tools/lgo_m4_closure_check.sh --package-ready
./tools/lgo_m4_closure_check.sh --runtime
```

Non-claims:

- full M0 runtime not claimed
- production auth not claimed
- DB persistence not claimed
- full MMO gameplay not claimed
- combat not claimed
- final production art not claimed
