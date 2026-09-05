# LGO Runtime UI Typography Ownership Split Review v1.0

Status: `LGO_RUNTIME_UI_TYPOGRAPHY_OWNERSHIP_SPLIT_READY`

## Scope

This task creates a dedicated runtime typography owner and moves reusable label/status font metrics out of `RuntimeUiSpacing`.

## Changed

- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiTypography.cs`
- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiTypography.cs.meta`
- `client/Unity/Assets/Game/UI/Runtime/RuntimeUiSpacing.cs`
- `client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs`
- typography-related validators and docs

## Result

- `RuntimeUiTypography` now owns login, Character Hall, world HUD, dialogue, and top-status label font sizes.
- `RuntimeUiSpacing` remains focused on layout and component dimensions.
- Existing visual values are preserved; only source ownership changed.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No visual redesign claim.
- No gameplay, auth, protocol, GameData, ADR, or design-token change.

## Follow-Up

Continue with `LGO-RUNTIME-UI-TYPOGRAPHY-OWNERSHIP-EVIDENCE-REFRESH-v1.0`: refresh screenshots and review that typography ownership split did not regress readability.
