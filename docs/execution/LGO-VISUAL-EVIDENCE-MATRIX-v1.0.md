# Linh Gioi Visual Evidence Matrix v1.0

Marker: `LGO_VISUAL_EVIDENCE_MATRIX_READY`

## Purpose

This matrix keeps visual acceptance focused on actual runtime screenshots and review aids, not source inspection or concept images.

## Required Views

| View | Evidence source | Claim |
|---|---|---|
| Login / Gate Entry | runtime screenshot or Unity capture | player sees current login/gate UI |
| Character Hall | runtime screenshot or Unity capture | account/character shell is readable |
| World HUD | runtime screenshot or Unity capture | HUD/status/objective shell is readable |
| First Playable Loop | runtime screenshot or Unity capture | Gate Keeper/Training Stone flow is visible |
| Combat Readiness HUD | runtime screenshot or Unity capture | target label, readiness, cooldown, and feedback are readable |
| Combat Placeholder Assets | contact sheet or runtime screenshot | assets are wired as placeholders/candidates only |

## Allowed Evidence

- Runtime screenshots from local Unity/player builds.
- Logs produced by existing smoke runners.
- Contact sheets for human review only when clearly marked as review aids.
- Reference boards only as visual direction comparison, never as runtime proof.

## Forbidden Evidence Claims

- Do not claim production art from V1, V2, or V3B assets.
- Do not claim runtime UI quality from reference-only mockups.
- Do not import composite sheets into Unity to satisfy evidence.
- Do not crop/slice boards to create evidence.

## Current One-Command Visual Gate

Use:

```bash
./tools/lgo_playable_closure_check.sh --visual-evidence
```

Expected closure marker:

```text
LGO_PLAYABLE_VISUAL_EVIDENCE_READY
```

Screenshot capture may still report screenshot environment limitations. That is acceptable only when clearly recorded as `UNVERIFIED_ENVIRONMENT` or screenshot-unavailable evidence, not as human acceptance.
