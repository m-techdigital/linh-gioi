# LGO Art V2 Unity Import

Status: LGO_ART_V2_UNITY_IMPORT_CLOSED

Quality classification: STRUCTURAL_RUNTIME_PLACEHOLDER_V2

This task imports separated V2 PNG assets into Unity runtime paths and wires a small runtime registry for temporary visual integration, mapping, validator coverage, and UI layout proof.

The imported V2 assets are not final visual quality and are not production art.

Scope:

- import `images/runtime-ready/**` through docs provenance into Unity as structural placeholders;
- create deterministic `.meta` files;
- add `LgoVisualAssetRegistryV2`;
- preserve Art v1.1 as reference/mockup only;
- do not add gameplay mechanics.

Frozen surfaces unchanged:

- `protocol/**`
- `gamedata/schemas/**`
- `docs/adr/**`
- `client/Unity/Assets/Game/UI/design-tokens.json`
