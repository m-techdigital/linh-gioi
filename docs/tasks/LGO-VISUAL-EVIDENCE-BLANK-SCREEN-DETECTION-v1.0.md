# LGO Visual Evidence Blank Screen Detection v1.0

Status: `LGO_VISUAL_EVIDENCE_BLANK_SCREEN_DETECTION_READY`

## Scope

This task prevents visual evidence validators from passing on filename/byte-size checks when the screenshot content is blank or flat.

## Implemented

- `tools/analyze_lgo_visual_runtime_evidence.py` accepts explicit expected screenshot lists and resolution overrides.
- `tools/run_m5_visual_evidence_review.sh` launches the visible player path instead of `-batchmode`.
- `tools/run_m5_visual_evidence_review.sh` runs the shared analyzer for M5 legacy screenshots.
- `tools/validate_lgo_visual_evidence_blank_screen_detection.py` validates the evidence gate wiring.

## Validation

- `python3.12 tools/validate_lgo_visual_evidence_blank_screen_detection.py`
- `python3.12 tools/analyze_lgo_visual_runtime_evidence.py build/visual-evidence/latest`
- `./tools/run_m5_visual_evidence_review.sh --open-existing` when runtime player is available.

## Follow-Up

Continue with `LGO-VISUAL-EVIDENCE-REVIEW-SUMMARY-VI-v1.0`: make evidence summaries more owner-readable in Vietnamese so long-running Codex/autopilot output is easier to inspect.
