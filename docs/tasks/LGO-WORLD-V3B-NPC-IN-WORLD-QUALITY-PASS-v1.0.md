# LGO World V3B NPC In-World Quality Pass v1.0

Marker: `LGO_WORLD_V3B_NPC_IN_WORLD_QUALITY_PASS_READY`

## Scope

- Improve Gate Keeper runtime staging and visual evidence reliability.
- Preserve the existing dialogue semantics and interaction flow.
- Do not add gameplay, protocol, GameData schema, ADR, design-token, auth, DB, economy, social, or liveops changes.

## Changes

- The visual evidence dialogue checkpoint now moves the player near the Gate Keeper before triggering interaction.
- The Gate Keeper smoke position is offset within valid interaction range so player and NPC sprites do not overlap heavily in screenshots.
- The prior `npc-dialogue.png` checkpoint naming bug is fixed; the screenshot now shows the Vietnamese dialogue panel and NPC staging instead of stale local-combat state.

## Evidence

- `build/visual-evidence/latest/npc-dialogue.png`
- `build/visual-evidence/latest/world-hub.png`
- `build/visual-evidence/latest/visual-runtime-evidence-manifest.json`

## Validation

- `git --no-pager diff --check`
- `python3.12 tools/validate_m5_visual_evidence.py`
- `python3.12 tools/validate_m4_visible_ui.py`
- `LGO_VISUAL_RUNTIME_SOURCE_GATES=fast LGO_VISUAL_RUNTIME_SERVER_BUILD=skip LGO_VISUAL_RUNTIME_CLEAR_UNITY_CACHE=0 LGO_VISUAL_RUNTIME_TIMEOUT_SECONDS=420 ./tools/lgo_visual_runtime_review.sh`

## Decision

`LGO_WORLD_V3B_NPC_IN_WORLD_QUALITY_PASS_READY`

