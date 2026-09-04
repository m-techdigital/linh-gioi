# LGO Art Pack v1 Asset Quality Review

Status: LGO_ART_V1_EXPERIMENTAL_SLICE_REVIEW_REQUIRED

## Classification

- `reference-only`: visual direction board, screen mockup pack, overview guide, mood/reference composition.
- `experimental-source-only`: composite UI/world/combat sheets and any crops sliced from them.
- `runtime-approved`: only individual transparent PNG assets with clean alpha, no baked text, no neighboring asset contamination, validated import settings, and provenance notes.

## Current Result

Art Pack v1.1 does not satisfy runtime-approved requirements as a final runtime asset pack because the usable-looking art came from composite AI sheets. Cropping from those sheets can damage borders, glow, alpha, scale, labels, and import quality.

The previously generated crop set is kept under `docs/reference-art/v1/runtime-asset-pack/**` as experimental evidence only. It must not be used to claim production art, final runtime art, or a controlled runtime asset pack.

## Runtime Decision

Current Unity runtime must continue using the existing approved individual placeholder PNGs from v0.45/v0.46:

- `client/Unity/Assets/Game/Art/Combat/Placeholders/Resources/CombatPlaceholders/**`

Missing login, lobby, world, NPC, prop, UI frame, icon, and combat assets for a future visual pass are tracked as `MISSING_RUNTIME_ASSET` and deferred to ART_V2.

Final decision for this review: `LGO_ART_V1_EXPERIMENTAL_SLICE_REVIEW_REQUIRED`.
