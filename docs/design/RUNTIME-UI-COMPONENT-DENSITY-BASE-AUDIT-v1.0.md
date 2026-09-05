# Runtime UI Component Density Base Audit v1.0

Status: `LGO_RUNTIME_UI_COMPONENT_DENSITY_BASE_READY`

## Decision

Runtime UI now has a dedicated `RuntimeUiDensityProfile` owner for compact component density. It separates screen placement from per-component visual density:

- `RuntimeUiLayoutProfile`: viewport and screen composition.
- `RuntimeUiDensityProfile`: padding/margin density for repeated component groups.
- `RuntimeUiSpacing`: named raw spacing constants.
- `RuntimeUiFactory`: component construction and density application helpers.

## Applied Surface

Character Hall is the first adoption target because it had repeated list/card/status padding in both initial build and responsive refresh paths.

## Non-Claims

- No gameplay, account/character flow, auth, protocol, GameData, ADR, design-token, or art payload change.
- No `VISUAL_RUNTIME_PASS` claim.

## Follow-Up

Refresh runtime evidence and then continue applying the density profile to other repeated UI families only when the source pattern is genuinely duplicated.
