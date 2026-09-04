# LGO World Label Readability Pass v1.0

Marker: `LGO_WORLD_LABEL_READABILITY_PASS_READY`

## Scope

- Improve world-space label readability for current playable world screenshots.
- Preserve all gameplay, combat prototype semantics, protocol, GameData schemas, ADRs, and design tokens.
- Do not add new image assets.

## Changes

- Increased world label font size slightly for better screenshot readability.
- Added a lightweight shadow text layer to each world label.
- Added a `Cảnh báo` label over the non-combat shadow warning visual.

## Evidence

- `build/visual-evidence/latest/world-hub.png`
- `build/visual-evidence/latest/npc-dialogue.png`

## Validation

- `LGO_VISUAL_RUNTIME_SOURCE_GATES=fast LGO_VISUAL_RUNTIME_SERVER_BUILD=skip LGO_VISUAL_RUNTIME_CLEAR_UNITY_CACHE=0 LGO_VISUAL_RUNTIME_TIMEOUT_SECONDS=420 ./tools/lgo_visual_runtime_review.sh`
- `git --no-pager diff --check`
- `python3.12 tools/validate_m5_world_hub_readability.py`
- `python3.12 tools/validate_m5_visual_evidence.py`
- `python3.12 tools/validate_package_hygiene.py`

## Decision

`LGO_WORLD_LABEL_READABILITY_PASS_READY`

