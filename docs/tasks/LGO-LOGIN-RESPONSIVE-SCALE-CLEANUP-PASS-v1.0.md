# LGO Login Responsive Scale Cleanup Pass v1.0

Status: `LGO_LOGIN_RESPONSIVE_SCALE_CLEANUP_READY`

Date: `2026-09-05`

## Scope

This pass tightens the Login/Gate Entry first-screen proportions using the indexed desktop, tablet, and mobile evidence. It keeps the V3B background, logo, Gate Keeper, server selector, and enter button, but reduces visual crowding and avoids fixed oversize presentation.

## Runtime Layout Changes

- Desktop logo width now scales around the viewport instead of staying oversized.
- CTA panel is narrower and lighter so the Spirit Gate background remains visible.
- Gate Keeper staging is smaller and farther right, reducing overlap with the central CTA.
- Mobile and tablet use tighter logo/card clamps to preserve breathing room on small screens.
- Account/login semantics are unchanged.

## Evidence Used

- `build/visual-evidence/profiles/desktop/login.png`
- `build/visual-evidence/profiles/tablet/login.png`
- `build/visual-evidence/profiles/mobile/login.png`
- `build/visual-evidence/profiles/index.md`

## Non-Claims

- No new runtime art import.
- No production art claim.
- No gameplay change.
- No account/auth contract change.
- No VISUAL_RUNTIME_PASS claim.
- No frozen-surface change.

## Validation

```bash
git --no-pager diff --check
python3.12 tools/validate_lgo_login_responsive_scale_cleanup.py
./tools/lgo_visual_runtime_review_profiles.sh
./tools/lgo_playable_closure_check.sh --source-only
```

## Follow-Up

Continue with `LGO-WORLD-HUB-VISUAL-READABILITY-CLEANUP-PASS-v1.0`, focusing on world hub staging and asset hierarchy without adding gameplay mechanics.
