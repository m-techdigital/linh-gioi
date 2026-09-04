# LGO Art Pack v1 Codex Usage

Status: LGO_ART_V1_EXPERIMENTAL_SLICE_REVIEW_REQUIRED

Use `tools/slice_lgo_art_pack_v1.py` after the external pack is unzipped at:

```text
/Users/minhdc/Projects/LGO-ArtPacks/LGO-ART-PACK-v1.1
```

The legacy script may preserve previous crop outputs only as docs-side experimental evidence and emits `MAPPING.csv`. It must not mirror Art v1.1 crops to Unity Resources and must not promote any composite-sheet crop into runtime-approved or production art.

Runtime Unity loading starts at:

```text
Do not import `docs/reference-art/v1/runtime-asset-pack/**` into Unity runtime. These crops are experimental-source-only and exist for audit/review.
```

Do not use the reference-only boards as runtime assets.

Do not continue auto-slicing/cropping Art v1.1 composite sheets for runtime asset production.
