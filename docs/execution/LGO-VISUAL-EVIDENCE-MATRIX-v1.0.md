# Linh Gioi Visual Evidence Matrix v1.0

Marker: `LGO_VISUAL_EVIDENCE_MATRIX_READY`

## Purpose

This matrix keeps visual acceptance focused on actual runtime screenshots and review aids, not source inspection or concept images.


## 2D Onboarding Current Views

The current `feature/2d` slice has a dedicated visual evidence subset. These screenshots prove runtime visibility only; they are not production art claims.

| View ID | Screenshot | Required runtime proof | Non-claim |
|---|---|---|---|
| `two_d_initial` | `build/2d-onboarding-visual/01-initial.bmp` | HUD snapshot and `runtimeTilemapSnapshot` exist | not production art |
| `two_d_gate_focus` | `build/2d-onboarding-visual/02-gate-focus.bmp` | route progress and base character snapshot exist | not production social hub |
| `two_d_skill_ready` | `build/2d-onboarding-visual/07-skill-ready.bmp` | combat and animation snapshots exist | not production combat |
| `two_d_inventory_try` | `build/2d-onboarding-visual/09-inventory-try.bmp` | inventory try-on/input snapshots exist | not production inventory economy |
| `two_d_inventory_applied` | `build/2d-onboarding-visual/10-inventory-applied.bmp` | inventory input and equipment snapshots exist | not persistent equipment save |

Run current verification with:

```bash
python3.12 tools/lgo_visual_evidence_matrix.py --verify-current
```

Expected marker:

```text
LGO_VISUAL_EVIDENCE_MATRIX_2D_CURRENT_PASS
```

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
