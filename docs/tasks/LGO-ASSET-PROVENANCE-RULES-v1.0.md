# LGO Asset Provenance Rules v1.0

Marker: `LGO_ASSET_PROVENANCE_READY`

## Scope

Define provenance rules for reference, placeholder, candidate, and future production art. This task does not replace runtime art.

## Non-Claims

- No runtime art replacement.
- No production art claim.
- No image generation.
- No composite-sheet slicing.
- No protocol, GameData schema, ADR, or design-token change.

## Exit Gate

`tools/validate_lgo_asset_provenance.py` prints `LGO_ASSET_PROVENANCE_VALIDATION_PASS`.
