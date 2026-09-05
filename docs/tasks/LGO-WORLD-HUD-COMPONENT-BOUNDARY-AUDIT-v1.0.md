# LGO World HUD Component Boundary Audit v1.0

Status: `LGO_WORLD_HUD_COMPONENT_BOUNDARY_AUDIT_READY`

## Scope

This pass reduces World HUD UI duplication by moving stateless shell and divider primitives into `RuntimeUiFactory`.

## Source Changes

- `RuntimeUiFactory.NewWorldHudRoot` now creates the reusable World HUD root shell.
- `RuntimeUiFactory.NewOrnamentRule` now owns the thin ornamental divider shared by lobby/world surfaces.
- `M4PlayableClientController.BuildWorldHud` composes those primitives and keeps stateful runtime behavior.

## Validation

- `python3.12 tools/validate_lgo_world_hud_component_boundary_audit.py`
- `./tools/lgo_playable_closure_check.sh --source-only`

## Non-Claims

- No gameplay change.
- No combat mechanic change.
- No protocol, GameData, ADR, or design-token change.
- No `VISUAL_RUNTIME_PASS` claim.
