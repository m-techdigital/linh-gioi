# LGO M5 Visual Evidence Runner Skin Adoption Evidence v1.0

Status: `LGO_M5_VISUAL_EVIDENCE_RUNNER_SKIN_ADOPTION_EVIDENCE_READY`

## Scope

This task keeps the legacy M5 visual evidence runner useful without letting it drift away from shared runtime UI helpers or overwrite the newer visual runtime harness output.

## Changes

- `M5VisualEvidenceRunner` now uses shared runtime UI skin, spacing, text, and panel helpers.
- `run_m5_visual_evidence_review.sh` now writes to `build/visual-evidence/m5-latest` instead of deleting the full `build/visual-evidence` directory.
- The main visual runtime harness can keep `build/visual-evidence/latest` intact.

## Evidence

Runtime-generated screenshots are not required for source-only validation.
- `run_m5_visual_evidence_review.sh` writes runtime evidence to `build/visual-evidence/m5-latest` when the player path is available.
- The runtime command runs PNG heuristics so blank/flat captures fail honestly instead of passing by filename alone.

## Review Boundary

The legacy M5 runner is a compatibility harness. Its PNG files prove the runner and output path still work, but visual acceptance and UI quality review must use the current runtime harness under `build/visual-evidence/latest/**`.

If the legacy M5 player path times out or produces blank frames, classify it as runtime/evidence debt and keep using the current visual runtime harness for actual UI/UX acceptance.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` claim.
- No human visual acceptance claim from M5 compatibility screenshots.
- No gameplay, protocol, GameData, ADR, design-token, auth, DB, economy, social, or liveops change.

## Follow-Up

Continue with `LGO-VISUAL-EVIDENCE-OUTPUT-ISOLATION-AUDIT-v1.0`: verify all visual/evidence tooling writes to isolated paths and does not clobber current review output.
