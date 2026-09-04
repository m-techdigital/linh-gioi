#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PLAYER=""
PORT="17844"
RESULT="$ROOT/build/m6-unity-java-combat-e2e/lgo-m6-unity-java-combat-e2e-result.json"

while [[ $# -gt 0 ]]; do
  case "$1" in
    --unity-player) PLAYER="$2"; shift 2 ;;
    --port) PORT="$2"; shift 2 ;;
    --result) RESULT="$2"; shift 2 ;;
    *) echo "ERROR: unknown argument: $1" >&2; exit 2 ;;
  esac
done

if [[ -z "$PLAYER" || ! -x "$PLAYER" ]]; then
  echo "UNVERIFIED_ENVIRONMENT: Unity player executable missing or not executable" >&2
  exit 30
fi

cd "$ROOT"
./server/build.sh
mkdir -p "$ROOT/build/m6-unity-java-combat-e2e"
java -cp "$ROOT/server/realtime/target/server-realtime-0.1.0-SNAPSHOT-runtime.jar" com.linhgioi.server.realtime.combat.CombatSmokeServer --port "$PORT" > "$ROOT/build/m6-unity-java-combat-e2e/server.log" 2>&1 &
SERVER_PID="$!"
trap 'kill "$SERVER_PID" >/dev/null 2>&1' EXIT
sleep 2
"$PLAYER" --batchmode --nographics --lgo-m6-unity-java-combat-e2e --lgo-m6-combat-host 127.0.0.1 --lgo-m6-combat-port "$PORT" --lgo-m6-unity-java-combat-e2e-result "$RESULT"
grep -q "M6_UNITY_JAVA_COMBAT_E2E_PASS_v0.52.0" "$RESULT"
echo "M6_UNITY_JAVA_COMBAT_E2E_PASS_v0.52.0 result=$RESULT"
