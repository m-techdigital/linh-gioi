# LGO World Mobile Camera Framing Pass v1.0

Status: `LGO_WORLD_MOBILE_CAMERA_FRAMING_READY`

Date: `2026-09-05`

## Scope

This pass improves world-hub readability on smaller screens by making the orthographic camera size responsive to the current viewport. It is presentation-only and preserves gameplay positions, movement, interaction ranges, and combat semantics.

## Runtime Presentation Changes

- Desktop keeps the existing `7.0` orthographic framing.
- Tablet uses a closer `6.15` orthographic framing.
- Mobile uses a closer `5.45` orthographic framing so player, NPC, dummy, and key props read larger.

## Non-Claims

- No gameplay change.
- No production art claim.
- No protocol or GameData schema change.
- No VISUAL_RUNTIME_PASS claim.
- No frozen-surface change.

## Validation

```bash
git --no-pager diff --check
python3.12 tools/validate_lgo_world_mobile_camera_framing.py
./tools/lgo_playable_closure_check.sh --source-only
./tools/lgo_visual_runtime_review_profiles.sh
```
