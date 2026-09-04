# Next Task Selection Rules

Marker: `LGO_NEXT_TASK_SELECTION_RULES_READY_v1.0`

## Purpose

These rules let Codex continue development without waiting for a new external prompt while still obeying frozen contracts and milestone ownership.

## Selection Priority

1. Fix failing source/package/runtime gates in the current source.
2. Remove automation friction that repeatedly causes false stops.
3. Improve runtime evidence and diagnostics for existing gameplay.
4. Improve UI/UX clarity without changing account, character, movement, or combat semantics.
5. Improve asset import hygiene and size budgets without creating new art during code tasks.
6. Write docs/specs for future work when implementation would require frozen contract approval.

## Safe Task Classes

- validator fixes that do not weaken gates;
- smoke runner diagnostics;
- report/package automation;
- stale status docs correction;
- UI copy/readability polish in Vietnamese;
- Unity import setting audits;
- source cleanup that is behavior-preserving and dependency-safe.

## Unsafe Task Classes

Open a contract-change request instead of implementation for:

- protocol message changes;
- GameData schema changes;
- ADR changes;
- auth/session/DB work while the current owner instruction keeps those areas closed;
- economy, rewards, inventory, guild, chat, market, party, or live ops;
- production-final art acceptance;
- any design-token mutation.

## Dirty Worktree Rule

When the worktree has many unrelated or unreviewed untracked files, do not blindly commit all files. Generate a report and mark commit status as `DEFERRED_DIRTY_WORKTREE_REVIEW_REQUIRED`.

## Internet Research Rule

Use current official documentation when implementation depends on engine/tool behavior that may change. Prefer Unity official docs for Unity import, memory, Resources, Addressables, build, and runtime behavior.
