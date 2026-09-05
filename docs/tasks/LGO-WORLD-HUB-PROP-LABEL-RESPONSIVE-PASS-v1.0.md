# LGO World Hub Prop Label Responsive Pass v1.0

Marker: `LGO_WORLD_HUB_PROP_LABEL_RESPONSIVE_READY`

## Goal

Reduce world-hub visual clutter by making scene labels stateful and objective-aware instead of showing every label at all times.

## Scope

- Keep only relevant labels visible for the current guided step, nearby interactables, target-dummy state, or warning state.
- Lower the default world-label size so labels read as captions instead of large UI blocks.
- Reposition key labels closer to their assets to improve desktop/tablet/mobile evidence framing.
- Preserve current gameplay behavior and runtime asset set.

## Non-Claims

- No gameplay mechanic is added.
- No protocol, gamedata schema, ADR, or design-token surface is changed.
- This is presentation-only polish, not final visual acceptance.
- This does not claim `VISUAL_RUNTIME_PASS`.

## Validation

```bash
python3.12 tools/validate_lgo_world_hub_prop_label_responsive.py
git --no-pager diff --check
./tools/lgo_playable_closure_check.sh --source-only
./tools/lgo_visual_runtime_review.sh
```

## Decision

`LGO_WORLD_HUB_PROP_LABEL_RESPONSIVE_READY`
