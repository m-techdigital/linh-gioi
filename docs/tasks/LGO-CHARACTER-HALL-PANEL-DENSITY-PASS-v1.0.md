# LGO Character Hall Panel Density Pass v1.0

Marker: `LGO_CHARACTER_HALL_PANEL_DENSITY_READY`

## Goal

Make the Character Hall feel closer to the polished login screen by reducing heavy panels, tightening selection density, and preserving a clear create/select/enter-world flow.

## Scope

- Reduce the overall lobby panel width and inner padding.
- Use lighter dark-glass treatment for the character list, selected profile card, and create panel.
- Enlarge the V3B cultivator portrait slightly while lowering surrounding chrome.
- Keep mobile landscape in a two-zone layout with visible create/select actions.

## Non-Claims

- No account semantics changed.
- No character creation or selection behavior changed.
- No new runtime asset, gameplay, protocol, gamedata schema, ADR, or design-token change.
- Runtime capture is evidence only and does not claim `VISUAL_RUNTIME_PASS`.

## Validation

```bash
python3.12 tools/validate_lgo_character_hall_panel_density.py
git --no-pager diff --check
./tools/lgo_visual_runtime_review.sh
./tools/lgo_playable_closure_check.sh --source-only
```

## Decision

`LGO_CHARACTER_HALL_PANEL_DENSITY_READY`
