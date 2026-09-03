#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"
./tools/validate_m2_source.sh
python3 tools/validate_m3_persistence.py
python3 tools/validate_project_state.py
printf '%s\n' 'M3 SOURCE VALIDATION PASS'
