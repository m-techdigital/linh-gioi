#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"
./server/build.sh
./server/scripts/runtime-smoke.sh
./server/test.sh
./tools/unity_batch_test.sh
printf '%s\n' 'M0 RUNTIME VALIDATION PASS'
