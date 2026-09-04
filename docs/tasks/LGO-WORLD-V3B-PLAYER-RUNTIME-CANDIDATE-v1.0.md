# LGO World V3B Player Runtime Candidate v1.0

Status: `DONE`

Decision: `LGO_WORLD_V3B_PLAYER_RUNTIME_CANDIDATE_READY`

## Scope

Introduce a corrected V3B-aligned male cultivator player sprite for the world hub and replace the low-quality V2 player placeholder at runtime.

No gameplay, combat, protocol, GameData schema, ADR, or design-token changes are included.

## Asset Review

- Rejected candidate: generated/reference player art where weapon handling read as an incorrect sword grip for idle use.
- Accepted candidate: idle non-combat pose with relaxed hands and sword sheathed/back-mounted rather than held by the hilt.
- Runtime copy is a separated transparent PNG, not a composite crop.

## Runtime Asset

- Source review asset: `docs/reference-art/v3b/runtime-candidates/world/characters/player_male_cultivator_idle_v3b_candidate.png`
- Unity runtime copy: `client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/World/characters/player_male_cultivator_idle_v3b_candidate.png`
- Runtime size: 320x480
- Runtime file size: 174 KB
- Classification: `LGO_ART_V3B_RUNTIME_CANDIDATE_NOT_PRODUCTION_FINAL`

## Runtime Integration

- `LgoVisualAssetRegistryV3B.PlayerMaleCultivator` now loads the V3B player sprite.
- `PlayableWorldController` prefers the V3B player sprite and falls back to V2 only if the V3B asset is missing.
- Player world scale was tuned after screenshot review so the sprite is readable without overwhelming the hub.
- Target dummy base art now prefers the existing V3B dummy candidate where available; selected/cooldown/hit feedback remains local visual feedback only.

## Evidence

- `build/visual-evidence/latest/world-hub.png`
- `build/visual-evidence/profiles/desktop/world-hub.png`
- `build/visual-evidence/profiles/tablet/world-hub.png`
- `build/visual-evidence/profiles/mobile/world-hub.png`

## Validation

- `git --no-pager diff --check`
- `python3.12 tools/validate_lgo_runtime_asset_weight.py`
- `python3.12 tools/validate_lgo_art_v3b_candidates.py`
- `python3.12 tools/validate_m5_world_hub_readability.py`
- `python3.12 tools/validate_m6_combat_visual_readability.py`
- `./tools/lgo_visual_runtime_review_profiles.sh` with fast source gates and cached player build

## Non-Claims

- This is not production-final art.
- This does not claim final visual acceptance.
- This does not add combat, progression, inventory, auth, DB, economy, social, or liveops scope.
