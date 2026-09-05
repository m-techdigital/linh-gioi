# Runtime UI Responsive Session Shell Helper Review v1.0

Status: `LGO_RUNTIME_UI_RESPONSIVE_SESSION_SHELL_HELPER_REVIEW_READY`

## Scope

This pass extracts pure responsive shell calculations for the World HUD and session menu.

## Result

`RuntimeUiLayoutProfile` now owns:

- World HUD base min/max width;
- World HUD dialogue-aware max width;
- World HUD max height;
- session menu width, left, right, top, and max-height placement.

`M4PlayableClientController` still owns the actual UI mutation, state checks, dialogue visibility, setting visibility, and all account/world/combat/evidence flow.

## Boundary

This is the farthest safe extraction for the current controller without a larger state ownership split. More extraction should focus on pure calculations or reusable leaf widgets only.

## Follow-Up

Continue with `LGO-RUNTIME-UI-FACTORY-ADOPTION-EVIDENCE-REFRESH-v1.0` to refresh visual evidence after the UI helper refactor chain when runtime capture is available.

## Non-Claims

- No gameplay change.
- No account/character flow semantics change.
- No combat mechanic change.
- No runtime image payload change.
- No visual runtime PASS claim.
