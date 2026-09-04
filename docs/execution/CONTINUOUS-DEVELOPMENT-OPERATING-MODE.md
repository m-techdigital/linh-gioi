# Linh Gioi Online Continuous Development Operating Mode

Marker: `LGO_CONTINUOUS_DEVELOPMENT_MODE_READY_v1.0`

## Purpose

This document turns repeated prompt-driven work into a repeatable local cycle. The goal is to let each development session choose the next valid task, implement within governance, validate, write evidence, package outside the source tree, and continue without stopping for a status-only handoff.

## Operating Loop

1. Read `docs/execution/TASK-LEDGER.md`, `docs/execution/LGO-MASTER-ROADMAP-v1.0.md`, and `docs/execution/LGO-NEXT-50-TASKS-BACKLOG-v1.0.md`.
2. Select the next task whose dependency is already closed and whose forbidden scope does not require owner or contract approval.
3. Prefer gameplay, UI/UX, runtime stability, validators, and maintainability over new image generation.
4. Keep image work separated from gameplay work unless the task explicitly requires runtime art integration.
5. Run source/package gates after each completed task.
6. Run runtime gates when the local Unity/player environment is available.
7. Write a short report file and update the task ledger.
8. Package artifacts outside the repo, under `/Users/minhdc/Projects/LGO-Handoffs`.
9. Run `tools/lgo_worktree_audit.py` when the worktree is large or mixed.
10. Commit and push only when the worktree content is intentionally reviewed for that phase and a Git remote is configured.
11. Continue to the next valid task unless a true blocker is reached.

## Stop Conditions

Stop only for:

- required changes to `protocol/**`, `gamedata/schemas/**`, `docs/adr/**`, or `client/Unity/Assets/Game/UI/design-tokens.json`;
- product decisions that cannot be inferred from roadmap/governance;
- human visual acceptance for production-final art;
- missing local runtime tooling that cannot be repaired in scope;
- a failing gate whose root cause is outside the active task's allowed ownership.

## Non-Stop Conditions

Do not stop only because:

- one validator failed and the fix is in scope;
- Unity/Maven/Python generated cache files that can be cleaned;
- a report/handoff file needs to be generated;
- an asset is not production quality but can remain clearly classified as placeholder/candidate;
- a task is complete and the next roadmap-valid task is clear.

## Asset Work Policy

Runtime art must be function-sized and optimized. Do not generate oversized buttons, icons, HUD frames, or VFX frames. Backgrounds may be larger because they cover the screen, but small UI and combat sprites must stay small and sharp.

Use:

- source reference art for direction only;
- V2 assets as structural placeholders only;
- V3B assets as runtime candidates only;
- future ChatGPT-generated assets only after they pass size, alpha, role, naming, and import-budget checks.

## Default Gate Set

Use `tools/lgo_continuous_cycle.py --phase source` for source/package hygiene and `tools/lgo_continuous_cycle.py --phase runtime` when Unity runtime evidence is required and available.

## Commit Discipline

The automation supports commit/push, but it must not blindly commit an ambiguous dirty worktree. Generated caches, build output, and local package archives must be excluded before commit. If many unrelated untracked files exist, the report must mark the commit as `DEFERRED_DIRTY_WORKTREE_REVIEW_REQUIRED` instead of pushing accidental source.
