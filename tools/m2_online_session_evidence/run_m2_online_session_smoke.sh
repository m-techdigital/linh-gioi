#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
PLAYER_ARCHIVE=""
OUT_DIR="$ROOT/build/m2-online-session-smoke-sandbox"
HOST="127.0.0.1"
PORT="17777"
PLAYER_ARGS_EXTRA=()

usage() {
  cat <<'USAGE'
Usage:
  ./tools/m2_online_session_evidence/run_m2_online_session_smoke.sh \
    --player-archive FILE [--host HOST] [--port PORT] [--output-dir DIR]

Runs the Unity-built Linux player in M2 online session smoke mode against an
already running Java realtime server. This verifies ClientHello -> ServerHello
then MoveIntent -> PlayerTransformSnapshot.
USAGE
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --player-archive) PLAYER_ARCHIVE="$2"; shift 2 ;;
    --host) HOST="$2"; shift 2 ;;
    --port) PORT="$2"; shift 2 ;;
    --output-dir) OUT_DIR="$2"; shift 2 ;;
    --) shift; PLAYER_ARGS_EXTRA+=("$@"); break ;;
    --help|-h) usage; exit 0 ;;
    *) echo "ERROR: unknown argument: $1" >&2; usage >&2; exit 2 ;;
  esac
done

if [[ -z "$PLAYER_ARCHIVE" || ! -f "$PLAYER_ARCHIVE" ]]; then
  echo "ERROR: --player-archive must point to a file." >&2
  exit 2
fi
if [[ ! "$PORT" =~ ^[0-9]+$ ]] || (( PORT < 1 || PORT > 65535 )); then
  echo "ERROR: --port must be an integer 1..65535." >&2
  exit 2
fi

mkdir -p "$OUT_DIR"
EXTRACT_DIR="$OUT_DIR/player"
PLAYER_LOG="$OUT_DIR/m2-online-session-player.log"
RESULT_JSON="$OUT_DIR/m2-online-session-result.json"
rm -rf "$EXTRACT_DIR"
mkdir -p "$EXTRACT_DIR"

tar -xzf "$PLAYER_ARCHIVE" -C "$EXTRACT_DIR"
find "$EXTRACT_DIR" -name '._*' -delete
PLAYER_EXE="$(find "$EXTRACT_DIR" -maxdepth 4 -type f -perm -111 -name 'LinhGioiM0PlayerSmoke.x86_64' ! -name '._*' | sort | head -n 1)"
if [[ -z "$PLAYER_EXE" ]]; then
  echo "ERROR: Unity player executable not found in archive." >&2
  find "$EXTRACT_DIR" -maxdepth 4 -type f ! -name '._*' | sort >&2
  exit 3
fi
chmod +x "$PLAYER_EXE"

PLAYER_ARGS=(
  -batchmode
  -nosound
  --lgo-m2-online-session-smoke
  --lgo-m2-host "$HOST"
  --lgo-m2-port "$PORT"
  --lgo-m2-result "$RESULT_JSON"
  -logFile "$PLAYER_LOG"
)
PLAYER_ARGS+=("${PLAYER_ARGS_EXTRA[@]}")

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
    raise SystemExit(f"M2 online session smoke did not pass: {data}")
if data.get('handshakeAccepted') is not True:
    raise SystemExit(f"M2 online session handshake was not accepted: {data}")
snapshot=data.get('snapshot') or {}
if snapshot.get('entityId') != 1001:
    raise SystemExit(f"M2 snapshot entity mismatch: {data}")
if snapshot.get('acknowledgedSequence') != 1:
    raise SystemExit(f"M2 snapshot acknowledgement mismatch: {data}")
if abs(float(snapshot.get('x', -999)) - 0.4) > 0.001:
    raise SystemExit(f"M2 snapshot position mismatch: {data}")
duplicate=data.get('duplicateSnapshot') or {}
if duplicate.get('acknowledgedSequence') != 1:
    raise SystemExit(f"M2 duplicate snapshot acknowledgement mismatch: {data}")
if abs(float(duplicate.get('x', -999)) - 0.4) > 0.001 or abs(float(duplicate.get('z', -999))) > 0.001:
    raise SystemExit(f"M2 duplicate snapshot position mismatch: {data}")
second=data.get('secondSnapshot') or {}
if second.get('acknowledgedSequence') != 2:
    raise SystemExit(f"M2 second snapshot acknowledgement mismatch: {data}")
if abs(float(second.get('x', -999)) - 0.4) > 0.001 or abs(float(second.get('z', -999)) - 0.2) > 0.001:
    raise SystemExit(f"M2 second snapshot position mismatch: {data}")
print('M2_ONLINE_SESSION_PLAYER_SMOKE_PASS', json.dumps(data, sort_keys=True))
PY

echo "M2_ONLINE_SESSION_PLAYER_SMOKE_PASS output=$OUT_DIR"
