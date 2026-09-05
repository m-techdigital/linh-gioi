# Codex Continuous Workflow

Last updated: `2026-09-05`

## Purpose

This workflow prevents Linh Giới Online development from stopping after a tiny patch. Every session should understand the current state, the next valid task, the allowed paths, the validation commands, and the real stop conditions.

## Required State Files

- `AGENTS.md`: standing rules for Codex behavior in this repository.
- `docs/execution/PROJECT-STATE.md`: current milestone, source baseline, closed gates, pending gates.
- `docs/execution/NEXT-ACTION.md`: one active next action plus validation and stop rules.
- `docs/execution/TASK-LEDGER.md`: append-only task history and evidence pointers.
- `docs/execution/TASK-LEDGER-ROLLUP.md`: compact recent-task view for routine resume/context loading.

## Loop

1. Read `AGENTS.md`.
2. Run `python3.12 tools/lgo_state_brief.py` for the compact project state.
3. Open `docs/execution/PROJECT-STATE.md`, `docs/execution/NEXT-ACTION.md`, or full `TASK-LEDGER.md` only when the compact brief is insufficient.
4. Read `docs/execution/TASK-LEDGER-ROLLUP.md`; open full `TASK-LEDGER.md` only when needed.
5. Implement the next task within allowed paths.
6. Run the validation commands listed in `NEXT-ACTION.md`.
7. Run visual/runtime review when Unity/player tooling is available.
8. Review screenshots before claiming any visual decision.
9. Update `TASK-LEDGER.md`, regenerate `TASK-LEDGER-ROLLUP.md`, update evidence docs and `NEXT-ACTION.md`.
10. Continue to the follow-up task when no stop condition applies.

## Full In-Chat Continuous Mode

When the owner enables continuous work in this chat, Codex should keep developing in coherent batches instead of waiting for a new prompt after each checkpoint. If a runtime gate is unavailable, Codex should write the blocker/evidence into project state and continue with another roadmap-valid source, tooling, UI/UX, asset-pipeline, cleanup, or validation task that remains safe.

Commit/push only after related changes validate together. Avoid tiny spam commits, cache artifacts, generated build folders, or changes to frozen surfaces. The default cadence is to finish a coherent feature/phase/tooling batch first, then checkpoint; small validator/text-only edits should usually be grouped into the next related checkpoint.

Routine local loops should prefer concise output: `tools/lgo_continue_dev_loop.sh` defaults to Quick Resume plus ledger rollup. Use `LGO_DEV_LOOP_CONTEXT_MODE=full` only when diagnosing state drift or handoff confusion.

## Change Budget

Continuous work should increase player-visible quality faster than it increases repository noise.

- Prefer improving existing runtime/UI/tooling owners over creating new one-off files.
- Reuse validators and task docs for routine polish; create a new validator only for a recurring regression risk, package gate, frozen boundary, or evidence contract.
- Use quick gates while iterating on low-risk UI/layout/source cleanup, then run full gates before a coherent checkpoint, handoff, package, or shared runtime foundation change.
- Keep `NEXT-ACTION.md` focused on the current task and only durable operating markers. Historical detail belongs in `TASK-LEDGER.md` and the rollup.
- Commit after a feature-sized or workflow-sized batch validates, not after every tiny edit.

Do not parallelize gates that clean or rewrite the same output roots. In particular, `./tools/lgo_visual_runtime_review.sh`, `./tools/lgo_visual_runtime_review_profiles.sh`, `./tools/run_m5_visual_evidence_review.sh`, `./tools/lgo_playable_closure_check.sh --source-only`, and source-only validators that read `build/visual-evidence/**` must run sequentially.

## Failure Classification

- `PASS`: all required source/runtime gates for the current task completed, and any visual evidence has been reviewed.
- `FIX_REQUIRED`: source/runtime gate failed but can be fixed within allowed paths.
- `VISUAL_CAPTURE_TIMEOUT`: Unity Player launched but visual screenshot capture did not finish before the bounded timeout.
- `VIDEO_CAPTURE_BLOCKED_ENV`: video capture was requested but no video capture harness or environment is available.
- `RUNTIME_BLOCKED_ENV`: Unity Editor, player runtime, Java, Maven, or local API runtime is unavailable.

## Non-Claims

- Source inspection is not runtime PASS.
- Unity build success is not visual PASS.
- Screenshot capture is not human acceptance.
- Placeholder or candidate art is not production-final art unless explicitly accepted as such.
