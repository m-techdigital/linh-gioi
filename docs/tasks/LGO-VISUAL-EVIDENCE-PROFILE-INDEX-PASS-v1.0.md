# LGO Visual Evidence Profile Index Pass v1.0

Status: `LGO_VISUAL_EVIDENCE_PROFILE_INDEX_READY`

Date: `2026-09-05`

## Scope

This pass adds a lightweight index for desktop, tablet, and mobile visual evidence so current screenshots, manifests, heuristics, sizes, and reference mappings can be reviewed quickly without rescanning the build tree or rerunning Unity unnecessarily.

## Tooling Changes

- `tools/report_lgo_visual_evidence_profile_index.py` writes:
  - `build/visual-evidence/profiles/index.json`
  - `build/visual-evidence/profiles/index.md`
- `tools/lgo_visual_runtime_review_profiles.sh` runs the indexer after profile captures.
- `tools/lgo_playable_closure_check.sh` validates the indexer source without requiring runtime screenshot files to exist.

## Review Coverage

- desktop
- tablet
- mobile
- login
- character lobby
- character select
- enter world
- world hub
- target dummy state
- NPC dialogue
- session menu

## Non-Claims

- No gameplay change.
- No new runtime art import.
- No production art claim.
- No VISUAL_RUNTIME_PASS claim.
- No frozen-surface change.

## Validation

```bash
git --no-pager diff --check
python3.12 tools/validate_lgo_visual_evidence_profile_index.py
./tools/lgo_visual_runtime_review_profiles.sh
./tools/lgo_playable_closure_check.sh --source-only
```

## Follow-Up

Continue with `LGO-LOGIN-RESPONSIVE-SCALE-CLEANUP-PASS-v1.0` after the latest indexed evidence is available.
