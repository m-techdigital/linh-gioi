#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
STATUS_FILE="$ROOT/build/codex-autopilot/status.json"
ROUND="${1:-manual}"
PUSH="${LGO_AUTOPILOT_PUSH:-0}"

cd "$ROOT"
test "$(basename "$PWD")" = "LinhGioiOnline"

if [[ ! -f "$STATUS_FILE" ]]; then
  echo "LGO_GIT_CHECKPOINT_SKIP missing status.json"
  exit 0
fi

status="$(python3.12 - "$STATUS_FILE" <<'PY'
import json
import sys
from pathlib import Path
data = json.loads(Path(sys.argv[1]).read_text(encoding="utf-8"))
print(data.get("status", ""))
PY
)"

case "$status" in
  CONTINUE|DONE|NEED_HUMAN_VISUAL_REVIEW) ;;
  *)
    echo "LGO_GIT_CHECKPOINT_SKIP status=$status"
    exit 0
    ;;
esac

if [[ -z "$(git --no-pager status --short --untracked-files=all)" ]]; then
  echo "LGO_GIT_CHECKPOINT_SKIP clean_worktree"
  exit 0
fi

frozen_changed="$(git --no-pager diff --name-only -- protocol gamedata/schemas docs/adr client/Unity/Assets/Game/UI/design-tokens.json)"
if [[ -n "$frozen_changed" ]]; then
  echo "LGO_GIT_CHECKPOINT_BLOCKED frozen surfaces changed:" >&2
  echo "$frozen_changed" >&2
  exit 3
fi

git --no-pager diff --check

stage_checkpoint_paths() {
  local staged_count=0
  while IFS= read -r path; do
    [[ -z "$path" ]] && continue
    case "$path" in
      protocol/*|gamedata/schemas/*|docs/adr/*|client/Unity/Assets/Game/UI/design-tokens.json)
        echo "LGO_GIT_CHECKPOINT_BLOCKED frozen path in checkpoint candidate: $path" >&2
        return 3
        ;;
      build/*|client/Unity/Library/*|client/Unity/Temp/*|client/Unity/Logs/*|client/Unity/UserSettings/*|client/Unity/obj/*|client/Unity/Build/*|client/Unity/Builds/*)
        echo "LGO_GIT_CHECKPOINT_SKIP generated_or_local $path"
        continue
        ;;
      *.zip|*.tar.gz|*.sha256|*.pyc|*__pycache__*|*.log|*.tmp|*.csproj|*.sln|*.user)
        echo "LGO_GIT_CHECKPOINT_SKIP generated_or_local $path"
        continue
        ;;
      AGENTS.md|README.md|START-HERE.md|VERSIONING.md|.gitignore|.vscode/*|client/*|server/*|tools/*|docs/*)
        git add -- "$path"
        staged_count=$((staged_count + 1))
        ;;
      *)
        echo "LGO_GIT_CHECKPOINT_SKIP outside_allowlist $path"
        ;;
    esac
  done < <(git ls-files -m -o -d --exclude-standard)

  if [[ "$staged_count" -eq 0 ]]; then
    echo "LGO_GIT_CHECKPOINT_SKIP no_allowlisted_changes"
    exit 0
  fi

  local staged_frozen
  staged_frozen="$(git --no-pager diff --cached --name-only -- protocol gamedata/schemas docs/adr client/Unity/Assets/Game/UI/design-tokens.json)"
  if [[ -n "$staged_frozen" ]]; then
    echo "LGO_GIT_CHECKPOINT_BLOCKED staged frozen surfaces:" >&2
    echo "$staged_frozen" >&2
    git restore --staged -- protocol gamedata/schemas docs/adr client/Unity/Assets/Game/UI/design-tokens.json
    exit 3
  fi
}

subject="$(python3.12 - "$STATUS_FILE" "$ROUND" <<'PY'
import json
import re
import sys
from pathlib import Path

data = json.loads(Path(sys.argv[1]).read_text(encoding="utf-8"))
round_id = sys.argv[2]
task = data.get("current_task") or data.get("phase") or "autopilot batch"
task = re.sub(r"\s+", " ", str(task)).strip()
task = task[:72].strip(" -:")
print(f"chore: lgo autopilot checkpoint {round_id} - {task}")
PY
)"

body="$(python3.12 - "$STATUS_FILE" <<'PY'
import json
import sys
from pathlib import Path

data = json.loads(Path(sys.argv[1]).read_text(encoding="utf-8"))
print("Status: " + str(data.get("status", "")))
print("Phase: " + str(data.get("phase", "")))
print("Next action: " + str(data.get("next_action", "")))
print("Reason: " + str(data.get("reason", "")))
validations = data.get("last_validation") or []
if validations:
    print("")
    print("Validation:")
    for item in validations:
        print("- " + str(item))
PY
)"

stage_checkpoint_paths
git commit -m "$subject" -m "$body"
echo "LGO_GIT_CHECKPOINT_COMMITTED $subject"

if [[ "$PUSH" == "1" ]]; then
  if [[ -z "$(git remote)" ]]; then
    echo "LGO_GIT_CHECKPOINT_PUSH_SKIPPED no_remote"
  else
    git push
    echo "LGO_GIT_CHECKPOINT_PUSHED"
  fi
else
  echo "LGO_GIT_CHECKPOINT_PUSH_SKIPPED set LGO_AUTOPILOT_PUSH=1"
fi
