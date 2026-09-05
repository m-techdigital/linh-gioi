# LGO Source Gate Evidence Preservation Pass v1.0

Status: `LGO_SOURCE_GATE_EVIDENCE_PRESERVATION_READY`

Date: `2026-09-05`

## Scope

This pass keeps runtime visual evidence artifacts from being erased by routine source-only validation. It improves continuous development speed without weakening generated-source cleanup, package hygiene, or frozen-surface protections.

## Tooling Changes

- `tools/lgo_m4_closure_check.sh` no longer deletes the whole `build` directory during source-only cleanup.
- Disposable generated/cache outputs are still removed explicitly:
  - `client/Unity/Assets/Game/Generated`
  - `client/Unity/Assets/Game/Protocol/Generated`
  - Unity `Library`, `Temp`, and `Logs`
  - `build/generated`
  - package/delta/full-source staging folders under `build`
- Evidence/log folders such as `build/visual-evidence`, `build/codex-autopilot`, `build/dev-loop`, and closure summaries can survive source-only validation.

## Proof

- A preservation sentinel under `build/visual-evidence/source-gate-preservation/` survives `./tools/lgo_playable_closure_check.sh --source-only`.
- Package hygiene remains covered by `python3.12 tools/validate_package_hygiene.py`.

## Follow-Up

Continue with `LGO-VISUAL-EVIDENCE-PROFILE-INDEX-PASS-v1.0` to add a lightweight index/summary for latest desktop/tablet/mobile screenshots so visual review is faster for the owner and for future Codex sessions.

## Non-Claims

- No gameplay change.
- No visual quality claim.
- No VISUAL_RUNTIME_PASS claim.
- No package hygiene weakening.
- No frozen-surface change.

## Validation

```bash
git --no-pager diff --check
python3.12 tools/validate_lgo_source_gate_evidence_preservation.py
./tools/lgo_playable_closure_check.sh --source-only
python3.12 tools/validate_package_hygiene.py
```
