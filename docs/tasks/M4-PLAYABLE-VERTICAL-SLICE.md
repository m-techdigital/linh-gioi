# M4-0 Playable Client Vertical Slice

Status: `M4_PLAYABLE_VERTICAL_SLICE_FOUNDATION_SOURCE_READY`

This slice connects the existing M3-B Unity account/character API client to a playable local shell:

- dev login against the existing API
- account/profile display
- character lobby with list, create, select, and enter world
- placeholder player marker spawned from persisted position
- WASD/arrow movement, Q/E rotation
- save position through the existing character position endpoint
- restart-aware player smoke marker: `M4_PLAYABLE_VERTICAL_SLICE_RUNTIME_SMOKE_PASS`

Runtime closure still requires a current macOS Unity player built from this source and run with:

```bash
./tools/run_m4_playable_vertical_slice_once.sh --unity-player "$PWD/build/unity-player-macos/LinhGioiOnline.app/Contents/MacOS/Unity"
```
