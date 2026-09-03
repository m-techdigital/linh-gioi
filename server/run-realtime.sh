#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$ROOT"
./scripts/require-java-25.sh
JAR="realtime/target/server-realtime-0.1.0-SNAPSHOT-runtime.jar"
if [[ ! -f "$JAR" ]]; then
  printf 'ERROR: %s is missing. Run ./build.sh first.\n' "$JAR" >&2
  exit 4
fi
exec java -jar "$JAR"
