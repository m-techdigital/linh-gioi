#!/usr/bin/env bash
set -euo pipefail

JAVA_VERSION_OUTPUT="$(java --version 2>&1)"
JAVAC_VERSION_OUTPUT="$(javac --version 2>&1)"
JAVA_VERSION_TOKEN="$(printf '%s\n' "$JAVA_VERSION_OUTPUT" | awk 'NR == 1 { print $2 }')"
JAVAC_VERSION_TOKEN="$(printf '%s\n' "$JAVAC_VERSION_OUTPUT" | awk 'NR == 1 { print $2 }')"
JAVA_MAJOR="${JAVA_VERSION_TOKEN%%.*}"
JAVAC_MAJOR="${JAVAC_VERSION_TOKEN%%.*}"

printf '%s\n' "$JAVA_VERSION_OUTPUT"
printf '%s\n' "$JAVAC_VERSION_OUTPUT"

if [[ "$JAVA_MAJOR" != "25" || "$JAVAC_MAJOR" != "25" ]]; then
  printf 'ERROR: Java 25 JDK is required. Detected java=%s javac=%s\n' "${JAVA_MAJOR:-unknown}" "${JAVAC_MAJOR:-unknown}" >&2
  exit 2
fi
