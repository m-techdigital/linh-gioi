#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
OUT_DIR="$ROOT/build/m2-local-runtime-candidate"
SERVER_PORT="17777"
RUN_SERVER_SMOKE="1"
RUN_UNITY_EVIDENCE="1"
PARTIAL_REASON=()

usage() {
  cat <<'USAGE'
Usage:
  ./tools/run_m2_local_runtime_once.sh [--output-dir DIR] [--port PORT] [--skip-server-smoke] [--skip-unity-evidence]

One-command local M2 runtime candidate runner for macOS. With no skip flags, this
must validate source, prepare local Unity assets, run Java server build/test,
run online-session smoke, build Unity editor/player evidence, and write an upload
manifest. Skip flags are diagnostics only: they produce PARTIAL output, never a
runtime-candidate READY marker.
USAGE
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --output-dir) OUT_DIR="$2"; shift 2 ;;
    --port) SERVER_PORT="$2"; shift 2 ;;
    --skip-server-smoke) RUN_SERVER_SMOKE="0"; PARTIAL_REASON+=("server-smoke-skipped-by-flag"); shift ;;
    --skip-unity-evidence) RUN_UNITY_EVIDENCE="0"; PARTIAL_REASON+=("unity-evidence-skipped-by-flag"); shift ;;
    --help|-h) usage; exit 0 ;;
    *) echo "ERROR: unknown argument: $1" >&2; usage >&2; exit 2 ;;
  esac
done

if [[ ! "$SERVER_PORT" =~ ^[0-9]+$ ]] || (( SERVER_PORT < 1 || SERVER_PORT > 65535 )); then
  echo "ERROR: --port must be an integer 1..65535." >&2
  exit 2
fi

mkdir -p "$OUT_DIR"
LOG="$OUT_DIR/m2-local-runtime-once.log"
: > "$LOG"

say() { printf '%s\n' "$*" | tee -a "$LOG"; }
run_logged() {
  say "+ $*"
  "$@" 2>&1 | tee -a "$LOG"
  local rc=${PIPESTATUS[0]:-0}
  if [[ "$rc" != "0" ]]; then
    say "COMMAND_FAILED rc=$rc command=$*"
    return "$rc"
  fi
}

cleanup() {
  if [[ -n "${SERVER_PID:-}" ]]; then
    kill "$SERVER_PID" >/dev/null 2>&1 || true
    wait "$SERVER_PID" >/dev/null 2>&1 || true
  fi
}
trap cleanup EXIT

cd "$ROOT"

say "== Linh Gioi Online M2 local runtime candidate =="
say "ROOT=$ROOT"
say "OUT_DIR=$OUT_DIR"
say "LOG=$LOG"

say
say "== Local environment setup =="
run_logged bash tools/local_macos_setup.sh
source "$ROOT/.lgo-local-env"

say
say "== Source validation before local generated outputs =="
run_logged ./tools/validate_m2_source.sh

say
say "== Prepare Unity local assets =="
run_logged ./tools/prepare_unity_local_assets.sh --output-dir "$OUT_DIR/toolchain"

if [[ "$RUN_SERVER_SMOKE" == "1" ]]; then
  say
  say "== Java server build/test and online-session smoke =="
  if ! command -v java >/dev/null 2>&1 || ! command -v mvn >/dev/null 2>&1; then
    say "ERROR: Java 25 and Maven 3.9.16 are required for a full M2 runtime candidate."
    say "Install them or rerun with --skip-server-smoke for a diagnostic-only PARTIAL run."
    exit 30
  fi
  java --version 2>&1 | tee -a "$LOG"
  mvn --version 2>&1 | tee -a "$LOG"
  run_logged ./server/scripts/require-java-25.sh
  run_logged ./server/test.sh
  # server/test.sh runs `mvn clean test`, so it removes the shaded runtime JAR.
  # Run build/verify after tests to recreate realtime/target/*-runtime.jar before
  # launching the local server smoke.
  run_logged ./server/build.sh

  SMOKE_DIR="$OUT_DIR/server-online-session-smoke"
  mkdir -p "$SMOKE_DIR"
  say "+ start realtime server port=$SERVER_PORT"
  LG_REALTIME_HOST=127.0.0.1 LG_REALTIME_PORT="$SERVER_PORT" ./server/run-realtime.sh > "$SMOKE_DIR/java-realtime.log" 2>&1 &
  SERVER_PID=$!
  say "SERVER_PID=$SERVER_PID"
  sleep 5
  run_logged python3 server/scripts/online-session-smoke.py --host 127.0.0.1 --port "$SERVER_PORT"
  kill "$SERVER_PID" >/dev/null 2>&1 || true
  wait "$SERVER_PID" >/dev/null 2>&1 || true
  unset SERVER_PID
  say "SERVER_ONLINE_SESSION_SMOKE_PASS log=$SMOKE_DIR/java-realtime.log"
else
  say
  say "SERVER_SMOKE_PARTIAL skipped by explicit flag. This run cannot close M2 runtime."
fi

if [[ "$RUN_UNITY_EVIDENCE" == "1" ]]; then
  say
  say "== Unity editor/player evidence build =="
  if [[ -z "${UNITY_EDITOR:-}" || ! -x "${UNITY_EDITOR:-}" ]]; then
    UNITY_EDITOR="$(find /Applications/Unity/Hub/Editor -type f -path '*/6000.3.2f1/Unity.app/Contents/MacOS/Unity' 2>/dev/null | head -n 1)"
    export UNITY_EDITOR
  fi
  if [[ -z "${UNITY_EDITOR:-}" || ! -x "${UNITY_EDITOR:-}" ]]; then
    say "ERROR: Unity 6000.3.2f1 is required for a full M2 runtime candidate."
    say "Install the editor or rerun with --skip-unity-evidence for a diagnostic-only PARTIAL run."
    exit 31
  fi
  say "UNITY_EDITOR=$UNITY_EDITOR"
  "$UNITY_EDITOR" -version 2>&1 | tee -a "$LOG"
  run_logged ./tools/m2_online_session_evidence/build_m2_online_session_evidence.sh --output-dir "$OUT_DIR/unity-evidence"
else
  say
  say "UNITY_EVIDENCE_PARTIAL skipped by explicit flag. This run cannot close M2 runtime."
fi

say
say "== Build upload manifest =="
UPLOAD_MANIFEST="$OUT_DIR/UPLOAD-THESE-FILES-M2-RUNTIME-CANDIDATE.txt"
{
  echo "Upload these files only after the script reports M2_LOCAL_RUNTIME_CANDIDATE_READY:"
  find "$OUT_DIR/unity-evidence" -maxdepth 1 -type f \( -name 'lgo-unity-player-smoke-linux-6000.3.2f1-*.tar.gz' -o -name 'lgo-unity-player-smoke-linux-6000.3.2f1-*.tar.gz.sha256' -o -name 'lgo-unity-editor-evidence-6000.3.2f1-*.zip' -o -name 'lgo-unity-editor-evidence-6000.3.2f1-*.zip.sha256' \) -print 2>/dev/null | sort
  echo "$LOG"
  if [[ -d "$OUT_DIR/server-online-session-smoke" ]]; then
    SERVER_BUNDLE="$OUT_DIR/lgo-m2-local-server-smoke-$(date -u +%Y%m%dT%H%M%SZ).tar.gz"
    tar -czf "$SERVER_BUNDLE" -C "$OUT_DIR" server-online-session-smoke
    shasum -a 256 "$SERVER_BUNDLE" > "$SERVER_BUNDLE.sha256" || sha256sum "$SERVER_BUNDLE" > "$SERVER_BUNDLE.sha256"
    echo "$SERVER_BUNDLE"
    echo "$SERVER_BUNDLE.sha256"
  fi
} > "$UPLOAD_MANIFEST"

cat "$UPLOAD_MANIFEST" | tee -a "$LOG"

if [[ ${#PARTIAL_REASON[@]} -gt 0 ]]; then
  say
  say "M2_LOCAL_RUNTIME_CANDIDATE_PARTIAL reasons=${PARTIAL_REASON[*]} output=$OUT_DIR"
  exit 0
fi

PLAYER_COUNT=$(find "$OUT_DIR/unity-evidence" -maxdepth 1 -type f -name 'lgo-unity-player-smoke-linux-6000.3.2f1-*.tar.gz' 2>/dev/null | wc -l | tr -d ' ')
EDITOR_COUNT=$(find "$OUT_DIR/unity-evidence" -maxdepth 1 -type f -name 'lgo-unity-editor-evidence-6000.3.2f1-*.zip' 2>/dev/null | wc -l | tr -d ' ')
if [[ "$PLAYER_COUNT" -lt 1 || "$EDITOR_COUNT" -lt 1 ]]; then
  say "ERROR: Full M2 runtime candidate did not produce required Unity player/editor evidence files."
  exit 32
fi
if ! grep -q 'M2_ONLINE_SESSION_SMOKE_PASS' "$LOG"; then
  say "ERROR: Full M2 runtime candidate did not record M2_ONLINE_SESSION_SMOKE_PASS."
  exit 33
fi

say
say "M2_LOCAL_RUNTIME_CANDIDATE_READY output=$OUT_DIR"
