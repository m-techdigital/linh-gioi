#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
"$ROOT/tools/bootstrap_m0_server_toolchain.sh"
"$ROOT/tools/with_m0_server_toolchain.sh" "$ROOT/server/build.sh"
"$ROOT/tools/with_m0_server_toolchain.sh" "$ROOT/server/test.sh"
"$ROOT/tools/with_m0_server_toolchain.sh" "$ROOT/server/scripts/runtime-smoke.sh"
printf '%s\n' 'M0_SERVER_RUNTIME_VALIDATION_PASS'
