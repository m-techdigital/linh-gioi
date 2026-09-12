#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd -P)"
STATUS_FILE="$ROOT/build/codex-autopilot/status.json"
ROUND="${1:-manual}"
PUSH="${LGO_AUTOPILOT_PUSH:-0}"

cd "$ROOT"
git_root="$(git rev-parse --show-toplevel)"
if [[ "$(cd "$git_root" && pwd -P)" != "$ROOT" ]]; then
  echo "LGO_GIT_CHECKPOINT_BLOCKED script is not at its git worktree root" >&2
  exit 3
fi

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

FROZEN=(protocol gamedata/schemas docs/adr client/Unity/Assets/Game/UI/design-tokens.json)
# Check both index and worktree before staging anything. Never unstage owner work.
frozen_changed="$(git --no-pager diff --name-only HEAD -- "${FROZEN[@]}";
  git --no-pager diff --cached --name-only -- "${FROZEN[@]}";
  git ls-files --others --exclude-standard -- "${FROZEN[@]}")"
if [[ -n "$frozen_changed" ]]; then
  echo "LGO_GIT_CHECKPOINT_BLOCKED frozen surfaces changed:" >&2
  echo "$frozen_changed" >&2
  exit 3
fi

git --no-pager diff --check
git --no-pager diff --cached --check

checkpoint_path_allowed() {
  case "$1" in
    protocol/*|gamedata/schemas/*|docs/adr/*|client/Unity/Assets/Game/UI/design-tokens.json) return 3 ;;
    build/*|client/Unity/Library/*|client/Unity/Temp/*|client/Unity/Logs/*|client/Unity/UserSettings/*|client/Unity/obj/*|client/Unity/Build/*|client/Unity/Builds/*) return 1 ;;
    *.zip|*.tar.gz|*.sha256|*.pyc|*__pycache__*|*.log|*.tmp|*.csproj|*.sln|*.user) return 1 ;;
    AGENTS.md|README.md|START-HERE.md|VERSIONING.md|.gitignore|.vscode/*|client/*|server/*|tools/*|docs/*) return 0 ;;
    *) return 1 ;;
  esac
}

# Pre-staged files must meet the same rules as unstaged candidates.
while IFS= read -r -d '' path; do
  if ! checkpoint_path_allowed "$path"; then
    echo "LGO_GIT_CHECKPOINT_BLOCKED disallowed staged path: $path" >&2
    exit 3
  fi
done < <(git diff --cached --name-only -z)

stage_checkpoint_paths() {
  local path
  while IFS= read -r -d '' path; do
    if checkpoint_path_allowed "$path"; then
      git add -- "$path"
    else
      echo "LGO_GIT_CHECKPOINT_SKIP generated_or_outside_allowlist $path"
    fi
  done < <(git ls-files --deduplicate -m -o -d -z --exclude-standard)
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
if git diff --cached --quiet; then
  echo "LGO_GIT_CHECKPOINT_SKIP no_allowlisted_changes"
else
  git --no-pager diff --cached --check
  git commit -m "$subject" -m "$body"
  echo "LGO_GIT_CHECKPOINT_COMMITTED $subject"
fi

if [[ "$PUSH" == "1" ]]; then
  branch="$(git symbolic-ref --quiet --short HEAD)" || {
    echo "LGO_GIT_CHECKPOINT_BLOCKED push requires a branch with configured upstream" >&2
    exit 3
  }
  remote="$(git config --get "branch.$branch.remote")" || remote=""
  merge_ref="$(git config --get "branch.$branch.merge")" || merge_ref=""
  if [[ -z "$remote" || "$remote" == "." || "$merge_ref" != refs/heads/* ]] || ! git check-ref-format "$merge_ref"; then
    echo "LGO_GIT_CHECKPOINT_BLOCKED push requires an explicit remote branch upstream" >&2
    exit 3
  fi
  # Read the actual destination, including when this local branch has a different name.
  git fetch --no-tags "$remote" "$merge_ref"
  remote_tip="$(git rev-parse FETCH_HEAD)"
  if ! git merge-base --is-ancestor "$remote_tip" HEAD; then
    echo "LGO_GIT_CHECKPOINT_BLOCKED upstream diverged; integrate before checkpoint push" >&2
    exit 3
  fi
  frozen_commits="$(git log --format= --name-only "$remote_tip..HEAD" -- "${FROZEN[@]}")"
  if [[ -n "$frozen_commits" ]]; then
    echo "LGO_GIT_CHECKPOINT_BLOCKED outgoing commits touch frozen surfaces:" >&2
    echo "$frozen_commits" >&2
    exit 3
  fi
  if [[ "$remote_tip" == "$(git rev-parse HEAD)" ]]; then
    echo "LGO_GIT_CHECKPOINT_PUSH_SKIPPED already_up_to_date"
  else
    git push "$remote" "HEAD:$merge_ref"
    echo "LGO_GIT_CHECKPOINT_PUSHED $remote $merge_ref"
  fi
else
  echo "LGO_GIT_CHECKPOINT_PUSH_SKIPPED set LGO_AUTOPILOT_PUSH=1"
fi
