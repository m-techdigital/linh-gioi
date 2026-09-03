#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"
./tools/validate_m3_source.sh
python3 tools/validate_m3b_unity_integration.py
printf '%s\n' 'M3B SOURCE VALIDATION PASS'
