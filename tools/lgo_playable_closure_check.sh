#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SUMMARY_DIR="$ROOT/build/lgo-playable-closure"
SUMMARY_TXT="$SUMMARY_DIR/latest-summary.txt"
SUMMARY_JSON="$SUMMARY_DIR/latest-summary.json"
MODE=""

usage() {
  cat <<'USAGE'
Usage:
  ./tools/lgo_playable_closure_check.sh --source-only
  ./tools/lgo_playable_closure_check.sh --runtime
  ./tools/lgo_playable_closure_check.sh --package-ready
  ./tools/lgo_playable_closure_check.sh --visual-evidence
USAGE
}

if [[ $# -ne 1 ]]; then
  usage >&2
  exit 2
fi

case "$1" in
  --source-only|--runtime|--package-ready|--visual-evidence) MODE="$1" ;;
  --help|-h) usage; exit 0 ;;
  *) echo "ERROR: unknown mode: $1" >&2; usage >&2; exit 2 ;;
esac

cd "$ROOT"
mkdir -p "$SUMMARY_DIR"
: > "$SUMMARY_TXT"

log() {
  mkdir -p "$SUMMARY_DIR"
  printf '%s\n' "$1" | tee -a "$SUMMARY_TXT"
}

write_json() {
  local status="$1"
  local reason="$2"
  python3.12 - "$SUMMARY_JSON" "$status" "$MODE" "$reason" <<'PY'
from __future__ import annotations
import json
import sys
from datetime import datetime, timezone
from pathlib import Path

path = Path(sys.argv[1])
payload = {
    "project": "linh-gioi-online",
    "task": "playable closure",
    "version": "0.15.0",
    "status": sys.argv[2],
    "mode": sys.argv[3],
    "reason": sys.argv[4],
    "timestampUtc": datetime.now(timezone.utc).isoformat(),
}
path.write_text(json.dumps(payload, indent=2, sort_keys=True) + "\n", encoding="utf-8")
PY
}

run_phase() {
  local label="$1"
  shift
  log "LGO_PLAYABLE_CLOSURE_PHASE_START ${label}"
  "$@" 2>&1 | tee -a "$SUMMARY_TXT"
  local rc=${PIPESTATUS[0]}
  if [[ "$rc" -ne 0 ]]; then
    log "LGO_PLAYABLE_CLOSURE_FIX_REQUIRED"
    log "LGO_PLAYABLE_CLOSURE_PHASE_FAIL ${label} rc=${rc}"
    write_json "FIX_REQUIRED" "${label} failed rc=${rc}"
    exit "$rc"
  fi
  log "LGO_PLAYABLE_CLOSURE_PHASE_PASS ${label}"
}

check_repo_root() {
  if [[ "$(basename "$PWD")" != "LinhGioiOnline" ]]; then
    echo "ERROR: must run from repo root LinhGioiOnline" >&2
    exit 2
  fi
}

source_only() {
  check_repo_root
  log "LGO_PLAYABLE_CLOSURE_MODE source-only"
  run_phase m4_source_gates ./tools/lgo_m4_closure_check.sh --source-only
  run_phase m5_first_playable_loop python3.12 tools/validate_m5_first_playable_loop.py
  run_phase m5_guided_training_loop python3.12 tools/validate_m5_guided_training_loop.py
  if [[ -f tools/validate_m5_playable_session_feedback.py ]]; then
    run_phase m5_playable_session_feedback python3.12 tools/validate_m5_playable_session_feedback.py
  fi
  if [[ -f tools/validate_m5_world_hub_readability.py ]]; then
    run_phase m5_world_hub_readability python3.12 tools/validate_m5_world_hub_readability.py
  fi
  if [[ -f tools/validate_m5_pose_animation_placeholder.py ]]; then
    run_phase m5_pose_animation_placeholder python3.12 tools/validate_m5_pose_animation_placeholder.py
  fi
  if [[ -f tools/validate_m5_ui_skinning.py ]]; then
    run_phase m5_ui_skinning python3.12 tools/validate_m5_ui_skinning.py
  fi
  if [[ -f tools/validate_m5_vfx_feedback_placeholder.py ]]; then
    run_phase m5_vfx_feedback_placeholder python3.12 tools/validate_m5_vfx_feedback_placeholder.py
  fi
  if [[ -f tools/validate_m5_lightweight_dialogue.py ]]; then
    run_phase m5_lightweight_dialogue python3.12 tools/validate_m5_lightweight_dialogue.py
  fi
  if [[ -f tools/validate_m5_training_objective_ux.py ]]; then
    run_phase m5_training_objective_ux python3.12 tools/validate_m5_training_objective_ux.py
  fi
  if [[ -f tools/validate_m5_input_camera_polish.py ]]; then
    run_phase m5_input_camera_polish python3.12 tools/validate_m5_input_camera_polish.py
  fi
  if [[ -f tools/validate_m5_session_menu.py ]]; then
    run_phase m5_session_menu python3.12 tools/validate_m5_session_menu.py
  fi
  if [[ -f tools/validate_m5_local_settings.py ]]; then
    run_phase m5_local_settings python3.12 tools/validate_m5_local_settings.py
  fi
  if [[ -f tools/validate_m5_api_error_resilience.py ]]; then
    run_phase m5_api_error_resilience python3.12 tools/validate_m5_api_error_resilience.py
  fi
  if [[ -f tools/validate_m6_skill_preview_sandbox.py ]]; then
    run_phase m6_skill_preview_sandbox python3.12 tools/validate_m6_skill_preview_sandbox.py
  fi
  if [[ -f tools/validate_m6_target_dummy_readability.py ]]; then
    run_phase m6_target_dummy_readability python3.12 tools/validate_m6_target_dummy_readability.py
  fi
  if [[ -f tools/validate_m6_combat_readiness_spec.py ]]; then
    run_phase m6_combat_readiness_spec python3.12 tools/validate_m6_combat_readiness_spec.py
  fi
  if [[ -f tools/validate_m6_contract_review.py || -f tools/validate_m6_minimal_local_combat.py ]]; then
    run_phase clean_pycache_before_m6_combat git clean -f tools/__pycache__
  fi
  if [[ -f tools/validate_m6_contract_review.py ]]; then
    run_phase m6_contract_review python3.12 tools/validate_m6_contract_review.py
  fi
  if [[ -f tools/validate_m6_minimal_local_combat.py ]]; then
    run_phase m6_minimal_local_combat python3.12 tools/validate_m6_minimal_local_combat.py
  fi
  if [[ -f tools/validate_m6_combat_ux_feedback.py ]]; then
    run_phase m6_combat_ux_feedback python3.12 tools/validate_m6_combat_ux_feedback.py
  fi
  if [[ -f tools/validate_m6_combat_visual_reference_pack.py ]]; then
    run_phase m6_combat_visual_reference_pack python3.12 tools/validate_m6_combat_visual_reference_pack.py
  fi
  if [[ -f tools/validate_m6_combat_visual_readability.py ]]; then
    run_phase m6_combat_visual_readability python3.12 tools/validate_m6_combat_visual_readability.py
  fi
  if [[ -f tools/validate_code_governance.py ]]; then
    run_phase clean_pycache_before_code_governance git clean -f tools/__pycache__
    run_phase code_governance python3.12 tools/validate_code_governance.py
  fi
  run_phase python_compile python3.12 -m py_compile \
    tools/validate_project_state.py \
    tools/validate_m4_playable_source.py \
    tools/validate_m4_visual_foundation.py \
    tools/validate_m4_2_playable_ui.py \
    tools/validate_m4_stabilization.py \
    tools/validate_m4_visible_ui.py \
    tools/validate_m5_first_playable_loop.py \
    tools/validate_m5_visual_evidence.py \
    tools/validate_m5_guided_training_loop.py \
    tools/validate_m5_playable_session_feedback.py \
    tools/validate_m5_world_hub_readability.py \
    tools/validate_m5_pose_animation_placeholder.py \
    tools/validate_m5_ui_skinning.py \
    tools/validate_m5_vfx_feedback_placeholder.py \
    tools/validate_m5_lightweight_dialogue.py \
    tools/validate_m5_training_objective_ux.py \
    tools/validate_m5_input_camera_polish.py \
    tools/validate_m5_session_menu.py \
    tools/validate_m5_local_settings.py \
    tools/validate_m5_api_error_resilience.py \
    tools/validate_m6_skill_preview_sandbox.py \
    tools/validate_m6_target_dummy_readability.py \
    tools/validate_m6_combat_readiness_spec.py \
    tools/validate_m6_contract_review.py \
    tools/validate_m6_minimal_local_combat.py \
    tools/validate_m6_combat_ux_feedback.py \
    tools/validate_m6_combat_visual_reference_pack.py \
    tools/validate_m6_combat_visual_readability.py \
    tools/validate_code_governance.py \
    tools/m4_playable_vertical_slice_runtime.py \
    tools/m4_visual_foundation_runtime.py \
    tools/m5_first_playable_loop_runtime.py \
    tools/m5_guided_training_loop_runtime.py \
    tools/m5_lightweight_dialogue_runtime.py
  log "LGO_PLAYABLE_CLOSURE_SOURCE_GATES_PASS"
  write_json "PASS" "source gates pass"
}

load_local_env() {
  if [[ -f "$ROOT/.lgo-local-env" ]]; then
    set -a
    source "$ROOT/.lgo-local-env"
    set +a
    log "LGO_PLAYABLE_CLOSURE_ENV loaded .lgo-local-env"
  else
    log "LGO_PLAYABLE_CLOSURE_ENV .lgo-local-env not present"
  fi
  if [[ -z "${UNITY_EDITOR:-}" ]]; then
    for candidate in \
      "/Applications/Unity/Hub/Editor/6000.3.2f1/Unity.app/Contents/MacOS/Unity" \
      "$HOME/Applications/Unity/Hub/Editor/6000.3.2f1/Unity.app/Contents/MacOS/Unity"; do
      if [[ -x "$candidate" ]]; then
        UNITY_EDITOR="$candidate"
        export UNITY_EDITOR
        log "LGO_PLAYABLE_CLOSURE_ENV detected UNITY_EDITOR=$UNITY_EDITOR"
        break
      fi
    done
  fi
}

runtime_unverified() {
  local reason="$1"
  log "LGO_PLAYABLE_CLOSURE_RUNTIME_UNVERIFIED_ENVIRONMENT"
  log "REASON=${reason}"
  write_json "UNVERIFIED_ENVIRONMENT" "$reason"
  exit 30
}

runtime_mode() {
  source_only
  load_local_env
  if [[ -z "${UNITY_EDITOR:-}" ]]; then
    runtime_unverified "UNITY_EDITOR is not set"
  fi
  if [[ ! -x "$UNITY_EDITOR" ]]; then
    runtime_unverified "UNITY_EDITOR is not executable: $UNITY_EDITOR"
  fi
  run_phase inherited_m4_runtime ./tools/lgo_m4_closure_check.sh --runtime
  local macos_player="$ROOT/build/unity-player-macos/LinhGioiOnline.app/Contents/MacOS/Unity"
  if [[ ! -x "$macos_player" ]]; then
    runtime_unverified "macOS Unity player executable missing after M4 runtime build: $macos_player"
  fi
  run_phase m5_first_playable_loop_runtime ./tools/run_m5_first_playable_loop_once.sh --unity-player "$macos_player"
  if ! grep -R "M5_FIRST_PLAYABLE_LOOP_RUNTIME_SMOKE_PASS" "$ROOT/build" >/dev/null; then
    log "LGO_PLAYABLE_CLOSURE_FIX_REQUIRED"
    echo "ERROR: M5 first playable loop runtime marker not observed" >&2
    exit 44
  fi
  run_phase m5_guided_training_loop_runtime ./tools/run_m5_guided_training_loop_once.sh --unity-player "$macos_player"
  if ! grep -R "M5_GUIDED_TRAINING_LOOP_RUNTIME_SMOKE_PASS" "$ROOT/build" >/dev/null; then
    log "LGO_PLAYABLE_CLOSURE_FIX_REQUIRED"
    echo "ERROR: M5 guided training loop runtime marker not observed" >&2
    exit 45
  fi
  if [[ -f tools/run_m5_lightweight_dialogue_once.sh ]]; then
    run_phase m5_lightweight_dialogue_runtime ./tools/run_m5_lightweight_dialogue_once.sh --unity-player "$macos_player"
    if ! grep -R "M5_LIGHTWEIGHT_NPC_DIALOGUE_RUNTIME_SMOKE_PASS" "$ROOT/build" >/dev/null; then
      log "LGO_PLAYABLE_CLOSURE_FIX_REQUIRED"
      echo "ERROR: M5 lightweight NPC dialogue runtime marker not observed" >&2
      exit 46
    fi
  fi
  if [[ -f tools/run_m6_minimal_local_combat_once.sh ]]; then
    run_phase m6_minimal_local_combat_runtime ./tools/run_m6_minimal_local_combat_once.sh --unity-player "$macos_player"
    if ! grep -R "M6_MINIMAL_LOCAL_COMBAT_RUNTIME_SMOKE_PASS" "$ROOT/build" >/dev/null; then
      log "LGO_PLAYABLE_CLOSURE_FIX_REQUIRED"
      echo "ERROR: M6 minimal local combat runtime marker not observed" >&2
      exit 47
    fi
  fi
  log "LGO_PLAYABLE_CLOSURE_RUNTIME_GATES_PASS"
  write_json "PASS" "runtime gates pass"
}

package_ready() {
  source_only
  run_phase inherited_m4_package_ready ./tools/lgo_m4_closure_check.sh --package-ready
  run_phase m5_first_playable_loop python3.12 tools/validate_m5_first_playable_loop.py
  run_phase m5_guided_training_loop python3.12 tools/validate_m5_guided_training_loop.py
  if [[ -f tools/validate_m5_playable_session_feedback.py ]]; then
    run_phase m5_playable_session_feedback python3.12 tools/validate_m5_playable_session_feedback.py
  fi
  if [[ -f tools/validate_m5_world_hub_readability.py ]]; then
    run_phase m5_world_hub_readability python3.12 tools/validate_m5_world_hub_readability.py
  fi
  if [[ -f tools/validate_m5_pose_animation_placeholder.py ]]; then
    run_phase m5_pose_animation_placeholder python3.12 tools/validate_m5_pose_animation_placeholder.py
  fi
  if [[ -f tools/validate_m5_ui_skinning.py ]]; then
    run_phase m5_ui_skinning python3.12 tools/validate_m5_ui_skinning.py
  fi
  if [[ -f tools/validate_m5_vfx_feedback_placeholder.py ]]; then
    run_phase m5_vfx_feedback_placeholder python3.12 tools/validate_m5_vfx_feedback_placeholder.py
  fi
  if [[ -f tools/validate_m5_lightweight_dialogue.py ]]; then
    run_phase m5_lightweight_dialogue python3.12 tools/validate_m5_lightweight_dialogue.py
  fi
  if [[ -f tools/validate_m5_training_objective_ux.py ]]; then
    run_phase m5_training_objective_ux python3.12 tools/validate_m5_training_objective_ux.py
  fi
  if [[ -f tools/validate_m5_input_camera_polish.py ]]; then
    run_phase m5_input_camera_polish python3.12 tools/validate_m5_input_camera_polish.py
  fi
  if [[ -f tools/validate_m5_session_menu.py ]]; then
    run_phase m5_session_menu python3.12 tools/validate_m5_session_menu.py
  fi
  if [[ -f tools/validate_m5_local_settings.py ]]; then
    run_phase m5_local_settings python3.12 tools/validate_m5_local_settings.py
  fi
  if [[ -f tools/validate_m5_api_error_resilience.py ]]; then
    run_phase m5_api_error_resilience python3.12 tools/validate_m5_api_error_resilience.py
  fi
  if [[ -f tools/validate_m6_skill_preview_sandbox.py ]]; then
    run_phase m6_skill_preview_sandbox python3.12 tools/validate_m6_skill_preview_sandbox.py
  fi
  if [[ -f tools/validate_m6_target_dummy_readability.py ]]; then
    run_phase m6_target_dummy_readability python3.12 tools/validate_m6_target_dummy_readability.py
  fi
  if [[ -f tools/validate_m6_combat_readiness_spec.py ]]; then
    run_phase m6_combat_readiness_spec python3.12 tools/validate_m6_combat_readiness_spec.py
  fi
  if [[ -f tools/validate_m6_contract_review.py || -f tools/validate_m6_minimal_local_combat.py ]]; then
    run_phase clean_pycache_before_m6_combat git clean -f tools/__pycache__
  fi
  if [[ -f tools/validate_m6_contract_review.py ]]; then
    run_phase m6_contract_review python3.12 tools/validate_m6_contract_review.py
  fi
  if [[ -f tools/validate_m6_minimal_local_combat.py ]]; then
    run_phase m6_minimal_local_combat python3.12 tools/validate_m6_minimal_local_combat.py
  fi
  if [[ -f tools/validate_m6_combat_ux_feedback.py ]]; then
    run_phase m6_combat_ux_feedback python3.12 tools/validate_m6_combat_ux_feedback.py
  fi
  if [[ -f tools/validate_m6_combat_visual_reference_pack.py ]]; then
    run_phase m6_combat_visual_reference_pack python3.12 tools/validate_m6_combat_visual_reference_pack.py
  fi
  if [[ -f tools/validate_m6_combat_visual_readability.py ]]; then
    run_phase m6_combat_visual_readability python3.12 tools/validate_m6_combat_visual_readability.py
  fi
  if [[ -f tools/validate_code_governance.py ]]; then
    run_phase clean_pycache_before_code_governance git clean -f tools/__pycache__
    run_phase code_governance python3.12 tools/validate_code_governance.py
  fi
  run_phase package_hygiene python3.12 tools/validate_package_hygiene.py
  log "LGO_PLAYABLE_CLOSURE_PACKAGE_READY"
  write_json "PASS" "package gates pass"
}

visual_evidence() {
  check_repo_root
  log "LGO_PLAYABLE_CLOSURE_MODE visual-evidence"
  run_phase m5_visual_evidence_source python3.12 tools/validate_m5_visual_evidence.py
  run_phase visual_evidence_review ./tools/run_m5_visual_evidence_review.sh --rebuild
  if grep -q "VISUAL_EVIDENCE_SCREENSHOT_UNAVAILABLE" "$ROOT/build/visual-evidence/visual-evidence-summary.json"; then
    log "LGO_PLAYABLE_VISUAL_EVIDENCE_SCREENSHOT_UNAVAILABLE"
  fi
  log "LGO_PLAYABLE_VISUAL_EVIDENCE_READY"
  write_json "PASS" "visual evidence ready"
}

case "$MODE" in
  --source-only) source_only ;;
  --runtime) runtime_mode ;;
  --package-ready) package_ready ;;
  --visual-evidence) visual_evidence ;;
esac
