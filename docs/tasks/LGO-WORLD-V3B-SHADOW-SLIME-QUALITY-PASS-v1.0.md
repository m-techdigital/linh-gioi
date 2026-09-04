# LGO World V3B Shadow Slime Quality Pass v1.0

Status: `DONE`

Decision: `LGO_WORLD_V3B_SHADOW_SLIME_QUALITY_PASS_READY`

## Scope

Replace the remaining V2 shadow slime placeholder with a lightweight V3B-aligned non-combat warning sprite.

This task preserves the existing local warning behavior. It does not add enemy AI, combat mechanics, loot, progression, protocol changes, GameData schema changes, ADR changes, design-token changes, auth, DB, economy, social, or liveops scope.

## Runtime Asset

- Source review asset: `docs/reference-art/v3b/runtime-candidates/world/creatures/shadow_slime_v3b_candidate.png`
- Unity runtime copy: `client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/World/creatures/shadow_slime_v3b_candidate.png`
- Runtime size: 192x192
- Runtime file size: 30 KB
- Classification: `LGO_ART_V3B_RUNTIME_CANDIDATE_NOT_PRODUCTION_FINAL`

## Runtime Integration

- `LgoVisualAssetRegistryV3B.ShadowSlime` loads the V3B sprite.
- `PlayableWorldController` prefers the V3B sprite and falls back to V2 only if missing.
- Runtime screenshot review confirms the east-side warning object no longer reads as a flat V2 placeholder.

## Evidence

- `build/visual-evidence/latest/world-hub.png`
- `build/visual-evidence/latest/target-dummy-state.png`

## Validation

- `git --no-pager diff --check`
- `python3.12 tools/validate_lgo_runtime_asset_weight.py`
- `python3.12 tools/validate_lgo_art_v3b_candidates.py`
- `python3.12 tools/validate_m5_world_hub_readability.py`
- `python3.12 tools/validate_m6_combat_visual_readability.py`
- `./tools/lgo_visual_runtime_review.sh` with fast gates and cached Unity assets

## Non-Claims

- This is not production-final creature art.
- This does not claim `VISUAL_RUNTIME_PASS`.
- This remains a local non-combat visual warning only.
