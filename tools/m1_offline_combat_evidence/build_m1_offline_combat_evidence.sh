#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
M0_BUILDER="$ROOT/tools/unity_player_evidence/build_unity_player_evidence.sh"
OUT_DIR="$ROOT/build/m1-offline-combat-evidence"
TIMESTAMP="$(date -u +%Y%m%dT%H%M%SZ)"

usage() {
  cat <<'USAGE'
Usage:
  UNITY_EDITOR=/path/to/Unity ./tools/m1_offline_combat_evidence/build_m1_offline_combat_evidence.sh [--output-dir DIR]

Builds the same Unity Linux smoke player used by M0 runtime closure, but labels
and verifies the evidence for M1 Offline Combat Prototype. It requires Unity
6000.3.2f1 with Linux Build Support. It does not package Unity Editor, Library,
Temp, or Logs caches.
USAGE
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --output-dir) OUT_DIR="$2"; shift 2 ;;
    --help|-h) usage; exit 0 ;;
    *) echo "ERROR: unknown argument: $1" >&2; usage >&2; exit 2 ;;
  esac
done

if [[ ! -x "$M0_BUILDER" ]]; then
  echo "ERROR: M0 Unity player evidence builder is missing or not executable: $M0_BUILDER" >&2
  exit 20
fi

mkdir -p "$OUT_DIR"
"$M0_BUILDER" --output-dir "$OUT_DIR"

PLAYER_ARCHIVE="$(find "$OUT_DIR" -maxdepth 1 -type f -name 'lgo-unity-player-smoke-linux-6000.3.2f1-*.tar.gz' | sort | tail -n 1)"
PLAYER_SHA="${PLAYER_ARCHIVE}.sha256"
EVIDENCE_ZIP="$(find "$OUT_DIR" -maxdepth 1 -type f -name 'lgo-unity-editor-evidence-6000.3.2f1-*.zip' | sort | tail -n 1)"
EVIDENCE_SHA="${EVIDENCE_ZIP}.sha256"

if [[ -z "$PLAYER_ARCHIVE" || ! -f "$PLAYER_SHA" || -z "$EVIDENCE_ZIP" || ! -f "$EVIDENCE_SHA" ]]; then
  echo "ERROR: expected Unity player/evidence artifacts were not generated." >&2
  exit 21
fi

python3 "$ROOT/tools/m1_offline_combat_evidence/verify_m1_evidence_bundle.py" \
  --player-archive "$PLAYER_ARCHIVE" \
  --player-sha256 "$PLAYER_SHA" \
  --evidence-zip "$EVIDENCE_ZIP" \
  --evidence-sha256 "$EVIDENCE_SHA"

cat > "$OUT_DIR/UPLOAD-THESE-FILES-M1.txt" <<EOF2
Upload these files for M1 runtime closure verification:
$(basename "$PLAYER_ARCHIVE")
$(basename "$PLAYER_SHA")
$(basename "$EVIDENCE_ZIP")
$(basename "$EVIDENCE_SHA")
EOF2

echo "M1_EVIDENCE_READY timestamp=$TIMESTAMP"
echo "player=$PLAYER_ARCHIVE"
echo "evidence=$EVIDENCE_ZIP"
