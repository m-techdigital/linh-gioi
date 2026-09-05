# Runtime UI Factory Coverage Audit v1.0

Status: `LGO_RUNTIME_UI_FACTORY_COVERAGE_AUDIT_READY`

## Decision

Small, stateless visual element construction belongs in `RuntimeUiFactory` when multiple screens repeat the same setup. Stateful screen composition still belongs in `M4PlayableClientController`.

## Added Helper

`RuntimeUiFactory.NewImageLayer` creates a non-interactive image-backed `VisualElement` with:

- optional element name;
- `PickingMode.Ignore`;
- background scale mode;
- optional texture binding;
- optional tooltip.

## Adopted Call Sites

- Login Gate Keeper NPC layer.
- Login V3B logo lockup layer.
- Character Hall cultivator portrait layer.

## Boundary

Sizing, margins, screen positioning, fallbacks, callbacks, and state remain at the call site because those values are screen-specific.

## Follow-Up

Refresh runtime evidence after image-layer helper adoption, then continue with a narrow audit of remaining controller-owned helper candidates.
