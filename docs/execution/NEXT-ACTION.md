# Linh Giới Online — Next Action

Last updated: `2026-09-05`

## Current focus

Post-login visual runtime hardening plus device-profile asset governance. Login has been upgraded to V3B-aligned runtime presentation, source-level post-login readability polish is implemented, and mobile/tablet/PC runtime asset profile budgets are now documented and validated. The standalone visual evidence harness now captures all seven screenshots, can continue in background, and auto-finishes the Unity Player after manifest completion so the operator should not need to click or close the player by hand. Character lobby usability, in-world HUD presentation, world hub scene readability, and desktop/tablet/mobile responsive evidence are now verified with fresh runtime screenshots. Login-to-character copy has been cleaned of player-facing dev wording, status chips now read correctly in runtime screenshots, the session menu/settings shell is responsive without tablet/mobile clipping, in-world interaction affordance now has stateful target labels, runtime asset size inventory is documented/validated, V3B runtime candidates now carry platform-specific Unity import profiles for Standalone/Android/iPhone, the in-world HUD is more compact/touch-oriented across desktop/tablet/mobile and now groups world guidance/action content into V3B-styled shell cards, the world hub camera now uses viewport-aware orthographic framing so mobile/tablet actors read larger than the fixed desktop view, and refreshed profile screenshots confirm mobile actor scale is improved without harmful cropping. The world hub ground now uses a lightweight procedural cultivation-platform texture instead of a debug-like grid, the login first screen now uses a V3B composition with a centered text logo/CTA cluster plus right-side Gate Keeper on desktop/tablet and a compact logo/CTA layout on mobile, the Character Hall now uses a V3B cultivator portrait with a mobile-specific two-zone lobby layout, lighter panel density, a game-facing create form with framed `Danh xưng` input and `Tạo tu sĩ` CTA, and refreshed desktop/tablet/mobile runtime screenshots, build-size budget reporting now separates Unity runtime payload from repository/reference/tooling weight, the visual evidence loop now writes PNG heuristics for checkpoint presence/dimensions/byte size/pixel variation/duplicate-frame detection, world-hub labels now show by guided state/proximity instead of cluttering the whole scene, the login CTA stack now uses a lighter dark-glass panel to reduce cyan/gold glare while retaining the V3B logo/button language, and shared runtime UI styling now lives in `RuntimeUiSkin` so repeated glass panel, framed row, compact button, and login backing rules can be reused instead of copy-written per screen. The World Hub now has lightweight procedural grounding shadows under the player, interactables, target dummy, warning slime, Spirit Gate, and key props. The visual review script now has a build-once/reuse-player profile wrapper for desktop/tablet/mobile screenshot refreshes. Source-only gates now preserve runtime evidence directories under `LGO_SOURCE_GATE_EVIDENCE_PRESERVATION_READY`.

Autopilot operating rule: when a task or phase is truly closed by its required gates, continue to the next roadmap-valid task/phase instead of stopping at the phase boundary. Stop only for a real blocker, unavailable runtime/tooling, required owner decision, or frozen contract/protocol/schema/ADR change.

## Next task

`LGO-RUNTIME-UI-PRIMITIVE-FACTORY-PASS-v1.0`

Introduce a small stateless UI factory for repeated leaf widgets after the factory split review. Marker: `LGO_RUNTIME_UI_FACTORY_SPLIT_REVIEW_READY`. Keep flow logic in `M4PlayableClientController`, keep style ownership in `RuntimeUiSkin`, and migrate only safe repeated construction helpers.

## Current blocker

No active blocker for source work. Visual runtime capture is currently available in this environment and has completed multiple consecutive rounds without manual player close.

Evidence:

- `tools/validate_lgo_device_profile_ui_budgets.py`
- `docs/tasks/LGO-MOBILE-TABLET-UI-PROFILE-HARDENING-v1.0.md`
- `docs/tasks/LGO-VISUAL-CAPTURE-TIMEOUT-HARDENING-v1.0.md`
- `docs/tasks/LGO-CHARACTER-LOBBY-VISUAL-POLISH-v1.0.md`
- `docs/tasks/LGO-WORLD-HUD-PLAYABLE-PRESENTATION-POLISH-v1.0.md`
- `docs/tasks/LGO-WORLD-HUB-SCENE-PRESENTATION-POLISH-v1.0.md`
- `docs/tasks/LGO-WORLD-GROUND-VISUAL-QUALITY-PASS-v1.0.md`
- `docs/tasks/LGO-LOGIN-NPC-COMPOSITING-POLISH-v1.0.md`
- `docs/tasks/LGO-CHARACTER-HALL-V3B-COMPOSITION-POLISH-v1.0.md`
- `docs/tasks/LGO-BUILD-SIZE-BUDGET-AND-CLEANUP-PASS-v1.0.md`
- `docs/tasks/LGO-VISUAL-RUNTIME-REVIEW-HEURISTICS-PASS-v1.0.md`
- `docs/tasks/LGO-WORLD-HUB-PROP-LABEL-RESPONSIVE-PASS-v1.0.md`
- `docs/tasks/LGO-LOGIN-PANEL-VISUAL-BALANCE-PASS-v1.0.md`
- `docs/tasks/LGO-CHARACTER-HALL-PANEL-DENSITY-PASS-v1.0.md`
- `docs/tasks/LGO-WORLD-SCENE-DEPTH-LAYERING-PASS-v1.0.md`
- `docs/tasks/LGO-WORLD-RESPONSIVE-EVIDENCE-REFRESH-v1.0.md`
- `docs/tasks/LGO-LOGIN-CTA-ORNAMENT-LIGHTWEIGHT-PASS-v1.0.md`
- `docs/tasks/LGO-CHARACTER-CREATE-FORM-PRESENTATION-PASS-v1.0.md`
- `docs/tasks/LGO-CHARACTER-HALL-RESPONSIVE-EVIDENCE-REFRESH-v1.0.md`
- `docs/tasks/LGO-VISUAL-RUNTIME-FAST-PROFILE-REUSE-PASS-v1.0.md`
- `docs/tasks/LGO-WORLD-HUD-ACTION-SHELL-V3B-SKIN-PASS-v1.0.md`
- `docs/tasks/LGO-WORLD-HUD-ACTION-SHELL-EVIDENCE-REFRESH-v1.0.md`
- `docs/tasks/LGO-WORLD-MOBILE-CAMERA-FRAMING-PASS-v1.0.md`
- `docs/tasks/LGO-WORLD-MOBILE-CAMERA-EVIDENCE-REFRESH-v1.0.md`
- `docs/tasks/LGO-WORLD-LABEL-SAFE-AREA-PASS-v1.0.md`
- `docs/tasks/LGO-WORLD-LABEL-SAFE-AREA-EVIDENCE-REFRESH-v1.0.md`
- `docs/tasks/LGO-WORLD-TOP-STATUS-MOBILE-READABILITY-PASS-v1.0.md`
- `docs/tasks/LGO-WORLD-TOP-STATUS-MOBILE-EVIDENCE-REFRESH-v1.0.md`
- `docs/tasks/LGO-WORLD-ACTOR-HUD-OCCLUSION-PASS-v1.0.md`
- `docs/tasks/LGO-WORLD-ACTOR-HUD-OCCLUSION-EVIDENCE-REFRESH-v1.0.md`
- `docs/tasks/LGO-WORLD-HUD-DIALOGUE-PANEL-VIEWPORT-POLISH-v1.0.md`
- `docs/tasks/LGO-WORLD-HUD-DIALOGUE-PANEL-EVIDENCE-REFRESH-v1.0.md`
- `docs/tasks/LGO-WORLD-HUD-MOBILE-HIERARCHY-POLISH-v1.0.md`
- `docs/tasks/LGO-WORLD-HUD-MOBILE-HIERARCHY-EVIDENCE-REFRESH-v1.0.md`
- `build/visual-evidence/latest/player.log`
- `build/visual-evidence/latest/unity-build.log`
- `build/codex-autopilot/status.json`

Next allowed action: introduce a leaf-level UI factory after reviewing factory split boundaries while keeping generated captures and package artifacts out of source control. The visual evidence harness records explicit review checklist categories and machine-readable heuristics for every checkpoint under marker `LGO_VISUAL_RUNTIME_REVIEW_HEURISTICS_READY`; world-hub label responsiveness is tracked under `LGO_WORLD_HUB_PROP_LABEL_RESPONSIVE_READY`; world hub visual staging is tracked under `LGO_WORLD_HUB_VISUAL_READABILITY_CLEANUP_READY`; world hub interaction readability is tracked under `LGO_WORLD_HUB_INTERACTION_READABILITY_READY`; world hub interaction evidence refresh is tracked under `LGO_WORLD_HUB_INTERACTION_EVIDENCE_REFRESH_READY`; near-interaction capture coverage is tracked under `LGO_NEAR_INTERACTION_CHECKPOINT_CAPTURE_READY`; near-interaction evidence refresh is tracked under `LGO_NEAR_INTERACTION_EVIDENCE_REFRESH_READY`; visual evidence upload packaging is tracked under `LGO_POST_LOGIN_VISUAL_EVIDENCE_UPLOAD_READY`; runtime asset budget refresh is tracked under `LGO_RUNTIME_ASSET_WEIGHT_BUDGET_REFRESH_READY`; runtime asset watch queue/profile polish is tracked under `LGO_RUNTIME_ASSET_WATCH_QUEUE_IMPORT_PROFILE_READY`; visual debt triage is tracked under `LGO_WORLD_HUB_VISUAL_DEBT_TRIAGE_READY`; session-menu focus evidence is tracked under `LGO_SESSION_MENU_FOCUS_EVIDENCE_REFRESH_READY`; Character Hall mobile density is tracked under `LGO_CHARACTER_HALL_MOBILE_COPY_DENSITY_READY`; Character Hall mobile evidence is tracked under `LGO_CHARACTER_HALL_MOBILE_COPY_EVIDENCE_REFRESH_READY`; Character Hall mobile selected CTA hierarchy is tracked under `LGO_CHARACTER_HALL_MOBILE_SELECTED_CTA_HIERARCHY_READY`; Character Hall selected CTA evidence is tracked under `LGO_CHARACTER_HALL_MOBILE_SELECTED_CTA_EVIDENCE_REFRESH_READY`; Character Hall style adoption is tracked under `LGO_CHARACTER_HALL_STYLE_ADOPTION_READY`; World HUD style adoption is tracked under `LGO_WORLD_HUD_STYLE_ADOPTION_READY`; runtime UI skin adoption evidence refresh is tracked under `LGO_RUNTIME_UI_SKIN_ADOPTION_EVIDENCE_REFRESH_READY`; runtime UI skin usage guide is tracked under `LGO_RUNTIME_UI_SKIN_USAGE_GUIDE_READY`; runtime UI style duplication audit is tracked under `LGO_RUNTIME_UI_STYLE_DUPLICATION_AUDIT_READY`; runtime UI factory split review is tracked under `LGO_RUNTIME_UI_FACTORY_SPLIT_REVIEW_READY`; login debug-dot cleanup is tracked under `LGO_LOGIN_CTA_DEBUG_DOT_CLEANUP_READY`; login debug-dot evidence is tracked under `LGO_LOGIN_CTA_DEBUG_DOT_EVIDENCE_REFRESH_READY`; login CTA backing balance is tracked under `LGO_LOGIN_CTA_BACKING_BALANCE_READY`; login CTA backing evidence is tracked under `LGO_LOGIN_CTA_BACKING_EVIDENCE_REFRESH_READY`; runtime UI skin foundation is tracked under `LGO_RUNTIME_UI_SKIN_FOUNDATION_READY`; runtime UI skin adoption audit is tracked under `LGO_RUNTIME_UI_SKIN_ADOPTION_AUDIT_READY`; login NPC grounding shadow balance is tracked under `LGO_LOGIN_NPC_GROUNDING_SHADOW_BALANCE_READY`; login NPC grounding evidence is tracked under `LGO_LOGIN_NPC_GROUNDING_SHADOW_EVIDENCE_REFRESH_READY`; the project still refuses to claim visual PASS from capture/build alone.

## Ready Marker Registry

This registry keeps historical source gates discoverable while `Next task` points only at the current task.

- `LGO-LOGIN-PANEL-VISUAL-BALANCE-PASS-v1.0` / `LGO_LOGIN_PANEL_VISUAL_BALANCE_READY`
- `LGO-LOGIN-CTA-ORNAMENT-LIGHTWEIGHT-PASS-v1.0` / `LGO_LOGIN_CTA_ORNAMENT_LIGHTWEIGHT_READY`
- `LGO-LOGIN-RESPONSIVE-SCALE-CLEANUP-PASS-v1.0` / `LGO_LOGIN_RESPONSIVE_SCALE_CLEANUP_READY`
- `LGO-CHARACTER-HALL-PANEL-DENSITY-PASS-v1.0` / `LGO_CHARACTER_HALL_PANEL_DENSITY_READY`
- `LGO-CHARACTER-CREATE-FORM-PRESENTATION-PASS-v1.0` / `LGO_CHARACTER_CREATE_FORM_PRESENTATION_READY`
- `LGO-CHARACTER-HALL-RESPONSIVE-EVIDENCE-REFRESH-v1.0` / `LGO_CHARACTER_HALL_RESPONSIVE_EVIDENCE_REFRESH_READY`
- `LGO-VISUAL-RUNTIME-FAST-PROFILE-REUSE-PASS-v1.0` / `LGO_VISUAL_RUNTIME_FAST_PROFILE_REUSE_READY`
- `LGO-WORLD-HUD-ACTION-SHELL-V3B-SKIN-PASS-v1.0` / `LGO_WORLD_HUD_ACTION_SHELL_V3B_SKIN_READY`
- `LGO-WORLD-HUD-ACTION-SHELL-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_HUD_ACTION_SHELL_EVIDENCE_REFRESH_READY`
- `LGO-WORLD-MOBILE-CAMERA-FRAMING-PASS-v1.0` / `LGO_WORLD_MOBILE_CAMERA_FRAMING_READY`
- `LGO-WORLD-MOBILE-CAMERA-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_MOBILE_CAMERA_EVIDENCE_REFRESH_READY`
- `LGO-WORLD-LABEL-SAFE-AREA-PASS-v1.0` / `LGO_WORLD_LABEL_SAFE_AREA_READY`
- `LGO-WORLD-LABEL-SAFE-AREA-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_LABEL_SAFE_AREA_EVIDENCE_REFRESH_READY`
- `LGO-WORLD-TOP-STATUS-MOBILE-READABILITY-PASS-v1.0` / `LGO_WORLD_TOP_STATUS_MOBILE_READABILITY_READY`
- `LGO-WORLD-TOP-STATUS-MOBILE-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_TOP_STATUS_MOBILE_EVIDENCE_REFRESH_READY`
- `LGO-WORLD-ACTOR-HUD-OCCLUSION-PASS-v1.0` / `LGO_WORLD_ACTOR_HUD_OCCLUSION_READY`
- `LGO-WORLD-ACTOR-HUD-OCCLUSION-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_ACTOR_HUD_OCCLUSION_EVIDENCE_REFRESH_READY`
- `LGO-WORLD-HUD-DIALOGUE-PANEL-VIEWPORT-POLISH-v1.0` / `LGO_WORLD_HUD_DIALOGUE_PANEL_VIEWPORT_POLISH_READY`
- `LGO-WORLD-HUD-DIALOGUE-PANEL-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_HUD_DIALOGUE_PANEL_EVIDENCE_REFRESH_READY`
- `LGO-WORLD-HUD-MOBILE-HIERARCHY-POLISH-v1.0` / `LGO_WORLD_HUD_MOBILE_HIERARCHY_POLISH_READY`
- `LGO-WORLD-HUD-MOBILE-HIERARCHY-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_HUD_MOBILE_HIERARCHY_EVIDENCE_REFRESH_READY`
- `LGO-SOURCE-GATE-EVIDENCE-PRESERVATION-PASS-v1.0` / `LGO_SOURCE_GATE_EVIDENCE_PRESERVATION_READY`
- `LGO-VISUAL-EVIDENCE-PROFILE-INDEX-PASS-v1.0` / `LGO_VISUAL_EVIDENCE_PROFILE_INDEX_READY`
- `LGO-VISUAL-RUNTIME-REVIEW-HEURISTICS-PASS-v1.0` / `LGO_VISUAL_RUNTIME_REVIEW_HEURISTICS_READY`
- `LGO-WORLD-HUB-PROP-LABEL-RESPONSIVE-PASS-v1.0` / `LGO_WORLD_HUB_PROP_LABEL_RESPONSIVE_READY`
- `LGO-WORLD-SCENE-DEPTH-LAYERING-PASS-v1.0` / `LGO_WORLD_SCENE_DEPTH_LAYERING_READY`
- `LGO-WORLD-RESPONSIVE-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_RESPONSIVE_EVIDENCE_REFRESH_READY`
- `LGO-WORLD-HUB-VISUAL-READABILITY-CLEANUP-PASS-v1.0` / `LGO_WORLD_HUB_VISUAL_READABILITY_CLEANUP_READY`
- `LGO-WORLD-HUB-INTERACTION-READABILITY-PASS-v1.0` / `LGO_WORLD_HUB_INTERACTION_READABILITY_READY`
- `LGO-WORLD-HUB-INTERACTION-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_HUB_INTERACTION_EVIDENCE_REFRESH_READY`
- `LGO-NEAR-INTERACTION-CHECKPOINT-CAPTURE-PASS-v1.0` / `LGO_NEAR_INTERACTION_CHECKPOINT_CAPTURE_READY`
- `LGO-NEAR-INTERACTION-EVIDENCE-REFRESH-v1.0` / `LGO_NEAR_INTERACTION_EVIDENCE_REFRESH_READY`
- `LGO-POST-LOGIN-VISUAL-EVIDENCE-UPLOAD-PACKAGING-v1.0` / `LGO_POST_LOGIN_VISUAL_EVIDENCE_UPLOAD_READY`
- `LGO-RUNTIME-ASSET-WEIGHT-BUDGET-REFRESH-v1.0` / `LGO_RUNTIME_ASSET_WEIGHT_BUDGET_REFRESH_READY`
- `LGO-RUNTIME-ASSET-WATCH-QUEUE-IMPORT-PROFILE-POLISH-v1.0` / `LGO_RUNTIME_ASSET_WATCH_QUEUE_IMPORT_PROFILE_READY`
- `LGO-WORLD-HUB-VISUAL-DEBT-TRIAGE-v1.0` / `LGO_WORLD_HUB_VISUAL_DEBT_TRIAGE_READY`
- `LGO-SESSION-MENU-FOCUS-EVIDENCE-REFRESH-v1.0` / `LGO_SESSION_MENU_FOCUS_EVIDENCE_REFRESH_READY`
- `LGO-CHARACTER-HALL-MOBILE-COPY-DENSITY-PASS-v1.0` / `LGO_CHARACTER_HALL_MOBILE_COPY_DENSITY_READY`
- `LGO-CHARACTER-HALL-MOBILE-COPY-EVIDENCE-REFRESH-v1.0` / `LGO_CHARACTER_HALL_MOBILE_COPY_EVIDENCE_REFRESH_READY`
- `LGO-CHARACTER-HALL-MOBILE-SELECTED-CTA-HIERARCHY-PASS-v1.0` / `LGO_CHARACTER_HALL_MOBILE_SELECTED_CTA_HIERARCHY_READY`
- `LGO-CHARACTER-HALL-MOBILE-SELECTED-CTA-EVIDENCE-REFRESH-v1.0` / `LGO_CHARACTER_HALL_MOBILE_SELECTED_CTA_EVIDENCE_REFRESH_READY`
- `LGO-LOGIN-CTA-DEBUG-DOT-CLEANUP-PASS-v1.0` / `LGO_LOGIN_CTA_DEBUG_DOT_CLEANUP_READY`
- `LGO-LOGIN-CTA-DEBUG-DOT-EVIDENCE-REFRESH-v1.0` / `LGO_LOGIN_CTA_DEBUG_DOT_EVIDENCE_REFRESH_READY`
- `LGO-LOGIN-CTA-BACKING-BALANCE-PASS-v1.0` / `LGO_LOGIN_CTA_BACKING_BALANCE_READY`
- `LGO-LOGIN-CTA-BACKING-EVIDENCE-REFRESH-v1.0` / `LGO_LOGIN_CTA_BACKING_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-SKIN-FOUNDATION-PASS-v1.0` / `LGO_RUNTIME_UI_SKIN_FOUNDATION_READY`
- `LGO-RUNTIME-UI-SKIN-ADOPTION-AUDIT-PASS-v1.0` / `LGO_RUNTIME_UI_SKIN_ADOPTION_AUDIT_READY`
- `LGO-CHARACTER-HALL-STYLE-ADOPTION-PASS-v1.0` / `LGO_CHARACTER_HALL_STYLE_ADOPTION_READY`
- `LGO-WORLD-HUD-STYLE-ADOPTION-PASS-v1.0` / `LGO_WORLD_HUD_STYLE_ADOPTION_READY`
- `LGO-RUNTIME-UI-SKIN-ADOPTION-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_SKIN_ADOPTION_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-SKIN-USAGE-GUIDE-PASS-v1.0` / `LGO_RUNTIME_UI_SKIN_USAGE_GUIDE_READY`
- `LGO-RUNTIME-UI-STYLE-DUPLICATION-AUDIT-v1.0` / `LGO_RUNTIME_UI_STYLE_DUPLICATION_AUDIT_READY`
- `LGO-RUNTIME-UI-FACTORY-SPLIT-REVIEW-v1.0` / `LGO_RUNTIME_UI_FACTORY_SPLIT_REVIEW_READY`
- `LGO-LOGIN-NPC-GROUNDING-SHADOW-BALANCE-PASS-v1.0` / `LGO_LOGIN_NPC_GROUNDING_SHADOW_BALANCE_READY`
- `LGO-LOGIN-NPC-GROUNDING-SHADOW-EVIDENCE-REFRESH-v1.0` / `LGO_LOGIN_NPC_GROUNDING_SHADOW_EVIDENCE_REFRESH_READY`

## Allowed paths

- `AGENTS.md`
- `.vscode/tasks.json`
- `tools/lgo_continue_dev_loop.sh`
- `tools/lgo_visual_runtime_review.sh`
- `tools/lgo_codex_autopilot.sh`
- `tools/lgo_codex_write_status.sh`
- `docs/execution/CODEX-AUTOPILOT.md`
- `client/Unity/Assets/Game/UI/Runtime/**`
- `client/Unity/Assets/Game/Bootstrap/**`
- `client/Unity/Assets/Game/World/**`
- `docs/execution/**`
- `docs/art/**`
- `docs/tasks/**`

## Forbidden paths

- `protocol/**`
- `gamedata/schemas/**`
- `docs/adr/**`
- `client/Unity/Assets/Game/UI/design-tokens.json`
- production auth, DB, economy, social, liveops

## Validation commands

```bash
git --no-pager diff --check
python3.12 tools/validate_lgo_login_gate_entry_visual_v1.py
python3.12 tools/validate_lgo_runtime_asset_weight.py
python3.12 tools/validate_lgo_runtime_asset_import_profiles.py
python3.12 tools/validate_lgo_runtime_asset_weight_budget_refresh.py
python3.12 tools/validate_lgo_runtime_asset_watch_queue_import_profile.py
python3.12 tools/validate_lgo_device_profile_ui_budgets.py
python3.12 tools/validate_lgo_login_npc_compositing_polish.py
python3.12 tools/validate_lgo_login_panel_visual_balance.py
python3.12 tools/validate_lgo_login_cta_ornament_lightweight.py
python3.12 tools/validate_lgo_login_cta_debug_dot_cleanup.py
python3.12 tools/validate_lgo_login_cta_debug_dot_evidence_refresh.py
python3.12 tools/validate_lgo_login_cta_backing_balance.py
python3.12 tools/validate_lgo_login_cta_backing_evidence_refresh.py
python3.12 tools/validate_lgo_runtime_ui_skin_foundation.py
python3.12 tools/validate_lgo_runtime_ui_skin_adoption_audit.py
python3.12 tools/validate_lgo_login_npc_grounding_shadow_balance.py
python3.12 tools/validate_lgo_login_npc_grounding_shadow_evidence_refresh.py
python3.12 tools/validate_lgo_login_responsive_scale_cleanup.py
python3.12 tools/validate_lgo_character_hall_v3b_composition.py
python3.12 tools/validate_lgo_character_hall_style_adoption.py
python3.12 tools/validate_lgo_character_hall_panel_density.py
python3.12 tools/validate_lgo_character_hall_mobile_copy_density.py
python3.12 tools/validate_lgo_character_hall_mobile_copy_evidence_refresh.py
python3.12 tools/validate_lgo_character_hall_mobile_selected_cta_hierarchy.py
python3.12 tools/validate_lgo_character_hall_mobile_selected_cta_evidence_refresh.py
python3.12 tools/validate_lgo_character_create_form_presentation.py
python3.12 tools/validate_lgo_character_hall_responsive_evidence_refresh.py
python3.12 tools/validate_lgo_visual_runtime_fast_profile_reuse.py
python3.12 tools/validate_lgo_world_hud_action_shell_v3b_skin.py
python3.12 tools/validate_lgo_world_hud_style_adoption.py
python3.12 tools/validate_lgo_runtime_ui_skin_adoption_evidence_refresh.py
python3.12 tools/validate_lgo_runtime_ui_skin_usage_guide.py
python3.12 tools/validate_lgo_runtime_ui_style_duplication_audit.py
python3.12 tools/validate_lgo_runtime_ui_factory_split_review.py
python3.12 tools/validate_lgo_world_hud_action_shell_evidence_refresh.py
python3.12 tools/validate_lgo_world_mobile_camera_framing.py
python3.12 tools/validate_lgo_world_mobile_camera_evidence_refresh.py
python3.12 tools/validate_lgo_world_label_safe_area.py
python3.12 tools/validate_lgo_world_label_safe_area_evidence_refresh.py
python3.12 tools/validate_lgo_world_top_status_mobile_readability.py
python3.12 tools/validate_lgo_world_top_status_mobile_evidence_refresh.py
python3.12 tools/validate_lgo_world_actor_hud_occlusion.py
python3.12 tools/validate_lgo_world_actor_hud_occlusion_evidence_refresh.py
python3.12 tools/validate_lgo_world_hud_dialogue_panel_viewport_polish.py
python3.12 tools/validate_lgo_world_hud_dialogue_panel_evidence_refresh.py
python3.12 tools/validate_lgo_world_hud_mobile_hierarchy_polish.py
python3.12 tools/validate_lgo_world_hud_mobile_hierarchy_evidence_refresh.py
python3.12 tools/validate_lgo_source_gate_evidence_preservation.py
python3.12 tools/validate_lgo_visual_evidence_profile_index.py
python3.12 tools/validate_lgo_build_size_budget.py
python3.12 tools/validate_lgo_world_hud_density_mobile_touch.py
python3.12 tools/validate_lgo_world_ground_visual_quality.py
python3.12 tools/validate_lgo_visual_runtime_review_heuristics.py
python3.12 tools/validate_lgo_world_hub_prop_label_responsive.py
python3.12 tools/validate_lgo_world_scene_depth_layering.py
python3.12 tools/validate_lgo_world_hub_visual_readability_cleanup.py
python3.12 tools/validate_lgo_world_hub_visual_debt_triage.py
python3.12 tools/validate_lgo_session_menu_focus_evidence_refresh.py
python3.12 tools/validate_lgo_world_hub_interaction_readability.py
python3.12 tools/validate_lgo_world_hub_interaction_evidence_refresh.py
python3.12 tools/validate_lgo_near_interaction_checkpoint_capture.py
python3.12 tools/validate_lgo_near_interaction_evidence_refresh.py
python3.12 tools/package_lgo_visual_evidence_upload.py --verify-only
python3.12 tools/validate_lgo_post_login_visual_evidence_upload_packaging.py
python3.12 tools/validate_lgo_world_responsive_evidence_refresh.py
python3.12 tools/validate_m4_2_playable_ui.py
python3.12 tools/validate_m4_visible_ui.py
python3.12 tools/validate_m6_combat_visual_readability.py
python3.12 tools/validate_m6_unity_combat_placeholder_asset_import.py
python3.12 tools/validate_package_hygiene.py
./tools/lgo_continue_dev_loop.sh
./tools/lgo_codex_autopilot.sh --dry-run
```

## Runtime evidence command

```bash
./tools/lgo_visual_runtime_review.sh
```

Fast UI/visual iteration command after nearby full gates are already green:

```bash
./tools/lgo_visual_runtime_review_profiles.sh
```

Expected classifications:

- `PASS`
- `FIX_REQUIRED`
- `VISUAL_CAPTURE_TIMEOUT`
- `VIDEO_CAPTURE_BLOCKED_ENV`
- `RUNTIME_BLOCKED_ENV`

## Stop conditions

- A frozen contract/protocol/schema/ADR change is required.
- Unity/player tooling is unavailable or visual capture is blocked by environment.
- A gate fails and cannot be fixed within allowed paths.
- Owner product/art decision is required.
- No valid next action remains.

## Follow-up task after current task

`LGO-WORLD-HUD-STYLE-ADOPTION-PASS-v1.0` is source-ready. Continue with `LGO-RUNTIME-UI-SKIN-ADOPTION-EVIDENCE-REFRESH-v1.0`; the next useful batch is refreshing profile screenshots and reviewing visual parity after shared-skin consolidation.

Recent visual passes improved scene depth, NPC staging, responsive HUD behavior, world staging density, label readability, and evidence review scoring without new gameplay or frozen-surface changes.
