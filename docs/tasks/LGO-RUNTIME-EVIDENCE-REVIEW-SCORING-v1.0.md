# LGO Runtime Evidence Review Scoring v1.0

Marker: `LGO_RUNTIME_EVIDENCE_REVIEW_SCORING_READY`

## Scope

- Improve visual runtime evidence review output.
- Keep the gate honest: screenshots and build success do not become `VISUAL_RUNTIME_PASS`.
- Do not change gameplay, protocol, GameData schemas, ADRs, design tokens, or runtime art.

## Changes

- `VisualRuntimeEvidenceRunner` now writes a review checklist for every checkpoint.
- Checklist categories cover layout, scale, spacing, sharpness, asset quality, hierarchy, readability, and reference similarity.
- `tools/validate_m5_visual_evidence.py` now requires the checklist marker and `pass_claim=false`.

## Evidence

- `build/visual-evidence/latest/visual-runtime-evidence-review.md`
- `build/visual-evidence/latest/visual-runtime-evidence-manifest.json`

## Validation

- `LGO_VISUAL_RUNTIME_SOURCE_GATES=fast LGO_VISUAL_RUNTIME_SERVER_BUILD=skip LGO_VISUAL_RUNTIME_CLEAR_UNITY_CACHE=0 LGO_VISUAL_RUNTIME_TIMEOUT_SECONDS=420 ./tools/lgo_visual_runtime_review.sh`
- `git --no-pager diff --check`
- `python3.12 tools/validate_m5_visual_evidence.py`
- `python3.12 tools/validate_package_hygiene.py`

## Decision

`LGO_RUNTIME_EVIDENCE_REVIEW_SCORING_READY`

