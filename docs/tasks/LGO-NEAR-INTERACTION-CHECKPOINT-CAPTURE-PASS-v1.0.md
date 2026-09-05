# LGO Near Interaction Checkpoint Capture Pass v1.0

Status: `LGO_NEAR_INTERACTION_CHECKPOINT_CAPTURE_READY`

## Scope

This pass extends the visual runtime evidence harness so future reviews can inspect the actual near-object interaction prompt states instead of relying on the generic World Hub screenshot.

## Changes

- Added `near-gatekeeper-prompt.png` after the player is positioned inside Người Giữ Cổng interaction range.
- Added `near-training-stone-prompt.png` after the player is positioned inside Đá Luyện interaction range.
- Updated the profile index and visual heuristics expected-screenshot lists so missing near-prompt screenshots fail evidence gates.
- Enlarged the world-space prompt presentation on tablet/mobile so `F Gặp` and `F Luyện` remain readable in captured evidence.
- Reused existing smoke-position methods; no gameplay state machine or input semantics were changed.

## Non-Claims

- No gameplay mechanic change.
- No protocol, GameData schema, ADR, or design-token change.
- No new runtime art import.
- No VISUAL_RUNTIME_PASS claim.

## Validation

- `python3.12 tools/validate_lgo_near_interaction_checkpoint_capture.py`
- `./tools/lgo_visual_runtime_review_profiles.sh`
- `./tools/lgo_playable_closure_check.sh --source-only`
- `python3.12 tools/validate_package_hygiene.py`
- `git --no-pager diff --check`

## Follow-Up

Run a profile evidence refresh and inspect `near-gatekeeper-prompt.png` / `near-training-stone-prompt.png` across desktop, tablet, and mobile. If the prompt scale or placement is weak, fix the presentation before advancing.
