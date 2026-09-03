#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
PLAYER_ARCHIVE=""
OUT_DIR="$ROOT/build/unity-player-smoke-sandbox"
HOST="127.0.0.1"
PORT="17791"
TIMEOUT_MS="15000"
SERVER_PID=""

usage() {
  cat <<'USAGE'
Usage:
  ./tools/unity_player_evidence/run_unity_player_smoke.sh --player-archive FILE [--output-dir DIR] [--port PORT]

Runs a Linux Unity player smoke executable against the real Java realtime server.
Requires Java 25/Maven server runtime already installed and source server built.
USAGE
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --player-archive) PLAYER_ARCHIVE="$2"; shift 2 ;;
    --output-dir) OUT_DIR="$2"; shift 2 ;;
    --host) HOST="$2"; shift 2 ;;
    --port) PORT="$2"; shift 2 ;;
    --timeout-ms) TIMEOUT_MS="$2"; shift 2 ;;
    --help|-h) usage; exit 0 ;;
    *) echo "ERROR: unknown argument: $1" >&2; usage >&2; exit 2 ;;
  esac
done

if [[ -z "$PLAYER_ARCHIVE" || ! -f "$PLAYER_ARCHIVE" ]]; then
  echo "ERROR: --player-archive must point to a file." >&2
  exit 2
fi

validate_port() {
  local value="$1"
  if [[ ! "$value" =~ ^[0-9]+$ ]] || (( value < 1 || value > 65535 )); then
    echo "ERROR: port must be an integer between 1 and 65535, got $value" >&2
    exit 2
  fi
}
validate_port "$PORT"

process_alive() {
  local pid="$1"
  kill -0 "$pid" 2>/dev/null
}

cleanup() {
  local rc=$?
  local wait_rc=0
  trap - EXIT
  if [[ -n "$SERVER_PID" ]] && process_alive "$SERVER_PID"; then
    kill -TERM "$SERVER_PID"
    if wait "$SERVER_PID"; then
      wait_rc=0
    else
      wait_rc=$?
    fi
    if [[ "$wait_rc" -ne 0 && "$wait_rc" -ne 143 ]]; then
      echo "WARN: realtime server exited during cleanup with status $wait_rc" >&2
    fi
  fi
  exit "$rc"
}
trap cleanup EXIT

mkdir -p "$OUT_DIR"
EXTRACT_DIR="$OUT_DIR/player"
SERVER_LOG="$OUT_DIR/realtime-server.log"
PLAYER_LOG="$OUT_DIR/unity-player.log"
RESULT_JSON="$OUT_DIR/unity-player-smoke-result.json"
rm -rf "$EXTRACT_DIR"
mkdir -p "$EXTRACT_DIR"

tar -xzf "$PLAYER_ARCHIVE" -C "$EXTRACT_DIR"
find "$EXTRACT_DIR" -name '._*' -delete
PLAYER_EXE="$(find "$EXTRACT_DIR" -maxdepth 3 -type f -perm -111 -name 'LinhGioiM0PlayerSmoke.x86_64' ! -name '._*' | sort | head -n 1)"
if [[ -z "$PLAYER_EXE" ]]; then
  echo "ERROR: Unity player executable not found in archive." >&2
  find "$EXTRACT_DIR" -maxdepth 3 -type f ! -name '._*' | sort >&2
  exit 3
fi
chmod +x "$PLAYER_EXE"

cd "$ROOT/server"
./scripts/require-java-25.sh
if [[ ! -f realtime/target/server-realtime-0.1.0-SNAPSHOT-runtime.jar ]]; then
  ./build.sh
fi
LG_REALTIME_HOST="$HOST" LG_REALTIME_PORT="$PORT" ./run-realtime.sh >"$SERVER_LOG" 2>&1 &
SERVER_PID=$!

python3 - "$HOST" "$PORT" "$SERVER_PID" <<'PY'
import os, socket, sys, time
host=sys.argv[1]
port=int(sys.argv[2])
pid=int(sys.argv[3])
for _ in range(100):
    try:
        os.kill(pid, 0)
    except OSError:
        raise SystemExit('realtime server exited before port was reachable')
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as sock:
        sock.settimeout(0.2)
        try:
            sock.connect((host, port))
            raise SystemExit(0)
        except OSError:
            time.sleep(0.1)
raise SystemExit('realtime server port did not become reachable')
PY

cd "$ROOT"
PLAYER_ARGS=(
  -batchmode
  -nosound
  --lgo-player-smoke
  --lgo-server-host "$HOST"
  --lgo-server-port "$PORT"
  --lgo-timeout-ms "$TIMEOUT_MS"
  --lgo-smoke-result "$RESULT_JSON"
  -logFile "$PLAYER_LOG"
)
if command -v xvfb-run >/dev/null 2>&1; then
  TERM="${TERM:-xterm}" xvfb-run -a "$PLAYER_EXE" "${PLAYER_ARGS[@]}"
else
  "$PLAYER_EXE" -nographics "${PLAYER_ARGS[@]}"
fi

python3 - "$RESULT_JSON" <<'PY'
import json, sys
path=sys.argv[1]
with open(path, encoding='utf-8') as f:
    data=json.load(f)
if data.get('status') != 'PASS':
    raise SystemExit(f"Unity player smoke did not pass: {data}")
if data.get('accepted') is not True:
    raise SystemExit(f"Unity player smoke was not accepted: {data}")
print('UNITY_PLAYER_SMOKE_PASS', json.dumps(data, sort_keys=True))
PY

kill -TERM "$SERVER_PID"
if wait "$SERVER_PID"; then
  server_wait_rc=0
else
  server_wait_rc=$?
fi
if [[ "$server_wait_rc" -ne 0 && "$server_wait_rc" -ne 143 ]]; then
  echo "ERROR: realtime server exited with unexpected status $server_wait_rc" >&2
  exit 7
fi
SERVER_PID=""

grep -Eq 'event="?realtime_started"?' "$SERVER_LOG"
grep -Eq 'event="?realtime_handshake_accepted"?' "$SERVER_LOG"
grep -Eq 'event="?realtime_stopped"?' "$SERVER_LOG"

echo "UNITY_PLAYER_TO_JAVA_HANDSHAKE_PASS output=$OUT_DIR"
