#!/usr/bin/env bash
set -euo pipefail
SERVER_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
REPO_ROOT="$(cd "$SERVER_ROOT/.." && pwd)"
cd "$REPO_ROOT"
./tools/protocol_codegen.sh generate --language java
GENERATED="$REPO_ROOT/build/generated/protocol/java"
if [[ ! -d "$GENERATED" ]] || [[ -z "$(find "$GENERATED" -type f -name '*.java' -print -quit)" ]]; then
  printf 'ERROR: canonical Java protocol generation produced no Java sources at %s.\n' "$GENERATED" >&2
  exit 8
fi
printf 'SERVER_PROTOCOL_PREPARE_PASS source=%s files=%s\n' "$GENERATED" "$(find "$GENERATED" -type f -name '*.java' | wc -l | tr -d ' ')"
