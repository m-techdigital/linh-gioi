# ART V2 Usage Boundary

Status: STRUCTURAL_RUNTIME_PLACEHOLDER_V2

Authoritative classification:

- V1 = reference/mockup/art direction only.
- V2 = structural runtime placeholder, useful for temporary integration and mapping.
- V3 = required for polished high-res runtime art.

V2 may be used for:

- proving Unity `Resources` import paths;
- validating per-file separated PNG provenance;
- temporary login, UI, world, combat, and VFX placeholder hookup;
- Vietnamese UI layout proof where text is rendered in Unity, not baked into sprites;
- review screenshots that explicitly label V2 as temporary placeholder quality.

V2 must not be used for:

- production art;
- final visual quality;
- marketing screenshots;
- final acceptance of game identity or polish;
- replacing ART V3 high-resolution separated source requirements.

Reference-only boundary:

- `docs/reference-art/v2/reference-only/**` is for human review only.
- Reference boards, mockups, overview guides, contact sheets, and composite sheets must not be imported into Unity runtime.
- ART PACK v1.1 composite slices remain experimental evidence only and must not be promoted to runtime-approved or production art.

Runtime placeholder boundary:

- `docs/reference-art/v2/runtime-ready/**` and matching Unity imports may exist only as temporary structural placeholders.
- The label `runtime-ready` means file-separated and technically importable for prototype wiring, not final quality.
- Any runtime-visible V2 use must remain replaceable by ART V3 without protocol, GameData schema, or gameplay semantics changes.
