# Runtime UI Component Margin Token Audit v1.0

Status: `LGO_RUNTIME_UI_COMPONENT_MARGIN_TOKEN_READY`

## Decision

Viewport-responsive layout spacing belongs in `RuntimeUiLayoutProfile`; component rhythm shared across factory and primitive classes belongs in `RuntimeUiSpacing`.

## Adopted Tokens

- Shared panel min width, panel padding, preview panel padding, row gaps, compact status padding, badge spacing, toast spacing, base button sizing, and runtime icon sizing now live in `RuntimeUiSpacing`.
- `RuntimeUiFactory` consumes the spacing tokens for repeated panel, row, status, badge, button, and icon construction.
- `UIPrimitives` consumes the same spacing tokens for base buttons, icon buttons, panels, and tab gaps.

## Boundary

Heading micro-spacing, ornament rules, hidden debug controls, and one-off field/toggle tuning remain local until they repeat or become responsive. This avoids turning the spacing layer into a pile of one-use names.

## Non-Claims

- No gameplay behavior change.
- No new runtime art import.
- No protocol, GameData, ADR, or design-token change.

## Follow-Up

Refresh focused runtime evidence to confirm token adoption did not alter screen hierarchy or readability.
