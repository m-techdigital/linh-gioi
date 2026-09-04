# Handoff - M6 Combat Visual Readability Polish v0.37.0

Status: `M6_COMBAT_VISUAL_READABILITY_RUNTIME_CLOSED_LOCAL_v0.37.0`

Source marker: `M6_COMBAT_VISUAL_READABILITY_POLISH_SOURCE_READY_v0.37.0`

## Summary

v0.37.0 applies the accepted v0.36.0 combat visual reference pack to the existing minimal local combat prototype as readability guidance only.

No production art was imported. No reference board was sliced into runtime sprites. No new combat mechanic was added.

## Changed Runtime Surface

- `client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs`
  - Added local target focus and cooldown placeholder rings.
  - Added target visual state text derived from existing local range/cooldown state.
  - Kept hit flash local-only.

- `client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs`
  - Added Vietnamese target readability label.
  - Tightened local-only prototype copy and attack tooltip.

## Frozen Surface Audit

- `protocol/**`: unchanged.
- `gamedata/schemas/**`: unchanged.
- `docs/adr/**`: unchanged.
- `client/Unity/Assets/Game/UI/design-tokens.json`: unchanged.

## Reference Art Boundary

Only `docs/reference-art/v0.36.0/` images were used as reference. Future reference images under `docs/reference-art/future-reference-v0.36.0/` were not used to expand v0.37.0 scope.

## Code Quality / Duplication / Ownership Audit

The change stays in the existing world/UI ownership boundaries and reuses existing placeholder creation helpers. The new readability state is derived from existing target position, range, cooldown, and VFX state; no duplicate combat simulation layer was introduced.

If runtime-usable combat art is required, create `M6_RUNTIME_USABLE_COMBAT_ASSET_PACK` with individual transparent PNG sprites, icons, UI panels, atlas rules, import settings, and provenance notes.

## Validation Evidence

- Source gates: PASS.
- Package-ready gates: PASS.
- Runtime gates: PASS, including `M6_MINIMAL_LOCAL_COMBAT_RUNTIME_SMOKE_PASS`.
- Visual evidence: READY with captured screenshots; human visual acceptance remains pending by design.
