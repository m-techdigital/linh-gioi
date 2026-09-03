#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
OUT_DIR="$ROOT/build/m5-first-playable-loop"
API_PORT="18086"
UNITY_PLAYER=""

usage() {
  cat <<'USAGE'
Usage:
  ./tools/run_m5_first_playable_loop_once.sh [--output-dir DIR] [--port PORT] [--unity-player PATH]

Runs the M5 first playable loop foundation smoke against a real Spring Boot API
process and a current Unity player executable built from this source.
Missing Unity player is classified by the runtime helper as UNVERIFIED_ENVIRONMENT.
USAGE
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --output-dir) OUT_DIR="$2"; shift 2 ;;
    --port) API_PORT="$2"; shift 2 ;;
    --unity-player) UNITY_PLAYER="$2"; shift 2 ;;
    --help|-h) usage; exit 0 ;;
    *) echo "ERROR: unknown argument: $1" >&2; usage >&2; exit 2 ;;
  esac
done

if [[ ! "$API_PORT" =~ ^[0-9]+$ ]] || (( API_PORT < 1 || API_PORT > 65535 )); then
  echo "ERROR: --port must be an integer 1..65535." >&2
  exit 2
fi

cd "$ROOT"
./server/scripts/require-java-25.sh
if [[ ! -f server/api/target/server-api-0.1.0-SNAPSHOT.jar ]]; then
  echo "ERROR: server/api/target/server-api-0.1.0-SNAPSHOT.jar missing. Run ./server/build.sh first." >&2
  exit 4
fi

args=(--root "$ROOT" --output-dir "$OUT_DIR" --port "$API_PORT")
if [[ -n "$UNITY_PLAYER" ]]; then args+=(--unity-player "$UNITY_PLAYER"); fi
python3 tools/m5_first_playable_loop_runtime.py "${args[@]}"
