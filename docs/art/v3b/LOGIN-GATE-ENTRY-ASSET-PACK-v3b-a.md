# Login Gate Entry Asset Pack v3b-a

Marker: `LGO_LOGIN_GATE_ENTRY_VISUAL_CLOSED_v1`

The local pack `LGO-ART-V3B-A-LOGIN-GATE-ENTRY-PRODUCTION-PACK` is accepted as a separated runtime-candidate source for the login and gate-entry presentation layer.

## Boundary

- Runtime import uses only `images/runtime-ready/**` from the local pack.
- `images/reference-only/**` remains visual reference only and is not imported.
- ART v1.1 composite slices remain non-final experimental material.
- V2 separated assets remain structural placeholders.
- V3B-A improves the playable login screen but is not claimed as final production art.

## Imported Runtime Groups

- Login background, logo, and Gate Keeper NPC.
- Enter-world button states.
- Server selector panel and status lights.
- Login utility icons.
- Shared small/main/dialogue panels for future UI reuse.
- World and VFX runtime candidates are staged for visual consistency, not new mechanics.

## Weight Policy

V3B-A is intentionally lightweight. The whole runtime-ready source pack is under 1 MB, and the imported Unity copy must stay free of `.DS_Store`, `__pycache__`, generated caches, and reference posters.

Large fullscreen backgrounds may remain PNG when the source pack is already compressed below budget. Future opaque background replacements may use JPEG or platform texture compression when visual quality remains acceptable.

## Final Login Background Candidate

The current login screen prioritizes dedicated `LGOFinalLogin/**` runtime candidates for the first screen:

- `login_background_spirit_gate_final_1920x1080.jpg`
- `logo_linh_gioi_online_final_420.png`
- `button_enter_world_final_384.png`

These are AI-generated specifically for the playable login surface from the accepted V3B gate-entry direction. The background is stored as a compressed opaque JPEG so the full-screen visual stays far smaller than a multi-megabyte PNG while keeping the gate-entry composition readable. The logo and primary button are purpose-sized transparent PNGs. V3B-A assets remain fallback/runtime-candidate material until replaced by accepted high-resolution separated production assets.
