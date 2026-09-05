# LGO Runtime UI Skin Usage Guide Pass v1.0

Status: `LGO_RUNTIME_UI_SKIN_USAGE_GUIDE_READY`

## Scope

This pass documents shared `RuntimeUiSkin` ownership boundaries after the login, Character Hall, and World HUD adoption work.

## Added

- `docs/design/RUNTIME-UI-SKIN-USAGE-GUIDE-v1.0.md`
- `tools/validate_lgo_runtime_ui_skin_usage_guide.py`

## Runtime Impact

- No gameplay change.
- No new runtime image payload.
- No layout rewrite.
- No `VISUAL_RUNTIME_PASS` claim.

## Frozen Surface Confirmation

Unchanged:

- `protocol/**`
- `gamedata/schemas/**`
- `docs/adr/**`
- `client/Unity/Assets/Game/UI/design-tokens.json`

## Follow-Up

Continue with `LGO-RUNTIME-UI-STYLE-DUPLICATION-AUDIT-v1.0`: classify remaining direct style use in `M4PlayableClientController` and identify safe factory/helper candidates without broad refactors.
