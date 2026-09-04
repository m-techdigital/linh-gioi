# LGO Art V2 Codex Usage

Status: LGO_ART_V2_SEPARATED_ASSETS_INGESTED

Quality classification: STRUCTURAL_RUNTIME_PLACEHOLDER_V2

Use:

- `docs/reference-art/v2/runtime-ready/**` as source provenance inside the repo.
- `client/Unity/Assets/Game/Art/Runtime/V2/Resources/LGOArtV2/**` as Unity runtime import for temporary structural placeholders.
- `LgoVisualAssetRegistryV2` for runtime `Resources.Load` access.

Do not use:

- `docs/reference-art/v2/reference-only/**` in Unity runtime.
- Art v1.1 composite sheets or experimental crops as runtime-approved assets.
- V2 assets as production art or final visual quality.
- baked English labels from any reference/contact image as UI copy.

Player-facing UI text remains code-owned and Vietnamese.
