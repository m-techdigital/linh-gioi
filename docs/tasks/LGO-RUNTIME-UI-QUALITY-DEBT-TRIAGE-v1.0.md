# LGO Runtime UI Quality Debt Triage v1.0

Marker: `LGO_RUNTIME_UI_QUALITY_DEBT_TRIAGE_READY`

## Purpose

Review current visual runtime evidence and source debt, then choose the next safe high-value UI/runtime quality task without opening new gameplay or frozen contract work.

## Scope

- Review existing screenshots and evidence summaries.
- Record prioritized quality debt in `docs/design/RUNTIME-UI-QUALITY-DEBT-TRIAGE-v1.0.md`.
- Select the next implementation task.
- Keep the task docs/source validators aligned with continuous workflow.

## Selected Next Task

`LGO-LOGIN-NPC-GROUNDING-AND-CTA-PANEL-POLISH-v1.0`

## Boundaries

- Documentation/tooling triage only.
- No gameplay, runtime UI behavior, asset replacement, protocol, GameData schema, ADR, or design-token changes.
- No visual PASS is claimed from existing screenshots.

## Validation

- `python3.12 tools/validate_lgo_runtime_ui_quality_debt_triage.py`
- `git --no-pager diff --check`
- `./tools/lgo_playable_closure_check.sh --source-only`
