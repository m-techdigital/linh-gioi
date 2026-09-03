#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
PLAYER_ARCHIVE=""
OUT_DIR="$ROOT/build/m1-offline-combat-smoke-sandbox"
MANIFEST="$ROOT/gamedata/compiled/gamedata-manifest.json"
PLAYER_ARGS_EXTRA=()

usage() {
  cat <<'USAGE'
Usage:
  ./tools/m1_offline_combat_evidence/run_m1_offline_combat_smoke.sh \
    --player-archive FILE [--gamedata-manifest FILE] [--output-dir DIR]

Runs the Unity-built Linux player in offline M1 combat smoke mode. This does not
start a Java server and does not claim M2/online behavior.
USAGE
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --player-archive) PLAYER_ARCHIVE="$2"; shift 2 ;;
    --gamedata-manifest) MANIFEST="$2"; shift 2 ;;
    --output-dir) OUT_DIR="$2"; shift 2 ;;
    --) shift; PLAYER_ARGS_EXTRA+=("$@"); break ;;
    --help|-h) usage; exit 0 ;;
    *) echo "ERROR: unknown argument: $1" >&2; usage >&2; exit 2 ;;
  esac
done

if [[ -z "$PLAYER_ARCHIVE" || ! -f "$PLAYER_ARCHIVE" ]]; then
  echo "ERROR: --player-archive must point to a file." >&2
  exit 2
fi
if [[ ! -f "$MANIFEST" ]]; then
  echo "ERROR: --gamedata-manifest must point to a compiled manifest file: $MANIFEST" >&2
  exit 2
fi

mkdir -p "$OUT_DIR"
EXTRACT_DIR="$OUT_DIR/player"
PLAYER_LOG="$OUT_DIR/m1-offline-combat-player.log"
RESULT_JSON="$OUT_DIR/m1-offline-combat-result.json"
rm -rf "$EXTRACT_DIR"
mkdir -p "$EXTRACT_DIR"

tar -xzf "$PLAYER_ARCHIVE" -C "$EXTRACT_DIR"
find "$EXTRACT_DIR" -name '._*' -delete
PLAYER_EXE="$(find "$EXTRACT_DIR" -maxdepth 4 -type f -perm -111 -name 'LinhGioiM0PlayerSmoke.x86_64' ! -name '._*' | sort | head -n 1)"
if [[ -z "$PLAYER_EXE" ]]; then
  echo "ERROR: Unity player executable not found in archive." >&2
  find "$EXTRACT_DIR" -maxdepth 4 -type f ! -name '._*' | sort >&2
  exit 3
fi
chmod +x "$PLAYER_EXE"

PLAYER_ARGS=(
  -batchmode
  -nosound
  --lgo-m1-offline-combat-smoke
  --lgo-gamedata-manifest "$MANIFEST"
  --lgo-m1-result "$RESULT_JSON"
  -logFile "$PLAYER_LOG"
)
PLAYER_ARGS+=("${PLAYER_ARGS_EXTRA[@]}")

if command -v xvfb-run >/dev/null 2>&1; then
  TERM="${TERM:-xterm}" xvfb-run -a "$PLAYER_EXE" "${PLAYER_ARGS[@]}"
else
  "$PLAYER_EXE" -nographics "${PLAYER_ARGS[@]}"
fi

python3 - "$RESULT_JSON" <<'PY'
import json, sys
path=sys.argv[1]
with open(path, encoding='utf-8') as f:
    data=json.load(f)
if data.get('status') != 'PASS':
    raise SystemExit(f"M1 offline combat smoke did not pass: {data}")
result=data.get('result') or {}
required={
    'targetDefeated': True,
    'skillId': 'skill.sword.wind_slash',
    'enemyContentId': 'monster.shadow.slime',
}
for key, expected in required.items():
    actual=result.get(key)
    if actual != expected:
        raise SystemExit(f"M1 offline combat smoke result mismatch for {key}: expected {expected!r}, got {actual!r}; full={data}")
if result.get('actionsExecuted', 0) <= 0:
    raise SystemExit(f"M1 offline combat smoke executed no actions: {data}")
print('M1_OFFLINE_COMBAT_SMOKE_PASS', json.dumps(data, sort_keys=True))
PY

echo "M1_OFFLINE_COMBAT_PLAYER_SMOKE_PASS output=$OUT_DIR"
