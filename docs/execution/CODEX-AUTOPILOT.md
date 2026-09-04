# Linh Giới Online — Codex Autopilot Supervisor

This document defines the bounded supervisor used to continue governed Codex work across non-interactive CLI rounds.

## Command

```bash
./tools/lgo_codex_autopilot.sh
```

Useful environment overrides:

```bash
MAX_ROUNDS=5 MAX_SECONDS=7200 ./tools/lgo_codex_autopilot.sh
MAX_ROUNDS=1 ./tools/lgo_codex_autopilot.sh
CODEX_SANDBOX=danger-full-access ./tools/lgo_codex_autopilot.sh
CODEX_BYPASS_APPROVALS_AND_SANDBOX=0 CODEX_SANDBOX=workspace-write ./tools/lgo_codex_autopilot.sh
CODEX_RESUME_LAST=1 MAX_ROUNDS=20 ./tools/lgo_codex_autopilot.sh
LGO_AUTOPILOT_PUSH=1 MAX_ROUNDS=20 ./tools/lgo_codex_autopilot.sh
./tools/lgo_codex_autopilot.sh --dry-run
```

## Status File

Each Codex round must write:

```text
build/codex-autopilot/status.json
```

Allowed status values:

- `CONTINUE`
- `BLOCKED`
- `NEED_OWNER_DECISION`
- `NEED_HUMAN_VISUAL_REVIEW`
- `FIX_REQUIRED`
- `DONE`

If `docs/execution/NEXT-ACTION.md` still contains a valid next task, the status must be `CONTINUE`. Do not write `DONE` merely because one small task finished. Do not write `NEED_HUMAN_VISUAL_REVIEW` while safe cleanup, validation, evidence, or follow-up work remains.

## Supervisor Loop

For each bounded round, `tools/lgo_codex_autopilot.sh`:

1. checks it is running from the `LinhGioiOnline` repo;
2. checks `codex` exists, unless `--dry-run` is used;
3. writes a short prompt to `build/codex-autopilot/prompt-round-N.txt`;
4. runs `codex exec --dangerously-bypass-approvals-and-sandbox -C <repo>` by default so local sockets and Unity runtime evidence can execute without approval prompts;
5. fails with `FIX_REQUIRED` if `codex exec` exits non-zero;
6. reads `build/codex-autopilot/status.json`;
7. continues only when status is `CONTINUE`;
8. stops for `BLOCKED`, `NEED_OWNER_DECISION`, `NEED_HUMAN_VISUAL_REVIEW`, `FIX_REQUIRED`, or `DONE`.

The supervisor is bounded by `MAX_ROUNDS` and `MAX_SECONDS`. It must not run forever, and must not delete large data outside the workspace. It commits successful rounds by default and pushes only when `LGO_AUTOPILOT_PUSH=1`.

The bypass flag is the default for this local supervisor because Linh Giới Online validation uses localhost sockets and Unity player capture, and `workspace-write` blocks those gates in some Codex CLI hosts. Use `CODEX_BYPASS_APPROVALS_AND_SANDBOX=0 CODEX_SANDBOX=workspace-write` only for source-only batches that do not need runtime/socket gates.

## Git Checkpoints

After each successful round with status `CONTINUE`, `DONE`, or `NEED_HUMAN_VISUAL_REVIEW`, the supervisor runs:

```bash
./tools/lgo_codex_git_checkpoint.sh round-N
```

The checkpoint:

- skips commit when the worktree is clean;
- blocks commit if frozen surfaces changed;
- runs `git --no-pager diff --check`;
- creates one coherent checkpoint commit for the round;
- does not push unless `LGO_AUTOPILOT_PUSH=1`.

This keeps long autopilot runs from accumulating noisy dirty state while avoiding spam commits for every tiny edit.

## Prompt Contract

Every round instructs Codex to read:

- `AGENTS.md`
- `docs/execution/NEXT-ACTION.md`
- `docs/execution/TASK-LEDGER.md`

Then it performs one coherent batch:

```text
analyze -> implement -> integrate -> cleanup -> validate -> update evidence/report -> update NEXT-ACTION.md -> update TASK-LEDGER.md -> write status.json
```

The round is explicitly non-interactive. Codex must not request approval or escalation. If localhost sockets, Unity/player launch, video capture, or another runtime permission is blocked by the current sandbox, Codex must write `status.json` with the exact blocker and evidence path instead of waiting for user input.

The round prompt tells Codex to behave like a continuous local project agent:

- use Vietnamese for owner-facing progress, status, and docs;
- write `status.json` reason/current_task/next_action/stop_reason in Vietnamese unless preserving a machine marker or command;
- inspect logs and fix validator/runtime failures inside allowed paths before stopping;
- review visual screenshots instead of trusting source inspection;
- continue from ugly/broken screenshots when a safe fix is available;
- choose the next valid task from `NEXT-ACTION.md`, `PROJECT-STATE.md`, roadmap/backlog docs, validators, and latest evidence;
- keep going to the next valid task when valid work remains;
- advance to the next roadmap-valid phase/milestone when the current one is genuinely closed by required gates;
- stop at a phase boundary only when the next phase requires owner approval, frozen contract/protocol/schema/ADR changes, unavailable tooling, or another real blocker;
- prioritize real playable progress, UI/UX, runtime presentation, asset pipeline, performance/weight, maintainability, QA/evidence tooling, and debugging ergonomics;
- create asset requests/briefs when AI image generation is needed but unavailable in CLI, while continuing with code, compression, import settings, mapping, and validators.
- group related source/docs/tooling changes into coherent checkpoint commits after validation, and let the supervisor push only when explicitly configured.

## Boundaries

The autopilot inherits the repository frozen surfaces:

- `protocol/**`
- `gamedata/schemas/**`
- `docs/adr/**`
- `client/Unity/Assets/Game/UI/design-tokens.json`

It must not open production auth, DB, economy, social/guild/liveops, or full combat unless the roadmap explicitly allows it.
