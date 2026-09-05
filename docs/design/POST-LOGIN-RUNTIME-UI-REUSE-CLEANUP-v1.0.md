# Post-Login Runtime UI Reuse Cleanup v1.0

Status: `LGO_POST_LOGIN_RUNTIME_UI_REUSE_CLEANUP_READY`

## Scope

This pass reduces direct Character Hall layout composition inside `M4PlayableClientController` by moving stateless post-login UI construction into `RuntimeUiFactory`.

## Reused Component Ownership

- `RuntimeUiFactory` now owns the Character Hall content row, character list panel, selected-preview shell, selected-profile hero row, portrait frame, and flexible copy column.
- `M4PlayableClientController` still owns account state, character selection semantics, create/select/enter-world flow, and responsive state decisions.
- `RuntimeUiLayoutProfile`, `RuntimeUiSizing`, and `RuntimeUiSkin` remain the metric and visual-style owners.

## Non-Claims

- No gameplay change.
- No protocol, GameData, ADR, or design-token change.
- No runtime asset import or replacement.
- No `VISUAL_RUNTIME_PASS` claim from source inspection.

## Follow-Up

Continue with `LGO-POST-LOGIN-RUNTIME-UI-REUSE-EVIDENCE-REFRESH-v1.0`: refresh Character Hall and World Hub screenshots only after source gates pass.
