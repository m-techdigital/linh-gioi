#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PLAYER=""
RESULT="$ROOT/build/m6-unity-combat-intent-client/lgo-m6-unity-combat-intent-client-result.json"

while [[ $# -gt 0 ]]; do
  case "$1" in
    --unity-player) PLAYER="$2"; shift 2 ;;
    --result) RESULT="$2"; shift 2 ;;
    *) echo "ERROR: unknown argument: $1" >&2; exit 2 ;;
  esac
done

if [[ -z "$PLAYER" || ! -x "$PLAYER" ]]; then
  echo "UNVERIFIED_ENVIRONMENT: Unity player executable missing or not executable" >&2
  exit 30
fi

mkdir -p "$(dirname "$RESULT")"
"$PLAYER" --batchmode --nographics --lgo-m6-unity-combat-intent-client-smoke --lgo-m6-unity-combat-intent-result "$RESULT"
grep -q "M6_UNITY_COMBAT_INTENT_CLIENT_RUNTIME_SMOKE_PASS" "$RESULT"
echo "M6_UNITY_COMBAT_INTENT_CLIENT_RUNTIME_SMOKE_PASS result=$RESULT"
