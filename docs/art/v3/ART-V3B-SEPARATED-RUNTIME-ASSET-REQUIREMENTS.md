# ART V3B Separated Runtime Asset Requirements

Status: ART_V3B_SEPARATED_RUNTIME_ASSET_PRODUCTION_REQUIRED

ART V3B is the next required package for polished runtime art. It must contain actual separated high-resolution PNG source assets, not reference posters and not crops from composite sheets.

Required examples:

- `login_background_spirit_gate_1920x1080.png`
- `logo_linh_gioi_online_2048x1024.png`
- `gate_keeper_npc_1024x1536.png`
- `button_enter_world_normal_1024x256.png`
- `server_selector_panel_1024x256.png`
- `icon_notice_512x512.png`
- `icon_account_512x512.png`
- `icon_settings_512x512.png`
- `spirit_gate_1536x1536.png`
- `training_stone_1024x1536.png`
- `target_dummy_idle_1024x1024.png`
- `skill_wind_slash_icon_512x512.png`
- `wind_slash_vfx_01_1024x1024.png`
- `impact_spark_1024x1024.png`

Minimum quality requirements:

- high-resolution separated source assets;
- login background at least 1920x1080;
- character and NPC assets at least 1024px height;
- UI panels and buttons generated separately at 2x or 4x runtime size;
- skill icons at least 512x512;
- VFX frames at least 1024x1024, or clean 512x512 when intentionally authored at that size;
- transparent PNG when needed;
- no baked text unless explicitly intended and documented;
- no composite sheet crop as final runtime source.

Required metadata:

- manifest with path, dimensions, SHA256, usage, alpha expectation, and provenance;
- per-role Unity runtime texture budget; keep large source candidates for review, but cap imported runtime texture size to the smallest readable size;
- Unity import intent for each asset;
- replacement mapping from V2 placeholder identifiers to V3B final candidates;
- explicit reference-only folder separate from runtime source assets.

Runtime texture budget guidance:

- login background: max 2048 in Unity import unless a measured device target needs more;
- character/NPC/world props: max 1024 until camera framing proves larger is required;
- combat VFX frames: max 1024, with 512 preferred for small flashes/rings;
- cooldown rings and small HUD indicators: max 512;
- UI buttons/panels: max 1024, generated as separate assets at 2x or 4x runtime display size, not oversized poster art.

Acceptance gate:

Only ART V3B separated runtime assets may be considered for polished runtime-art acceptance after human visual review.
