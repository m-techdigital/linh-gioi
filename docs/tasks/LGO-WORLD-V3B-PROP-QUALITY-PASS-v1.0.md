# LGO World V3B Prop Quality Pass v1.0

Status: `DONE`

Decision: `LGO_WORLD_V3B_PROP_QUALITY_PASS_READY`

## Scope

Replace the most visible flat V2 world props with lightweight V3B-aligned separated runtime candidates.

No gameplay, combat rule, protocol, GameData schema, ADR, design-token, auth, DB, economy, social, or liveops changes are included.

## Assets Added

Runtime V3B prop candidates:

- `tree_cherry_v3b_candidate.png`: 224x224, 81 KB
- `tree_pine_v3b_candidate.png`: 256x256, 84 KB
- `lantern_prop_v3b_candidate.png`: 192x288, 49 KB
- `rock_moss_v3b_candidate.png`: 192x192, 49 KB
- `bridge_wood_v3b_candidate.png`: 384x192, 80 KB
- `banner_cultivation_v3b_candidate.png`: 192x288, 44 KB

All assets are separated transparent PNGs, not composite sheet crops. They are classified as `LGO_ART_V3B_RUNTIME_CANDIDATE_NOT_PRODUCTION_FINAL`.

## Runtime Integration

- `LgoVisualAssetRegistryV3B` now exposes V3B world prop sprites.
- `PlayableWorldController` prefers V3B props and falls back to V2 if a V3B asset is missing.
- Runtime scale was tuned after screenshot review to keep props readable without crowding the playable hub.

## Evidence

- `build/visual-evidence/latest/world-hub.png`
- `build/visual-evidence/latest/target-dummy-state.png`

Observed result:

- Cherry tree, pine tree, lanterns, rock, bridge, and banner now match the higher-detail V3B direction more closely.
- Remaining visible mismatch: shadow slime still uses V2 structural placeholder art and should be replaced next.

## Validation

- `git --no-pager diff --check`
- `python3.12 tools/validate_lgo_runtime_asset_weight.py`
- `python3.12 tools/validate_lgo_art_v3b_candidates.py`
- `python3.12 tools/validate_m5_world_hub_readability.py`
- `python3.12 tools/validate_m6_combat_visual_readability.py`
- `./tools/lgo_visual_runtime_review.sh` with fast gates and cached Unity assets

## Non-Claims

- These are not production-final props.
- This does not claim `VISUAL_RUNTIME_PASS`.
- This does not import reference posters, V1/V3 mockups, or composite sheets.
