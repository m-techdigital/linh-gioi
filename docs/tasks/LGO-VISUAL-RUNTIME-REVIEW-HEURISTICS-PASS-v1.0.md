# LGO Visual Runtime Review Heuristics Pass v1.0

Marker: `LGO_VISUAL_RUNTIME_REVIEW_HEURISTICS_READY`

## Goal

Strengthen the visual runtime evidence loop so every capture round produces machine-readable heuristics before manual/Codex visual review.

## Scope

- Add lightweight PNG evidence analysis without adding image-processing dependencies.
- Check expected checkpoint presence, resolution, file size, pixel variation, and duplicate frame hashes.
- Emit `FIX_REQUIRED` when a screenshot is missing, undersized, flat/blank, wrong resolution, or byte-identical to another checkpoint.
- Write `visual-runtime-evidence-heuristics.json` and `visual-runtime-evidence-heuristics.md` beside captured screenshots.
- Keep the existing non-claim rule: capture/build/heuristics do not equal `VISUAL_RUNTIME_PASS`.

## Non-Claims

- This is not final visual acceptance.
- This is not production art approval.
- Heuristics do not judge composition quality by themselves.
- No gameplay, protocol, gamedata schema, ADR, or design-token changes are included.

## Validation

```bash
python3.12 tools/validate_lgo_visual_runtime_review_heuristics.py
python3.12 tools/analyze_lgo_visual_runtime_evidence.py build/visual-evidence/latest --allow-review-required
git --no-pager diff --check
./tools/lgo_playable_closure_check.sh --source-only
```

## Decision

`LGO_VISUAL_RUNTIME_REVIEW_HEURISTICS_READY`
