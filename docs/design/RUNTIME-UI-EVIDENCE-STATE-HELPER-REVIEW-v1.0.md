# Runtime UI Evidence State Helper Review v1.0

Marker: `LGO_RUNTIME_UI_EVIDENCE_STATE_HELPER_READY`

## Decision

Evidence-only UI states need a named helper instead of ad hoc controller booleans. This keeps runtime gameplay behavior separate from screenshot harness needs while still allowing visual evidence checkpoints to expose hidden compact HUD surfaces.

## Implemented

- `RuntimeUiEvidenceState` defines named evidence focus states.
- `RuntimeUiEvidenceState.None` is the default gameplay-facing state.
- `RuntimeUiEvidenceState.CombatPanelFocus` exposes the compact combat panel and hides the compact guidance card during target-dummy screenshot capture.
- `M4PlayableClientController` now reads semantic evidence state properties instead of a one-off `_forceCombatPanelForEvidence` flag.

## Boundaries

- Evidence state may alter temporary UI visibility for capture.
- Evidence state must not alter account, character, world, combat, protocol, or GameData semantics.
- New evidence states should be added only when a runtime screenshot cannot otherwise show a required review surface.

## Non-Claims

- No gameplay change.
- No combat mechanic change.
- No protocol, GameData, ADR, or design-token change.
- No `VISUAL_RUNTIME_PASS` claim.
