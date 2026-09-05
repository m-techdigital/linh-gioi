# LGO Visual Evidence Output Isolation v1.0

Status: `LGO_VISUAL_EVIDENCE_OUTPUT_ISOLATION_READY`

## Policy

Visual evidence tools must write to isolated output roots:

- Current runtime review: `build/visual-evidence/latest`
- Device profile review: `build/visual-evidence/profiles/<profile>`
- Legacy M5 compatibility review: `build/visual-evidence/m5-latest`

No visual evidence script should delete the whole `build/visual-evidence` tree during normal review. A script may clean only its own output directory.

## Reason

The project now has multiple evidence producers. Keeping output directories isolated prevents a compatibility harness from clobbering the current runtime screenshots used for UI/UX review.

## Non-Claims

- Evidence capture is not `VISUAL_RUNTIME_PASS`.
- Legacy M5 screenshots are compatibility evidence only, not human visual acceptance.
- No gameplay, protocol, GameData, ADR, design-token, auth, DB, economy, social, or liveops change.
