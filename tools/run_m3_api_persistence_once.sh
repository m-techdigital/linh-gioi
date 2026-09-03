#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
OUT_DIR="$ROOT/build/m3-api-persistence"
API_PORT="18083"

usage() {
  cat <<'USAGE'
Usage:
  ./tools/run_m3_api_persistence_once.sh [--output-dir DIR] [--port PORT]

Runs the M3 server-side dev account + character persistence runtime smoke against
a real Spring Boot API process, including a restart to prove JSON persistence
reload. Run ./tools/validate_m3_source.sh and ./server/build.sh first on a clean
workspace so the API runtime jar exists.
USAGE
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --output-dir) OUT_DIR="$2"; shift 2 ;;
    --port) API_PORT="$2"; shift 2 ;;
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

mkdir -p "$OUT_DIR"
python3 tools/m3_api_persistence_runtime.py --root "$ROOT" --output-dir "$OUT_DIR" --port "$API_PORT"
