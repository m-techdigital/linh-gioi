#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
OUT_DIR="$ROOT/build/m5-lightweight-dialogue"
UNITY_PLAYER=""

usage() {
  cat <<'USAGE'
Usage:
  ./tools/run_m5_lightweight_dialogue_once.sh [--output-dir DIR] [--unity-player PATH]
USAGE
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --output-dir) OUT_DIR="$2"; shift 2 ;;
    --unity-player) UNITY_PLAYER="$2"; shift 2 ;;
    --help|-h) usage; exit 0 ;;
    *) echo "ERROR: unknown argument: $1" >&2; usage >&2; exit 2 ;;
  esac
done

cd "$ROOT"
args=(--root "$ROOT" --output-dir "$OUT_DIR")
if [[ -n "$UNITY_PLAYER" ]]; then args+=(--unity-player "$UNITY_PLAYER"); fi
python3.12 tools/m5_lightweight_dialogue_runtime.py "${args[@]}"
