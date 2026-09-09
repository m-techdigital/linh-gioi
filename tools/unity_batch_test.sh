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
"$UNITY_EDITOR" -batchmode -nographics \
  -projectPath "$UNITY_PROJECT" \
  -runTests -testPlatform EditMode \
  -testResults "$RESULTS" \
  -logFile -
python3 - "$RESULTS" <<'PY'
import sys
import xml.etree.ElementTree as ET
from pathlib import Path

path = Path(sys.argv[1])
if not path.is_file():
    print(f"ERROR: Unity EditMode test results missing: {path}", file=sys.stderr)
    raise SystemExit(21)

root = ET.parse(path).getroot()
total = int(root.attrib.get("total", "0"))
failed = int(root.attrib.get("failed", "0"))
passed = int(root.attrib.get("passed", "0"))
result = root.attrib.get("result", "")
if total <= 0:
    print(f"ERROR: Unity EditMode executed zero tests: {path}", file=sys.stderr)
    raise SystemExit(22)
if failed != 0 or passed <= 0:
    print(f"ERROR: Unity EditMode failed: result={result} total={total} passed={passed} failed={failed}", file=sys.stderr)
    raise SystemExit(23)
print(f"UNITY_EDITMODE_RESULTS_VERIFIED total={total} passed={passed} failed={failed} result={result}")
PY
printf 'UNITY_EDITMODE_PASS results=%s\n' "$RESULTS"
