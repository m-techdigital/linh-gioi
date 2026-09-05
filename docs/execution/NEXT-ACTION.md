# Linh Giới Online — Next Action

Last updated: `2026-09-05`

## Current focus

Post-login visual runtime hardening plus device-profile asset governance. Login has been upgraded to V3B-aligned runtime presentation, source-level post-login readability polish is implemented, and mobile/tablet/PC runtime asset profile budgets are now documented and validated. The standalone visual evidence harness now captures all seven screenshots, can continue in background, and auto-finishes the Unity Player after manifest completion so the operator should not need to click or close the player by hand. Character lobby usability, in-world HUD presentation, world hub scene readability, and desktop/tablet/mobile responsive evidence are now verified with fresh runtime screenshots. Login-to-character copy has been cleaned of player-facing dev wording, status chips now read correctly in runtime screenshots, the session menu/settings shell is responsive without tablet/mobile clipping, in-world interaction affordance now has stateful target labels, runtime asset size inventory is documented/validated, V3B runtime candidates now carry platform-specific Unity import profiles for Standalone/Android/iPhone, the in-world HUD is more compact/touch-oriented across desktop/tablet/mobile and now groups world guidance/action content into V3B-styled shell cards, the world hub camera now uses viewport-aware orthographic framing so mobile/tablet actors read larger than the fixed desktop view, and refreshed profile screenshots confirm mobile actor scale is improved without harmful cropping. The world hub ground now uses a lightweight procedural cultivation-platform texture instead of a debug-like grid, the login first screen now uses a V3B composition with a centered text logo/CTA cluster plus right-side Gate Keeper on desktop/tablet and a compact logo/CTA layout on mobile, the Character Hall now uses a V3B cultivator portrait with a mobile-specific two-zone lobby layout, lighter panel density, a game-facing create form with framed `Danh xưng` input and `Tạo tu sĩ` CTA, and refreshed desktop/tablet/mobile runtime screenshots, build-size budget reporting now separates Unity runtime payload from repository/reference/tooling weight, the visual evidence loop now writes PNG heuristics for checkpoint presence/dimensions/byte size/pixel variation/duplicate-frame detection, world-hub labels now show by guided state/proximity instead of cluttering the whole scene, the login CTA stack now uses a lighter dark-glass panel to reduce cyan/gold glare while retaining the V3B logo/button language, and the World Hub now has lightweight procedural grounding shadows under the player, interactables, target dummy, warning slime, Spirit Gate, and key props. The visual review script now has a build-once/reuse-player profile wrapper for desktop/tablet/mobile screenshot refreshes.

Autopilot operating rule: when a task or phase is truly closed by its required gates, continue to the next roadmap-valid task/phase instead of stopping at the phase boundary. Stop only for a real blocker, unavailable runtime/tooling, required owner decision, or frozen contract/protocol/schema/ADR change.

## Next task

`LGO-WORLD-TOP-STATUS-MOBILE-READABILITY-PASS-v1.0`

Improve top status/action chip readability on mobile/tablet world-hub screenshots without adding gameplay or changing combat/session semantics.

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
- `build/visual-evidence/latest/player.log`
- `build/visual-evidence/latest/unity-build.log`
- `build/codex-autopilot/status.json`

Next allowed action: improve top status/action chip readability on mobile/tablet world-hub screenshots. The visual evidence harness records explicit review checklist categories and machine-readable heuristics for every checkpoint under marker `LGO_VISUAL_RUNTIME_REVIEW_HEURISTICS_READY`; world-hub label responsiveness is tracked under `LGO_WORLD_HUB_PROP_LABEL_RESPONSIVE_READY`; login visual balance is tracked under `LGO_LOGIN_PANEL_VISUAL_BALANCE_READY`; Character Hall density is tracked under `LGO_CHARACTER_HALL_PANEL_DENSITY_READY`; World Hub depth layering is tracked under `LGO_WORLD_SCENE_DEPTH_LAYERING_READY`; responsive evidence refresh is tracked under `LGO_WORLD_RESPONSIVE_EVIDENCE_REFRESH_READY`; login CTA ornament polish is tracked under `LGO_LOGIN_CTA_ORNAMENT_LIGHTWEIGHT_READY`; Character Hall create-form presentation is tracked under `LGO_CHARACTER_CREATE_FORM_PRESENTATION_READY`; Character Hall responsive evidence refresh is tracked under `LGO_CHARACTER_HALL_RESPONSIVE_EVIDENCE_REFRESH_READY`; fast profile reuse is tracked under `LGO_VISUAL_RUNTIME_FAST_PROFILE_REUSE_READY`; world HUD action shell skin is tracked under `LGO_WORLD_HUD_ACTION_SHELL_V3B_SKIN_READY`; world HUD evidence refresh is tracked under `LGO_WORLD_HUD_ACTION_SHELL_EVIDENCE_REFRESH_READY`; mobile world camera framing is tracked under `LGO_WORLD_MOBILE_CAMERA_FRAMING_READY`; mobile camera evidence refresh is tracked under `LGO_WORLD_MOBILE_CAMERA_EVIDENCE_REFRESH_READY`; world label safe-area is tracked under `LGO_WORLD_LABEL_SAFE_AREA_READY`; label safe-area evidence is tracked under `LGO_WORLD_LABEL_SAFE_AREA_EVIDENCE_REFRESH_READY`; the project still refuses to claim visual PASS from capture/build alone.

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
python3.12 tools/validate_lgo_device_profile_ui_budgets.py
python3.12 tools/validate_lgo_login_npc_compositing_polish.py
python3.12 tools/validate_lgo_login_panel_visual_balance.py
python3.12 tools/validate_lgo_login_cta_ornament_lightweight.py
python3.12 tools/validate_lgo_character_hall_v3b_composition.py
python3.12 tools/validate_lgo_character_hall_panel_density.py
python3.12 tools/validate_lgo_character_create_form_presentation.py
python3.12 tools/validate_lgo_character_hall_responsive_evidence_refresh.py
python3.12 tools/validate_lgo_visual_runtime_fast_profile_reuse.py
python3.12 tools/validate_lgo_world_hud_action_shell_v3b_skin.py
python3.12 tools/validate_lgo_world_hud_action_shell_evidence_refresh.py
python3.12 tools/validate_lgo_world_mobile_camera_framing.py
python3.12 tools/validate_lgo_world_mobile_camera_evidence_refresh.py
python3.12 tools/validate_lgo_world_label_safe_area.py
python3.12 tools/validate_lgo_world_label_safe_area_evidence_refresh.py
python3.12 tools/validate_lgo_build_size_budget.py
python3.12 tools/validate_lgo_world_hud_density_mobile_touch.py
python3.12 tools/validate_lgo_world_ground_visual_quality.py
python3.12 tools/validate_lgo_visual_runtime_review_heuristics.py
python3.12 tools/validate_lgo_world_hub_prop_label_responsive.py
python3.12 tools/validate_lgo_world_scene_depth_layering.py
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

`LGO-WORLD-LABEL-SAFE-AREA-EVIDENCE-REFRESH-v1.0` is evidence-ready. Continue with top status mobile readability polish, preserving current gameplay and contract semantics.

Recent visual passes improved scene depth, NPC staging, responsive HUD behavior, world staging density, label readability, and evidence review scoring without new gameplay or frozen-surface changes.
