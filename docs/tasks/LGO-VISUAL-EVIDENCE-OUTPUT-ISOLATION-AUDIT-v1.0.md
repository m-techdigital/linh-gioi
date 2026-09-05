# LGO Visual Evidence Output Isolation Audit v1.0

Status: `LGO_VISUAL_EVIDENCE_OUTPUT_ISOLATION_READY`

## Scope

This task validates that visual evidence tooling writes into isolated output directories and does not erase unrelated review outputs.

## Covered Paths

- `tools/lgo_visual_runtime_review.sh` defaults to `build/visual-evidence/latest`.
- `tools/lgo_visual_runtime_review_profiles.sh` writes profile output under `build/visual-evidence/profiles`.
- `tools/run_m5_visual_evidence_review.sh` writes legacy compatibility output under `build/visual-evidence/m5-latest`.

## Validation

- `validate_lgo_visual_evidence_output_isolation.py`
- `./tools/lgo_playable_closure_check.sh --source-only`

## Follow-Up

Continue with `LGO-QUICK-FULL-GATE-STRATEGY-v1.0`: document and validate when to run quick source/runtime gates versus full rebuild gates so iteration stays fast without lying about PASS.
