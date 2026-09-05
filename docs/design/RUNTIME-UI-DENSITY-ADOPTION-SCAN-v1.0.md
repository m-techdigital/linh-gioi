# Runtime UI Density Adoption Scan v1.0

Status: `LGO_RUNTIME_UI_DENSITY_ADOPTION_SCAN_READY`

## Scan Result

The main playable runtime UI already routes most repeated component structure through `RuntimeUiFactory`, `RuntimeUiSkin`, `RuntimeUiSpacing`, `RuntimeUiSizing`, `RuntimeUiTypography`, `RuntimeUiLayoutProfile`, and the new `RuntimeUiDensityProfile`.

Useful follow-up candidates:

- `M5VisualEvidenceRunner`: source-visible evidence shell still used direct padding, text, and panel frame assignments. This is safe to adopt because it is tooling/runtime evidence UI, not gameplay.
- `UIShowcaseController`: editor/showcase foundation still has demo-local spacing. Keep as future cleanup unless it becomes part of player runtime or active evidence.

## Decision

Adopt shared runtime UI primitives in `M5VisualEvidenceRunner` now. Defer `UIShowcaseController` until a real runtime/showcase maintenance task needs it.

## Non-Claims

- No gameplay, protocol, GameData, ADR, design-token, auth, DB, economy, social, or liveops change.
- No `VISUAL_RUNTIME_PASS` claim.
