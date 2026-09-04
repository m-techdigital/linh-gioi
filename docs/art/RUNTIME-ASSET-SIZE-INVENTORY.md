# Runtime Asset Size Inventory

Status: `LGO_RUNTIME_ASSET_SIZE_INVENTORY_READY`

Date: 2026-09-05

## Boundary

This inventory covers Unity runtime image assets currently used by the playable slice. All V3B files listed here are runtime candidate, not production final art.

Rules:

- Do not crop composite/reference boards.
- Do not import reference-only posters or mockups as runtime sprites.
- Optimize per role and per device profile: mobile/tablet/desktop.
- Prefer JPEG only for opaque fullscreen backgrounds.
- Keep transparent UI, character, prop, VFX, and combat sprites as PNG.
- Treat V2 assets as structural placeholders and V3B assets as current runtime candidates until final art is accepted.

## Current Largest Runtime Assets

| Role | Runtime Path | Current Size | Current Decision |
|---|---:|---:|---|
| `login_background` | `client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/Login/login_background_spirit_gate_1920x1080_v3b_candidate.jpg` | 445 KB | acceptable desktop/tablet candidate; create lower-resolution mobile variant later |
| `world_spirit_gate` | `client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/World/gate/spirit_gate_v3b_candidate.png` | 295 KB | acceptable for current hub, but first target for future PNG quantization |
| `login_logo` | `client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/Login/logo_linh_gioi_online_v3b_light_runtime_candidate.png` | 191 KB | acceptable; keep one runtime logo, avoid duplicate logo variants |
| `world_player_male_cultivator` | `client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/World/characters/player_male_cultivator_idle_v3b_candidate.png` | 174 KB | acceptable near current budget, monitor before adding animation frames |
| `gate_keeper_npc_login` | `client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/Login/gate_keeper_npc_login_v3b_candidate.png` | 151 KB | acceptable; future mobile can use smaller atlas copy |
| `combat_target_dummy_selected` | `client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/Combat/target-dummy/target_dummy_selected_v3b_candidate.png` | 125 KB | acceptable |
| `combat_target_dummy_recover` | `client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/Combat/target-dummy/target_dummy_recover_v3b_candidate.png` | 123 KB | acceptable |
| `combat_target_dummy_hit` | `client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/Combat/target-dummy/target_dummy_hit_v3b_candidate.png` | 123 KB | acceptable |

## Immediate Optimization Targets

1. `world_spirit_gate`: evaluate lossless/lossy PNG optimization or a smaller runtime max texture before adding more gates.
2. `login_background`: add profile variants only when build pipeline supports device-specific bundles.
3. `world_player_male_cultivator`: require animation-frame budget before adding idle/walk/attack frame sets.
4. V2 fallback assets: keep while referenced, but retire only after dependency checks prove V3B coverage is complete.

## Optimization Pass v1

The first optimization pass uses Unity import profiles instead of duplicate image folders:

- `Standalone` keeps current runtime role sizes for PC evidence.
- `Android` caps mobile delivery aggressively: background 1024 px, normal sprites 512 px, VFX 256 px, cooldown rings 128 px.
- `iPhone` keeps a tablet-friendly middle ground: background 1536 px, normal sprites 768 px, VFX 256 px, cooldown rings 128 px.
- Transparent PNG source files are not recompressed blindly because alpha/glow edge quality needs visual comparison before replacement.

## Operating Notes

- Use `python3.12 tools/report_lgo_runtime_asset_size_inventory.py` to print the current sorted runtime asset table from the V3B manifest.
- Use `python3.12 tools/validate_lgo_runtime_asset_size_inventory.py` before package-ready gates.
- Use `python3.12 tools/enforce_lgo_runtime_asset_import_profiles.py` after regenerating V3B candidates.
- Use `python3.12 tools/validate_lgo_runtime_asset_import_profiles.py` to verify platform import settings.
- Full build size is not equal to runtime image payload. Unity/Mono/player framework overhead can dominate early builds, so asset work must track both source image size and final platform build reports.
