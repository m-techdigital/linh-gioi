#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
MODE=""
PORT="18083"
OUT_DIR="$ROOT/build/manual-ui"
PLAYER_APP="$ROOT/build/unity-player-macos/LinhGioiOnline.app"
PLAYER_EXE="$PLAYER_APP/Contents/MacOS/Unity"
API_PID_FILE="$OUT_DIR/api.pid"
SUMMARY_JSON="$OUT_DIR/visible-ui-review-summary.json"

usage() {
  cat <<'USAGE'
Usage:
  ./tools/run_m4_visible_ui_review.sh --rebuild
  ./tools/run_m4_visible_ui_review.sh --open-existing
  ./tools/run_m4_visible_ui_review.sh --stop
USAGE
}

if [[ $# -ne 1 ]]; then
  usage >&2
  exit 2
fi

case "$1" in
  --rebuild|--open-existing|--stop) MODE="$1" ;;
  --help|-h) usage; exit 0 ;;
  *) echo "ERROR: unknown mode: $1" >&2; usage >&2; exit 2 ;;
esac

cd "$ROOT"
if [[ "$(basename "$PWD")" != "LinhGioiOnline" ]]; then
  echo "ERROR: must run from repo root LinhGioiOnline" >&2
  exit 2
fi

mkdir -p "$OUT_DIR"

write_summary() {
  local status="$1"
  local screenshot_status="$2"
  local screenshot_reason="$3"
  python3.12 - "$SUMMARY_JSON" "$status" "$screenshot_status" "$screenshot_reason" "$PLAYER_EXE" "$PORT" <<'PY'
from __future__ import annotations
import json
import sys
from datetime import datetime, timezone
from pathlib import Path

path = Path(sys.argv[1])
payload = {
    "project": "linh-gioi-online",
    "task": "M4 visible UI review",
    "version": "0.14.0",
    "reviewWindow": {"width": 1280, "height": 720, "fullscreen": False},
    "reviewStates": ["login/gate entry", "character hall after login", "world HUD after enter world"],
    "layoutSanity": {
        "rootBoundsTarget": "1280x720",
        "mainPanelMaxWidth": 960,
        "criticalActions": ["Open Gate", "Create", "Enter World", "Save Position", "Back to Lobby", "Quit"],
        "exitAffordances": ["Quit button", "Escape key"],
    },
    "status": sys.argv[2],
    "screenshotStatus": sys.argv[3],
    "screenshotReason": sys.argv[4],
    "playerExecutable": sys.argv[5],
    "apiPort": sys.argv[6],
    "timestampUtc": datetime.now(timezone.utc).isoformat(),
}
path.parent.mkdir(parents=True, exist_ok=True)
path.write_text(json.dumps(payload, indent=2, sort_keys=True) + "\n", encoding="utf-8")
PY
}

attempt_screenshot() {
  local screenshot="$OUT_DIR/m4-visible-ui-login.png"
  if ! command -v screencapture >/dev/null 2>&1; then
    echo "VISIBLE_UI_SCREENSHOT_UNAVAILABLE reason=screencapture command not available"
    write_summary "REVIEW_WINDOW_OPENED" "VISIBLE_UI_SCREENSHOT_UNAVAILABLE" "screencapture command not available"
    return
  fi
  set +e
  screencapture -x "$screenshot" > "$OUT_DIR/screencapture.log" 2>&1
  local rc=$?
  set -e
  if [[ "$rc" -eq 0 && -s "$screenshot" ]]; then
    echo "VISIBLE_UI_SCREENSHOT_CAPTURED path=$screenshot"
    write_summary "REVIEW_WINDOW_OPENED" "VISIBLE_UI_SCREENSHOT_CAPTURED" "$screenshot"
  else
    local reason
    reason="$(tr '\n' ' ' < "$OUT_DIR/screencapture.log" | sed 's/[[:space:]]\\+/ /g')"
    echo "VISIBLE_UI_SCREENSHOT_UNAVAILABLE reason=$reason"
    write_summary "REVIEW_WINDOW_OPENED" "VISIBLE_UI_SCREENSHOT_UNAVAILABLE" "$reason"
  fi
}

stop_running() {
  if [[ -f "$API_PID_FILE" ]]; then
    local pid
    pid="$(cat "$API_PID_FILE")"
    if [[ "$pid" =~ ^[0-9]+$ ]] && kill -0 "$pid" 2>/dev/null; then
      kill "$pid"
      set +e
      wait "$pid" 2>/dev/null
      set -e
    fi
    rm -f "$API_PID_FILE"
  fi
  local listen_pids
  set +e
  listen_pids="$(lsof -tiTCP:"$PORT" -sTCP:LISTEN 2>/dev/null)"
  set -e
  while IFS= read -r pid; do
    if [[ "$pid" =~ ^[0-9]+$ ]] && kill -0 "$pid" 2>/dev/null; then
      set +e
      kill "$pid" 2>/dev/null
      set -e
    fi
  done <<< "$listen_pids"
}

if [[ "$MODE" == "--stop" ]]; then
  stop_running
  set +e
  pkill -f "$PLAYER_EXE" 2>/dev/null
  set -e
  echo "M4_VISIBLE_UI_REVIEW_STOPPED"
  exit 0
fi

if [[ -f "$ROOT/.lgo-local-env" ]]; then
  set -a
  source "$ROOT/.lgo-local-env"
  set +a
fi

PROJECT_PROTOC="$ROOT/tools/protobuf/darwin-arm64/protoc"
PROJECT_PROTOC_SHA="$ROOT/tools/protobuf/darwin-arm64/SHA256"
if [[ -x "$PROJECT_PROTOC" && -f "$PROJECT_PROTOC_SHA" ]]; then
  export PROTOC_BIN="$PROJECT_PROTOC"
  export PROTOC_SHA256
  PROTOC_SHA256="$(awk '{print $1}' "$PROJECT_PROTOC_SHA")"
fi

if [[ -z "${UNITY_EDITOR:-}" ]]; then
  for candidate in \
    "/Applications/Unity/Hub/Editor/6000.3.2f1/Unity.app/Contents/MacOS/Unity" \
    "$HOME/Applications/Unity/Hub/Editor/6000.3.2f1/Unity.app/Contents/MacOS/Unity"; do
    if [[ -x "$candidate" ]]; then
      UNITY_EDITOR="$candidate"
      export UNITY_EDITOR
      break
    fi
  done
fi

if [[ "$MODE" == "--rebuild" ]]; then
  if [[ -z "${UNITY_EDITOR:-}" ]]; then
    echo "ERROR: UNITY_EDITOR is not set and Unity 6000.3.2f1 was not found in common macOS paths." >&2
    exit 30
  fi
  if [[ ! -x "$UNITY_EDITOR" ]]; then
    echo "ERROR: UNITY_EDITOR is not executable: $UNITY_EDITOR" >&2
    exit 31
  fi
  ./tools/lgo_m4_closure_check.sh --source-only
  mkdir -p "$OUT_DIR"
  ./tools/prepare_unity_local_assets.sh
  python3.12 tools/prepare_unity_protocol.py --output "$ROOT/client/Unity/Assets/Game/Protocol/Generated"
  ./server/build.sh
  mkdir -p "$ROOT/build/unity-player-macos"
  "$UNITY_EDITOR" -batchmode -nographics -quit -projectPath "$ROOT/client/Unity" -executeMethod LinhGioi.Foundation.Editor.M0LinuxPlayerEvidenceBuilder.BuildMacOSPlayerSmoke --lgo-player-output "$PLAYER_APP" -logFile "$OUT_DIR/unity-build.log"
fi

if [[ ! -x "$PLAYER_EXE" ]]; then
  echo "ERROR: macOS Unity player missing. Run ./tools/run_m4_visible_ui_review.sh --rebuild first." >&2
  exit 32
fi

if [[ ! -f server/api/target/server-api-0.1.0-SNAPSHOT.jar ]]; then
  echo "ERROR: server API jar missing. Run ./tools/run_m4_visible_ui_review.sh --rebuild first." >&2
  exit 33
fi

stop_running
LG_API_HOST="127.0.0.1" LG_API_PORT="$PORT" LG_API_PERSISTENCE_DIR="$OUT_DIR/store" ./server/run-api.sh > "$OUT_DIR/api.log" 2>&1 &
echo "$!" > "$API_PID_FILE"

echo "M4_VISIBLE_UI_REVIEW_API_STARTED port=$PORT pid=$(cat "$API_PID_FILE")"
echo "M4_VISIBLE_UI_REVIEW_PLAYER=$PLAYER_EXE"
echo "M4_VISIBLE_UI_REVIEW_OPEN_WINDOWED 1280x720"

"$PLAYER_EXE" \
  -screen-fullscreen 0 \
  -screen-width 1280 \
  -screen-height 720 \
  --lgo-m4-api-url "http://127.0.0.1:$PORT" \
  > "$OUT_DIR/player.log" 2>&1 &

echo "$!" > "$OUT_DIR/player.pid"
sleep 5
attempt_screenshot
cat <<'CHECKLIST'
M4_VISIBLE_UI_MANUAL_CHECKLIST
1. Login screen: compact logo, server selector, Vào Thế Giới button, and account status are readable.
2. Character Hall: empty/list/create/select state, selected preview, and Enter World action are visible.
3. World HUD: top status strip, position/debug panel, Save Position, Back to Lobby, movement hint are visible.
4. Exit: Quit button and Escape key can close the player safely.
5. Screenshot at 1280x720 for Login, Character Hall, and World HUD.
Stop command: ./tools/run_m4_visible_ui_review.sh --stop
CHECKLIST
