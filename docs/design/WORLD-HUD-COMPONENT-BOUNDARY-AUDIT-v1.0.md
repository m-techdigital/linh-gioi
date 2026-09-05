# World HUD Component Boundary Audit v1.0

Marker: `LGO_WORLD_HUD_COMPONENT_BOUNDARY_AUDIT_READY`

## Decision

World HUD construction should continue moving stateless visual primitives into `RuntimeUiFactory`, while `M4PlayableClientController` keeps stateful gameplay flow, API calls, evidence checkpoint state, and runtime label updates.

## Extracted Now

- `RuntimeUiFactory.NewWorldHudRoot` owns the base HUD shell frame, width, alignment, and starting padding.
- `RuntimeUiFactory.NewOrnamentRule` owns the reusable thin divider used by lobby and world HUD screens.
- `M4PlayableClientController.BuildWorldHud` now composes the HUD with factory primitives instead of retyping shell setup.

## Keep In Controller

- Login, lobby, world, dialogue, session, and evidence mode transitions.
- Runtime label text updates from `PlayableWorldController`.
- Local combat button state transitions and tooltips.
- Responsive profile application that depends on current viewport, dialogue state, session state, and evidence focus.

## Next Safe Extraction Candidates

- A typed World HUD header block if additional screens need the same title plus ornament grouping.
- Reusable compact evidence-state helpers after more checkpoints share the same forced-display pattern.
- A non-stateful action-shell builder only after combat, dialogue, and footer actions converge on the same layout contract.

## Non-Claims

- No gameplay change.
- No combat mechanic change.
- No protocol, GameData, ADR, or design-token change.
- No `VISUAL_RUNTIME_PASS` claim.
