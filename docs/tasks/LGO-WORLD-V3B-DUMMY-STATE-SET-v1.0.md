# LGO World V3B Dummy State Set v1.0

Status: `DONE`

Decision: `LGO_WORLD_V3B_DUMMY_STATE_SET_READY`

## Scope

Replace the remaining V2 target dummy state fallbacks with lightweight V3B-aligned runtime candidates for selected, hit, and recover states.

This task only changes visual presentation and evidence coverage. It does not add combat mechanics, damage rules, rewards, server authority, protocol changes, GameData schema changes, ADR changes, or design-token changes.

## Asset Review

- Rejected direct generated candidate because it carried a semi-transparent backdrop that would create a visible rectangle in Unity.
- Accepted runtime path uses a cleaned transparent cutout with no composite-sheet crop and no reference-board import.
- Hit and recover variants are controlled color-state derivatives from the accepted selected cutout, preserving silhouette and scale while keeping the runtime footprint small.

## Runtime Assets

- `target_dummy_selected_v3b_candidate.png`: 256x384, 128 KB
- `target_dummy_hit_v3b_candidate.png`: 256x384, 126 KB
- `target_dummy_recover_v3b_candidate.png`: 256x384, 127 KB

All three are classified as `LGO_ART_V3B_RUNTIME_CANDIDATE_NOT_PRODUCTION_FINAL`.

## Runtime Integration

- `LgoVisualAssetRegistryV3B` now exposes selected, hit, and recover target dummy sprites.
- `PlayableWorldController` resolves target dummy state from existing local feedback state:
  - selected when the player is near the training target;
  - hit while local hit feedback is active;
  - recover while cooldown is active;
  - idle otherwise.
- `VisualRuntimeEvidenceRunner` now captures `target-dummy-state.png` after moving to the target dummy and triggering the local-only combat preview.

## Validation

Run after this task:

- `git --no-pager diff --check`
- `python3.12 tools/validate_lgo_runtime_asset_weight.py`
- `python3.12 tools/validate_lgo_art_v3b_candidates.py`
- `python3.12 tools/validate_m5_visual_evidence.py`
- `python3.12 tools/validate_m5_world_hub_readability.py`
- `python3.12 tools/validate_m6_combat_visual_readability.py`
- `./tools/lgo_visual_runtime_review.sh`

## Non-Claims

- These are not production-final combat assets.
- This does not claim `VISUAL_RUNTIME_PASS` before runtime screenshot review.
- This does not open full combat, auth, DB, economy, social, guild, market, liveops, or production backend scope.
