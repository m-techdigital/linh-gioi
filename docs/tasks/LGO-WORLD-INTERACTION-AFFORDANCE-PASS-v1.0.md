# LGO World Interaction Affordance Pass v1.0

Date: 2026-09-05

## Scope

Improve in-world interaction clarity without changing gameplay semantics, interaction range, combat rules, protocol, GameData, ADR, or design tokens.

## Changes

- Added a world-space `F / Space` prompt label for nearby interactables, hidden while dialogue is open.
- Updated target dummy world labels to reflect visual state: selected, hit, recovering, or idle.
- Added validator markers so world interaction affordance and target label state feedback remain covered.

## Evidence

- `build/visual-evidence/latest/world-hub.png`
- `build/visual-evidence/latest/npc-dialogue.png`
- `build/visual-evidence/latest/target-dummy-state.png`

## Validation

- `git --no-pager diff --check`
- `python3.12 tools/validate_m5_world_hub_readability.py`
- `python3.12 tools/validate_m6_combat_visual_readability.py`
- `python3.12 tools/validate_m6_unity_combat_placeholder_asset_import.py`
- `python3.12 tools/validate_package_hygiene.py`
- `LGO_VISUAL_RUNTIME_SOURCE_GATES=fast LGO_VISUAL_RUNTIME_SERVER_BUILD=skip LGO_VISUAL_RUNTIME_CLEAR_UNITY_CACHE=0 LGO_VISUAL_RUNTIME_TIMEOUT_SECONDS=420 ./tools/lgo_visual_runtime_review.sh`

## Decision

`LGO_WORLD_INTERACTION_AFFORDANCE_READY`

No `VISUAL_RUNTIME_PASS` is claimed from capture alone.
