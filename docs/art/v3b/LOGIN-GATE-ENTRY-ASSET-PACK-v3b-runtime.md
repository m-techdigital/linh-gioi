# Login Gate Entry Asset Pack v3b Runtime

Marker: `LGO_LOGIN_GATE_ENTRY_V3B_RUNTIME_ONLY`

## Boundary

- Runtime login uses `client/Unity/Assets/Game/Art/Runtime/V3B/**` first.
- V3BA and FinalLogin runtime candidate folders are removed from Unity runtime assets to avoid fallback confusion.
- V2 remains a low-level structural fallback only when a V3B runtime asset is missing.
- V1 reference/mockup boards remain reference-only and must not be imported, sliced, or cropped into runtime assets.
- V3B assets are runtime candidates, not final production art.

## Runtime Login Assets

- `Login/login_background_spirit_gate_1920x1080_v3b_candidate.jpg`
- `Login/logo_linh_gioi_online_v3b_light_runtime_candidate.png`
- `Login/panel_main_dark_gold_v3b_candidate.png`
- `Login/button_enter_world_gold_v3b_candidate.png`
- `Login/gate_keeper_npc_login_v3b_candidate.png`

## Presentation Rules

- The login first screen must not show V2/V3BA utility placeholder icons.
- `logo_linh_gioi_online_v3b_light_runtime_candidate.png` is a purpose-built logo asset with intended baked brand text and reduced blue backdrop mass; regular UI labels and controls remain Unity-rendered Vietnamese text.
- Player-facing login copy stays Vietnamese.
- Asset weights remain bounded so the source stays lightweight while visual quality improves.
