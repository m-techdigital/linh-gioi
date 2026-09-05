# LGO Evidence Gate Sequential Run Policy v1.0

Status: `LGO_EVIDENCE_GATE_SEQUENTIAL_RUN_POLICY_READY`

## Purpose

Evidence-producing commands may clean and rewrite their own output directories. Validators that read those directories must not run at the same time, or they can fail on transient missing files.

## Policy

Run these commands sequentially when they touch `build/visual-evidence/**`:

- `./tools/lgo_visual_runtime_review.sh`
- `./tools/lgo_visual_runtime_review_profiles.sh`
- `./tools/run_m5_visual_evidence_review.sh`
- `./tools/lgo_playable_closure_check.sh --source-only`
- evidence refresh validators that read `build/visual-evidence/latest/**`

Parallel execution is allowed only for read-only checks or independent output roots.

## Continuous Work Rule

If a source-only gate fails because another live process is cleaning evidence output, rerun the gate after the evidence producer finishes. Do not mark the project broken until the sequential rerun fails.

## Non-Claims

- This policy does not claim visual pass.
- This policy does not change gameplay or runtime presentation.
