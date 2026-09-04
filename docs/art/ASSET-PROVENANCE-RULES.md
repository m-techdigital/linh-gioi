# Linh Gioi Asset Provenance Rules

Marker: `LGO_ASSET_PROVENANCE_READY`

## Purpose

Every asset must state where it came from, what it may be used for, and what it must not be claimed as.

## Required Metadata

For each runtime candidate:

- asset id;
- source path;
- Unity runtime path;
- creator/tool/source pack;
- generation prompt or source pack version when available;
- license/provenance note;
- classification;
- role;
- width and height;
- alpha requirement;
- file size;
- SHA-256;
- Unity import max texture size;
- production-final status.

## Classification Rules

- `REFERENCE_ONLY`: direction boards and mockups. Never import into Unity runtime.
- `EXPERIMENTAL_SOURCE_ONLY`: crops/experiments. Never claim as runtime-approved.
- `STRUCTURAL_RUNTIME_PLACEHOLDER_V2`: small separated placeholder assets. Usable for layout and mapping only.
- `RUNTIME_CANDIDATE_SIZE_BUDGETED`: role-sized candidate assets with import budget evidence.
- `PRODUCTION_FINAL_REVIEW_REQUIRED`: polished assets awaiting human visual acceptance.
- `PRODUCTION_FINAL_ACCEPTED`: only after explicit owner acceptance and provenance review.

## Forbidden

- Do not import reference-only images into Unity runtime folders.
- Do not crop composite sheets as final runtime source.
- Do not use downloaded third-party art without provenance/license notes.
- Do not bake player-facing text into sprites except approved logo/brand assets.
- Do not claim production quality from placeholder/candidate assets.

## Runtime Budget Link

All generated or imported runtime candidates must obey `docs/art/RUNTIME-ASSET-SIZE-BUDGET.md`.
