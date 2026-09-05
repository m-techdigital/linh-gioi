#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
LOG_DIR="$ROOT/build/dev-loop"
LOG="$LOG_DIR/latest.log"
VISUAL_TIMEOUT_SECONDS="${LGO_DEV_LOOP_VISUAL_TIMEOUT_SECONDS:-${LGO_VISUAL_RUNTIME_TIMEOUT_SECONDS:-300}}"
GATE_PROFILE="${LGO_DEV_LOOP_GATE_PROFILE:-quick}"
CONTEXT_MODE="${LGO_DEV_LOOP_CONTEXT_MODE:-quick}"
VISUAL_RUNTIME_MODE="${LGO_DEV_LOOP_VISUAL_RUNTIME:-auto}"

mkdir -p "$LOG_DIR"
cd "$ROOT"

run_logged() {
  local label="$1"
  shift
  echo "LGO_DEV_LOOP_PHASE_START $label"
  "$@"
  echo "LGO_DEV_LOOP_PHASE_PASS $label"
}

print_context() {
  case "$CONTEXT_MODE" in
    quick)
      echo "LGO_DEV_LOOP_CONTEXT_MODE quick"
      python3.12 "$ROOT/tools/lgo_state_brief.py"
      ;;
    full)
      echo "LGO_DEV_LOOP_CONTEXT_MODE full"
      echo "LGO_PROJECT_STATE_BEGIN"
      sed -n '1,180p' "$ROOT/docs/execution/PROJECT-STATE.md"
      echo "LGO_PROJECT_STATE_END"
      echo "LGO_NEXT_ACTION_BEGIN"
      sed -n '1,220p' "$ROOT/docs/execution/NEXT-ACTION.md"
      echo "LGO_NEXT_ACTION_END"
      ;;
    none)
      echo "LGO_DEV_LOOP_CONTEXT_MODE none"
      ;;
    *)
      echo "FIX_REQUIRED unsupported LGO_DEV_LOOP_CONTEXT_MODE=$CONTEXT_MODE; expected quick, full, or none"
      exit 2
      ;;
  esac
}

run_visual_review_if_available() {
  case "$VISUAL_RUNTIME_MODE" in
    skip|0|false|off)
      echo "SKIP visual_runtime_review mode=$VISUAL_RUNTIME_MODE"
      return 0
      ;;
    force|1|true|on|auto)
      ;;
    *)
      echo "FIX_REQUIRED unsupported LGO_DEV_LOOP_VISUAL_RUNTIME=$VISUAL_RUNTIME_MODE; expected auto, force, or skip"
      exit 2
      ;;
  esac

  if [[ "$VISUAL_RUNTIME_MODE" == "auto" && "$GATE_PROFILE" == "quick" ]]; then
    echo "SKIP visual_runtime_review quick_profile_auto"
    echo "LGO_OWNER_NOTE Quick gate bỏ qua visual runtime để lặp nhanh; chạy LGO_DEV_LOOP_VISUAL_RUNTIME=force hoặc GATE_PROFILE=full khi cần ảnh evidence."
    return 0
  fi

  if [[ ! -x "$ROOT/tools/lgo_visual_runtime_review.sh" ]]; then
    echo "RUNTIME_BLOCKED_ENV missing tools/lgo_visual_runtime_review.sh"
    return 0
  fi

  if [[ -z "${UNITY_EDITOR:-}" ]]; then
    for candidate in \
      "/Applications/Unity/Hub/Editor/6000.3.2f1/Unity.app/Contents/MacOS/Unity" \
      "$HOME/Applications/Unity/Hub/Editor/6000.3.2f1/Unity.app/Contents/MacOS/Unity"; do
      if [[ -x "$candidate" ]]; then
        UNITY_EDITOR="$candidate"
        export UNITY_EDITOR
        break
      fi
    done
  fi

  if [[ -z "${UNITY_EDITOR:-}" || ! -x "$UNITY_EDITOR" ]]; then
    echo "RUNTIME_BLOCKED_ENV UNITY_EDITOR unavailable"
    return 0
  fi

  echo "LGO_DEV_LOOP_PHASE_START visual_runtime_review"
  set +e
  LGO_VISUAL_RUNTIME_TIMEOUT_SECONDS="$VISUAL_TIMEOUT_SECONDS" LGO_VISUAL_RUNTIME_SOURCE_GATES="${LGO_VISUAL_RUNTIME_SOURCE_GATES:-fast}" LGO_VISUAL_RUNTIME_SERVER_BUILD="${LGO_VISUAL_RUNTIME_SERVER_BUILD:-fast}" "$ROOT/tools/lgo_visual_runtime_review.sh"
  local status="$?"
  set -e
  case "$status" in
    0)
      echo "PASS visual_runtime_review"
      echo "LGO_DEV_LOOP_PHASE_PASS visual_runtime_review"
      ;;
    41)
      echo "VISUAL_CAPTURE_TIMEOUT visual runtime player exceeded ${VISUAL_TIMEOUT_SECONDS}s"
      exit 41
      ;;
    30|32|34|35|40)
      echo "RUNTIME_BLOCKED_ENV visual runtime review exited with code $status"
      exit "$status"
      ;;
    *)
      echo "FIX_REQUIRED visual runtime review exited with code $status"
      exit "$status"
      ;;
  esac
}

run_source_validation_profile() {
  case "$GATE_PROFILE" in
    quick)
      echo "LGO_DEV_LOOP_GATE_PROFILE quick"
      run_logged diff_check git --no-pager diff --check
      run_logged login_gate_entry python3.12 tools/validate_lgo_login_gate_entry_visual_v1.py
      run_logged runtime_asset_weight python3.12 tools/validate_lgo_runtime_asset_weight.py
      run_logged device_profile_ui_budgets python3.12 tools/validate_lgo_device_profile_ui_budgets.py
      run_logged m4_2_ui python3.12 tools/validate_m4_2_playable_ui.py
      run_logged m4_visible_ui python3.12 tools/validate_m4_visible_ui.py
      run_logged m6_combat_visual_readability python3.12 tools/validate_m6_combat_visual_readability.py
      run_logged m6_unity_combat_placeholder_asset_import python3.12 tools/validate_m6_unity_combat_placeholder_asset_import.py
      run_logged package_hygiene python3.12 tools/validate_package_hygiene.py
      ;;
    full)
      echo "LGO_DEV_LOOP_GATE_PROFILE full"
      run_logged diff_check git --no-pager diff --check
      run_logged playable_source_only ./tools/lgo_playable_closure_check.sh --source-only
      ;;
    *)
      echo "FIX_REQUIRED unsupported LGO_DEV_LOOP_GATE_PROFILE=$GATE_PROFILE; expected quick or full"
      exit 2
      ;;
  esac
}

{
  echo "LGO_CONTINUE_DEV_LOOP_START $(date -u +%Y-%m-%dT%H:%M:%SZ)"
  echo "LGO_REPO_ROOT $ROOT"
  echo "LGO_OWNER_NOTE Đang chạy dev loop: đọc state ngắn, chạy validation $GATE_PROFILE, visual runtime mode=$VISUAL_RUNTIME_MODE."
  test "$(basename "$PWD")" = "LinhGioiOnline"

  print_context

  run_source_validation_profile
  run_visual_review_if_available

  echo "LGO_OWNER_NOTE Dev loop hoàn tất: source gates sạch; visual runtime chỉ chạy khi mode/profile yêu cầu."
  echo "LGO_CONTINUE_DEV_LOOP_RESULT PASS"
} 2>&1 | tee "$LOG"
