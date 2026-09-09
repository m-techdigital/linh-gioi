#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
UNITY_PROJECT="$ROOT/client/Unity"
UNITY_EDITOR="${UNITY_EDITOR:-}"
OUT_DIR="${LGO_2D_ONBOARDING_OUT_DIR:-$ROOT/build/2d-onboarding}"
if [[ -z "$UNITY_EDITOR" ]]; then
  for candidate in \
    "/Applications/Unity/Hub/Editor/6000.3.2f1/Unity.app/Contents/MacOS/Unity" \
    "$HOME/Applications/Unity/Hub/Editor/6000.3.2f1/Unity.app/Contents/MacOS/Unity"; do
    if [[ -x "$candidate" ]]; then
      UNITY_EDITOR="$candidate"
      break
    fi
  done
fi
if [[ -z "$UNITY_EDITOR" || ! -x "$UNITY_EDITOR" ]]; then
  echo "ERROR: Unity Editor 6000.3.2f1 is required. Set UNITY_EDITOR=/absolute/path/to/Unity." >&2
  exit 20
fi
mkdir -p "$OUT_DIR"
RESULT="$OUT_DIR/twod-onboarding-smoke.json"
LOG="$OUT_DIR/twod-onboarding-smoke.log"
python3.12 "$ROOT/tools/prepare_unity_protocol.py"
"$UNITY_EDITOR" -batchmode -nographics -quit \
  -projectPath "$UNITY_PROJECT" \
  -executeMethod LinhGioi.World.TwoDOnboardingSmokeRunner.RunFromCommandLine \
  --lgo-2d-result "$RESULT" \
  -logFile "$LOG"
python3.12 - "$RESULT" <<'PY'
from pathlib import Path
import json
import sys
path = Path(sys.argv[1])
if not path.exists():
    raise SystemExit(f"ERROR: 2D onboarding smoke result missing: {path}")
data = json.loads(path.read_text(encoding='utf-8'))
if data.get('status') != 'PASS':
    raise SystemExit(f"ERROR: 2D onboarding smoke failed: {data}")
if data.get('finalStep') != 'Complete':
    raise SystemExit(f"ERROR: 2D onboarding final step mismatch: {data}")
print('LGO_2D_ONBOARDING_SMOKE_PASS', json.dumps(data, ensure_ascii=False, sort_keys=True))
PY
