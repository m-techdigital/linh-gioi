# LGO World Responsive HUD Viewport Polish v1.0

Marker: `LGO_WORLD_RESPONSIVE_HUD_VIEWPORT_POLISH_READY`

## Scope

- Improve world HUD behavior across desktop, tablet, and mobile visual-evidence profiles.
- Preserve gameplay semantics, current UI contract, protocol, GameData schemas, ADRs, and design tokens.
- Do not add gameplay systems or production art claims.

## Changes

- Tablet world HUD now uses a compact profile similar to mobile for auxiliary/details panels.
- Tablet HUD width is ratio-based and capped to keep more of the world visible.
- Gate Keeper landmark position is nudged inward so the primary NPC target is not hidden by the left HUD on narrower profiles.

## Evidence

- `build/visual-evidence/profiles/desktop/world-hub.png`
- `build/visual-evidence/profiles/tablet/world-hub.png`
- `build/visual-evidence/profiles/mobile/world-hub.png`

## Validation

- `git --no-pager diff --check`
- `python3.12 tools/validate_lgo_device_profile_ui_budgets.py`
- `python3.12 tools/validate_m5_visual_evidence.py`
- `python3.12 tools/validate_m5_world_hub_readability.py`
- `LGO_VISUAL_RUNTIME_SOURCE_GATES=fast LGO_VISUAL_RUNTIME_SERVER_BUILD=skip LGO_VISUAL_RUNTIME_CLEAR_UNITY_CACHE=0 LGO_VISUAL_RUNTIME_TIMEOUT_SECONDS=420 ./tools/lgo_visual_runtime_review_profiles.sh`

## Decision

`LGO_WORLD_RESPONSIVE_HUD_VIEWPORT_POLISH_READY`

