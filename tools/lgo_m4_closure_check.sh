#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SUMMARY_DIR="$ROOT/build/lgo-m4-closure"
SUMMARY_TXT="$SUMMARY_DIR/latest-summary.txt"
SUMMARY_JSON="$SUMMARY_DIR/latest-summary.json"
ROOT_SUMMARY="$ROOT/LGO-M4-PLAYABLE-STABILIZATION-CLOSURE-SUMMARY-v0.13.0.txt"
MODE=""

usage() {
  cat <<'USAGE'
Usage:
  ./tools/lgo_m4_closure_check.sh --source-only
  ./tools/lgo_m4_closure_check.sh --runtime
  ./tools/lgo_m4_closure_check.sh --package-ready
USAGE
}

if [[ $# -ne 1 ]]; then
  usage >&2
  exit 2
fi

case "$1" in
  --source-only|--runtime|--package-ready) MODE="$1" ;;
  --help|-h) usage; exit 0 ;;
  *) echo "ERROR: unknown mode: $1" >&2; usage >&2; exit 2 ;;
esac

cd "$ROOT"

mkdir -p "$SUMMARY_DIR"
: > "$SUMMARY_TXT"

log() {
  printf '%s\n' "$1" | tee -a "$SUMMARY_TXT"
}

write_json() {
  local status="$1"
  local mode="$2"
  local reason="$3"
  python3.12 - "$SUMMARY_JSON" "$status" "$mode" "$reason" <<'PY'
from __future__ import annotations
import json
import sys
from datetime import datetime, timezone
from pathlib import Path

path = Path(sys.argv[1])
status = sys.argv[2]
mode = sys.argv[3]
reason = sys.argv[4]
payload = {
    "project": "linh-gioi-online",
    "task": "M4 playable slice stabilization",
    "version": "0.13.0",
    "mode": mode,
    "status": status,
    "reason": reason,
    "timestampUtc": datetime.now(timezone.utc).isoformat(),
}
path.write_text(json.dumps(payload, indent=2, sort_keys=True) + "\n", encoding="utf-8")
PY
}

clean_disposable_outputs() {
  python3.12 - "$ROOT" <<'PY'
from __future__ import annotations
from pathlib import Path
import shutil
import sys

root = Path(sys.argv[1])
for rel in [
    "client/Unity/Assets/Game/Generated",
    "client/Unity/Assets/Game/Generated.meta",
    "client/Unity/Assets/Game/Protocol/Generated",
    "client/Unity/Assets/Game/Protocol/Generated.meta",
    "client/Unity/Library",
    "client/Unity/Temp",
    "client/Unity/Logs",
    "build",
]:
    path = root / rel
    if path.is_dir():
        shutil.rmtree(path)
    elif path.exists():
        path.unlink()
PY
  mkdir -p "$SUMMARY_DIR"
}

run_phase() {
  local label="$1"
  shift
  log "LGO_M4_CLOSURE_PHASE_START ${label}"
  "$@" 2>&1 | tee -a "$SUMMARY_TXT"
  local rc=${PIPESTATUS[0]}
  if [[ "$rc" -ne 0 ]]; then
    log "LGO_M4_CLOSURE_PHASE_FAIL ${label} rc=${rc}"
    write_json "FAIL" "$MODE" "${label} failed rc=${rc}"
    exit "$rc"
  fi
  log "LGO_M4_CLOSURE_PHASE_PASS ${label}"
}

check_repo_root() {
  if [[ "$(basename "$PWD")" != "LinhGioiOnline" ]]; then
    echo "ERROR: must run from repo root LinhGioiOnline" >&2
    exit 2
  fi
}

check_package_hygiene() {
  python3.12 - "$ROOT" <<'PY'
from __future__ import annotations
from pathlib import Path
import sys

root = Path(sys.argv[1])
forbidden = [
    "client/Unity/Library",
    "client/Unity/Temp",
    "client/Unity/Logs",
    "client/Unity/Assets/Game/Generated",
    "client/Unity/Assets/Game/Protocol/Generated",
]
errors = []
for rel in forbidden:
    if (root / rel).exists() or (root / (rel + ".meta")).exists():
        errors.append(rel)
if errors:
    for rel in errors:
        print(f"PACKAGE_HYGIENE_FAIL forbidden output remains: {rel}", file=sys.stderr)
    raise SystemExit(1)
print("PACKAGE_HYGIENE_PASS")
PY
}

remove_build_after_summary_copy() {
  python3.12 - "$ROOT" <<'PY'
from pathlib import Path
import shutil
import sys

build = Path(sys.argv[1]) / 'build'
if build.exists():
    shutil.rmtree(build)
PY
}

source_only() {
  check_repo_root
  log "LGO_M4_CLOSURE_MODE source-only"
  run_phase clean_disposable_outputs clean_disposable_outputs
  run_phase diff_check git --no-pager diff --check
  run_phase project_state python3.12 tools/validate_project_state.py
  run_phase m4_playable python3.12 tools/validate_m4_playable_source.py
  run_phase m4_visual python3.12 tools/validate_m4_visual_foundation.py
  run_phase m4_2_ui python3.12 tools/validate_m4_2_playable_ui.py
  run_phase m4_stabilization python3.12 tools/validate_m4_stabilization.py
  run_phase m4_source ./tools/validate_m4_source.sh
  run_phase python_compile python3.12 -m py_compile \
    tools/validate_project_state.py \
    tools/validate_m4_playable_source.py \
    tools/validate_m4_visual_foundation.py \
    tools/validate_m4_2_playable_ui.py \
    tools/validate_m4_stabilization.py \
    tools/m4_playable_vertical_slice_runtime.py \
    tools/m4_visual_foundation_runtime.py
  log "LGO_M4_CLOSURE_SOURCE_GATES_PASS"
  write_json "PASS" "$MODE" "source gates pass"
}

load_local_env() {
  if [[ -f "$ROOT/.lgo-local-env" ]]; then
    set -a
    source "$ROOT/.lgo-local-env"
    set +a
    log "LGO_M4_CLOSURE_ENV loaded .lgo-local-env"
  else
    log "LGO_M4_CLOSURE_ENV .lgo-local-env not present"
  fi
}

runtime_unverified() {
  local reason="$1"
  log "LGO_M4_CLOSURE_RUNTIME_UNVERIFIED_ENVIRONMENT"
  log "REASON=${reason}"
  write_json "UNVERIFIED_ENVIRONMENT" "$MODE" "$reason"
  exit 30
}

runtime_mode() {
  source_only
  load_local_env
  run_phase protoc_verify ./tools/protocol_codegen.sh verify
  if [[ -z "${UNITY_EDITOR:-}" ]]; then
    runtime_unverified "UNITY_EDITOR is not set"
  fi
  if [[ ! -x "$UNITY_EDITOR" ]]; then
    runtime_unverified "UNITY_EDITOR is not executable: $UNITY_EDITOR"
  fi
  run_phase prepare_unity_assets ./tools/prepare_unity_local_assets.sh
  run_phase prepare_unity_protocol python3.12 tools/prepare_unity_protocol.py --output "$ROOT/client/Unity/Assets/Game/Protocol/Generated"
  run_phase server_build ./server/build.sh
  mkdir -p "$ROOT/build/unity-player-macos"
  run_phase unity_player_build "$UNITY_EDITOR" -batchmode -nographics -quit -projectPath "$ROOT/client/Unity" -executeMethod LinhGioi.Foundation.Editor.M0LinuxPlayerEvidenceBuilder.BuildMacOSPlayerSmoke --lgo-player-output "$ROOT/build/unity-player-macos/LinhGioiOnline.app" -logFile "$ROOT/build/unity-player-macos/build.log"
  local macos_player="$ROOT/build/unity-player-macos/LinhGioiOnline.app/Contents/MacOS/Unity"
  if [[ ! -x "$macos_player" ]]; then
    runtime_unverified "macOS Unity player executable missing after build: $macos_player"
  fi
  run_phase m3b_runtime ./tools/run_m3b_unity_account_character_once.sh --unity-player "$macos_player"
  if ! grep -R "M3B_UNITY_ACCOUNT_CHARACTER_RUNTIME_SMOKE_PASS" "$ROOT/build" >/dev/null; then
    echo "ERROR: M3B runtime marker not observed" >&2
    exit 41
  fi
  run_phase m4_playable_runtime ./tools/run_m4_playable_vertical_slice_once.sh --unity-player "$macos_player"
  if ! grep -R "M4_PLAYABLE_VERTICAL_SLICE_RUNTIME_SMOKE_PASS" "$ROOT/build" >/dev/null; then
    echo "ERROR: M4 playable runtime marker not observed" >&2
    exit 42
  fi
  run_phase m4_visual_runtime ./tools/run_m4_visual_foundation_once.sh --unity-player "$macos_player"
  if ! grep -R "M4_VISUAL_PLACEHOLDER_FOUNDATION_SMOKE_PASS" "$ROOT/build" >/dev/null; then
    echo "ERROR: M4 visual runtime marker not observed" >&2
    exit 43
  fi
  log "LGO_M4_CLOSURE_RUNTIME_GATES_PASS"
  write_json "PASS" "$MODE" "runtime gates pass"
}

package_ready_mode() {
  source_only
  run_phase clean_for_package clean_disposable_outputs
  run_phase package_hygiene check_package_hygiene
  log "LGO_M4_CLOSURE_PACKAGE_READY"
  write_json "PASS" "$MODE" "package hygiene pass"
  cp "$SUMMARY_TXT" "$ROOT_SUMMARY"
  remove_build_after_summary_copy
}

case "$MODE" in
  --source-only) source_only ;;
  --runtime) runtime_mode ;;
  --package-ready) package_ready_mode ;;
esac
