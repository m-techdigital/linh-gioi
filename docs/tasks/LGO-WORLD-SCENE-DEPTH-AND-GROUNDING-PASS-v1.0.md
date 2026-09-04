# LGO World Scene Depth And Grounding Pass v1.0

Marker: `LGO_WORLD_SCENE_DEPTH_AND_GROUNDING_PASS_READY`

## Scope

- Improve the playable world hub presentation without adding gameplay systems.
- Replace the flat single-color ground read with a lightweight runtime procedural training-ground texture.
- Keep all world objects, dialogue flow, combat prototype semantics, protocol, GameData schemas, ADRs, and design tokens unchanged.

## Runtime Approach

- `PlayableWorldController` now creates a 512x512 procedural ground texture at runtime.
- The texture adds restrained stone tiling, a central training-circle read, and low-intensity spirit path cues.
- No composite sheet is sliced and no new large runtime image is added.
- This pass is a visual grounding improvement only, not final map art.

## Evidence

- Latest desktop runtime evidence: `build/visual-evidence/latest/world-hub.png`
- Latest dialogue/runtime checkpoint: `build/visual-evidence/latest/npc-dialogue.png`
- The visual review loop completed capture and intentionally did not claim `VISUAL_RUNTIME_PASS`.

## Validation

- `git --no-pager diff --check`
- `python3.12 tools/validate_m5_world_hub_readability.py`
- `python3.12 tools/validate_m5_visual_evidence.py`
- `python3.12 tools/validate_m6_combat_visual_readability.py`
- `LGO_VISUAL_RUNTIME_SOURCE_GATES=fast LGO_VISUAL_RUNTIME_SERVER_BUILD=skip LGO_VISUAL_RUNTIME_CLEAR_UNITY_CACHE=0 LGO_VISUAL_RUNTIME_TIMEOUT_SECONDS=420 ./tools/lgo_visual_runtime_review.sh`

## Decision

`LGO_WORLD_SCENE_DEPTH_AND_GROUNDING_PASS_READY`

