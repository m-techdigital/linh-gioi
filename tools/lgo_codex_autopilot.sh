#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
OUT_DIR="$ROOT/build/codex-autopilot"
LOG="$OUT_DIR/autopilot.log"
MAX_ROUNDS="${MAX_ROUNDS:-5}"
MAX_SECONDS="${MAX_SECONDS:-7200}"
CODEX_SANDBOX="${CODEX_SANDBOX:-danger-full-access}"
CODEX_BYPASS_APPROVALS_AND_SANDBOX="${CODEX_BYPASS_APPROVALS_AND_SANDBOX:-1}"
CODEX_RESUME_LAST="${CODEX_RESUME_LAST:-0}"
LGO_AUTOPILOT_COMMIT="${LGO_AUTOPILOT_COMMIT:-1}"
DRY_RUN=0

case "$CODEX_SANDBOX" in
  seatbelt)
    CODEX_SANDBOX="workspace-write"
    ;;
esac

usage() {
  cat <<'EOF'
Usage: tools/lgo_codex_autopilot.sh [--dry-run]

Runs a bounded Codex CLI supervisor loop for Linh Giới Online.

Environment:
  MAX_ROUNDS   default 5
  MAX_SECONDS  default 7200
  CODEX_SANDBOX default danger-full-access
  CODEX_BYPASS_APPROVALS_AND_SANDBOX default 1
  CODEX_RESUME_LAST default 0
  LGO_AUTOPILOT_COMMIT default 1
  LGO_AUTOPILOT_PUSH default 0
EOF
}

if [[ "${1:-}" == "--help" ]]; then
  usage
  exit 0
fi

if [[ "${1:-}" == "--dry-run" ]]; then
  DRY_RUN=1
elif [[ $# -gt 0 ]]; then
  usage >&2
  exit 2
fi

mkdir -p "$OUT_DIR"
cd "$ROOT"
test "$(basename "$PWD")" = "LinhGioiOnline"

write_prompt() {
  local round="$1"
  local prompt_file="$OUT_DIR/prompt-round-$round.txt"
  cat > "$prompt_file" <<'EOF'
# Linh Giới Online — Autopilot Round

Read first:

- AGENTS.md
- docs/execution/PROJECT-STATE.md
- docs/execution/NEXT-ACTION.md
- docs/execution/TASK-LEDGER.md
- docs/execution/CODEX-CONTINUOUS-WORKFLOW.md
- docs/execution/CODEX-AUTOPILOT.md

Continue Linh Giới Online development.

Use Vietnamese for owner-facing progress summaries, docs/status reasons, handoff notes, task ledger updates, and the final round summary. When writing `status.json`, use Vietnamese for `reason`, `current_task`, `next_action`, and `stop_reason` unless a machine-readable marker or command must stay unchanged. Keep code identifiers, commands, validator markers, and generated protocol/API names in their existing language.

Do not stop after one small task if a valid next task exists. Work like a senior local project agent, not like a one-command wrapper.

Complete one coherent batch:
analyze -> implement -> integrate -> cleanup -> validate -> update evidence/report -> update NEXT-ACTION.md -> update TASK-LEDGER.md -> write build/codex-autopilot/status.json.

This is a non-interactive autopilot round. Do not request approval or escalation. If a command cannot run because the sandbox blocks local sockets, Unity/player launch, video capture, or another runtime permission, classify it honestly in status.json instead of waiting for user input.

Within the round, keep going through this inner loop while it remains safe and in scope:

1. Inspect current project state, latest status.json, latest logs, and visual evidence.
2. Identify the next smallest valuable task from NEXT-ACTION.md and the roadmap.
3. Implement the fix or improvement.
4. Run targeted validation.
5. If validation fails, inspect the exact log and fix the cause when it is inside allowed paths.
6. If visual evidence exists, open/review screenshots for layout, scale, spacing, readability, sharpness, hierarchy, and reference similarity.
7. If screenshots are ugly, broken, overlapped, blurry, too heavy, or clearly off-reference but the fix is in scope, continue improving instead of claiming success.
8. Update docs/evidence/status only after the batch has real evidence.

Autonomous project scope:

- Develop Linh Giới Online continuously from the current authoritative source, roadmap, project state, and task ledger.
- Do not limit yourself to only the small task named in the previous prompt when a higher-value valid next task exists.
- Pick the next valid task by reading NEXT-ACTION.md, PROJECT-STATE.md, MILESTONE-ROADMAP.md, roadmap/backlog docs, task docs, validators, and latest evidence.
- Prefer coherent value batches that move the actual playable game forward: UI/UX, gameplay presentation, runtime evidence, maintainability, asset pipeline, performance/weight, QA tooling, debugging ergonomics, and docs/handoff needed for operation.
- Finish the current task cleanly before opening the next one, then update NEXT-ACTION.md and continue if valid work remains.
- If the current task/phase is truly complete and its required source/runtime/visual gates have passed, advance to the next valid phase or milestone from the roadmap instead of stopping. Do not treat phase boundaries as blockers by themselves.
- If the next phase requires a frozen contract, protocol/schema/ADR change, or major product decision, create/update the appropriate contract-change/request/decision document and stop only when owner approval is genuinely required.
- If the next phase is allowed by roadmap and does not require frozen-surface changes, start it in the next coherent batch.
- Create missing task docs/validators/reports when doing so improves long-term operation and does not open forbidden product scope.

Git hygiene:

- Keep changes grouped by coherent batch, not by tiny edit.
- Before finishing a successful batch, remove generated caches and avoid staging Unity Library/Temp/Logs, build caches, pycache, toolchain archives, or evidence blobs unless a task explicitly owns them.
- Do not create many small commits for the same logical task.
- Do not push manually from inside the round; the supervisor handles push only when `LGO_AUTOPILOT_PUSH=1`.
- If the worktree contains unrelated user changes that you did not make, leave them intact and explain the grouping in TASK-LEDGER.md; do not revert them.

Current priority order:

1. keep the game runnable and verifiable;
2. gameplay and real player experience already allowed by roadmap;
3. UI/UX and visual runtime quality;
4. visual screenshot/video evidence and self-review;
5. asset pipeline quality, compression, import settings, and cleanup;
6. PC/tablet/mobile UI profiles and asset weight budgets;
7. architecture/code reuse/maintainability/debuggability;
8. tests, validators, package hygiene, release/evidence automation;
9. docs/roadmap/task handoff only when needed to keep development moving.

Image/asset policy:

- Do not crop or slice composite/reference sheets.
- Do not claim V1/V2/V3BA assets as final production art.
- Prefer V3B-aligned runtime candidates when available.
- If new AI image generation is needed and no image generation tool is available in the CLI environment, create or update an asset request/brief and continue with code, mapping, optimization, import settings, validation, and UI integration that can proceed safely.
- Optimize runtime assets for their actual use: logo/background/button/icon/HUD sizes must be fit-for-purpose, not unnecessarily huge.

Do not open:

- production auth;
- production DB;
- economy;
- social/guild/liveops;
- full combat beyond the currently approved contract/roadmap phase;

unless roadmap explicitly allows.

Do not modify frozen surfaces unless there is an explicit approved contract-change task:

- protocol/**
- gamedata/schemas/**
- docs/adr/**
- client/Unity/Assets/Game/UI/design-tokens.json

Stop only for:

- real blocker;
- frozen contract change;
- required owner decision;
- unavailable runtime/tooling;
- no valid next action.

Do not stop merely because a milestone or phase finished. Close it with evidence, update PROJECT-STATE.md / NEXT-ACTION.md / TASK-LEDGER.md, choose the next roadmap-valid phase, and continue.

If validation reaches a local runtime/socket gate and the sandbox returns PermissionError, Operation not permitted, or another environment permission failure, write status FIX_REQUIRED or BLOCKED with the exact stop_reason and evidence log path. Never leave the round waiting at an approval prompt.

Do not write DONE merely because this round completed one task. If NEXT-ACTION.md still contains a valid next task, write CONTINUE.

Status file must be valid JSON at build/codex-autopilot/status.json and include:

- status
- phase
- current_task
- next_action
- reason
- last_validation
- stop_reason

At the end, write status.json with one allowed status:
CONTINUE, BLOCKED, NEED_OWNER_DECISION, NEED_HUMAN_VISUAL_REVIEW, FIX_REQUIRED, DONE.
EOF
  echo "$prompt_file"
}

read_status() {
  python3.12 - "$OUT_DIR/status.json" <<'PY'
import json
import sys
from pathlib import Path

path = Path(sys.argv[1])
if not path.exists():
    raise SystemExit("FIX_REQUIRED missing build/codex-autopilot/status.json")
data = json.loads(path.read_text(encoding="utf-8"))
status = data.get("status")
allowed = {
    "CONTINUE",
    "BLOCKED",
    "NEED_OWNER_DECISION",
    "NEED_HUMAN_VISUAL_REVIEW",
    "FIX_REQUIRED",
    "DONE",
}
if status not in allowed:
    raise SystemExit(f"FIX_REQUIRED invalid status: {status!r}")
print(status)
PY
}

run_loop() {
  local start now elapsed round prompt_file status
  start="$(date +%s)"

  echo "LGO_CODEX_AUTOPILOT_START $(date -u +%Y-%m-%dT%H:%M:%SZ)"
  echo "LGO_CODEX_AUTOPILOT_ROOT $ROOT"
  echo "LGO_CODEX_AUTOPILOT_LIMITS rounds=$MAX_ROUNDS seconds=$MAX_SECONDS sandbox=$CODEX_SANDBOX bypass=$CODEX_BYPASS_APPROVALS_AND_SANDBOX resume_last=$CODEX_RESUME_LAST autocommit=$LGO_AUTOPILOT_COMMIT push=${LGO_AUTOPILOT_PUSH:-0}"

  if [[ "$DRY_RUN" -eq 1 ]]; then
    prompt_file="$(write_prompt 1)"
    "$ROOT/tools/lgo_codex_write_status.sh" \
      CONTINUE \
      AUTOPILOT_DRY_RUN \
      "validate autopilot supervisor wiring" \
      "run MAX_ROUNDS=1 tools/lgo_codex_autopilot.sh when codex CLI is available" \
      "dry run generated prompt and status successfully" \
      null
    echo "LGO_CODEX_AUTOPILOT_DRY_RUN prompt=$prompt_file status=$OUT_DIR/status.json"
    echo "LGO_CODEX_AUTOPILOT_RESULT CONTINUE"
    return 0
  fi

  if ! command -v codex >/dev/null 2>&1; then
    "$ROOT/tools/lgo_codex_write_status.sh" \
      FIX_REQUIRED \
      AUTOPILOT_SUPERVISOR \
      "start Codex CLI supervisor" \
      "install or expose codex CLI in PATH, then rerun autopilot" \
      "codex command unavailable" \
      "codex command unavailable"
    echo "FIX_REQUIRED codex command unavailable" >&2
    return 127
  fi

  for ((round = 1; round <= MAX_ROUNDS; round += 1)); do
    now="$(date +%s)"
    elapsed=$((now - start))
    if (( elapsed >= MAX_SECONDS )); then
      "$ROOT/tools/lgo_codex_write_status.sh" \
        BLOCKED \
        AUTOPILOT_SUPERVISOR \
        "continue governed development" \
        "rerun with a larger MAX_SECONDS only after reviewing logs" \
        "MAX_SECONDS reached" \
        "MAX_SECONDS reached"
      echo "BLOCKED MAX_SECONDS reached"
      return 124
    fi

    prompt_file="$(write_prompt "$round")"
    echo "LGO_CODEX_AUTOPILOT_ROUND_START $round prompt=$prompt_file"

    set +e
    local codex_round_log="$OUT_DIR/codex-round-$round.log"
    local codex_prompt
    codex_prompt="$(cat "$prompt_file")"
    if [[ "$CODEX_BYPASS_APPROVALS_AND_SANDBOX" == "1" ]]; then
      if [[ "$CODEX_RESUME_LAST" == "1" ]]; then
        codex exec resume --last --dangerously-bypass-approvals-and-sandbox -C "$ROOT" "$codex_prompt" 2>&1 | tee "$codex_round_log"
      else
        codex exec --dangerously-bypass-approvals-and-sandbox -C "$ROOT" "$codex_prompt" 2>&1 | tee "$codex_round_log"
      fi
    else
      if [[ "$CODEX_RESUME_LAST" == "1" ]]; then
        codex exec resume --last --sandbox "$CODEX_SANDBOX" -C "$ROOT" "$codex_prompt" 2>&1 | tee "$codex_round_log"
      else
        codex exec --sandbox "$CODEX_SANDBOX" -C "$ROOT" "$codex_prompt" 2>&1 | tee "$codex_round_log"
      fi
    fi
    local codex_status="$?"
    set -e

    if [[ "$codex_status" -ne 0 ]]; then
      if grep -q "failed to initialize in-process app-server client: Operation not permitted" "$codex_round_log"; then
        "$ROOT/tools/lgo_codex_write_status.sh" \
          BLOCKED \
          AUTOPILOT_SUPERVISOR \
          "codex exec round $round" \
          "rerun autopilot from an environment where codex exec can initialize its in-process app-server client" \
          "Codex CLI startup blocked by environment permission gate; see $codex_round_log" \
          "failed to initialize in-process app-server client: Operation not permitted"
        echo "BLOCKED codex exec environment permission gate"
        return "$codex_status"
      fi
      "$ROOT/tools/lgo_codex_write_status.sh" \
        FIX_REQUIRED \
        AUTOPILOT_SUPERVISOR \
        "codex exec round $round" \
        "inspect $LOG and rerun after fixing the failing batch" \
        "codex exec exited non-zero" \
        "codex exec exited with code $codex_status"
      echo "FIX_REQUIRED codex exec exited with code $codex_status"
      return "$codex_status"
    fi

    status="$(read_status)"
    echo "LGO_CODEX_AUTOPILOT_ROUND_STATUS $round $status"
    if [[ "$LGO_AUTOPILOT_COMMIT" == "1" ]]; then
      "$ROOT/tools/lgo_codex_git_checkpoint.sh" "round-$round"
    fi
    if [[ "$status" == "CONTINUE" ]]; then
      continue
    fi

    python3.12 - "$OUT_DIR/status.json" <<'PY'
import json
import sys
from pathlib import Path

data = json.loads(Path(sys.argv[1]).read_text(encoding="utf-8"))
print("LGO_CODEX_AUTOPILOT_STOP_REASON", data.get("stop_reason") or data.get("reason") or data.get("status"))
PY
    echo "LGO_CODEX_AUTOPILOT_RESULT $status"
    return 0
  done

  status="$(read_status)"
  echo "LGO_CODEX_AUTOPILOT_MAX_ROUNDS_REACHED status=$status"
  echo "LGO_CODEX_AUTOPILOT_RESULT $status"
}

run_loop 2>&1 | tee "$LOG"
