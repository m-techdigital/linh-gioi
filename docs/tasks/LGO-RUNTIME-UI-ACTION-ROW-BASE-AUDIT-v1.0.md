# LGO Runtime UI Action Row Base Audit v1.0

Status: `LGO_RUNTIME_UI_ACTION_ROW_BASE_READY`

## Scope

This task reduces repeated button/action sizing code in runtime UI factory paths.

## Changed

- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs`
- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiFactory.cs`
- `tools/validate_lgo_runtime_ui_action_row_base_audit.py`
- stale button/style validators updated to the helper-owned metrics path.

## Result

- Added `RuntimeUiSkin.ApplyButtonMetrics`.
- Reused it for base, primary, compact primary, compact secondary, quiet, icon, list, and combat button paths.
- Kept callbacks, player-facing Vietnamese copy, textures, account flow, and combat prototype behavior unchanged.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No production art claim.
- No gameplay behavior change.
- No design-token JSON change.
- No protocol, GameData, or ADR change.

## Follow-Up

Continue with `LGO-RUNTIME-UI-ACTION-ROW-EVIDENCE-REFRESH-v1.0`: refresh screenshots and review Login CTA, Character Hall actions, World HUD actions, session menu actions, and combat button readability after the helper consolidation.
