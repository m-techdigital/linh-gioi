#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"
./tools/validate_m3b_source.sh
python3 tools/validate_m4_playable_source.py
echo "M4 SOURCE VALIDATION PASS"
