# LGO World HUD Mobile Hierarchy Evidence Refresh v1.0

Status: `LGO_WORLD_HUD_MOBILE_HIERARCHY_EVIDENCE_REFRESH_READY`

Date: `2026-09-05`

## Scope

This pass refreshes runtime profile screenshots after the normal mobile World Hub HUD hierarchy polish.

## Evidence Captured

- desktop: `build/visual-evidence/profiles/desktop/world-hub.png`
- tablet: `build/visual-evidence/profiles/tablet/world-hub.png`
- mobile: `build/visual-evidence/profiles/mobile/world-hub.png`
- mobile dialogue: `build/visual-evidence/profiles/mobile/npc-dialogue.png`

## Visual Review Notes

- desktop: world HUD keeps full guidance and action rows without covering the main staging area.
- tablet: world HUD remains readable, with the Gate Keeper and scene props still clear of the left panel.
- mobile: normal world HUD now hides redundant direction copy, uses a narrower/lighter panel, and leaves more scene space visible.
- mobile dialogue: guidance remains hidden while dialogue is active, keeping `Tiếp tục` and `Đóng` visible inside the viewport.
- remaining polish: future passes should improve top-right action safe areas and longer-term world scene composition, but this batch improves mobile hierarchy without changing gameplay.

## Follow-Up

Continue with `LGO-SOURCE-GATE-EVIDENCE-PRESERVATION-PASS-v1.0` to keep visual evidence artifacts from being unnecessarily erased during source-only validation and make the continuous workflow faster.

## Non-Claims

- No VISUAL_RUNTIME_PASS claim.
- No production art claim.
- No gameplay change.
- No frozen-surface change.

## Validation

```bash
git --no-pager diff --check
python3.12 tools/validate_lgo_world_hud_mobile_hierarchy_polish.py
python3.12 tools/validate_lgo_world_hud_mobile_hierarchy_evidence_refresh.py
./tools/lgo_playable_closure_check.sh --source-only
./tools/lgo_visual_runtime_review_profiles.sh
```
