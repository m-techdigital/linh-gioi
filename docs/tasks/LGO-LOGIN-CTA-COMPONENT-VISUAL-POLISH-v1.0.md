# LGO Login CTA Component Visual Polish v1.0

Status: `LGO_LOGIN_CTA_COMPONENT_VISUAL_POLISH_READY`

## Scope

This task improves the login server/CTA component using existing runtime UI style code only.

## Changed

- `client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs`
- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiLayoutProfile.cs`
- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiSkin.cs`
- `tools/validate_lgo_login_cta_backing_balance.py`
- `tools/validate_lgo_login_cta_component_visual_polish.py`

## Result

- Login CTA backing is more readable against the V3B scene.
- Server selector frame is visually tied to the CTA component.
- Desktop/tablet/mobile card padding and min-height remain profile-owned.
- No new runtime image was imported.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No production art claim.
- No gameplay behavior change.
- No design-token JSON change.
- No protocol, GameData, or ADR change.

## Follow-Up

Continue with `LGO-LOGIN-CTA-COMPONENT-EVIDENCE-REFRESH-v1.0`: refresh runtime screenshots and review the Login checkpoint before deciding if another visual pass is needed.
