# LGO Login Panel Visual Balance Pass v1.0

Marker: `LGO_LOGIN_PANEL_VISUAL_BALANCE_READY`

## Goal

Bring the login screen closer to the V3B north-star composition by reducing panel/button glare and making the logo, CTA, and Gate Keeper feel more integrated with the background.

## Scope

- Use the V3B text logo and CTA button asset while replacing the heavy CTA panel texture with a lighter dark-glass runtime panel.
- Reduce desktop/tablet logo and CTA footprint so the background gate remains visible.
- Lower and soften the Gate Keeper stage so the character reads as staged beside the gate rather than pasted over the scene.
- Keep mobile compact and avoid adding heavy new image assets.

## Non-Claims

- No new runtime image asset is added.
- No production art or final visual acceptance is claimed.
- No gameplay, protocol, gamedata schema, ADR, or design-token surface is changed.
- Screenshot capture and heuristics still do not claim `VISUAL_RUNTIME_PASS`.

## Validation

```bash
python3.12 tools/validate_lgo_login_panel_visual_balance.py
git --no-pager diff --check
./tools/lgo_visual_runtime_review.sh
./tools/lgo_playable_closure_check.sh --source-only
```

## Decision

`LGO_LOGIN_PANEL_VISUAL_BALANCE_READY`
