# M6 Combat Visual Readability Polish Final Report v0.37.0

Final decision: `M6_COMBAT_VISUAL_READABILITY_RUNTIME_CLOSED_LOCAL_v0.37.0`

## Scope Result

The existing M6 minimal local combat prototype was polished for readability using the v0.36.0 image pack as reference-only guidance.

Implemented:

- target highlight placeholder ring;
- local hit flash continuity;
- cooldown/readiness display;
- target label in Vietnamese;
- explicit local-only prototype label;
- tooltip/help text clarifying that this is not real combat.

## Non-Claims

- No production art claim.
- No server-authoritative combat.
- No real combat implementation.
- No new combat mechanic.
- No protocol or GameData schema change.

## Reference Boundary

Only `docs/reference-art/v0.36.0/` was used as reference. Future-reference images were not used for v0.37.0 runtime scope.

## Validation

Source validation includes `tools/validate_m6_combat_visual_readability.py` and the existing playable closure harness.

Runtime PASS was claimed only after the Unity runtime smoke executed locally and emitted:

- `M6_MINIMAL_LOCAL_COMBAT_RUNTIME_SMOKE_PASS`
- `LGO_PLAYABLE_CLOSURE_RUNTIME_GATES_PASS`

Visual evidence review emitted `LGO_PLAYABLE_VISUAL_EVIDENCE_READY` with `screenshotStatus=CAPTURED` and `humanVisualAcceptancePending=true`.
