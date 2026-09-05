# Linh Giới Online — Task Ledger Rollup

Marker: `LGO_EXECUTION_LEDGER_ROLLUP_VIEW_READY`

## Quick State

- Current phase: runtime UI/visual quality hardening and execution workflow cleanup.
- Next task: `LGO-RUNTIME-UI-NEXT-HOTSPOT-SELECTION-v1.0`
- Source of truth: `docs/execution/TASK-LEDGER.md` remains append-only.
- Purpose: scan recent work quickly without deleting historical task rows or marker coverage.

## Recent Tasks

| Recent | Task ID | Status / Decision | Next allowed step |
|---:|---|---|---|
| 231 | LGO-COMBAT-BUTTON-COOLDOWN-VISUAL-EVIDENCE-REFRESH v1.0 | evidence ready / no visual pass claim; `LGO_COMBAT_BUTTON_COOLDOWN_VISUAL_EVIDENCE_REFRESH_READY`: target dummy screenshot refreshed after cooldown button lightness pass; cooldown reads lighter and remains placeholder-quality, no visual pass claimed | Continue with `LGO-RUNTIME-UI-STATE-DOC-COMPACTION-AUDIT-v1.0` |
| 232 | LGO-RUNTIME-UI-STATE-DOC-COMPACTION-AUDIT v1.0 | source ready / no visual pass claim; `LGO_RUNTIME_UI_STATE_DOC_COMPACTION_AUDIT_READY`: `NEXT-ACTION.md` now has a concise Quick Resume block while preserving historical marker registry compatibility | Continue with `LGO-EXECUTION-LEDGER-ROLLUP-VIEW-v1.0` |
| 233 | LGO-EXECUTION-LEDGER-ROLLUP-VIEW v1.0 | source ready / no visual pass claim; `LGO_EXECUTION_LEDGER_ROLLUP_VIEW_READY`: append-only task history now has a compact generated rollup with stale-output validation | Continue with `LGO-COMMIT-CADENCE-POLICY-HARDENING-v1.0` |
| 234 | LGO-COMMIT-CADENCE-POLICY-HARDENING v1.0 | source ready / no visual pass claim; `LGO_COMMIT_CADENCE_POLICY_HARDENING_READY`: autopilot now defaults to no auto-commit, with checkpoint commits and push both explicitly opt-in for cleaner history | Continue with `LGO-RUNTIME-UI-QUALITY-DEBT-TRIAGE-v1.0` |
| 235 | LGO-RUNTIME-UI-QUALITY-DEBT-TRIAGE v1.0 | source ready / no visual pass claim; `LGO_RUNTIME_UI_QUALITY_DEBT_TRIAGE_READY`: visual/source debt triage selected login NPC grounding and CTA panel polish as the next highest-value safe fix | Continue with `LGO-LOGIN-NPC-GROUNDING-AND-CTA-PANEL-POLISH-v1.0` |
| 236 | LGO-LOGIN-NPC-GROUNDING-AND-CTA-PANEL-POLISH v1.0 | source ready / runtime evidence pending; `LGO_LOGIN_NPC_GROUNDING_CTA_PANEL_POLISH_READY`: login Gate Keeper stage is lowered/anchored with a lightweight foot bloom and CTA backing is less boxy without adding image payload or changing flow | Continue with `LGO-LOGIN-NPC-GROUNDING-EVIDENCE-REFRESH-v1.0` |
| 237 | LGO-LOGIN-NPC-GROUNDING-EVIDENCE-REFRESH v1.0 | evidence reviewed / no visual pass claim; login desktop/tablet/mobile screenshots refreshed and reviewed; NPC grounding and CTA panel are improved, still not production-final, and no `VISUAL_RUNTIME_PASS` is claimed | Continue with `LGO-RUNTIME-UI-QUALITY-DEBT-FIRST-FIX-v1.0` |
| 238 | LGO-RUNTIME-UI-QUALITY-DEBT-FIRST-FIX v1.0 | source ready / evidence reviewed / no visual pass claim; `LGO_RUNTIME_UI_QUALITY_DEBT_FIRST_FIX_READY`: World Hub now has lightweight procedural cultivation-stage glow, directional path glows, and focus glows under key interactables; screenshots reviewed as improved but still placeholder-quality, no production/final visual claim | Continue with `LGO-CHARACTER-HALL-ACTION-DENSITY-FIRST-FIX-v1.0` |
| 239 | LGO-CHARACTER-HALL-ACTION-DENSITY-FIRST-FIX v1.0 | source ready / evidence reviewed / no visual pass claim; `LGO_CHARACTER_HALL_ACTION_DENSITY_FIRST_FIX_READY`: Character Hall shell no longer stretches ornate panel art over the full screen; glass/code-side frame improves hierarchy and mobile readability without changing character flow | Continue with `LGO-RUNTIME-UI-CONTROLLER-SIZE-REDUCTION-FIRST-PASS-v1.0` |
| 240 | LGO-RUNTIME-UI-CONTROLLER-SIZE-REDUCTION-FIRST-PASS v1.0 | source ready / evidence reviewed / no visual pass claim; `LGO_RUNTIME_UI_CONTROLLER_SIZE_REDUCTION_READY`: procedural world ground, shadows, focus glows, and path glows now live outside the playable world controller; runtime screenshot capture remains stable, no production/final visual claim | Continue with `LGO-WORKFLOW-FAST-GATE-NOISE-REDUCTION-v1.0` |
| 241 | LGO-WORKFLOW-FAST-GATE-NOISE-REDUCTION v1.0 | source/runtime tooling ready / no visual pass claim; `LGO_WORKFLOW_FAST_GATE_NOISE_REDUCTION_READY`: routine dev-loop context now uses Quick Resume plus ledger rollup, fast server build output is logged quietly, and visual runtime review restores Unity ProjectSettings after capture | Continue with `LGO-RUNTIME-CODE-HOTSPOT-REDUCTION-AUDIT-v1.0` |
| 242 | LGO-RUNTIME-CODE-HOTSPOT-REDUCTION-AUDIT v1.0 | source ready / runtime evidence reviewed / no visual pass claim; `LGO_RUNTIME_CODE_HOTSPOT_REDUCTION_READY`: login responsive application moved from the main playable UI controller into a dedicated helper, reducing controller size while preserving login runtime visuals and historical validation coverage | Continue with `LGO-WORLD-CONTROLLER-INTERACTION-PRESENTATION-SPLIT-v1.0` |
| 243 | LGO-WORLD-CONTROLLER-INTERACTION-PRESENTATION-SPLIT v1.0 | source ready / runtime evidence reviewed / no visual pass claim; `LGO_WORLD_CONTROLLER_INTERACTION_PRESENTATION_SPLIT_READY`: world-space label creation, shadowing, text refresh, and active toggling moved into `WorldLabelPresenter`; interaction/gameplay decisions remain in the world controller | Continue with `LGO-PREPARE-UNITY-ASSETS-QUIET-PROFILE-v1.0` |
| 244 | LGO-PREPARE-UNITY-ASSETS-QUIET-PROFILE v1.0 | tooling/runtime evidence ready / no visual pass claim; `LGO_PREPARE_UNITY_ASSETS_QUIET_PROFILE_READY`: Unity local asset preparation can now run in concise mode for routine visual evidence while still generating protocol and surfacing failure logs honestly | Continue with `LGO-VISUAL-RUNTIME-CAPTURE-FOCUS-ROBUSTNESS-AUDIT-v1.0` |
| 245 | LGO-VISUAL-RUNTIME-CAPTURE-FOCUS-ROBUSTNESS-AUDIT v1.0 | tooling/runtime evidence ready / no visual pass claim; `LGO_VISUAL_RUNTIME_CAPTURE_FOCUS_ROBUSTNESS_READY`: Unity evidence runner configures background-friendly capture in `Awake`, the launcher re-requests focus with bounded progress logging, and latest desktop evidence captured all 10 checkpoints without a manual click in the runner log | Continue with `LGO-WORLD-HUB-VISUAL-DEPTH-AND-WEIGHT-PASS-v1.0` |
| 246 | LGO-WORLD-HUB-VISUAL-DEPTH-AND-WEIGHT-PASS v1.0 | source ready / runtime evidence reviewed / no visual pass claim; `LGO_WORLD_HUB_VISUAL_DEPTH_WEIGHT_READY`: oversized procedural ground rings no longer dominate the World Hub, lightweight runtime-generated mist support is available, and stale M6 readiness docs-only diff policing no longer blocks current non-frozen world/UI implementation tasks | Continue with `LGO-CHANGESET-NOISE-AND-HOTSPOT-AUDIT-v1.0` |
| 247 | LGO-CHANGESET-NOISE-AND-HOTSPOT-AUDIT v1.0 | source ready / runtime evidence reviewed / no visual pass claim; `LGO_CHANGESET_NOISE_HOTSPOT_AUDIT_READY`: obsolete M6 readiness allowlist code was removed and visual evidence hooks moved into a partial evidence boundary so the main playable UI controller is smaller without changing runtime flow | Continue with `LGO-LOGIN-AND-CHARACTER-HALL-VISUAL-QUALITY-TRIAGE-v1.0` |
| 248 | LGO-LOGIN-AND-CHARACTER-HALL-VISUAL-QUALITY-TRIAGE v1.0 | source ready / runtime evidence reviewed / no visual pass claim; `LGO_CHARACTER_HALL_SELECTED_CTA_PRIORITY_READY`: selected Character Hall state now makes `Vào sân luyện` the first CTA on every profile, demotes `Tạo thêm`, and reduces create-panel emphasis without changing character flow | Continue with `LGO-RUNTIME-UI-NEXT-HOTSPOT-SELECTION-v1.0` |

## Operating Notes

- Do not edit this rollup as the canonical task history; update `TASK-LEDGER.md` first.
- Regenerate this file with `python3.12 tools/report_lgo_task_ledger_rollup.py` after a coherent batch closes.
- Keep historical markers in `NEXT-ACTION.md` until a dedicated registry migration is validated.
- This report does not claim runtime or visual PASS.
