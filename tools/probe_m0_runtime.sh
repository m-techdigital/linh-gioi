#!/usr/bin/env bash
set -u
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
# shellcheck disable=SC1091
source "$ROOT/tools/runtime/toolchain.env"
status=0
printf '%s\n' '=== Java ==='
java --version 2>&1 || status=1
javac --version 2>&1 || status=1
printf '%s\n' '=== Maven ==='
mvn --version 2>&1 || status=1
printf '%s\n' '=== Unity ==='
unity_path="${UNITY_EDITOR:-}"
if [[ -z "$unity_path" ]]; then
  for candidate in unity-editor Unity unity; do
    if command -v "$candidate" >/dev/null 2>&1; then unity_path="$(command -v "$candidate")"; break; fi
  done
fi
if [[ -n "$unity_path" && -x "$unity_path" ]]; then
  printf 'Unity editor candidate: %s (required %s)\n' "$unity_path" "$M0_UNITY_VERSION"
else
  printf 'Unity editor not found; required version: %s\n' "$M0_UNITY_VERSION"
  status=1
fi
exit "$status"
