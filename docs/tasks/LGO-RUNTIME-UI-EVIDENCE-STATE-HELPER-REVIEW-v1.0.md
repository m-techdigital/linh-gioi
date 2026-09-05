# LGO Runtime UI Evidence State Helper Review v1.0

Status: `LGO_RUNTIME_UI_EVIDENCE_STATE_HELPER_READY`

## Scope

This pass replaces one-off evidence visibility flags with a named `RuntimeUiEvidenceState` helper so screenshot checkpoints remain maintainable as more runtime review surfaces are added.

## Source Changes

- Added `RuntimeUiEvidenceState`.
- Replaced `_forceCombatPanelForEvidence` with `_evidenceState`.
- Kept normal compact HUD behavior unchanged while target-dummy evidence can still expose the combat panel.

## Validation

- `python3.12 tools/validate_lgo_runtime_ui_evidence_state_helper.py`
- `python3.12 tools/validate_lgo_combat_button_mobile_responsive_evidence.py`
- `./tools/lgo_playable_closure_check.sh --source-only`

## Non-Claims

- No gameplay change.
- No combat mechanic change.
- No protocol, GameData, ADR, or design-token change.
- No `VISUAL_RUNTIME_PASS` claim.
