#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$ROOT"

./scripts/require-java-25.sh
./scripts/prepare-protocol.sh
if ! command -v mvn >/dev/null 2>&1; then
  printf '%s\n' 'ERROR: Maven is required. Recommended/pinned developer version: 3.9.16.' >&2
  exit 3
fi
mvn --version
mvn -B -ntp clean verify
java scripts/VerifySurefireReports.java \
  shared/target/surefire-reports \
  api/target/surefire-reports \
  realtime/target/surefire-reports
