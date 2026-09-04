# LGO World V3B Runtime Asset Brief v1.0

Status: `DONE`

Decision: `LGO_WORLD_V3B_RUNTIME_ASSET_BRIEF_READY`

## Scope

Define separated runtime asset requirements for replacing low-quality V2 structural placeholders visible in the world hub.

This task prepares the next art/runtime integration pass. It does not add gameplay, alter combat semantics, or import composite/reference sheets.

## Outputs

- `docs/art/v3b/WORLD-RUNTIME-ASSET-BRIEF-v3b.md`
- Updated V3B manifest consistency so the lighter login logo is tracked in CSV and JSON.
- Updated runtime asset weight validator to detect CSV/JSON manifest drift.
- Added `world_player_male_cultivator` manifest coverage for a corrected V3B male cultivator idle runtime candidate.

## Key Decisions

- V2 world assets remain temporary structural placeholders.
- V3B runtime world assets must be separated transparent PNGs with role-specific size budgets.
- Player and target dummy state sprites are highest priority because they appear in every world hub review screenshot.
- Props must stay lightweight; they should improve silhouette and style without becoming multi-hundred-KB decorative burden.
- Idle player weapon handling must be credible: sword stays sheathed/back-mounted or attached to the waist; if a hand touches the weapon it should touch the sheath/strap, not grip the hilt like an unintended draw pose.

## Validation

- `git --no-pager diff --check`
- `python3.12 tools/validate_lgo_runtime_asset_weight.py`
- `python3.12 tools/validate_lgo_device_profile_ui_budgets.py`
- `python3.12 tools/validate_lgo_art_v3b_candidates.py`
- `python3.12 tools/validate_m5_world_hub_readability.py`
- `LGO_VISUAL_RUNTIME_PROFILE=desktop LGO_VISUAL_RUNTIME_WIDTH=1920 LGO_VISUAL_RUNTIME_HEIGHT=1080 LGO_VISUAL_RUNTIME_OUT_DIR=build/visual-evidence/latest LGO_VISUAL_RUNTIME_SOURCE_GATES=fast LGO_VISUAL_RUNTIME_SERVER_BUILD=skip LGO_VISUAL_RUNTIME_PLAYER_BUILD=build LGO_VISUAL_RUNTIME_CLEAR_UNITY_CACHE=0 LGO_VISUAL_RUNTIME_TIMEOUT_SECONDS=360 ./tools/lgo_visual_runtime_review.sh`

## Asset Evidence

- Rejected generated candidate: weapon pose/grip read incorrectly for idle use.
- Accepted runtime candidate: `client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/World/characters/player_male_cultivator_idle_v3b_candidate.png`
- Runtime copy: 320x480 transparent PNG, 174 KB, under the 180 KB role budget.

## Non-Claims

- No production art claim.
- No `VISUAL_RUNTIME_PASS` claim.
- No frozen contract, protocol, GameData schema, ADR, or design-token change.
