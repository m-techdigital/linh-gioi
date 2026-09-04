#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
OUT_ROOT="$ROOT/build/visual-evidence/profiles"
TIMEOUT_SECONDS="${LGO_VISUAL_RUNTIME_TIMEOUT_SECONDS:-360}"
FIRST_PLAYER_BUILD="${LGO_VISUAL_RUNTIME_PROFILES_FIRST_PLAYER_BUILD:-build}"

cd "$ROOT"
mkdir -p "$OUT_ROOT"

run_profile() {
  local profile="$1"
  local width="$2"
  local height="$3"
  local player_build="$4"
  local out_dir="$OUT_ROOT/$profile"

  echo "LGO_VISUAL_RUNTIME_PROFILE_START $profile ${width}x${height}"
  LGO_VISUAL_RUNTIME_PROFILE="$profile" \
  LGO_VISUAL_RUNTIME_WIDTH="$width" \
  LGO_VISUAL_RUNTIME_HEIGHT="$height" \
  LGO_VISUAL_RUNTIME_OUT_DIR="$out_dir" \
  LGO_VISUAL_RUNTIME_SOURCE_GATES="${LGO_VISUAL_RUNTIME_SOURCE_GATES:-fast}" \
  LGO_VISUAL_RUNTIME_SERVER_BUILD="${LGO_VISUAL_RUNTIME_SERVER_BUILD:-skip}" \
  LGO_VISUAL_RUNTIME_CLEAR_UNITY_CACHE="${LGO_VISUAL_RUNTIME_CLEAR_UNITY_CACHE:-0}" \
  LGO_VISUAL_RUNTIME_PLAYER_BUILD="$player_build" \
  LGO_VISUAL_RUNTIME_TIMEOUT_SECONDS="$TIMEOUT_SECONDS" \
  "$ROOT/tools/lgo_visual_runtime_review.sh"
  echo "LGO_VISUAL_RUNTIME_PROFILE_PASS $profile $out_dir"
}

run_profile desktop 1920 1080 "$FIRST_PLAYER_BUILD"
run_profile tablet 1366 768 skip
run_profile mobile 844 390 skip

echo "LGO_VISUAL_RUNTIME_PROFILES_READY $OUT_ROOT"
