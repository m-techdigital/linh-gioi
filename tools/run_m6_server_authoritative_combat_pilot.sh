#!/usr/bin/env bash
set -euo pipefail
export PYTHONDONTWRITEBYTECODE=1

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"

./server/test.sh

if ! grep -R "Tests run: 7, Failures: 0, Errors: 0, Skipped: 0" server/realtime/target/surefire-reports >/dev/null; then
  echo "ERROR: CombatValidationServiceTest did not report the expected v0.51 case count" >&2
  exit 51
fi

echo "M6_SERVER_AUTHORITATIVE_COMBAT_PILOT_PASS_v0.51.0"
