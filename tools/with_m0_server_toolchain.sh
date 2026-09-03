#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
# shellcheck disable=SC1091
source "$ROOT/tools/runtime/toolchain.env"
TOOLCHAIN_ROOT="${M0_TOOLCHAIN_ROOT:-$ROOT/.toolchains}"
JAVA_HOME="$TOOLCHAIN_ROOT/temurin-${M0_JDK_VERSION}"
MAVEN_HOME="$TOOLCHAIN_ROOT/apache-maven-${M0_MAVEN_VERSION}"
if [[ ! -x "$JAVA_HOME/bin/java" || ! -x "$MAVEN_HOME/bin/mvn" ]]; then
  printf '%s\n' 'ERROR: local M0 server toolchain is not bootstrapped. Run ./tools/bootstrap_m0_server_toolchain.sh first.' >&2
  exit 20
fi
export JAVA_HOME
export PATH="$JAVA_HOME/bin:$MAVEN_HOME/bin:$PATH"
if [[ $# -eq 0 ]]; then
  exec bash
fi
exec "$@"
