# LGO Visual Evidence Blank Screen Detection v1.0

Status: `LGO_VISUAL_EVIDENCE_BLANK_SCREEN_DETECTION_READY`

## Purpose

Visual evidence must prove that screenshots contain real rendered information, not just files with the expected names. This gate adds reusable PNG content heuristics for both the current runtime review and the legacy M5 compatibility review.

## Runtime Policy

- Current runtime review continues to write to `build/visual-evidence/latest`.
- Legacy M5 compatibility review writes to `build/visual-evidence/m5-latest`.
- Both review paths must run `tools/analyze_lgo_visual_runtime_evidence.py`.
- The analyzer must fail on likely blank, flat, transparent, or suspiciously tiny screenshots.
- Duplicate screenshots are marked review-required and must not be used as human visual acceptance without inspection.

## M5 Legacy Fix

`tools/run_m5_visual_evidence_review.sh` no longer launches the player with `-batchmode`, because that path can produce flat gray frames while still writing PNG files. It now captures through a visible player path and runs the shared PNG heuristics against the M5 checkpoint list.

## Non-Claims

- This is an evidence quality gate, not a visual acceptance claim.
- Screenshot heuristic success is not `VISUAL_RUNTIME_PASS`.
- No gameplay, protocol, GameData, ADR, design-token, auth, DB, economy, social, or liveops change.
