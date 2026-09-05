# Runtime UI Style Debt Follow-Up Audit v1.0

Status: `LGO_RUNTIME_UI_STYLE_DEBT_FOLLOWUP_AUDIT_READY`

## Decision

The remaining playable UI controller still owns stateful screen composition, but compact HUD status labels are a safe stateless factory candidate.

## Added Helper

`RuntimeUiFactory.NewCompactStatusLabel` creates a role-colored status label and immediately applies the shared compact HUD frame through `ApplyHudStatusCompact`.

## Adopted Call Sites

- World area status.
- World guided step status.
- World direction status.
- World objective touch-priority status.
- World interaction touch hint.
- Combat target status.
- Combat range status.
- Combat feedback status.

## Boundary

The helper does not own runtime text mutation, element names, display toggles, or gameplay state. Those remain in `M4PlayableClientController`.

## Follow-Up

Refresh runtime evidence for World HUD and target dummy screens after compact status helper adoption.
