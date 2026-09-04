# LGO World Hub Staging Density Pass v1.0

Marker: `LGO_WORLD_HUB_STAGING_DENSITY_PASS_READY`

## Scope

- Improve world hub visual density using existing V3B runtime assets.
- Do not add gameplay systems, new mechanics, protocol changes, GameData schema changes, ADR changes, or design-token changes.
- Do not add new runtime image weight.

## Changes

- Added secondary set-dressing instances for existing V3B trees, lanterns, rocks, and banners.
- Kept the main interactable landmarks readable: Gate Keeper, Training Stone, Spirit Gate, target dummy, and shadow warning.
- Preserved all interaction positions and semantics except the already-reviewed Gate Keeper inward nudge for viewport readability.

## Evidence

- `build/visual-evidence/latest/world-hub.png`
- `build/visual-evidence/profiles/desktop/world-hub.png`
- `build/visual-evidence/profiles/tablet/world-hub.png`
- `build/visual-evidence/profiles/mobile/world-hub.png`

## Validation

- `LGO_VISUAL_RUNTIME_SOURCE_GATES=fast LGO_VISUAL_RUNTIME_SERVER_BUILD=skip LGO_VISUAL_RUNTIME_CLEAR_UNITY_CACHE=0 LGO_VISUAL_RUNTIME_TIMEOUT_SECONDS=420 ./tools/lgo_visual_runtime_review.sh`
- `git --no-pager diff --check`
- `python3.12 tools/validate_m5_world_hub_readability.py`
- `python3.12 tools/validate_package_hygiene.py`

## Decision

`LGO_WORLD_HUB_STAGING_DENSITY_PASS_READY`

