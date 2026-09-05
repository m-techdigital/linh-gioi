#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PROFILE="${LGO_VISUAL_RUNTIME_PROFILE:-desktop}"
OUT_DIR="${LGO_VISUAL_RUNTIME_OUT_DIR:-$ROOT/build/visual-evidence/latest}"
PLAYER_APP="$ROOT/build/unity-player-macos/LinhGioiOnline.app"
PLAYER_EXE="$PLAYER_APP/Contents/MacOS/Unity"
PORT="${LGO_VISUAL_RUNTIME_API_PORT:-18083}"
TIMEOUT_SECONDS="${LGO_VISUAL_RUNTIME_TIMEOUT_SECONDS:-300}"
BUILD_TIMEOUT_SECONDS="${LGO_VISUAL_RUNTIME_BUILD_TIMEOUT_SECONDS:-420}"
SOURCE_GATE_MODE="${LGO_VISUAL_RUNTIME_SOURCE_GATES:-fast}"
SERVER_BUILD_MODE="${LGO_VISUAL_RUNTIME_SERVER_BUILD:-fast}"
CLEAR_UNITY_CACHE="${LGO_VISUAL_RUNTIME_CLEAR_UNITY_CACHE:-0}"
SCREEN_WIDTH="${LGO_VISUAL_RUNTIME_WIDTH:-1920}"
SCREEN_HEIGHT="${LGO_VISUAL_RUNTIME_HEIGHT:-1080}"
PLAYER_BUILD_MODE="${LGO_VISUAL_RUNTIME_PLAYER_BUILD:-build}"
API_PID=""
export PYTHONDONTWRITEBYTECODE=1

cd "$ROOT"
case "$OUT_DIR" in
  /*) ;;
  *) OUT_DIR="$ROOT/$OUT_DIR" ;;
esac
if [[ "$(basename "$PWD")" != "LinhGioiOnline" ]]; then
  echo "ERROR: must run from repo root LinhGioiOnline" >&2
  exit 2
fi

if [[ -f "$ROOT/.lgo-local-env" ]]; then
  set -a
  source "$ROOT/.lgo-local-env"
  set +a
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

if [[ -z "${UNITY_EDITOR:-}" || ! -x "$UNITY_EDITOR" ]]; then
  echo "ERROR: UNITY_EDITOR is not set or not executable." >&2
  exit 30
fi

stop_api_on_port() {
  local listen_pids
  set +e
  listen_pids="$(lsof -tiTCP:"$PORT" -sTCP:LISTEN 2>/dev/null)"
  set -e
  while IFS= read -r listen_pid; do
    if [[ "$listen_pid" =~ ^[0-9]+$ ]] && kill -0 "$listen_pid" 2>/dev/null; then
      set +e
      kill "$listen_pid" 2>/dev/null
      set -e
    fi
  done <<< "$listen_pids"
}

cleanup_runtime_processes() {
  if [[ -n "$API_PID" ]] && kill -0 "$API_PID" 2>/dev/null; then
    set +e
    kill "$API_PID" 2>/dev/null
    set -e
  fi
}

trap cleanup_runtime_processes EXIT

cleanup_outputs() {
  python3.12 - "$OUT_DIR" <<'PY'
from pathlib import Path
import shutil
import sys
out = Path(sys.argv[1])
if out.exists():
    shutil.rmtree(out)
out.mkdir(parents=True, exist_ok=True)
PY
}

wait_for_api() {
  python3.12 - "$PORT" <<'PY'
from __future__ import annotations
import sys
import time
import urllib.request
port = sys.argv[1]
url = f"http://127.0.0.1:{port}/health"
deadline = time.time() + 30
last = None
while time.time() < deadline:
    try:
        with urllib.request.urlopen(url, timeout=1) as response:
            if response.status < 500:
                sys.exit(0)
    except Exception as exc:
        last = exc
    time.sleep(0.5)
print(f"ERROR: API did not become healthy at {url}: {last}", file=sys.stderr)
sys.exit(34)
PY
}

run_source_gates() {
  case "$SOURCE_GATE_MODE" in
    full)
      echo "LGO_VISUAL_RUNTIME_REVIEW_SOURCE_GATES full"
      PYTHONDONTWRITEBYTECODE=1 ./tools/lgo_playable_closure_check.sh --source-only
      ;;
    fast)
      echo "LGO_VISUAL_RUNTIME_REVIEW_SOURCE_GATES fast"
      git --no-pager diff --check
      python3.12 tools/validate_lgo_login_gate_entry_visual_v1.py
      python3.12 tools/validate_lgo_runtime_asset_weight.py
      python3.12 tools/validate_lgo_device_profile_ui_budgets.py
      python3.12 tools/validate_m4_2_playable_ui.py
      python3.12 tools/validate_m4_visible_ui.py
      python3.12 tools/validate_m5_input_camera_polish.py
      python3.12 tools/validate_m5_world_hub_readability.py
      python3.12 tools/validate_m5_session_menu.py
      python3.12 tools/validate_m6_combat_visual_readability.py
      python3.12 tools/validate_m6_unity_combat_placeholder_asset_import.py
      python3.12 tools/validate_package_hygiene.py
      ;;
    skip)
      echo "LGO_VISUAL_RUNTIME_REVIEW_SOURCE_GATES skip"
      echo "LGO_VISUAL_RUNTIME_REVIEW_WARNING source gates skipped by LGO_VISUAL_RUNTIME_SOURCE_GATES=skip"
      ;;
    *)
      echo "ERROR: unsupported LGO_VISUAL_RUNTIME_SOURCE_GATES=$SOURCE_GATE_MODE; expected fast, full, or skip" >&2
      exit 31
      ;;
  esac
}

prepare_server_runtime() {
  case "$SERVER_BUILD_MODE" in
    full)
      echo "LGO_VISUAL_RUNTIME_REVIEW_SERVER_BUILD full"
      ./server/build.sh
      ;;
    fast)
      echo "LGO_VISUAL_RUNTIME_REVIEW_SERVER_BUILD fast"
      (
        cd "$ROOT/server"
        ./scripts/require-java-25.sh
        ./scripts/prepare-protocol.sh
        if ! command -v mvn >/dev/null 2>&1; then
          printf '%s\n' 'ERROR: Maven is required. Recommended/pinned developer version: 3.9.16.' >&2
          exit 3
        fi
        mvn -B -ntp -pl shared,api -am package -DskipTests
      )
      ;;
    skip)
      echo "LGO_VISUAL_RUNTIME_REVIEW_SERVER_BUILD skip"
      if [[ ! -f "$ROOT/server/api/target/server-api-0.1.0-SNAPSHOT.jar" ]]; then
        echo "ERROR: cannot skip server build; API jar is missing. Run with LGO_VISUAL_RUNTIME_SERVER_BUILD=fast or full." >&2
        exit 33
      fi
      ;;
    *)
      echo "ERROR: unsupported LGO_VISUAL_RUNTIME_SERVER_BUILD=$SERVER_BUILD_MODE; expected fast, full, or skip" >&2
      exit 31
      ;;
  esac
}

PROJECT_PROTOC="$ROOT/tools/protobuf/darwin-arm64/protoc"
PROJECT_PROTOC_SHA="$ROOT/tools/protobuf/darwin-arm64/SHA256"
if [[ -x "$PROJECT_PROTOC" && -f "$PROJECT_PROTOC_SHA" ]]; then
  export PROTOC_BIN="$PROJECT_PROTOC"
  export PROTOC_SHA256
  PROTOC_SHA256="$(awk '{print $1}' "$PROJECT_PROTOC_SHA")"
fi

cleanup_outputs
stop_api_on_port

echo "LGO_VISUAL_RUNTIME_REVIEW_PROFILE $PROFILE ${SCREEN_WIDTH}x${SCREEN_HEIGHT}"
echo "LGO_VISUAL_RUNTIME_REVIEW_PHASE source_gates"
run_source_gates

echo "LGO_VISUAL_RUNTIME_REVIEW_PHASE prepare_assets"
LGO_UNITY_LOCAL_ASSETS_CLEAR_CACHE="$CLEAR_UNITY_CACHE" ./tools/prepare_unity_local_assets.sh
prepare_server_runtime

echo "LGO_VISUAL_RUNTIME_REVIEW_PHASE build_player"
mkdir -p "$ROOT/build/unity-player-macos"
case "$PLAYER_BUILD_MODE" in
  build)
    python3.12 - "$BUILD_TIMEOUT_SECONDS" "$UNITY_EDITOR" "$ROOT/client/Unity" "$PLAYER_APP" "$OUT_DIR/unity-build.log" <<'PY'
from __future__ import annotations
import subprocess
import sys
from pathlib import Path

timeout = int(sys.argv[1])
unity_editor = sys.argv[2]
project_path = sys.argv[3]
player_app = sys.argv[4]
log_path = Path(sys.argv[5])
command = [
    unity_editor,
    "-batchmode",
    "-nographics",
    "-quit",
    "-projectPath",
    project_path,
    "-executeMethod",
    "LinhGioi.Foundation.Editor.M0LinuxPlayerEvidenceBuilder.BuildMacOSPlayerSmoke",
    "--lgo-player-output",
    player_app,
    "-logFile",
    str(log_path),
]
process = subprocess.Popen(command)
try:
    returncode = process.wait(timeout=timeout)
except subprocess.TimeoutExpired:
    process.kill()
    process.wait(timeout=20)
    print("RUNTIME_BLOCKED_ENV", file=sys.stderr)
    print(f"ERROR: Unity player build timed out after {timeout}s; see {log_path}", file=sys.stderr)
    sys.exit(35)
if returncode != 0:
    print("FIX_REQUIRED", file=sys.stderr)
    print(f"ERROR: Unity player build exited with code {returncode}; see {log_path}", file=sys.stderr)
    sys.exit(returncode)
PY
    ;;
  skip)
    echo "LGO_VISUAL_RUNTIME_REVIEW_PLAYER_BUILD skip"
    ;;
  *)
    echo "ERROR: unsupported LGO_VISUAL_RUNTIME_PLAYER_BUILD=$PLAYER_BUILD_MODE; expected build or skip" >&2
    exit 31
    ;;
esac

if [[ ! -x "$PLAYER_EXE" ]]; then
  echo "ERROR: Unity player missing: $PLAYER_EXE" >&2
  exit 32
fi

echo "LGO_VISUAL_RUNTIME_REVIEW_PHASE start_api"
LG_API_HOST="127.0.0.1" LG_API_PORT="$PORT" LG_API_PERSISTENCE_DIR="$OUT_DIR/api-store" ./server/run-api.sh > "$OUT_DIR/api.log" 2>&1 &
API_PID="$!"
echo "$API_PID" > "$OUT_DIR/api.pid"
wait_for_api

echo "LGO_VISUAL_RUNTIME_REVIEW_PHASE capture_player"
python3.12 - "$TIMEOUT_SECONDS" "$PLAYER_EXE" "$OUT_DIR" "$PORT" "$SCREEN_WIDTH" "$SCREEN_HEIGHT" "$PROFILE" <<'PY'
from __future__ import annotations
import json
import subprocess
import sys
import time
from pathlib import Path

timeout = int(sys.argv[1])
player = sys.argv[2]
out_dir = Path(sys.argv[3])
port = sys.argv[4]
screen_width = sys.argv[5]
screen_height = sys.argv[6]
profile = sys.argv[7]
log_path = out_dir / "player.log"
unity_log_path = out_dir / "player-unity.log"
timeout_report_path = out_dir / "visual-capture-timeout.json"
manifest_path = out_dir / "visual-runtime-evidence-manifest.json"
expected = [
    "login.png",
    "character-lobby.png",
    "character-select.png",
    "enter-world.png",
    "world-hub.png",
    "near-gatekeeper-prompt.png",
    "near-training-stone-prompt.png",
    "target-dummy-state.png",
    "npc-dialogue.png",
    "session-menu.png",
]
command = [
    player,
    "-logFile", str(unity_log_path),
    "-screen-fullscreen", "0",
    "-screen-width", screen_width,
    "-screen-height", screen_height,
    "--lgo-m4-api-url", f"http://127.0.0.1:{port}",
    "--lgo-device-profile", profile,
    "--lgo-visual-runtime-review",
    "--lgo-visual-runtime-width", screen_width,
    "--lgo-visual-runtime-height", screen_height,
    "--lgo-visual-runtime-evidence-dir", str(out_dir),
]

def tail(path: Path, limit: int = 80) -> list[str]:
    if not path.exists():
        return []
    lines = path.read_text(encoding="utf-8", errors="replace").splitlines()
    return lines[-limit:]

def capture_complete() -> bool:
    if not manifest_path.is_file():
        return False
    return all((out_dir / name).is_file() for name in expected)

def terminate_after_capture(process: subprocess.Popen[bytes]) -> int:
    if process.poll() is not None:
        return process.returncode
    process.terminate()
    try:
        return process.wait(timeout=8)
    except subprocess.TimeoutExpired:
        process.kill()
        return process.wait(timeout=8)

def request_player_focus(pid: int) -> None:
    script = (
        'tell application "System Events"\n'
        f'  set frontmost of first process whose unix id is {pid} to true\n'
        'end tell\n'
    )
    try:
        subprocess.run(["osascript", "-e", script], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL, timeout=3, check=False)
    except Exception:
        pass

started_at = time.time()
with log_path.open("w", encoding="utf-8") as log:
    process = subprocess.Popen(command, stdout=log, stderr=subprocess.STDOUT)
    print("LGO_VISUAL_RUNTIME_PLAYER_STARTED pid=" + str(process.pid), file=log, flush=True)
    print("LGO_VISUAL_RUNTIME_PLAYER_PROFILE " + profile + " " + screen_width + "x" + screen_height, file=log, flush=True)
    request_player_focus(process.pid)
    returncode = None
    deadline = started_at + timeout
    while time.time() < deadline:
        returncode = process.poll()
        if returncode is not None:
            break
        if capture_complete():
            print("LGO_VISUAL_RUNTIME_CAPTURE_COMPLETE terminating_player_after_manifest", file=log, flush=True)
            returncode = terminate_after_capture(process)
            break
        time.sleep(0.5)
    if returncode is None:
        if capture_complete():
            returncode = terminate_after_capture(process)
        else:
            process.kill()
            process.wait(timeout=10)
            timeout_report = {
                "marker": "VISUAL_CAPTURE_TIMEOUT",
                "timeout_seconds": timeout,
                "elapsed_seconds": round(time.time() - started_at, 3),
                "player": player,
                "output_dir": str(out_dir),
                "player_log": str(log_path),
                "unity_log": str(unity_log_path),
                "existing_outputs": sorted(path.name for path in out_dir.iterdir() if path.is_file()),
                "player_log_tail": tail(log_path),
                "unity_log_tail": tail(unity_log_path),
                "next_allowed_action": "Rerun with LGO_VISUAL_RUNTIME_TIMEOUT_SECONDS set higher, then inspect screenshots before claiming visual acceptance.",
            }
            timeout_report_path.write_text(json.dumps(timeout_report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
            print("VISUAL_CAPTURE_TIMEOUT", file=sys.stderr)
            print(f"ERROR: visual runtime player timed out after {timeout}s; see {log_path} and {timeout_report_path}", file=sys.stderr)
            sys.exit(41)
if returncode != 0:
    if capture_complete():
        print(f"LGO_VISUAL_RUNTIME_CAPTURE_COMPLETE_PLAYER_EXIT code={returncode}", file=sys.stderr)
    else:
        print("FIX_REQUIRED", file=sys.stderr)
        print(f"ERROR: visual runtime player exited with code {returncode}; see {log_path}", file=sys.stderr)
        sys.exit(returncode)
PY

cleanup_runtime_processes
API_PID=""

if [[ ! -f "$OUT_DIR/visual-runtime-evidence-manifest.json" ]]; then
  echo "ERROR: visual runtime evidence manifest missing: $OUT_DIR/visual-runtime-evidence-manifest.json" >&2
  exit 40
fi

python3.12 - "$OUT_DIR" <<'PY'
from __future__ import annotations
import json
import struct
import sys
from pathlib import Path

out = Path(sys.argv[1])
manifest = json.loads((out / "visual-runtime-evidence-manifest.json").read_text(encoding="utf-8"))
expected_width = int(manifest.get("width", 1920))
expected_height = int(manifest.get("height", 1080))
errors = []
for checkpoint in manifest.get("checkpoints", []):
    path = out / checkpoint["file"]
    if not path.is_file():
        errors.append(f"missing screenshot: {checkpoint['file']}")
        continue
    with path.open("rb") as handle:
        header = handle.read(24)
    if header[:8] != b"\x89PNG\r\n\x1a\n":
        errors.append(f"not a PNG: {checkpoint['file']}")
        continue
    width, height = struct.unpack(">II", header[16:24])
    if (width, height) != (expected_width, expected_height):
        errors.append(f"unexpected resolution {width}x{height}: {checkpoint['file']}")
if errors:
    print("LGO_VISUAL_RUNTIME_EVIDENCE_FAILED", file=sys.stderr)
    for error in errors:
        print(" - " + error, file=sys.stderr)
    sys.exit(1)
print("LGO_VISUAL_RUNTIME_EVIDENCE_READY output=" + str(out))
print("LGO_VISUAL_RUNTIME_PASS_NOT_CLAIMED")
PY

python3.12 tools/analyze_lgo_visual_runtime_evidence.py "$OUT_DIR"

if [[ ! -f "$OUT_DIR/visual-runtime-evidence-heuristics.json" ]]; then
  echo "ERROR: visual runtime evidence heuristics missing: $OUT_DIR/visual-runtime-evidence-heuristics.json" >&2
  exit 42
fi
