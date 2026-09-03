#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
python3 "$ROOT/tools/validate_proto_contract.py"
python3 "$ROOT/tools/validate_gamedata.py" --check
python3 -m json.tool "$ROOT/m0-manifest.json" >/dev/null
python3 -m json.tool "$ROOT/client/Unity/Assets/Game/UI/design-tokens.json" >/dev/null
printf 'M0 FOUNDATION STATIC VALIDATION PASS\n'
