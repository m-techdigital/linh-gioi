# LGO Runtime UI One-Edge Layout Helper Audit v1.0

Status: `LGO_RUNTIME_UI_ONE_EDGE_LAYOUT_HELPER_READY`

## Scope

This pass reduces UI layout drift by moving repeated one-edge and margin values into reusable runtime helpers and layout profile properties.

## Changed Runtime Ownership

- `RuntimeUiSkin` now owns shared margin application helpers.
- `RuntimeUiLayoutProfile` now owns repeated login, lobby, world, dialogue, session menu, combat, and settings spacing values.
- `M4PlayableClientController` uses the same profile-owned values during first build and responsive refresh.

## Preserved Local Values

- Reset margins such as `marginTop = 0` or `marginRight = 0`.
- Hidden debug/developer-only label spacing.
- Component-local micro-spacing that does not repeat across responsive states.

## Validation

- `python3.12 tools/validate_lgo_runtime_ui_one_edge_layout_helper_audit.py`
- `./tools/lgo_playable_closure_check.sh --source-only`

## Follow-Up

Continue with `LGO-RUNTIME-UI-ONE-EDGE-LAYOUT-EVIDENCE-REFRESH-v1.0`.
