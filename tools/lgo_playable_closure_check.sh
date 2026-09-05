#!/usr/bin/env bash
set -euo pipefail
export PYTHONDONTWRITEBYTECODE=1

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
    run_phase clean_pycache_before_m6_combat find server tests tools -type d -name __pycache__ -prune -exec rm -rf {} +
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
  if [[ -f tools/validate_m6_combat_input_feedback_stability.py ]]; then
    run_phase m6_combat_input_feedback_stability python3.12 tools/validate_m6_combat_input_feedback_stability.py
  fi
  if [[ -f tools/validate_m6_server_combat_contract_spec.py ]]; then
    run_phase m6_server_combat_contract_spec python3.12 tools/validate_m6_server_combat_contract_spec.py
  fi
  if [[ -f tools/validate_m6_combat_protocol_gamedata_contract.py ]]; then
    run_phase m6_combat_protocol_gamedata_contract python3.12 tools/validate_m6_combat_protocol_gamedata_contract.py
  fi
  if [[ -f tools/validate_m6_java_server_combat_validation.py ]]; then
    run_phase m6_java_server_combat_validation python3.12 tools/validate_m6_java_server_combat_validation.py
  fi
  if [[ -f tools/validate_m6_unity_combat_intent_client.py ]]; then
    run_phase m6_unity_combat_intent_client python3.12 tools/validate_m6_unity_combat_intent_client.py
  fi
  if [[ -f tools/validate_m6_unity_java_combat_smoke.py ]]; then
    run_phase m6_unity_java_combat_smoke python3.12 tools/validate_m6_unity_java_combat_smoke.py
  fi
  if [[ -f tools/validate_m6_server_authoritative_combat_closure.py ]]; then
    run_phase m6_server_authoritative_combat_closure python3.12 tools/validate_m6_server_authoritative_combat_closure.py
  fi
  if [[ -f tools/validate_m6_package_hygiene_hotfix.py ]]; then
    run_phase m6_package_hygiene_hotfix python3.12 tools/validate_m6_package_hygiene_hotfix.py
  fi
  if [[ -f tools/validate_m6_runtime_usable_combat_asset_pack.py ]]; then
    run_phase m6_runtime_usable_combat_asset_pack python3.12 tools/validate_m6_runtime_usable_combat_asset_pack.py
  fi
  if [[ -f tools/validate_m6_unity_combat_placeholder_asset_import.py ]]; then
    run_phase m6_unity_combat_placeholder_asset_import python3.12 tools/validate_m6_unity_combat_placeholder_asset_import.py
  fi
  if [[ -f tools/validate_m6_local_combat_prototype.py ]]; then
    run_phase m6_local_combat_prototype python3.12 tools/validate_m6_local_combat_prototype.py
  fi
  if [[ -f tools/validate_m6_local_combat_runtime_closure.py ]]; then
    run_phase m6_local_combat_runtime_closure python3.12 tools/validate_m6_local_combat_runtime_closure.py
  fi
  if [[ -f tools/validate_m6_server_authoritative_combat_pilot.py ]]; then
    run_phase m6_server_authoritative_combat_pilot python3.12 tools/validate_m6_server_authoritative_combat_pilot.py
  fi
  if [[ -f tools/validate_m6_unity_java_combat_e2e.py ]]; then
    run_phase m6_unity_java_combat_e2e python3.12 tools/validate_m6_unity_java_combat_e2e.py
  fi
  if [[ -f tools/validate_m6_combat_ux_readability_polish.py ]]; then
    run_phase m6_combat_ux_readability_polish python3.12 tools/validate_m6_combat_ux_readability_polish.py
  fi
  if [[ -f tools/validate_m6_combat_gamedata_balance.py ]]; then
    run_phase m6_combat_gamedata_balance python3.12 tools/validate_m6_combat_gamedata_balance.py
  fi
  if [[ -f tools/validate_m6_combat_foundation_closure.py ]]; then
    run_phase m6_combat_foundation_closure python3.12 tools/validate_m6_combat_foundation_closure.py
  fi
  if [[ -f tools/validate_m6_combat_hardening_continuation.py ]]; then
    run_phase m6_combat_hardening_continuation python3.12 tools/validate_m6_combat_hardening_continuation.py
  fi
  if [[ -f tools/validate_lgo_art_pack_v1.py ]]; then
    run_phase lgo_art_pack_v1 python3.12 tools/validate_lgo_art_pack_v1.py
  fi
  if [[ -f tools/validate_lgo_art_v2_separated_assets.py ]]; then
    run_phase lgo_art_v2_separated_assets python3.12 tools/validate_lgo_art_v2_separated_assets.py
  fi
  if [[ -f tools/validate_lgo_art_v3_course_correction.py ]]; then
    run_phase lgo_art_v3_course_correction python3.12 tools/validate_lgo_art_v3_course_correction.py
  fi
  if [[ -f tools/validate_lgo_art_v3b_candidates.py ]]; then
    run_phase lgo_art_v3b_candidates python3.12 tools/validate_lgo_art_v3b_candidates.py
  fi
  if [[ -f tools/validate_lgo_login_gate_entry_visual_v1.py ]]; then
    run_phase login_gate_entry_visual python3.12 tools/validate_lgo_login_gate_entry_visual_v1.py
  fi
  if [[ -f tools/validate_lgo_login_npc_compositing_polish.py ]]; then
    run_phase login_npc_compositing_polish python3.12 tools/validate_lgo_login_npc_compositing_polish.py
  fi
  if [[ -f tools/validate_lgo_login_panel_visual_balance.py ]]; then
    run_phase login_panel_visual_balance python3.12 tools/validate_lgo_login_panel_visual_balance.py
  fi
  if [[ -f tools/validate_lgo_login_cta_ornament_lightweight.py ]]; then
    run_phase login_cta_ornament_lightweight python3.12 tools/validate_lgo_login_cta_ornament_lightweight.py
  fi
  if [[ -f tools/validate_lgo_login_cta_debug_dot_cleanup.py ]]; then
    run_phase login_cta_debug_dot_cleanup python3.12 tools/validate_lgo_login_cta_debug_dot_cleanup.py
  fi
  if [[ -f tools/validate_lgo_login_cta_debug_dot_evidence_refresh.py ]]; then
    run_phase login_cta_debug_dot_evidence_refresh python3.12 tools/validate_lgo_login_cta_debug_dot_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_login_cta_backing_balance.py ]]; then
    run_phase login_cta_backing_balance python3.12 tools/validate_lgo_login_cta_backing_balance.py
  fi
  if [[ -f tools/validate_lgo_login_cta_backing_evidence_refresh.py ]]; then
    run_phase login_cta_backing_evidence_refresh python3.12 tools/validate_lgo_login_cta_backing_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_runtime_ui_skin_foundation.py ]]; then
    run_phase runtime_ui_skin_foundation python3.12 tools/validate_lgo_runtime_ui_skin_foundation.py
  fi
  if [[ -f tools/validate_lgo_runtime_ui_skin_adoption_audit.py ]]; then
    run_phase runtime_ui_skin_adoption_audit python3.12 tools/validate_lgo_runtime_ui_skin_adoption_audit.py
  fi
  if [[ -f tools/validate_lgo_login_npc_grounding_shadow_balance.py ]]; then
    run_phase login_npc_grounding_shadow_balance python3.12 tools/validate_lgo_login_npc_grounding_shadow_balance.py
  fi
  if [[ -f tools/validate_lgo_login_npc_grounding_shadow_evidence_refresh.py ]]; then
    run_phase login_npc_grounding_shadow_evidence_refresh python3.12 tools/validate_lgo_login_npc_grounding_shadow_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_login_responsive_scale_cleanup.py ]]; then
    run_phase login_responsive_scale_cleanup python3.12 tools/validate_lgo_login_responsive_scale_cleanup.py
  fi
  if [[ -f tools/validate_lgo_character_hall_v3b_composition.py ]]; then
    run_phase character_hall_v3b_composition python3.12 tools/validate_lgo_character_hall_v3b_composition.py
  fi
  if [[ -f tools/validate_lgo_character_hall_style_adoption.py ]]; then
    run_phase character_hall_style_adoption python3.12 tools/validate_lgo_character_hall_style_adoption.py
  fi
  if [[ -f tools/validate_lgo_character_hall_panel_density.py ]]; then
    run_phase character_hall_panel_density python3.12 tools/validate_lgo_character_hall_panel_density.py
  fi
  if [[ -f tools/validate_lgo_character_hall_mobile_copy_density.py ]]; then
    run_phase character_hall_mobile_copy_density python3.12 tools/validate_lgo_character_hall_mobile_copy_density.py
  fi
  if [[ -f tools/validate_lgo_character_hall_mobile_copy_evidence_refresh.py ]]; then
    run_phase character_hall_mobile_copy_evidence_refresh python3.12 tools/validate_lgo_character_hall_mobile_copy_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_character_hall_mobile_selected_cta_hierarchy.py ]]; then
    run_phase character_hall_mobile_selected_cta_hierarchy python3.12 tools/validate_lgo_character_hall_mobile_selected_cta_hierarchy.py
  fi
  if [[ -f tools/validate_lgo_character_hall_mobile_selected_cta_evidence_refresh.py ]]; then
    run_phase character_hall_mobile_selected_cta_evidence_refresh python3.12 tools/validate_lgo_character_hall_mobile_selected_cta_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_character_create_form_presentation.py ]]; then
    run_phase character_create_form_presentation python3.12 tools/validate_lgo_character_create_form_presentation.py
  fi
  if [[ -f tools/validate_lgo_character_hall_responsive_evidence_refresh.py ]]; then
    run_phase character_hall_responsive_evidence_refresh python3.12 tools/validate_lgo_character_hall_responsive_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_visual_runtime_fast_profile_reuse.py ]]; then
    run_phase visual_runtime_fast_profile_reuse python3.12 tools/validate_lgo_visual_runtime_fast_profile_reuse.py
  fi
  if [[ -f tools/validate_lgo_world_hud_action_shell_v3b_skin.py ]]; then
    run_phase world_hud_action_shell_v3b_skin python3.12 tools/validate_lgo_world_hud_action_shell_v3b_skin.py
  fi
  if [[ -f tools/validate_lgo_world_hud_style_adoption.py ]]; then
    run_phase world_hud_style_adoption python3.12 tools/validate_lgo_world_hud_style_adoption.py
  fi
  if [[ -f tools/validate_lgo_runtime_ui_skin_adoption_evidence_refresh.py ]]; then
    run_phase runtime_ui_skin_adoption_evidence_refresh python3.12 tools/validate_lgo_runtime_ui_skin_adoption_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_runtime_ui_skin_usage_guide.py ]]; then
    run_phase runtime_ui_skin_usage_guide python3.12 tools/validate_lgo_runtime_ui_skin_usage_guide.py
  fi
  if [[ -f tools/validate_lgo_runtime_ui_style_duplication_audit.py ]]; then
    run_phase runtime_ui_style_duplication_audit python3.12 tools/validate_lgo_runtime_ui_style_duplication_audit.py
  fi
  if [[ -f tools/validate_lgo_runtime_ui_factory_split_review.py ]]; then
    run_phase runtime_ui_factory_split_review python3.12 tools/validate_lgo_runtime_ui_factory_split_review.py
  fi
  if [[ -f tools/validate_lgo_runtime_ui_primitive_factory.py ]]; then
    run_phase runtime_ui_primitive_factory python3.12 tools/validate_lgo_runtime_ui_primitive_factory.py
  fi
  if [[ -f tools/validate_lgo_runtime_ui_button_factory_adoption.py ]]; then
    run_phase runtime_ui_button_factory_adoption python3.12 tools/validate_lgo_runtime_ui_button_factory_adoption.py
  fi
  if [[ -f tools/validate_lgo_runtime_ui_controller_responsibility_map.py ]]; then
    run_phase runtime_ui_controller_responsibility_map python3.12 tools/validate_lgo_runtime_ui_controller_responsibility_map.py
  fi
  if [[ -f tools/validate_lgo_runtime_ui_responsive_layout_helper_review.py ]]; then
    run_phase runtime_ui_responsive_layout_helper_review python3.12 tools/validate_lgo_runtime_ui_responsive_layout_helper_review.py
  fi
  if [[ -f tools/validate_lgo_runtime_ui_responsive_constants_audit.py ]]; then
    run_phase runtime_ui_responsive_constants_audit python3.12 tools/validate_lgo_runtime_ui_responsive_constants_audit.py
  fi
  if [[ -f tools/validate_lgo_runtime_ui_responsive_session_shell_helper_review.py ]]; then
    run_phase runtime_ui_responsive_session_shell_helper_review python3.12 tools/validate_lgo_runtime_ui_responsive_session_shell_helper_review.py
  fi
  if [[ -f tools/validate_lgo_runtime_ui_factory_adoption_evidence_refresh.py ]]; then
    run_phase runtime_ui_factory_adoption_evidence_refresh python3.12 tools/validate_lgo_runtime_ui_factory_adoption_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_session_menu_setting_row_visual_polish.py ]]; then
    run_phase session_menu_setting_row_visual_polish python3.12 tools/validate_lgo_session_menu_setting_row_visual_polish.py
  fi
  if [[ -f tools/validate_lgo_session_menu_setting_row_evidence_refresh.py ]]; then
    run_phase session_menu_setting_row_evidence_refresh python3.12 tools/validate_lgo_session_menu_setting_row_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_runtime_ui_screen_shell_component_review.py ]]; then
    run_phase runtime_ui_screen_shell_component_review python3.12 tools/validate_lgo_runtime_ui_screen_shell_component_review.py
  fi
  if [[ -f tools/validate_lgo_world_pose_pulse_visual_cleanup.py ]]; then
    run_phase world_pose_pulse_visual_cleanup python3.12 tools/validate_lgo_world_pose_pulse_visual_cleanup.py
  fi
  if [[ -f tools/validate_lgo_runtime_ui_screen_shell_evidence_refresh.py ]]; then
    run_phase runtime_ui_screen_shell_evidence_refresh python3.12 tools/validate_lgo_runtime_ui_screen_shell_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_runtime_ui_action_row_component_review.py ]]; then
    run_phase runtime_ui_action_row_component_review python3.12 tools/validate_lgo_runtime_ui_action_row_component_review.py
  fi
  if [[ -f tools/validate_lgo_runtime_ui_action_row_evidence_refresh.py ]]; then
    run_phase runtime_ui_action_row_evidence_refresh python3.12 tools/validate_lgo_runtime_ui_action_row_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_runtime_ui_responsive_style_application_audit.py ]]; then
    run_phase runtime_ui_responsive_style_application_audit python3.12 tools/validate_lgo_runtime_ui_responsive_style_application_audit.py
  fi
  if [[ -f tools/validate_lgo_runtime_ui_responsive_style_evidence_refresh.py ]]; then
    run_phase runtime_ui_responsive_style_evidence_refresh python3.12 tools/validate_lgo_runtime_ui_responsive_style_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_runtime_ui_factory_coverage_audit.py ]]; then
    run_phase runtime_ui_factory_coverage_audit python3.12 tools/validate_lgo_runtime_ui_factory_coverage_audit.py
  fi
  if [[ -f tools/validate_lgo_runtime_ui_image_layer_evidence_refresh.py ]]; then
    run_phase runtime_ui_image_layer_evidence_refresh python3.12 tools/validate_lgo_runtime_ui_image_layer_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_runtime_ui_style_debt_followup_audit.py ]]; then
    run_phase runtime_ui_style_debt_followup_audit python3.12 tools/validate_lgo_runtime_ui_style_debt_followup_audit.py
  fi
  if [[ -f tools/validate_lgo_runtime_ui_compact_status_evidence_refresh.py ]]; then
    run_phase runtime_ui_compact_status_evidence_refresh python3.12 tools/validate_lgo_runtime_ui_compact_status_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_combat_button_state_readability_polish.py ]]; then
    run_phase combat_button_state_readability_polish python3.12 tools/validate_lgo_combat_button_state_readability_polish.py
  fi
  if [[ -f tools/validate_lgo_combat_button_state_evidence_refresh.py ]]; then
    run_phase combat_button_state_evidence_refresh python3.12 tools/validate_lgo_combat_button_state_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_world_hud_action_shell_evidence_refresh.py ]]; then
    run_phase world_hud_action_shell_evidence_refresh python3.12 tools/validate_lgo_world_hud_action_shell_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_world_mobile_camera_framing.py ]]; then
    run_phase world_mobile_camera_framing python3.12 tools/validate_lgo_world_mobile_camera_framing.py
  fi
  if [[ -f tools/validate_lgo_world_mobile_camera_evidence_refresh.py ]]; then
    run_phase world_mobile_camera_evidence_refresh python3.12 tools/validate_lgo_world_mobile_camera_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_world_label_safe_area.py ]]; then
    run_phase world_label_safe_area python3.12 tools/validate_lgo_world_label_safe_area.py
  fi
  if [[ -f tools/validate_lgo_world_label_safe_area_evidence_refresh.py ]]; then
    run_phase world_label_safe_area_evidence_refresh python3.12 tools/validate_lgo_world_label_safe_area_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_world_top_status_mobile_readability.py ]]; then
    run_phase world_top_status_mobile_readability python3.12 tools/validate_lgo_world_top_status_mobile_readability.py
  fi
  if [[ -f tools/validate_lgo_world_top_status_mobile_evidence_refresh.py ]]; then
    run_phase world_top_status_mobile_evidence_refresh python3.12 tools/validate_lgo_world_top_status_mobile_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_world_actor_hud_occlusion.py ]]; then
    run_phase world_actor_hud_occlusion python3.12 tools/validate_lgo_world_actor_hud_occlusion.py
  fi
  if [[ -f tools/validate_lgo_world_actor_hud_occlusion_evidence_refresh.py ]]; then
    run_phase world_actor_hud_occlusion_evidence_refresh python3.12 tools/validate_lgo_world_actor_hud_occlusion_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_world_hud_dialogue_panel_viewport_polish.py ]]; then
    run_phase world_hud_dialogue_panel_viewport_polish python3.12 tools/validate_lgo_world_hud_dialogue_panel_viewport_polish.py
  fi
  if [[ -f tools/validate_lgo_world_hud_dialogue_panel_evidence_refresh.py ]]; then
    run_phase world_hud_dialogue_panel_evidence_refresh python3.12 tools/validate_lgo_world_hud_dialogue_panel_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_world_hud_mobile_hierarchy_polish.py ]]; then
    run_phase world_hud_mobile_hierarchy_polish python3.12 tools/validate_lgo_world_hud_mobile_hierarchy_polish.py
  fi
  if [[ -f tools/validate_lgo_world_hud_mobile_hierarchy_evidence_refresh.py ]]; then
    run_phase world_hud_mobile_hierarchy_evidence_refresh python3.12 tools/validate_lgo_world_hud_mobile_hierarchy_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_source_gate_evidence_preservation.py ]]; then
    run_phase source_gate_evidence_preservation python3.12 tools/validate_lgo_source_gate_evidence_preservation.py
  fi
  if [[ -f tools/validate_lgo_visual_evidence_profile_index.py ]]; then
    run_phase visual_evidence_profile_index python3.12 tools/validate_lgo_visual_evidence_profile_index.py
  fi
  if [[ -f tools/validate_lgo_build_size_budget.py ]]; then
    run_phase build_size_budget python3.12 tools/validate_lgo_build_size_budget.py
  fi
  if [[ -f tools/validate_code_governance.py ]]; then
    run_phase clean_pycache_before_code_governance find server tests tools -type d -name __pycache__ -prune -exec rm -rf {} +
    run_phase code_governance python3.12 tools/validate_code_governance.py
  fi
  if [[ -f tools/validate_lgo_continuous_development_mode.py ]]; then
    run_phase continuous_development_mode python3.12 tools/validate_lgo_continuous_development_mode.py
  fi
  if [[ -f tools/validate_lgo_runtime_smoke_matrix.py ]]; then
    run_phase runtime_smoke_matrix python3.12 tools/validate_lgo_runtime_smoke_matrix.py
  fi
  if [[ -f tools/validate_lgo_visual_evidence_matrix.py ]]; then
    run_phase visual_evidence_matrix python3.12 tools/validate_lgo_visual_evidence_matrix.py
  fi
  if [[ -f tools/validate_lgo_crash_error_reporting_plan.py ]]; then
    run_phase crash_error_reporting_plan python3.12 tools/validate_lgo_crash_error_reporting_plan.py
  fi
  if [[ -f tools/validate_lgo_release_checklist.py ]]; then
    run_phase release_checklist python3.12 tools/validate_lgo_release_checklist.py
  fi
  if [[ -f tools/validate_lgo_asset_provenance.py ]]; then
    run_phase asset_provenance python3.12 tools/validate_lgo_asset_provenance.py
  fi
  if [[ -f tools/validate_lgo_ui_atlas_plan.py ]]; then
    run_phase ui_atlas_plan python3.12 tools/validate_lgo_ui_atlas_plan.py
  fi
  if [[ -f tools/validate_lgo_content_taxonomy.py ]]; then
    run_phase content_taxonomy python3.12 tools/validate_lgo_content_taxonomy.py
  fi
  if [[ -f tools/validate_lgo_zone_model.py ]]; then
    run_phase zone_model python3.12 tools/validate_lgo_zone_model.py
  fi
  if [[ -f tools/validate_lgo_dialogue_pipeline.py ]]; then
    run_phase dialogue_pipeline python3.12 tools/validate_lgo_dialogue_pipeline.py
  fi
  if [[ -f tools/validate_lgo_skill_effect_pipeline.py ]]; then
    run_phase skill_effect_pipeline python3.12 tools/validate_lgo_skill_effect_pipeline.py
  fi
  if [[ -f tools/validate_lgo_sprite_import_plan.py ]]; then
    run_phase sprite_import_plan python3.12 tools/validate_lgo_sprite_import_plan.py
  fi
  if [[ -f tools/validate_lgo_animation_direction.py ]]; then
    run_phase animation_direction python3.12 tools/validate_lgo_animation_direction.py
  fi
  if [[ -f tools/validate_lgo_runtime_asset_weight.py ]]; then
    run_phase runtime_asset_weight python3.12 tools/validate_lgo_runtime_asset_weight.py
  fi
  if [[ -f tools/validate_lgo_runtime_asset_size_inventory.py ]]; then
    run_phase runtime_asset_size_inventory python3.12 tools/validate_lgo_runtime_asset_size_inventory.py
  fi
  if [[ -f tools/validate_lgo_runtime_asset_weight_budget_refresh.py ]]; then
    run_phase runtime_asset_weight_budget_refresh python3.12 tools/validate_lgo_runtime_asset_weight_budget_refresh.py
  fi
  if [[ -f tools/validate_lgo_runtime_asset_watch_queue_import_profile.py ]]; then
    run_phase runtime_asset_watch_queue_import_profile python3.12 tools/validate_lgo_runtime_asset_watch_queue_import_profile.py
  fi
  if [[ -f tools/validate_lgo_runtime_asset_import_profiles.py ]]; then
    run_phase runtime_asset_import_profiles python3.12 tools/validate_lgo_runtime_asset_import_profiles.py
  fi
  if [[ -f tools/validate_lgo_device_profile_ui_budgets.py ]]; then
    run_phase device_profile_ui_budgets python3.12 tools/validate_lgo_device_profile_ui_budgets.py
  fi
  if [[ -f tools/validate_lgo_world_hud_density_mobile_touch.py ]]; then
    run_phase world_hud_density_mobile_touch python3.12 tools/validate_lgo_world_hud_density_mobile_touch.py
  fi
  if [[ -f tools/validate_lgo_world_ground_visual_quality.py ]]; then
    run_phase world_ground_visual_quality python3.12 tools/validate_lgo_world_ground_visual_quality.py
  fi
  if [[ -f tools/validate_lgo_visual_runtime_review_heuristics.py ]]; then
    run_phase visual_runtime_review_heuristics python3.12 tools/validate_lgo_visual_runtime_review_heuristics.py
  fi
  if [[ -f tools/validate_lgo_world_hub_prop_label_responsive.py ]]; then
    run_phase world_hub_prop_label_responsive python3.12 tools/validate_lgo_world_hub_prop_label_responsive.py
  fi
  if [[ -f tools/validate_lgo_world_scene_depth_layering.py ]]; then
    run_phase world_scene_depth_layering python3.12 tools/validate_lgo_world_scene_depth_layering.py
  fi
  if [[ -f tools/validate_lgo_world_hub_visual_readability_cleanup.py ]]; then
    run_phase world_hub_visual_readability_cleanup python3.12 tools/validate_lgo_world_hub_visual_readability_cleanup.py
  fi
  if [[ -f tools/validate_lgo_world_hub_visual_debt_triage.py ]]; then
    run_phase world_hub_visual_debt_triage python3.12 tools/validate_lgo_world_hub_visual_debt_triage.py
  fi
  if [[ -f tools/validate_lgo_session_menu_focus_evidence_refresh.py ]]; then
    run_phase session_menu_focus_evidence_refresh python3.12 tools/validate_lgo_session_menu_focus_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_world_hub_interaction_readability.py ]]; then
    run_phase world_hub_interaction_readability python3.12 tools/validate_lgo_world_hub_interaction_readability.py
  fi
  if [[ -f tools/validate_lgo_world_hub_interaction_evidence_refresh.py ]]; then
    run_phase world_hub_interaction_evidence_refresh python3.12 tools/validate_lgo_world_hub_interaction_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_near_interaction_checkpoint_capture.py ]]; then
    run_phase near_interaction_checkpoint_capture python3.12 tools/validate_lgo_near_interaction_checkpoint_capture.py
  fi
  if [[ -f tools/validate_lgo_near_interaction_evidence_refresh.py ]]; then
    run_phase near_interaction_evidence_refresh python3.12 tools/validate_lgo_near_interaction_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_post_login_visual_evidence_upload_packaging.py ]]; then
    run_phase post_login_visual_evidence_upload_packaging python3.12 tools/validate_lgo_post_login_visual_evidence_upload_packaging.py
  fi
  if [[ -f tools/validate_lgo_world_responsive_evidence_refresh.py ]]; then
    run_phase world_responsive_evidence_refresh python3.12 tools/validate_lgo_world_responsive_evidence_refresh.py
  fi
  if [[ -f tools/validate_lgo_telemetry_schema_plan.py ]]; then
    run_phase telemetry_schema_plan python3.12 tools/validate_lgo_telemetry_schema_plan.py
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
    tools/validate_m6_combat_input_feedback_stability.py \
    tools/validate_m6_server_combat_contract_spec.py \
    tools/validate_m6_combat_protocol_gamedata_contract.py \
    tools/validate_m6_java_server_combat_validation.py \
    tools/validate_m6_unity_combat_intent_client.py \
    tools/validate_m6_unity_java_combat_smoke.py \
    tools/validate_m6_server_authoritative_combat_closure.py \
    tools/validate_m6_local_combat_prototype.py \
    tools/validate_m6_local_combat_runtime_closure.py \
    tools/validate_m6_server_authoritative_combat_pilot.py \
    tools/validate_m6_unity_java_combat_e2e.py \
    tools/validate_m6_combat_ux_readability_polish.py \
    tools/validate_m6_combat_gamedata_balance.py \
    tools/validate_m6_combat_foundation_closure.py \
    tools/validate_m6_combat_hardening_continuation.py \
    tools/validate_lgo_art_pack_v1.py \
    tools/slice_lgo_art_pack_v1.py \
    tools/validate_lgo_art_v2_separated_assets.py \
    tools/ingest_lgo_art_v2_assets.py \
    tools/validate_lgo_art_v3_course_correction.py \
    tools/prepare_lgo_art_v3b_candidates.py \
    tools/validate_lgo_art_v3b_candidates.py \
    tools/validate_lgo_login_gate_entry_visual_v1.py \
    tools/validate_lgo_login_npc_compositing_polish.py \
    tools/validate_lgo_login_panel_visual_balance.py \
    tools/validate_lgo_login_cta_ornament_lightweight.py \
    tools/validate_lgo_login_cta_debug_dot_cleanup.py \
    tools/validate_lgo_login_cta_debug_dot_evidence_refresh.py \
    tools/validate_lgo_login_cta_backing_balance.py \
    tools/validate_lgo_login_cta_backing_evidence_refresh.py \
    tools/validate_lgo_runtime_ui_skin_foundation.py \
    tools/validate_lgo_runtime_ui_skin_adoption_audit.py \
    tools/validate_lgo_login_npc_grounding_shadow_balance.py \
    tools/validate_lgo_login_npc_grounding_shadow_evidence_refresh.py \
    tools/validate_lgo_login_responsive_scale_cleanup.py \
    tools/validate_lgo_character_hall_v3b_composition.py \
    tools/validate_lgo_character_hall_style_adoption.py \
    tools/validate_lgo_character_hall_panel_density.py \
    tools/validate_lgo_character_hall_mobile_copy_density.py \
    tools/validate_lgo_character_hall_mobile_copy_evidence_refresh.py \
    tools/validate_lgo_character_hall_mobile_selected_cta_hierarchy.py \
    tools/validate_lgo_character_hall_mobile_selected_cta_evidence_refresh.py \
    tools/validate_lgo_character_create_form_presentation.py \
    tools/validate_lgo_character_hall_responsive_evidence_refresh.py \
    tools/validate_lgo_visual_runtime_fast_profile_reuse.py \
    tools/validate_lgo_world_hud_action_shell_v3b_skin.py \
    tools/validate_lgo_world_hud_style_adoption.py \
    tools/validate_lgo_runtime_ui_skin_adoption_evidence_refresh.py \
    tools/validate_lgo_runtime_ui_skin_usage_guide.py \
    tools/validate_lgo_runtime_ui_style_duplication_audit.py \
    tools/validate_lgo_runtime_ui_factory_split_review.py \
    tools/validate_lgo_runtime_ui_primitive_factory.py \
    tools/validate_lgo_runtime_ui_button_factory_adoption.py \
    tools/validate_lgo_runtime_ui_controller_responsibility_map.py \
    tools/validate_lgo_runtime_ui_responsive_layout_helper_review.py \
    tools/validate_lgo_runtime_ui_responsive_constants_audit.py \
    tools/validate_lgo_runtime_ui_responsive_session_shell_helper_review.py \
    tools/validate_lgo_runtime_ui_factory_adoption_evidence_refresh.py \
    tools/validate_lgo_session_menu_setting_row_visual_polish.py \
    tools/validate_lgo_session_menu_setting_row_evidence_refresh.py \
    tools/validate_lgo_runtime_ui_screen_shell_component_review.py \
    tools/validate_lgo_world_pose_pulse_visual_cleanup.py \
    tools/validate_lgo_runtime_ui_screen_shell_evidence_refresh.py \
    tools/validate_lgo_runtime_ui_action_row_component_review.py \
    tools/validate_lgo_runtime_ui_action_row_evidence_refresh.py \
    tools/validate_lgo_runtime_ui_responsive_style_application_audit.py \
    tools/validate_lgo_runtime_ui_responsive_style_evidence_refresh.py \
    tools/validate_lgo_runtime_ui_factory_coverage_audit.py \
    tools/validate_lgo_runtime_ui_image_layer_evidence_refresh.py \
    tools/validate_lgo_runtime_ui_style_debt_followup_audit.py \
    tools/validate_lgo_runtime_ui_compact_status_evidence_refresh.py \
    tools/validate_lgo_combat_button_state_readability_polish.py \
    tools/validate_lgo_combat_button_state_evidence_refresh.py \
    tools/validate_lgo_world_hud_action_shell_evidence_refresh.py \
    tools/validate_lgo_world_mobile_camera_framing.py \
    tools/validate_lgo_world_mobile_camera_evidence_refresh.py \
    tools/validate_lgo_world_label_safe_area.py \
    tools/validate_lgo_world_label_safe_area_evidence_refresh.py \
    tools/validate_lgo_world_top_status_mobile_readability.py \
    tools/validate_lgo_world_top_status_mobile_evidence_refresh.py \
    tools/validate_lgo_world_actor_hud_occlusion.py \
    tools/validate_lgo_world_actor_hud_occlusion_evidence_refresh.py \
    tools/validate_lgo_world_hud_dialogue_panel_viewport_polish.py \
    tools/validate_lgo_world_hud_dialogue_panel_evidence_refresh.py \
    tools/validate_lgo_world_hud_mobile_hierarchy_polish.py \
    tools/validate_lgo_world_hud_mobile_hierarchy_evidence_refresh.py \
    tools/validate_lgo_source_gate_evidence_preservation.py \
    tools/report_lgo_visual_evidence_profile_index.py \
    tools/validate_lgo_visual_evidence_profile_index.py \
    tools/report_lgo_build_size_budget.py \
    tools/validate_lgo_build_size_budget.py \
    tools/lgo_continuous_cycle.py \
    tools/lgo_next_task.py \
    tools/lgo_worktree_audit.py \
    tools/validate_lgo_continuous_development_mode.py \
    tools/lgo_runtime_smoke_matrix.py \
    tools/validate_lgo_runtime_smoke_matrix.py \
    tools/lgo_visual_evidence_matrix.py \
    tools/validate_lgo_visual_evidence_matrix.py \
    tools/lgo_error_report_summary.py \
    tools/validate_lgo_crash_error_reporting_plan.py \
    tools/validate_lgo_release_checklist.py \
    tools/validate_lgo_asset_provenance.py \
    tools/validate_lgo_ui_atlas_plan.py \
    tools/validate_lgo_content_taxonomy.py \
    tools/validate_lgo_zone_model.py \
    tools/validate_lgo_dialogue_pipeline.py \
    tools/validate_lgo_skill_effect_pipeline.py \
    tools/validate_lgo_sprite_import_plan.py \
    tools/validate_lgo_animation_direction.py \
    tools/validate_lgo_runtime_asset_weight.py \
    tools/validate_lgo_runtime_asset_size_inventory.py \
    tools/validate_lgo_runtime_asset_weight_budget_refresh.py \
    tools/report_lgo_runtime_asset_watch_queue.py \
    tools/validate_lgo_runtime_asset_watch_queue_import_profile.py \
    tools/validate_lgo_runtime_asset_import_profiles.py \
    tools/validate_lgo_device_profile_ui_budgets.py \
    tools/validate_lgo_world_hud_density_mobile_touch.py \
    tools/validate_lgo_world_ground_visual_quality.py \
    tools/analyze_lgo_visual_runtime_evidence.py \
    tools/validate_lgo_visual_runtime_review_heuristics.py \
    tools/validate_lgo_world_hub_prop_label_responsive.py \
    tools/validate_lgo_world_scene_depth_layering.py \
    tools/validate_lgo_world_hub_visual_readability_cleanup.py \
    tools/validate_lgo_world_hub_visual_debt_triage.py \
    tools/validate_lgo_session_menu_focus_evidence_refresh.py \
    tools/validate_lgo_world_hub_interaction_readability.py \
    tools/validate_lgo_world_hub_interaction_evidence_refresh.py \
    tools/validate_lgo_near_interaction_checkpoint_capture.py \
    tools/validate_lgo_near_interaction_evidence_refresh.py \
    tools/package_lgo_visual_evidence_upload.py \
    tools/validate_lgo_post_login_visual_evidence_upload_packaging.py \
    tools/validate_lgo_world_responsive_evidence_refresh.py \
    tools/validate_lgo_telemetry_schema_plan.py \
    tools/validate_code_governance.py \
    tools/m4_playable_vertical_slice_runtime.py \
    tools/m4_visual_foundation_runtime.py \
    tools/m5_first_playable_loop_runtime.py \
    tools/m5_guided_training_loop_runtime.py \
    tools/m5_lightweight_dialogue_runtime.py
  run_phase clean_pycache_after_python_compile find server tests tools -type d -name __pycache__ -prune -exec rm -rf {} +
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
  if [[ -f tools/run_m6_unity_combat_intent_client_once.sh ]]; then
    run_phase m6_unity_combat_intent_client_runtime ./tools/run_m6_unity_combat_intent_client_once.sh --unity-player "$macos_player"
    if ! grep -R "M6_UNITY_COMBAT_INTENT_CLIENT_RUNTIME_SMOKE_PASS" "$ROOT/build" >/dev/null; then
      log "LGO_PLAYABLE_CLOSURE_FIX_REQUIRED"
      echo "ERROR: M6 Unity combat intent client runtime marker not observed" >&2
      exit 48
    fi
  fi
  if [[ -f tools/run_m6_unity_java_combat_smoke.sh ]]; then
    run_phase m6_unity_java_combat_runtime ./tools/run_m6_unity_java_combat_smoke.sh --unity-player "$macos_player" --port 17843
    if ! grep -R "M6_UNITY_JAVA_COMBAT_SMOKE_PASS" "$ROOT/build" >/dev/null; then
      log "LGO_PLAYABLE_CLOSURE_FIX_REQUIRED"
      echo "ERROR: M6 Unity Java combat smoke marker not observed" >&2
      exit 49
    fi
  fi
  if [[ -f tools/validate_m6_local_combat_runtime_closure.py ]]; then
    log "M6_LOCAL_COMBAT_RUNTIME_CLOSURE_PASS_v0.50.0"
  fi
  if [[ -f tools/run_m6_server_authoritative_combat_pilot.sh ]]; then
    run_phase m6_server_authoritative_combat_pilot_runtime ./tools/run_m6_server_authoritative_combat_pilot.sh
  fi
  if [[ -f tools/run_m6_unity_java_combat_e2e.sh ]]; then
    run_phase m6_unity_java_combat_e2e_runtime ./tools/run_m6_unity_java_combat_e2e.sh --unity-player "$macos_player" --port 17844
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
  if [[ -f tools/validate_m6_combat_input_feedback_stability.py ]]; then
    run_phase m6_combat_input_feedback_stability python3.12 tools/validate_m6_combat_input_feedback_stability.py
  fi
  if [[ -f tools/validate_m6_server_combat_contract_spec.py ]]; then
    run_phase m6_server_combat_contract_spec python3.12 tools/validate_m6_server_combat_contract_spec.py
  fi
  if [[ -f tools/validate_m6_combat_protocol_gamedata_contract.py ]]; then
    run_phase m6_combat_protocol_gamedata_contract python3.12 tools/validate_m6_combat_protocol_gamedata_contract.py
  fi
  if [[ -f tools/validate_m6_java_server_combat_validation.py ]]; then
    run_phase m6_java_server_combat_validation python3.12 tools/validate_m6_java_server_combat_validation.py
  fi
  if [[ -f tools/validate_m6_unity_combat_intent_client.py ]]; then
    run_phase m6_unity_combat_intent_client python3.12 tools/validate_m6_unity_combat_intent_client.py
  fi
  if [[ -f tools/validate_m6_unity_java_combat_smoke.py ]]; then
    run_phase m6_unity_java_combat_smoke python3.12 tools/validate_m6_unity_java_combat_smoke.py
  fi
  if [[ -f tools/validate_m6_server_authoritative_combat_closure.py ]]; then
    run_phase m6_server_authoritative_combat_closure python3.12 tools/validate_m6_server_authoritative_combat_closure.py
  fi
  if [[ -f tools/validate_m6_local_combat_prototype.py ]]; then
    run_phase m6_local_combat_prototype python3.12 tools/validate_m6_local_combat_prototype.py
  fi
  if [[ -f tools/validate_m6_local_combat_runtime_closure.py ]]; then
    run_phase m6_local_combat_runtime_closure python3.12 tools/validate_m6_local_combat_runtime_closure.py
  fi
  if [[ -f tools/validate_m6_server_authoritative_combat_pilot.py ]]; then
    run_phase m6_server_authoritative_combat_pilot python3.12 tools/validate_m6_server_authoritative_combat_pilot.py
  fi
  if [[ -f tools/validate_m6_unity_java_combat_e2e.py ]]; then
    run_phase m6_unity_java_combat_e2e python3.12 tools/validate_m6_unity_java_combat_e2e.py
  fi
  if [[ -f tools/validate_m6_combat_ux_readability_polish.py ]]; then
    run_phase m6_combat_ux_readability_polish python3.12 tools/validate_m6_combat_ux_readability_polish.py
  fi
  if [[ -f tools/validate_m6_combat_gamedata_balance.py ]]; then
    run_phase m6_combat_gamedata_balance python3.12 tools/validate_m6_combat_gamedata_balance.py
  fi
  if [[ -f tools/validate_m6_combat_foundation_closure.py ]]; then
    run_phase m6_combat_foundation_closure python3.12 tools/validate_m6_combat_foundation_closure.py
  fi
  if [[ -f tools/validate_m6_combat_hardening_continuation.py ]]; then
    run_phase m6_combat_hardening_continuation python3.12 tools/validate_m6_combat_hardening_continuation.py
  fi
  if [[ -f tools/validate_lgo_art_pack_v1.py ]]; then
    run_phase lgo_art_pack_v1 python3.12 tools/validate_lgo_art_pack_v1.py
  fi
  if [[ -f tools/validate_lgo_art_v2_separated_assets.py ]]; then
    run_phase lgo_art_v2_separated_assets python3.12 tools/validate_lgo_art_v2_separated_assets.py
  fi
  if [[ -f tools/validate_lgo_art_v3_course_correction.py ]]; then
    run_phase lgo_art_v3_course_correction python3.12 tools/validate_lgo_art_v3_course_correction.py
  fi
  if [[ -f tools/validate_lgo_art_v3b_candidates.py ]]; then
    run_phase lgo_art_v3b_candidates python3.12 tools/validate_lgo_art_v3b_candidates.py
  fi
  if [[ -f tools/validate_lgo_login_gate_entry_visual_v1.py ]]; then
    run_phase login_gate_entry_visual python3.12 tools/validate_lgo_login_gate_entry_visual_v1.py
  fi
  if [[ -f tools/validate_lgo_login_npc_compositing_polish.py ]]; then
    run_phase login_npc_compositing_polish python3.12 tools/validate_lgo_login_npc_compositing_polish.py
  fi
  if [[ -f tools/validate_lgo_character_hall_v3b_composition.py ]]; then
    run_phase character_hall_v3b_composition python3.12 tools/validate_lgo_character_hall_v3b_composition.py
  fi
  if [[ -f tools/validate_lgo_build_size_budget.py ]]; then
    run_phase build_size_budget python3.12 tools/validate_lgo_build_size_budget.py
  fi
  if [[ -f tools/validate_code_governance.py ]]; then
    run_phase clean_pycache_before_code_governance find server tests tools -type d -name __pycache__ -prune -exec rm -rf {} +
    run_phase code_governance python3.12 tools/validate_code_governance.py
  fi
  if [[ -f tools/validate_lgo_continuous_development_mode.py ]]; then
    run_phase continuous_development_mode python3.12 tools/validate_lgo_continuous_development_mode.py
  fi
  if [[ -f tools/validate_lgo_runtime_smoke_matrix.py ]]; then
    run_phase runtime_smoke_matrix python3.12 tools/validate_lgo_runtime_smoke_matrix.py
  fi
  if [[ -f tools/validate_lgo_visual_evidence_matrix.py ]]; then
    run_phase visual_evidence_matrix python3.12 tools/validate_lgo_visual_evidence_matrix.py
  fi
  if [[ -f tools/validate_lgo_crash_error_reporting_plan.py ]]; then
    run_phase crash_error_reporting_plan python3.12 tools/validate_lgo_crash_error_reporting_plan.py
  fi
  if [[ -f tools/validate_lgo_release_checklist.py ]]; then
    run_phase release_checklist python3.12 tools/validate_lgo_release_checklist.py
  fi
  if [[ -f tools/validate_lgo_asset_provenance.py ]]; then
    run_phase asset_provenance python3.12 tools/validate_lgo_asset_provenance.py
  fi
  if [[ -f tools/validate_lgo_ui_atlas_plan.py ]]; then
    run_phase ui_atlas_plan python3.12 tools/validate_lgo_ui_atlas_plan.py
  fi
  if [[ -f tools/validate_lgo_content_taxonomy.py ]]; then
    run_phase content_taxonomy python3.12 tools/validate_lgo_content_taxonomy.py
  fi
  if [[ -f tools/validate_lgo_zone_model.py ]]; then
    run_phase zone_model python3.12 tools/validate_lgo_zone_model.py
  fi
  if [[ -f tools/validate_lgo_dialogue_pipeline.py ]]; then
    run_phase dialogue_pipeline python3.12 tools/validate_lgo_dialogue_pipeline.py
  fi
  if [[ -f tools/validate_lgo_skill_effect_pipeline.py ]]; then
    run_phase skill_effect_pipeline python3.12 tools/validate_lgo_skill_effect_pipeline.py
  fi
  if [[ -f tools/validate_lgo_sprite_import_plan.py ]]; then
    run_phase sprite_import_plan python3.12 tools/validate_lgo_sprite_import_plan.py
  fi
  if [[ -f tools/validate_lgo_animation_direction.py ]]; then
    run_phase animation_direction python3.12 tools/validate_lgo_animation_direction.py
  fi
  if [[ -f tools/validate_lgo_runtime_asset_weight.py ]]; then
    run_phase runtime_asset_weight python3.12 tools/validate_lgo_runtime_asset_weight.py
  fi
  if [[ -f tools/validate_lgo_runtime_asset_size_inventory.py ]]; then
    run_phase runtime_asset_size_inventory python3.12 tools/validate_lgo_runtime_asset_size_inventory.py
  fi
  if [[ -f tools/validate_lgo_runtime_asset_import_profiles.py ]]; then
    run_phase runtime_asset_import_profiles python3.12 tools/validate_lgo_runtime_asset_import_profiles.py
  fi
  if [[ -f tools/validate_lgo_world_hud_density_mobile_touch.py ]]; then
    run_phase world_hud_density_mobile_touch python3.12 tools/validate_lgo_world_hud_density_mobile_touch.py
  fi
  if [[ -f tools/validate_lgo_world_ground_visual_quality.py ]]; then
    run_phase world_ground_visual_quality python3.12 tools/validate_lgo_world_ground_visual_quality.py
  fi
  if [[ -f tools/validate_lgo_telemetry_schema_plan.py ]]; then
    run_phase telemetry_schema_plan python3.12 tools/validate_lgo_telemetry_schema_plan.py
  fi
  run_phase package_hygiene python3.12 tools/validate_package_hygiene.py
  log "LGO_PLAYABLE_CLOSURE_PACKAGE_READY"
  write_json "PASS" "package gates pass"
}

visual_evidence() {
  check_repo_root
  log "LGO_PLAYABLE_CLOSURE_MODE visual-evidence"
  run_phase m5_visual_evidence_source python3.12 tools/validate_m5_visual_evidence.py
  run_phase visual_runtime_review ./tools/lgo_visual_runtime_review.sh
  if grep -q "VISUAL_RUNTIME_SCREENSHOT_UNAVAILABLE" "$ROOT/build/visual-evidence/latest/visual-runtime-evidence-manifest.json"; then
    log "LGO_PLAYABLE_VISUAL_RUNTIME_SCREENSHOT_UNAVAILABLE"
  fi
  log "LGO_PLAYABLE_VISUAL_RUNTIME_EVIDENCE_READY"
  write_json "PASS" "visual runtime evidence ready"
}

case "$MODE" in
  --source-only) source_only ;;
  --runtime) runtime_mode ;;
  --package-ready) package_ready ;;
  --visual-evidence) visual_evidence ;;
esac
