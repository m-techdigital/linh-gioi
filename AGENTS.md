# Linh Giới Online — Codex Continuous Work Rules

This repository uses persistent continuous-work mode. Read this file before making changes.

## Operating Loop

- Do not stop after one small task when a valid next task exists.
- In this chat, operate in full continuous-work mode within safe project boundaries: analyze, implement, integrate, clean up, validate, review evidence, update state files, commit/push safe checkpoints, then move to the next roadmap-valid task.
- If runtime/tooling is blocked but other source-safe work remains, record the blocker and evidence path, then continue with a valid task that does not depend on that blocked gate.
- After each task: validate the change, update report/evidence as needed, update `docs/execution/NEXT-ACTION.md`, then continue to the next valid task.
- Use `docs/execution/PROJECT-STATE.md`, `docs/execution/NEXT-ACTION.md`, and `docs/execution/TASK-LEDGER.md` as the handoff spine for future sessions.
- When running under autopilot, write `build/codex-autopilot/status.json` at the end of each coherent batch.
- If `docs/execution/NEXT-ACTION.md` still contains a valid next task, autopilot status must be `CONTINUE`, not `DONE`.
- Under `tools/lgo_codex_autopilot.sh`, never request approval or escalation; classify blocked local sockets, Unity/player launch, video capture, or runtime permissions in `status.json` instead of waiting for user input.
- `tools/lgo_codex_autopilot.sh` is a local-runtime supervisor and may run Codex CLI with `--dangerously-bypass-approvals-and-sandbox` so localhost sockets and Unity evidence can execute; keep its own bounded rounds/time limits and repository frozen surfaces intact.
- Autopilot should be roadmap-driven, not prompt-fragment-driven: after closing one task, select the next valid task from `NEXT-ACTION.md`, `PROJECT-STATE.md`, milestone roadmap/backlog docs, validators, and latest evidence, then continue within allowed scope.
- Phase/milestone completion is not a stop condition by itself. If gates truly pass and the roadmap has a valid next phase, update project state and continue to that phase.
- If the next phase needs frozen contract/protocol/schema/ADR changes or a major product decision, create the needed request/decision artifact and stop only for that explicit approval gate.
- Owner-facing autopilot progress, `status.json` reason fields, handoff notes, and ledger updates should be Vietnamese so the project owner can understand what is happening during long runs.
- Prefer batches that improve the actual playable game: gameplay already allowed by roadmap, UI/UX, runtime presentation, asset pipeline, performance/weight, maintainability, QA/evidence tooling, and debugging ergonomics.
- If a gate fails, inspect logs and fix the root cause inside allowed paths before stopping. Stop only when the failure is outside allowed scope or the environment truly blocks it.
- If runtime screenshots exist, review them visually. If the UI is ugly, overlapped, blurry, too heavy, or clearly off-reference and the fix is in scope, continue improving rather than reporting success.
- If AI image generation is needed but unavailable in Codex CLI, write an asset request/brief and continue with asset mapping, compression, import settings, UI wiring, and validators that do not require image generation.
- Keep git history clean during continuous work: group related changes into one coherent checkpoint commit after validation, avoid spam commits for tiny edits, and push only through the configured supervisor path.
- Do not run validation gates in parallel when they share mutable outputs, especially visual evidence directories under `build/visual-evidence/**`; run those phases sequentially to avoid false missing-evidence failures.
- Do not commit generated caches, Unity `Library/Temp/Logs`, pycache, local toolchains, or bulky evidence artifacts unless a task explicitly owns the artifact.
- Prefer source/runtime evidence over assumptions. Never claim PASS from source inspection only.
- Never mask failures with `|| true`.
- Never claim PASS with `executed=0`.
- Keep generated outputs, Unity caches, build folders, pycache, and package artifacts out of source control unless a task explicitly owns the artifact.

## Stop Conditions

Stop only for a real blocker, required frozen contract change, required owner/product decision, unavailable runtime/tooling, unsafe destructive operation, or no valid next action.

Do not stop merely because a task, phase, evidence refresh, or commit just completed. Continue to the next valid action unless every remaining action is blocked or unsafe.

When stopping, update `docs/execution/NEXT-ACTION.md` with the blocker, exact gate, evidence path, and the next allowed action.

Allowed autopilot status values:

- `CONTINUE`
- `BLOCKED`
- `NEED_OWNER_DECISION`
- `NEED_HUMAN_VISUAL_REVIEW`
- `FIX_REQUIRED`
- `DONE`

## Frozen Surfaces

Do not change these without explicit approval and a contract-change task:

- `protocol/**`
- `gamedata/schemas/**`
- `docs/adr/**`
- `client/Unity/Assets/Game/UI/design-tokens.json`

## Runtime / Visual Rules

- `./tools/lgo_visual_runtime_review.sh` is the visual evidence command.
- Do not use `-nographics` for player visual evidence.
- Build/capture success is not `VISUAL_RUNTIME_PASS`; screenshots must be reviewed.
- If capture fails, classify honestly as `FIX_REQUIRED`, `VISUAL_CAPTURE_TIMEOUT`, `VIDEO_CAPTURE_BLOCKED_ENV`, or `RUNTIME_BLOCKED_ENV`.
