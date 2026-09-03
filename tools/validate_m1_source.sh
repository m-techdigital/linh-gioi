#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"
./tools/validate_m0_source.sh
python3 tools/validate_m1_offline_combat.py
python3 tools/validate_project_state.py
printf '%s\n' 'M1 SOURCE VALIDATION PASS'
