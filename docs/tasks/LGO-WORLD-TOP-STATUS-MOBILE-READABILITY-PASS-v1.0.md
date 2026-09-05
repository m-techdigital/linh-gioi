# LGO World Top Status Mobile Readability Pass v1.0

Status: `LGO_WORLD_TOP_STATUS_MOBILE_READABILITY_READY`

Date: `2026-09-05`

## Scope

This pass improves the top status/action chip presentation on mobile and tablet world-hub profiles. It keeps session semantics unchanged and only adjusts responsive UI sizing/copy for the existing top status and quit controls.

## Runtime Presentation Changes

- Mobile world status uses compact `Sẵn sàng: Bước 1/2` copy instead of a long sentence when the player is idle and ready.
- Tablet world status also uses the compact ready copy when appropriate.
- Top chip width, padding, font size, and action row max width now scale from viewport width instead of relying on a fixed desktop-looking size.
- Busy/error status text is not overwritten by the responsive copy rule.

## Non-Claims

- No gameplay change.
- No production art claim.
- No protocol or GameData schema change.
- No VISUAL_RUNTIME_PASS claim.
- No frozen-surface change.

## Validation

```bash
git --no-pager diff --check
python3.12 tools/validate_lgo_world_top_status_mobile_readability.py
./tools/lgo_playable_closure_check.sh --source-only
./tools/lgo_visual_runtime_review_profiles.sh
```
