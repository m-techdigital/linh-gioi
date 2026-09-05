#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
MODE=""
OUT_DIR="$ROOT/build/visual-evidence/m5-latest"
PLAYER_APP="$ROOT/build/unity-player-macos/LinhGioiOnline.app"
PLAYER_EXE="$PLAYER_APP/Contents/MacOS/Unity"
PLAYER_TIMEOUT_SECONDS="${LGO_VISUAL_EVIDENCE_TIMEOUT_SECONDS:-45}"

usage() {
  cat <<'USAGE'
Usage:
  ./tools/run_m5_visual_evidence_review.sh --rebuild
  ./tools/run_m5_visual_evidence_review.sh --open-existing
USAGE
}

if [[ $# -ne 1 ]]; then
  usage >&2
  exit 2
fi

case "$1" in
  --rebuild|--open-existing) MODE="$1" ;;
  --help|-h) usage; exit 0 ;;
  *) echo "ERROR: unknown mode: $1" >&2; usage >&2; exit 2 ;;
esac

cd "$ROOT"
if [[ "$(basename "$PWD")" != "LinhGioiOnline" ]]; then
  echo "ERROR: must run from repo root LinhGioiOnline" >&2
  exit 2
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
    echo "LGO_PLAYABLE_VISUAL_EVIDENCE_SCREENSHOT_UNAVAILABLE"
    echo "ERROR: UNITY_EDITOR is not set and Unity 6000.3.2f1 was not found in common macOS paths." >&2
    exit 30
  fi
  if [[ ! -x "$UNITY_EDITOR" ]]; then
    echo "LGO_PLAYABLE_VISUAL_EVIDENCE_SCREENSHOT_UNAVAILABLE"
    echo "ERROR: UNITY_EDITOR is not executable: $UNITY_EDITOR" >&2
    exit 31
  fi
  ./tools/lgo_playable_closure_check.sh --source-only
  ./tools/prepare_unity_local_assets.sh
  python3.12 tools/prepare_unity_protocol.py --output "$ROOT/client/Unity/Assets/Game/Protocol/Generated"
  mkdir -p "$ROOT/build/unity-player-macos"
  "$UNITY_EDITOR" -batchmode -nographics -quit -projectPath "$ROOT/client/Unity" -executeMethod LinhGioi.Foundation.Editor.M0LinuxPlayerEvidenceBuilder.BuildMacOSPlayerSmoke --lgo-player-output "$PLAYER_APP" -logFile "$OUT_DIR/unity-build.log"
fi

if [[ ! -x "$PLAYER_EXE" ]]; then
  echo "LGO_PLAYABLE_VISUAL_EVIDENCE_SCREENSHOT_UNAVAILABLE"
  echo "ERROR: macOS Unity player missing. Run ./tools/run_m5_visual_evidence_review.sh --rebuild first." >&2
  exit 32
fi

python3.12 - "$OUT_DIR" <<'PY'
from pathlib import Path
import shutil
import sys
out = Path(sys.argv[1])
if out.exists():
    shutil.rmtree(out)
out.mkdir(parents=True, exist_ok=True)
PY

python3.12 - "$PLAYER_TIMEOUT_SECONDS" "$PLAYER_EXE" "$OUT_DIR" <<'PY'
from __future__ import annotations

import subprocess
import sys
from pathlib import Path

timeout = int(sys.argv[1])
player = sys.argv[2]
out_dir = Path(sys.argv[3])
log_path = out_dir / 'player.log'
args = [
    player,
    '-batchmode',
    '-screen-fullscreen', '0',
    '-screen-width', '1280',
    '-screen-height', '720',
    '--lgo-m5-visual-evidence-review',
    '--lgo-visual-evidence-dir', str(out_dir),
]
with log_path.open('w', encoding='utf-8') as log:
    process = subprocess.Popen(args, stdout=log, stderr=subprocess.STDOUT)
    try:
        returncode = process.wait(timeout=timeout)
    except subprocess.TimeoutExpired:
        process.kill()
        process.wait(timeout=10)
        print('LGO_PLAYABLE_VISUAL_EVIDENCE_SCREENSHOT_UNAVAILABLE')
        print(f'ERROR: visual evidence player timed out after {timeout}s; see {log_path}', file=sys.stderr)
        sys.exit(41)
if returncode != 0:
    print('LGO_PLAYABLE_VISUAL_EVIDENCE_SCREENSHOT_UNAVAILABLE')
    print(f'ERROR: visual evidence player exited with code {returncode}; see {log_path}', file=sys.stderr)
    sys.exit(returncode)
PY

if [[ ! -f "$OUT_DIR/visual-evidence-summary.json" ]]; then
  echo "LGO_PLAYABLE_VISUAL_EVIDENCE_SCREENSHOT_UNAVAILABLE"
  echo "ERROR: visual evidence summary missing: $OUT_DIR/visual-evidence-summary.json" >&2
  exit 40
fi

cat "$OUT_DIR/visual-evidence-summary.txt"
if grep -q "VISUAL_EVIDENCE_SCREENSHOT_UNAVAILABLE" "$OUT_DIR/visual-evidence-summary.json"; then
  echo "LGO_PLAYABLE_VISUAL_EVIDENCE_SCREENSHOT_UNAVAILABLE"
fi
echo "LGO_PLAYABLE_VISUAL_EVIDENCE_READY output=$OUT_DIR"
