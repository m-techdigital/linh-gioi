#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"

./scripts/require-java-25.sh
for command_name in curl python3 ps; do
  if ! command -v "$command_name" >/dev/null 2>&1; then
    printf 'ERROR: runtime smoke requires %s.\n' "$command_name" >&2
    exit 5
  fi
done

API_PORT="${LG_SMOKE_API_PORT:-18080}"
REALTIME_PORT="${LG_SMOKE_REALTIME_PORT:-17777}"
LOG_DIR="${LG_SMOKE_LOG_DIR:-target/runtime-smoke}"
API_LOG="$LOG_DIR/api.log"
REALTIME_LOG="$LOG_DIR/realtime.log"
API_PID=''
REALTIME_PID=''

validate_port() {
  local label="$1"
  local value="$2"
  if [[ ! "$value" =~ ^[0-9]+$ ]] || (( value < 1 || value > 65535 )); then
    printf 'ERROR: %s must be an integer between 1 and 65535, got %s.\n' "$label" "$value" >&2
    return 1
  fi
}

assert_port_available() {
  local label="$1"
  local port="$2"
  if python3 - "$port" <<'PY'
import socket
import sys

port = int(sys.argv[1])
with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as sock:
    sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 0)
    try:
        sock.bind(("127.0.0.1", port))
    except OSError:
        raise SystemExit(1)
PY
  then
    return 0
  fi
  printf 'ERROR: %s port %s is already in use or unavailable.\n' "$label" "$port" >&2
  return 1
}

process_alive() {
  local pid="$1"
  local state
  if ! kill -0 "$pid" 2>/dev/null; then
    return 1
  fi
  state="$(ps -o stat= -p "$pid" 2>/dev/null | tr -d '[:space:]')"
  [[ -n "$state" && "$state" != Z* ]]
}

wait_for_process_exit() {
  local label="$1"
  local pid="$2"
  local max_attempts=100
  local attempt
  local wait_rc

  for ((attempt = 1; attempt <= max_attempts; attempt++)); do
    if ! process_alive "$pid"; then
      if wait "$pid"; then
        wait_rc=0
      else
        wait_rc=$?
      fi
      if [[ "$wait_rc" -eq 0 || "$wait_rc" -eq 143 ]]; then
        return 0
      fi
      printf 'ERROR: %s exited with unexpected status %s.\n' "$label" "$wait_rc" >&2
      return 1
    fi
    sleep 0.1
  done

  printf 'ERROR: %s did not terminate after SIGTERM; forcing SIGKILL.\n' "$label" >&2
  if kill -KILL "$pid" 2>/dev/null; then
    :
  fi
  if wait "$pid"; then
    wait_rc=0
  else
    wait_rc=$?
  fi
  return 1
}

terminate_process() {
  local label="$1"
  local pid="$2"
  if ! process_alive "$pid"; then
    local early_rc
    if wait "$pid"; then
      early_rc=0
    else
      early_rc=$?
    fi
    printf 'ERROR: %s exited before shutdown request with status %s.\n' "$label" "$early_rc" >&2
    return 1
  fi

  if ! kill -TERM "$pid" 2>/dev/null; then
    printf 'ERROR: failed to send SIGTERM to %s pid=%s.\n' "$label" "$pid" >&2
    return 1
  fi
  wait_for_process_exit "$label" "$pid"
}

cleanup_one() {
  local label="$1"
  local pid="$2"
  if [[ -z "$pid" ]]; then
    return 0
  fi
  if ! process_alive "$pid"; then
    if wait "$pid"; then
      return 0
    fi
    return 0
  fi
  if terminate_process "$label" "$pid"; then
    return 0
  fi
  if process_alive "$pid"; then
    if kill -KILL "$pid" 2>/dev/null; then
      :
    fi
    if wait "$pid"; then
      :
    else
      local ignored_wait_rc=$?
      printf 'WARN: cleanup reaped %s pid=%s with status %s after forced termination.\n' \
        "$label" "$pid" "$ignored_wait_rc" >&2
    fi
  fi
  return 1
}

cleanup() {
  local original_rc=$?
  local cleanup_failed=0
  trap - EXIT

  if ! cleanup_one 'api' "$API_PID"; then
    cleanup_failed=1
  fi
  if ! cleanup_one 'realtime' "$REALTIME_PID"; then
    cleanup_failed=1
  fi

  if [[ "$original_rc" -eq 0 && "$cleanup_failed" -ne 0 ]]; then
    exit 10
  fi
  exit "$original_rc"
}
trap cleanup EXIT

if ! validate_port 'LG_SMOKE_API_PORT' "$API_PORT"; then
  exit 5
fi
if ! validate_port 'LG_SMOKE_REALTIME_PORT' "$REALTIME_PORT"; then
  exit 5
fi
if [[ "$API_PORT" == "$REALTIME_PORT" ]]; then
  printf 'ERROR: API and realtime smoke ports must be different, got %s.\n' "$API_PORT" >&2
  exit 5
fi
if ! assert_port_available 'API smoke' "$API_PORT"; then
  exit 5
fi
if ! assert_port_available 'realtime smoke' "$REALTIME_PORT"; then
  exit 5
fi

mkdir -p "$LOG_DIR"
: > "$API_LOG"
: > "$REALTIME_LOG"

LG_API_HOST=127.0.0.1 LG_API_PORT="$API_PORT" ./run-api.sh >"$API_LOG" 2>&1 &
API_PID=$!
LG_REALTIME_HOST=127.0.0.1 LG_REALTIME_PORT="$REALTIME_PORT" ./run-realtime.sh >"$REALTIME_LOG" 2>&1 &
REALTIME_PID=$!

health_payload=''
for _attempt in $(seq 1 30); do
  if ! process_alive "$API_PID"; then
    printf '%s\n' 'ERROR: API process exited before /health became ready.' >&2
    cat "$API_LOG" >&2
    exit 6
  fi
  if ! process_alive "$REALTIME_PID"; then
    printf '%s\n' 'ERROR: realtime process exited during API startup.' >&2
    cat "$REALTIME_LOG" >&2
    exit 6
  fi
  if health_payload="$(curl --fail --silent --show-error "http://127.0.0.1:${API_PORT}/health" 2>/dev/null)"; then
    break
  fi
  sleep 1
done

if ! process_alive "$API_PID"; then
  printf '%s\n' 'ERROR: API process is not alive after health response.' >&2
  exit 6
fi
if ! grep -Eq 'event="?api_started"?' "$API_LOG"; then
  printf '%s\n' 'ERROR: API startup lifecycle evidence is missing.' >&2
  exit 6
fi
if ! python3 - "$health_payload" <<'PY'
import json
import sys

try:
    payload = json.loads(sys.argv[1])
except json.JSONDecodeError as exc:
    raise SystemExit(f"invalid health JSON: {exc}")
if payload.get("status") != "UP":
    raise SystemExit(f"unexpected health status: {payload.get('status')!r}")
if payload.get("service") != "api":
    raise SystemExit(f"unexpected health service: {payload.get('service')!r}")
java_version = str(payload.get("javaVersion", ""))
if java_version.split(".", 1)[0] != "25":
    raise SystemExit(f"unexpected health javaVersion: {java_version!r}")
PY
then
  printf '%s\n' 'ERROR: API /health response did not match the S2-A runtime contract.' >&2
  exit 6
fi
printf 'API_HEALTH_PASS payload=%s\n' "$health_payload"

realtime_ready=0
for _attempt in $(seq 1 30); do
  if ! process_alive "$REALTIME_PID"; then
    printf '%s\n' 'ERROR: realtime process exited before handshake became reachable.' >&2
    cat "$REALTIME_LOG" >&2
    exit 6
  fi
  if ./scripts/handshake-smoke.py --host 127.0.0.1 --port "$REALTIME_PORT"; then
    realtime_ready=1
    break
  fi
  sleep 1
done
if [[ "$realtime_ready" -ne 1 ]]; then
  printf 'ERROR: realtime ClientHello -> ServerHello handshake did not become ready on port %s.\n' "$REALTIME_PORT" >&2
  cat "$REALTIME_LOG" >&2
  exit 6
fi
if ! grep -Eq 'event="?realtime_started"?' "$REALTIME_LOG"; then
  printf '%s\n' 'ERROR: realtime bind lifecycle evidence is missing.' >&2
  exit 6
fi
if ! grep -Eq 'event="?realtime_handshake_accepted"?' "$REALTIME_LOG"; then
  printf '%s\n' 'ERROR: accepted handshake lifecycle evidence is missing.' >&2
  exit 6
fi
printf 'REALTIME_HANDSHAKE_PASS port=%s\n' "$REALTIME_PORT"

if ! terminate_process 'realtime' "$REALTIME_PID"; then
  exit 7
fi
if ! grep -Eq 'event="?realtime_stopped"?' "$REALTIME_LOG"; then
  printf '%s\n' 'ERROR: realtime graceful shutdown evidence was not found in the runtime log.' >&2
  exit 7
fi
REALTIME_PID=''
printf '%s\n' 'REALTIME_GRACEFUL_SHUTDOWN_PASS'

if ! terminate_process 'api' "$API_PID"; then
  exit 7
fi
if ! grep -Eq 'event="?api_stopping"?' "$API_LOG"; then
  printf '%s\n' 'ERROR: API graceful shutdown lifecycle evidence was not found in the runtime log.' >&2
  exit 7
fi
API_PID=''
printf '%s\n' 'API_GRACEFUL_SHUTDOWN_PASS'
printf 'RUNTIME_SMOKE_PASS logs=%s\n' "$LOG_DIR"
