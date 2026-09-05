# LGO Visual Runtime Fast Profile Reuse Pass v1.0

Status: `LGO_VISUAL_RUNTIME_FAST_PROFILE_REUSE_READY`

Date: `2026-09-05`

## Scope

This pass adds a repeatable profile capture wrapper for fast UI/UX iteration. It builds the desktop Unity Player once, then reuses that player for tablet and mobile screenshot captures.

## Policy

Marker: `build_once_reuse_player`

- desktop: run source gates and build the player at `1920x1080`.
- tablet: reuse the same player at `1366x1024`.
- mobile: reuse the same player at `960x540`.
- output: `build/visual-evidence/profiles/**`.
- logs: `build/visual-evidence/profiles/profile-review.log`.

## Non-Claims

- No VISUAL_RUNTIME_PASS claim.
- No gameplay change.
- No production art claim.
- No frozen-surface change.

## Command

```bash
./tools/lgo_visual_runtime_review_profiles.sh
```

This command is also exposed as the VS Code task `LGO: Visual Runtime Profiles`.
