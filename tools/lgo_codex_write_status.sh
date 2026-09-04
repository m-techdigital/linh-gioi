#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
OUT_DIR="$ROOT/build/codex-autopilot"
STATUS="${1:-CONTINUE}"
PHASE="${2:-VISUAL_ASSET_RUNTIME}"
CURRENT_TASK="${3:-improve login/world visual runtime quality}"
NEXT_ACTION="${4:-continue next valid task from docs/execution/NEXT-ACTION.md}"
REASON="${5:-valid next task exists}"
STOP_REASON="${6:-null}"
shift $(( $# < 6 ? $# : 6 ))

case "$STATUS" in
  CONTINUE|BLOCKED|NEED_OWNER_DECISION|NEED_HUMAN_VISUAL_REVIEW|FIX_REQUIRED|DONE) ;;
  *)
    echo "FIX_REQUIRED invalid autopilot status: $STATUS" >&2
    exit 2
    ;;
esac

mkdir -p "$OUT_DIR"
VALIDATION_JSON="$(python3.12 - "$@" <<'PY'
import json
import sys

validations = sys.argv[1:] or [
    "git --no-pager diff --check",
    "./tools/lgo_continue_dev_loop.sh",
]
print(json.dumps(validations, ensure_ascii=False))
PY
)"

python3.12 - "$OUT_DIR/status.json" "$STATUS" "$PHASE" "$CURRENT_TASK" "$NEXT_ACTION" "$REASON" "$STOP_REASON" "$VALIDATION_JSON" <<'PY'
import json
import sys
from pathlib import Path

path = Path(sys.argv[1])
status, phase, current_task, next_action, reason, stop_reason = sys.argv[2:8]
last_validation = json.loads(sys.argv[8])
payload = {
    "status": status,
    "phase": phase,
    "current_task": current_task,
    "next_action": next_action,
    "reason": reason,
    "last_validation": last_validation,
    "stop_reason": None if stop_reason == "null" else stop_reason,
}
path.write_text(json.dumps(payload, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
print(path)
PY
