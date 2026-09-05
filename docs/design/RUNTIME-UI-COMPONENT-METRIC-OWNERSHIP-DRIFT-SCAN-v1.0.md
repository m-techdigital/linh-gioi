# Runtime UI Component Metric Ownership Drift Scan v1.0

Status: `LGO_RUNTIME_UI_COMPONENT_METRIC_OWNERSHIP_DRIFT_SCAN_READY`

## Purpose

Keep Linh Giới Online runtime UI maintainable by preventing reusable layout, typography, and component metrics from spreading across screen controllers.

## Decision

Reusable metrics now have clearer owners:

- `RuntimeUiSizing`: shell widths, header dimensions, login NPC/CTA asset slots, Character Hall panel/list/portrait sizing, and icon-button sizing.
- `RuntimeUiSpacing`: repeated spacing, ornament thickness, row gaps, button heights, margins, and hairline dimensions.
- `RuntimeUiTypography`: reusable section, badge, login, lobby, world, dialogue, and primary button font sizes.
- `RuntimeUiFactory` and `RuntimeUiSkin`: shared application points for panels, rows, buttons, text, and frames.

Controller-local values are allowed only when the value is viewport/composition specific and would lose intent if moved into a generic owner.

## Updated Runtime Surfaces

- Login shell max width, auth panel height, Gate Keeper stage dimensions, grounding glow dimensions, server status dot, and CTA max width now use named sizing constants.
- Header shell dimensions now use named sizing constants.
- Character Hall list, selected preview, portrait, and name-field dimensions now use named sizing constants.
- Section sigil/heading/title, badge labels, and primary button font size now use `RuntimeUiTypography`.
- Login ornament and generic ornament rule metrics now use named spacing constants.
- Icon button dimensions and label gap now use named metrics.

## Non-Goals

- No UI redesign.
- No gameplay behavior change.
- No asset import or generation.
- No protocol, GameData, ADR, or design-token change.

## Follow-Up

Refresh runtime evidence with `LGO-RUNTIME-UI-COMPONENT-METRIC-OWNERSHIP-EVIDENCE-REFRESH-v1.0` so login, Character Hall, world, dialogue, and session screens are visually checked after the metric owner cleanup.
