#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
LOG_DIR="$ROOT/build/visual-evidence/profiles"
LOG="$LOG_DIR/profile-review.log"
PROFILE_TIMEOUT_SECONDS="${LGO_VISUAL_PROFILE_TIMEOUT_SECONDS:-180}"
BUILD_TIMEOUT_SECONDS="${LGO_VISUAL_PROFILE_BUILD_TIMEOUT_SECONDS:-420}"

mkdir -p "$LOG_DIR"
cd "$ROOT"
test "$(basename "$PWD")" = "LinhGioiOnline"

run_profile() {
  local profile="$1"
  local width="$2"
  local height="$3"
  local build_mode="$4"
  local source_gates="$5"
  local out_dir="$LOG_DIR/$profile"

  echo "LGO_VISUAL_PROFILE_REVIEW_PHASE_START $profile ${width}x${height} build=$build_mode source_gates=$source_gates"
  PYTHONDONTWRITEBYTECODE=1 \
  LGO_VISUAL_RUNTIME_PROFILE="$profile" \
  LGO_VISUAL_RUNTIME_WIDTH="$width" \
  LGO_VISUAL_RUNTIME_HEIGHT="$height" \
  LGO_VISUAL_RUNTIME_OUT_DIR="$out_dir" \
  LGO_VISUAL_RUNTIME_SOURCE_GATES="$source_gates" \
  LGO_VISUAL_RUNTIME_SERVER_BUILD=skip \
  LGO_VISUAL_RUNTIME_CLEAR_UNITY_CACHE=0 \
  LGO_VISUAL_RUNTIME_PLAYER_BUILD="$build_mode" \
  LGO_VISUAL_RUNTIME_TIMEOUT_SECONDS="$PROFILE_TIMEOUT_SECONDS" \
  LGO_VISUAL_RUNTIME_BUILD_TIMEOUT_SECONDS="$BUILD_TIMEOUT_SECONDS" \
  "$ROOT/tools/lgo_visual_runtime_review.sh"
  echo "LGO_VISUAL_PROFILE_REVIEW_PHASE_PASS $profile"
}

{
  echo "LGO_VISUAL_PROFILE_REVIEW_START $(date -u +%Y-%m-%dT%H:%M:%SZ)"
  echo "LGO_VISUAL_PROFILE_REVIEW_ROOT $ROOT"
  echo "LGO_VISUAL_PROFILE_REVIEW_POLICY build_once_reuse_player"
  echo "LGO_VISUAL_PROFILE_REVIEW_TIMEOUTS capture=$PROFILE_TIMEOUT_SECONDS build=$BUILD_TIMEOUT_SECONDS"

  run_profile desktop 1920 1080 build fast
  run_profile tablet 1366 1024 skip skip
  run_profile mobile 960 540 skip skip

  echo "LGO_VISUAL_PROFILE_INDEX_PHASE start"
  python3.12 tools/report_lgo_visual_evidence_profile_index.py
  echo "LGO_VISUAL_PROFILE_INDEX_PHASE pass"

  echo "LGO_VISUAL_PROFILE_REVIEW_RESULT EVIDENCE_CAPTURED_FOR_REVIEW"
  echo "LGO_VISUAL_RUNTIME_PASS_NOT_CLAIMED"
} 2>&1 | tee "$LOG"
