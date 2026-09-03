#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
UNITY_PROJECT="$ROOT/client/Unity"
UNITY_EDITOR="${UNITY_EDITOR:-}"
if [[ -z "$UNITY_EDITOR" ]]; then
  for candidate in unity-editor Unity unity; do
    if command -v "$candidate" >/dev/null 2>&1; then UNITY_EDITOR="$(command -v "$candidate")"; break; fi
  done
fi
if [[ -z "$UNITY_EDITOR" || ! -x "$UNITY_EDITOR" ]]; then
  printf '%s\n' 'ERROR: Unity 6000.3.2f1 editor is required for runtime/batch verification. Set UNITY_EDITOR=/path/to/Unity.' >&2
  exit 20
fi
python3 "$ROOT/tools/prepare_unity_protocol.py"
RESULTS="$ROOT/client/Unity/Logs/m0-editmode-results.xml"
mkdir -p "$(dirname "$RESULTS")"
"$UNITY_EDITOR" -batchmode -nographics -quit \
  -projectPath "$UNITY_PROJECT" \
  -runTests -testPlatform EditMode \
  -testResults "$RESULTS" \
  -logFile -
printf 'UNITY_EDITMODE_PASS results=%s\n' "$RESULTS"
