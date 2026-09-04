#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
UNITY_PLAYER=""

if [[ $# -eq 2 && "$1" == "--unity-player" ]]; then
  UNITY_PLAYER="$2"
else
  echo "Usage: ./tools/run_m6_minimal_local_combat_once.sh --unity-player <path>" >&2
  exit 2
fi

if [[ ! -x "$UNITY_PLAYER" ]]; then
  echo "ERROR: Unity player executable not found: $UNITY_PLAYER" >&2
  exit 2
fi

RESULT_DIR="$ROOT/build/m6-minimal-local-combat"
RESULT_JSON="$RESULT_DIR/result.json"
mkdir -p "$RESULT_DIR"

"$UNITY_PLAYER" --batchmode --nographics --lgo-m6-minimal-local-combat-smoke --lgo-m6-combat-result "$RESULT_JSON"

python3.12 - "$RESULT_JSON" <<'PY'
import json
import sys
from pathlib import Path

path = Path(sys.argv[1])
payload = json.loads(path.read_text(encoding='utf-8'))
if payload.get('status') != 'PASS':
    raise SystemExit('M6 minimal local combat smoke did not PASS')
if payload.get('marker') != 'M6_LOCAL_COMBAT_PROTOTYPE_SMOKE_PASS_v0.49.0':
    raise SystemExit('M6 v0.49 local combat marker missing')
if payload.get('legacyMarker') != 'M6_MINIMAL_LOCAL_COMBAT_RUNTIME_SMOKE_PASS':
    raise SystemExit('M6 legacy local combat marker missing')
if payload.get('executedChecks', 0) <= 0:
    raise SystemExit('M6 local combat smoke executed zero checks')
for key in ['rejectedNoTarget', 'rejectedOutOfRange', 'attackTriggered', 'cooldownBlockedAfterRepeatedInput', 'attackAfterCooldownRecovered']:
    if not payload.get(key):
        raise SystemExit(f'M6 local combat smoke missing expected case: {key}')
print('M6_LOCAL_COMBAT_PROTOTYPE_SMOKE_PASS_v0.49.0')
print('M6_MINIMAL_LOCAL_COMBAT_RUNTIME_SMOKE_PASS')
PY
