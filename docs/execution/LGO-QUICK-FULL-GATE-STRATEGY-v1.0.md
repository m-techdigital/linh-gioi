# LGO Quick / Full Gate Strategy v1.0

Status: `LGO_QUICK_FULL_GATE_STRATEGY_READY`

## Purpose

This strategy keeps continuous development fast without weakening validation. Quick gates are for tight iteration after low-risk changes. Full gates are required before phase-sized checkpoints, release/handoff, package creation, or any change that may alter runtime behavior, shared UI foundations, Unity import output, protocol/GameData consumption, server runtime, or visual acceptance.

## Quick Gate

Use quick gates for docs/tooling-only changes, narrowly scoped UI copy/layout tweaks, validator additions, and follow-up changes where the previous full gate is still relevant.

Quick gates are the default while iterating. They keep Codex from spending more time rebuilding evidence than improving the game, but they do not replace the full gate before a feature-sized checkpoint.

Default command:

```bash
LGO_DEV_LOOP_GATE_PROFILE=quick ./tools/lgo_continue_dev_loop.sh
```

Quick source/runtime review may use:

```bash
LGO_VISUAL_RUNTIME_SOURCE_GATES=fast \
LGO_VISUAL_RUNTIME_SERVER_BUILD=fast \
LGO_VISUAL_RUNTIME_PLAYER_BUILD=skip \
./tools/lgo_visual_runtime_review.sh
```

Quick gates may not be used to claim release readiness, package readiness, or human visual acceptance.

## Full Gate

Run full gates before any checkpoint commit that closes a feature-sized batch, before package/handoff, after shared UI foundation changes, after asset import/settings changes, after server/runtime code changes, and whenever quick evidence looks suspicious.

Do not run full gates reflexively after every tiny polish edit when the previous full gate is still relevant and the touched files are low-risk. Batch related changes, then run full validation once before committing or handing off.

Required baseline:

```bash
git --no-pager diff --check
PYTHONDONTWRITEBYTECODE=1 ./tools/lgo_playable_closure_check.sh --source-only
LGO_DEV_LOOP_GATE_PROFILE=full ./tools/lgo_continue_dev_loop.sh
```

For visual review, rebuild at least once before judging final runtime screenshots:

```bash
LGO_VISUAL_RUNTIME_SOURCE_GATES=full \
LGO_VISUAL_RUNTIME_SERVER_BUILD=fast \
LGO_VISUAL_RUNTIME_PLAYER_BUILD=build \
./tools/lgo_visual_runtime_review.sh
```

Run profile evidence when responsive layout or asset sizing changes:

```bash
./tools/lgo_visual_runtime_review_profiles.sh
```

## Commit Policy

Commit only after a coherent feature/tooling/quality batch validates. Do not commit every tiny edit. Do not push unless the remote path is configured and the checkpoint is intentionally ready to share.

Prefer no more than one checkpoint commit per coherent feature/tooling batch. If several tiny commits would describe the same player-facing improvement, continue locally and commit them together after validation.

## Failure Rules

- A skipped quick gate is not PASS.
- Build success is not visual PASS.
- Screenshot capture is not human acceptance.
- If a runtime gate is blocked, record the blocker and continue with safe source work when possible.
- If a quick gate fails, fix the root cause or escalate to full gates.

## Frozen Surfaces

No quick/full strategy task may modify:

- `protocol/**`
- `gamedata/schemas/**`
- `docs/adr/**`
- `client/Unity/Assets/Game/UI/design-tokens.json`
