# LGO Runtime UI Responsive Session Shell Helper Review v1.0

Status: `LGO_RUNTIME_UI_RESPONSIVE_SESSION_SHELL_HELPER_REVIEW_READY`

## Scope

This pass moves safe pure World HUD and session menu shell calculations into `RuntimeUiLayoutProfile`.

## Result

- World HUD width and height calculations now route through `RuntimeUiLayoutProfile`.
- Session menu placement and max-height calculations now route through `RuntimeUiLayoutProfile`.
- Live element mutation, visibility, dialogue state, combat preview state, and evidence hooks remain in `M4PlayableClientController`.

## Follow-Up

Continue with `LGO-RUNTIME-UI-FACTORY-ADOPTION-EVIDENCE-REFRESH-v1.0`.

## Non-Claims

- No gameplay change.
- No runtime image payload change.
- No production art claim.
- No visual runtime PASS claim.
