# LGO World Hub Visual Readability Cleanup Pass v1.0

Status: `LGO_WORLD_HUB_VISUAL_READABILITY_CLEANUP_READY`

Date: `2026-09-05`

## Scope

This pass improves the World Hub scene staging after profile evidence review. It keeps gameplay actors central, moves secondary set dressing farther to the edge, and scales decorative props down on tablet/mobile so the scene reads less like loose asset placement.

## Runtime Layout Changes

- Secondary trees, lanterns, banners, rocks, and bridge dressing now use viewport-aware positions and scales.
- Mobile/tablet profiles reduce decorative prop scale so player, Gate Keeper, Training Stone, and target dummy remain the primary read.
- Ground shadows continue to anchor props without adding runtime image weight.
- No interaction target, combat mechanic, movement, protocol, or GameData behavior is changed.

## Evidence Used

- `build/visual-evidence/profiles/desktop/world-hub.png`
- `build/visual-evidence/profiles/tablet/world-hub.png`
- `build/visual-evidence/profiles/mobile/world-hub.png`
- `build/visual-evidence/profiles/index.md`

## Non-Claims

- No new runtime art import.
- No production/final art claim.
- No gameplay mechanic change.
- No protocol or GameData change.
- No VISUAL_RUNTIME_PASS claim.
- No frozen-surface change.

## Validation

```bash
git --no-pager diff --check
python3.12 tools/validate_lgo_world_hub_visual_readability_cleanup.py
./tools/lgo_visual_runtime_review_profiles.sh
./tools/lgo_playable_closure_check.sh --source-only
```

## Follow-Up

Continue with `LGO-WORLD-HUB-INTERACTION-READABILITY-PASS-v1.0`, improving target affordance and interaction readability without adding new mechanics.
