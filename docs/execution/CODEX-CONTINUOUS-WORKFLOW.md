# Codex Continuous Workflow

Last updated: `2026-09-05`

## Purpose

This workflow prevents Linh Giới Online development from stopping after a tiny patch. Every session should understand the current state, the next valid task, the allowed paths, the validation commands, and the real stop conditions.

## Required State Files

- `AGENTS.md`: standing rules for Codex behavior in this repository.
- `docs/execution/PROJECT-STATE.md`: current milestone, source baseline, closed gates, pending gates.
- `docs/execution/NEXT-ACTION.md`: one active next action plus validation and stop rules.
- `docs/execution/TASK-LEDGER.md`: append-only task history and evidence pointers.

## Loop

1. Read `AGENTS.md`.
2. Read `docs/execution/PROJECT-STATE.md`.
3. Read `docs/execution/NEXT-ACTION.md`.
4. Implement the next task within allowed paths.
5. Run the validation commands listed in `NEXT-ACTION.md`.
6. Run visual/runtime review when Unity/player tooling is available.
7. Review screenshots before claiming any visual decision.
8. Update `TASK-LEDGER.md`, evidence docs, and `NEXT-ACTION.md`.
9. Continue to the follow-up task when no stop condition applies.

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

