#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"

run_step() {
  local label="$1"; shift
  printf 'M0_SOURCE_STEP_START: %s\n' "$label"
  set +e
  timeout --signal=TERM --kill-after=5s 120s bash -c 'exec "$@"' _ "$@"
  local rc=$?
  set -e
  if [ "$rc" -ne 0 ]; then
    printf 'M0_SOURCE_STEP_FAILED: %s rc=%s\n' "$label" "$rc" >&2
    return "$rc"
  fi
  printf 'M0_SOURCE_STEP_PASS: %s\n' "$label"
}

run_step protocol_verify ./tools/protocol_codegen.sh verify
run_step protocol_tests python3 -m unittest -v tests.protocol.test_protocol_codegen_tooling
run_step gamedata_tests python3 -m unittest -v tests.gamedata.test_gamedata_pipeline
run_step handshake_smoke_tool_tests python3 -m unittest -v tests.server.test_handshake_smoke
run_step gamedata_compiled_check python3 tools/validate_gamedata.py --check
run_step server_source python3 tools/validate_server_source.py
python3 - <<'PY'
from pathlib import Path
import shutil

root = Path.cwd()
for rel in [
    'client/Unity/Assets/Game/Generated',
    'client/Unity/Assets/Game/Generated.meta',
    'client/Unity/Assets/Game/Protocol/Generated',
    'client/Unity/Assets/Game/Protocol/Generated.meta',
]:
    path = root / rel
    if path.is_dir():
        shutil.rmtree(path)
    elif path.exists():
        path.unlink()
PY
run_step unity_source python3 tools/validate_unity_foundation.py

for script in \
  server/build.sh server/test.sh server/run-api.sh server/run-realtime.sh \
  server/scripts/require-java-25.sh server/scripts/prepare-protocol.sh server/scripts/runtime-smoke.sh \
  tools/protocol_codegen.sh tools/unity_batch_test.sh \
  tools/bootstrap_m0_server_toolchain.sh tools/with_m0_server_toolchain.sh \
  tools/probe_m0_runtime.sh tools/bootstrap_and_validate_m0_server.sh; do
  bash -n "$script"
done
printf '%s\n' 'M0_SOURCE_STEP_PASS: shell_syntax'

python3 -m py_compile \
  tools/protocol_codegen.py tools/validate_gamedata.py tools/validate_server_source.py \
  tools/validate_unity_foundation.py tools/prepare_unity_protocol.py \
  server/scripts/handshake-smoke.py
printf '%s\n' 'M0_SOURCE_STEP_PASS: python_compile'

run_step foundation ./tools/validate_foundation.sh
printf '%s\n' 'M0 SOURCE VALIDATION PASS'
