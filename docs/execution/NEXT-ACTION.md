# Linh Giới Online — Next Action

Last updated: `2026-09-06`

## Quick Resume

- Current phase: runtime UI/visual quality hardening and execution workflow cleanup.
- Active task: `LGO-RUNTIME-QUALITY-NEXT-COMPACT-BATCH-v1.0`.
- Current reason: Đá Luyện world label now uses a responsive offset so the step-2 prompt screenshot reads cleaner around the player and spirit VFX; screenshot evidence was reviewed, but no `VISUAL_RUNTIME_PASS` is claimed.
- Current batch scope: continue with player-visible layout/quality fixes, controller hotspot extraction, or dependency-driven V2 fallback retirement planning without opening new systems.
- Fast validation: `git --no-pager diff --check`; `bash -n tools/lgo_codex_git_checkpoint.sh tools/lgo_codex_autopilot.sh tools/lgo_continue_dev_loop.sh`; `python3.12 tools/report_lgo_change_budget.py`; `python3.12 tools/validate_package_hygiene.py`; `LGO_DEV_LOOP_GATE_PROFILE=quick LGO_DEV_LOOP_CONTEXT_MODE=quick ./tools/lgo_continue_dev_loop.sh`.
- Runtime validation: run `LGO_DEV_LOOP_VISUAL_RUNTIME=force ./tools/lgo_continue_dev_loop.sh` or `./tools/lgo_visual_runtime_review.sh` only when the next code change affects visible runtime UI. Do not claim `VISUAL_RUNTIME_PASS` from capture alone.
- Next implementation task after this fix: choose the next compact player-visible slice, likely movement/input feel, World Hub label spacing, or another screenshot-proven UI/UX issue; avoid heavy assets and one-off validators unless a real gate needs them.
- Historical marker registry stays in this file for validator compatibility until a dedicated registry migration is implemented and validated.

## Current focus

Post-login visual runtime hardening plus device-profile asset governance. Login has been upgraded to V3B-aligned runtime presentation, source-level post-login readability polish is implemented, and mobile/tablet/PC runtime asset profile budgets are now documented and validated. The standalone visual evidence harness now captures all seven screenshots, can continue in background, and auto-finishes the Unity Player after manifest completion so the operator should not need to click or close the player by hand. Character lobby usability, in-world HUD presentation, world hub scene readability, and desktop/tablet/mobile responsive evidence are now verified with fresh runtime screenshots. Login-to-character copy has been cleaned of player-facing dev wording, status chips now read correctly in runtime screenshots, the session menu/settings shell is responsive without tablet/mobile clipping, in-world interaction affordance now has stateful target labels, runtime asset size inventory is documented/validated, V3B runtime candidates now carry platform-specific Unity import profiles for Standalone/Android/iPhone, the in-world HUD is more compact/touch-oriented across desktop/tablet/mobile and now groups world guidance/action content into V3B-styled shell cards, the world hub camera now uses viewport-aware orthographic framing so mobile/tablet actors read larger than the fixed desktop view, and refreshed profile screenshots confirm mobile actor scale is improved without harmful cropping. The world hub ground now uses a lightweight procedural cultivation-platform texture instead of a debug-like grid, the login first screen now uses a V3B composition with a centered text logo/CTA cluster plus right-side Gate Keeper on desktop/tablet and a compact logo/CTA layout on mobile, the Character Hall now uses a V3B cultivator portrait with a mobile-specific two-zone lobby layout, lighter panel density, a game-facing create form with framed `Danh xưng` input and `Tạo tu sĩ` CTA, and refreshed desktop/tablet/mobile runtime screenshots, build-size budget reporting now separates Unity runtime payload from repository/reference/tooling weight, the visual evidence loop now writes PNG heuristics for checkpoint presence/dimensions/byte size/pixel variation/duplicate-frame detection, world-hub labels now show by guided state/proximity instead of cluttering the whole scene, the login CTA stack now uses a lighter dark-glass panel to reduce cyan/gold glare while retaining the V3B logo/button language, and shared runtime UI styling now lives in `RuntimeUiSkin` so repeated glass panel, framed row, compact button, and login backing rules can be reused instead of copy-written per screen. The World Hub now has lightweight procedural grounding shadows under the player, interactables, target dummy, warning slime, Spirit Gate, and key props. The visual review script now has a build-once/reuse-player profile wrapper for desktop/tablet/mobile screenshot refreshes. Source-only gates now preserve runtime evidence directories under `LGO_SOURCE_GATE_EVIDENCE_PRESERVATION_READY`. Character Hall post-login composition now routes list, preview, profile hero, portrait, and responsive row setup through `RuntimeUiFactory` under `LGO_POST_LOGIN_RUNTIME_UI_REUSE_CLEANUP_READY`, and fresh runtime screenshots confirm the helper extraction did not break post-login layout under `LGO_POST_LOGIN_RUNTIME_UI_REUSE_EVIDENCE_REFRESH_READY`. Character Hall V3B polish and evidence refresh are tracked under `LGO_CHARACTER_HALL_V3B_VISUAL_POLISH_READY` and `LGO_CHARACTER_HALL_V3B_VISUAL_POLISH_EVIDENCE_REFRESH_READY`. The unused controller-local Character Hall list-density wrapper has been removed under `LGO_RUNTIME_UI_FACTORY_CHARACTER_HALL_CLEANUP_FOLLOWUP_READY`. World HUD hidden pose/VFX/skin-source evidence labels now route through `RuntimeUiFactory.NewHiddenStatusLabel` under `LGO_WORLD_HUD_RUNTIME_UI_REUSE_AUDIT_READY`, fresh runtime evidence confirms World Hub/NPC Dialogue remain stable under `LGO_WORLD_HUD_RUNTIME_UI_REUSE_EVIDENCE_REFRESH_READY`, and the World HUD root/group frames now use a calmer V3B/fantasy panel hierarchy under `LGO_WORLD_HUD_FANTASY_PANEL_HIERARCHY_POLISH_READY`.

Current focus update: refreshed screenshots confirm the corrected no-stretched-texture World HUD panel version under `LGO_WORLD_HUD_FANTASY_PANEL_EVIDENCE_REFRESH_READY`.

Current focus update: hidden status/note composition now routes through reusable factory helpers under `LGO_RUNTIME_UI_STATUS_COMPOSITION_CLEANUP_READY`.

Current focus update: focused runtime screenshots confirm hidden status composition remains stable under `LGO_RUNTIME_UI_STATUS_COMPOSITION_EVIDENCE_REFRESH_READY`.

Current focus update: local combat cooldown button now uses a lighter code-styled glass state under `LGO_COMBAT_BUTTON_COOLDOWN_VISUAL_LIGHTNESS_READY`.

Current focus update: target-dummy runtime evidence confirms the lighter cooldown button state under `LGO_COMBAT_BUTTON_COOLDOWN_VISUAL_EVIDENCE_REFRESH_READY`.

Current focus update: execution state now has a Quick Resume block under `LGO_RUNTIME_UI_STATE_DOC_COMPACTION_AUDIT_READY` so future continuous sessions can identify the active task without losing historical marker coverage.

Current focus update: append-only task history now has a compact rollup view at `docs/execution/TASK-LEDGER-ROLLUP.md` under `LGO_EXECUTION_LEDGER_ROLLUP_VIEW_READY` so future sessions can scan recent closures without reading the full ledger.

Current focus update: autopilot commit cadence now defaults to no auto-commit under `LGO_COMMIT_CADENCE_POLICY_HARDENING_READY`; coherent checkpoint commits remain opt-in with `LGO_AUTOPILOT_COMMIT=1`, and push remains opt-in with `LGO_AUTOPILOT_PUSH=1`.

Current focus update: visual debt triage selected Login NPC grounding plus CTA panel polish under `LGO_RUNTIME_UI_QUALITY_DEBT_TRIAGE_READY`; no visual PASS is claimed from the triage alone.

Current focus update: login NPC grounding and CTA panel source polish is ready under `LGO_LOGIN_NPC_GROUNDING_CTA_PANEL_POLISH_READY`; runtime screenshot refresh is required before any visual claim.

Current focus update: refreshed desktop/tablet/mobile login screenshots confirm the source polish is visible; desktop and tablet are improved but still not production-final, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: World Hub now has a more readable procedural cultivation stage, directional path glows, and focus glows under key interactables under `LGO_RUNTIME_UI_QUALITY_DEBT_FIRST_FIX_READY`; desktop/tablet/mobile screenshots were reviewed, the scene is improved but still placeholder-quality, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: Character Hall no longer stretches the ornate V3B panel texture across the full shell under `LGO_CHARACTER_HALL_ACTION_DENSITY_FIRST_FIX_READY`; desktop/tablet/mobile screenshots are cleaner and more readable, still not production-final, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: World procedural ground texture, actor shadows, focus glows, and path glows now live in `WorldProceduralVisuals` under `LGO_RUNTIME_UI_CONTROLLER_SIZE_REDUCTION_READY`; `PlayableWorldController` is smaller and runtime screenshot capture remains stable, with no `VISUAL_RUNTIME_PASS` claim.

Current focus update: Dev-loop/autopilot output now defaults to compact context and fast visual runtime server build logs under `LGO_WORKFLOW_FAST_GATE_NOISE_REDUCTION_READY`; visual capture also snapshots/restores Unity ProjectSettings so evidence runs do not pollute source diffs.

Current focus update: Login responsive layout now lives in `RuntimeLoginResponsiveLayout` under `LGO_RUNTIME_CODE_HOTSPOT_REDUCTION_READY`; `M4PlayableClientController` is smaller, historical validators follow the new ownership boundary, and runtime login capture remains stable with no visual PASS claim.

Current focus update: World-space label creation, text refresh, shadow, and active toggling now live in `WorldLabelPresenter` under `LGO_WORLD_CONTROLLER_INTERACTION_PRESENTATION_SPLIT_READY`; gameplay state ownership remains in `PlayableWorldController`, and world-hub runtime capture remains stable with no visual PASS claim.

Current focus update: `prepare_unity_local_assets.sh` now supports `LGO_UNITY_LOCAL_ASSETS_QUIET=1`, and `lgo_visual_runtime_review.sh` uses it under `LGO_PREPARE_UNITY_ASSETS_QUIET_PROFILE_READY`; visual review logs are shorter while protocol generation/build/capture still fail honestly.

Current focus update: visual runtime review now re-requests player focus with bounded progress logging and the Unity evidence runner configures background capture earlier under `LGO_VISUAL_RUNTIME_CAPTURE_FOCUS_ROBUSTNESS_READY`; latest desktop evidence captured all 10 checkpoints without a manual click in the runner log, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: World Hub oversized procedural ground rings are toned down, lightweight runtime mist support is available, and the stale M6 readiness docs-only validator no longer blocks current non-frozen implementation work under `LGO_WORLD_HUB_VISUAL_DEPTH_WEIGHT_READY`; fresh desktop screenshots were reviewed as cleaner but still not final-production world art.

Current focus update: changeset noise audit removed obsolete M6 readiness allowlist code and split visual evidence hooks into `M4PlayableClientController.Evidence.cs` under `LGO_CHANGESET_NOISE_HOTSPOT_AUDIT_READY`; main UI controller is smaller, source-only passes, and visual evidence still captures all checkpoints.

Current focus update: Character Hall selected CTA hierarchy now prioritizes `Vào sân luyện` before `Tạo thêm` on desktop/tablet/mobile under `LGO_CHARACTER_HALL_SELECTED_CTA_PRIORITY_READY`; fresh desktop screenshot is cleaner, no visual PASS is claimed.

Current focus update: continuous workflow, task selection, quick/full gate strategy, and autopilot prompt now include change-budget rules under `LGO_CONTINUOUS_WORKFLOW_CHANGE_BUDGET_READY`; future batches should avoid new one-off docs/validators/markers unless they protect a real gate or handoff need.

Current focus update: World Hub ground texture now uses a smaller, softer procedural background and the desktop camera frame is closer under `LGO_WORLD_HUB_SOFT_BACKGROUND_CAMERA_READY`; fresh runtime screenshot is cleaner and actor/prop scale reads better, no visual PASS is claimed.

Current focus update: Character Hall selected state now collapses the create form under `LGO_CHARACTER_HALL_SELECTED_CREATE_COLLAPSE_READY`; fresh runtime screenshot shows a cleaner CTA band with `Vào sân luyện` first and no always-visible create input, no visual PASS is claimed.

Current focus update: local generated handoff archives were cleaned from `build/chatgpt-handoff`, reducing `build/` from 507MB to 169MB; `tools/lgo_local_artifact_cleanup.sh` now provides a dry-run/apply path under `LGO_LOCAL_ARTIFACT_CLEANUP_READY` while preserving source, evidence screenshots, and dev-loop logs.

Current focus update: enter-world evidence now captures a distinct Linh Môn transition state and world-hub resets to steady objective state under `LGO_ENTER_WORLD_EVIDENCE_DISTINCT_CHECKPOINT_READY`; fresh runtime screenshots were reviewed, duplicate-frame evidence is gone, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: session menu now owns focus on every profile, centers by viewport ratio, hides HUD/header while open, and dims the world behind it under `LGO_SESSION_MENU_CENTERED_FOCUS_POLISH_READY`; fresh runtime screenshot is cleaner, no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: selected mobile Character Hall now labels the action state as `Sẵn sàng` and lowers the CTA panel slightly under `LGO_MOBILE_CHARACTER_HALL_READY_COPY_POLISH_READY`; mobile screenshot was reviewed, no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: repeated session menu placement, responsive padding, background, and focus scrim rules now live in `RuntimeSessionMenuLayout` under `LGO_SESSION_MENU_LAYOUT_HELPER_READY`; source-only passes and runtime capture remained stable.

Current focus update: autopilot checkpoint staging now uses an allowlist and skips generated/cache/build artifacts under `LGO_AUTOPILOT_SAFE_CHECKPOINT_STAGING_READY`; commit/push remains opt-in and frozen surfaces remain blocked.

Current focus update: combat cooldown active sprite/texture now prefer V3B then lightweight `CombatPlaceholders` instead of V2 `CooldownFull` fallbacks under `LGO_RUNTIME_COMBAT_COOLDOWN_V2_DEPENDENCY_CLEANUP_READY`; target-dummy screenshot was reviewed as readable, V2 registry references are down to 24 and fallback-only properties to 8, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: Character Hall portrait fallback no longer references V2 `IconAccountTexture` under `LGO_CHARACTER_HALL_PORTRAIT_V2_FALLBACK_CLEANUP_READY`; fresh Character Hall screenshots still show the V3B cultivator portrait, V2 registry references are down to 23 and fallback-only properties to 7, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: target dummy idle/selected/hit art now prefer V3B then lightweight `CombatPlaceholders` instead of V2 dummy fallbacks under `LGO_RUNTIME_TARGET_DUMMY_V2_FALLBACK_CLEANUP_READY`; target-dummy screenshot still renders the training dummy and combat panel clearly, V2 registry references are down to 20 and fallback-only properties to 4, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: Shadow Slime runtime sprite and marker fallback now use V3B/null checks instead of V2 `ShadowSlimeAlt` under `LGO_RUNTIME_SHADOW_SLIME_V2_FALLBACK_CLEANUP_READY`; world-hub and target-dummy screenshots still show the shadow slime clearly, V2 registry references are down to 19 and fallback-only properties to 3, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: combat target marker and warning telegraph now use lightweight `CombatPlaceholders` directly instead of V2 fallback sprites under `LGO_RUNTIME_COMBAT_MARKER_V2_FALLBACK_CLEANUP_READY`; target-dummy screenshot remains readable, V2 registry references are down to 17 and fallback-only properties to 1, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: V2 dependency snapshot now uses exact property-name matching instead of prefix counting under `LGO_RUNTIME_ASSET_V2_DEPENDENCY_EXACT_SCAN_READY`; report now shows 16 remaining V2 references, all V3B-covered by exact-name coverage, with 0 fallback-only properties.

Current focus update: combat ready cooldown, wind slash, and impact spark now prefer V3B then lightweight `CombatPlaceholders` instead of V2 fallbacks under `LGO_RUNTIME_COMBAT_VFX_V2_FALLBACK_CLEANUP_READY`; source validators pass and report shows 12 remaining V2 references with 0 fallback-only properties.

Current focus update: Login background and Gate Keeper NPC texture now use V3B directly instead of V2 texture fallbacks under `LGO_LOGIN_V2_FALLBACK_CLEANUP_READY`; fresh login screenshot still renders background/logo/CTA/NPC clearly, report shows 10 remaining V2 references with 0 fallback-only properties, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: World actors and set dressing now use V3B registry assets directly instead of V2 fallbacks under `LGO_RUNTIME_WORLD_V2_FALLBACK_CLEANUP_READY`; refreshed world-hub and target-dummy screenshots still render key actors/props clearly, and exact V2 dependency scan now reports 0 V2 registry references with 0 fallback-only properties.

Current focus update: login CTA and server row sizing/tint were reduced under `LGO_LOGIN_CTA_FIT_FOR_PURPOSE_READY`; fresh desktop/tablet/mobile screenshots were reviewed as calmer and still readable, with no `VISUAL_RUNTIME_PASS` claim.

Current focus update: World Hub procedural floor colors, rings, and guide paths were tuned under `LGO_WORLD_GROUND_READABILITY_TUNING_READY`; fresh screenshot was reviewed as less flat but still not production-final world art.

Current focus update: next-task advisor fallback is ready under `LGO_NEXT_TASK_ADVISOR_ACTIVE_FALLBACK_READY`; if backlog has no safe standalone task but `NEXT-ACTION.md` has an active task, the advisor returns it instead of stopping.

Current focus update: compact state loading and Vietnamese visual-runtime owner notes are ready under `LGO_COMPACT_STATE_BRIEF_AND_VI_RUNTIME_NOTES_READY`; routine dev/autopilot loops should use `tools/lgo_state_brief.py` before opening long state files.

Current focus update: World Hub lightweight depth pass is ready under `LGO_WORLD_HUB_LIGHTWEIGHT_DEPTH_PASS_READY`; procedural ground now uses a smaller texture and extra runtime-generated mist/path depth without adding image payload, with desktop/tablet/mobile screenshots reviewed but no final visual PASS claim.

Current focus update: combat dummy state asset priority now prefers V3B selected/hit/recover sprites under `LGO_COMBAT_DUMMY_V3B_STATE_PRIORITY_READY`; target-dummy runtime screenshot was reviewed as more consistent with V3B, still placeholder-quality, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: compact state brief now stays under the routine 90-line budget under `LGO_STATE_BRIEF_TOKEN_BUDGET_READY`; routine resume should use this before opening long state/ledger files.

Current focus update: Character Hall desktop/tablet hierarchy now uses wider metric-owned shell sizing and lighter glass under `LGO_CHARACTER_HALL_WIDER_GLASS_BALANCE_READY`; desktop/tablet/mobile screenshots were reviewed as cleaner and still not production-final, with no `VISUAL_RUNTIME_PASS` claim.

Current focus update: Character Hall responsive layout now lives in `RuntimeCharacterHallResponsiveLayout` under `LGO_CHARACTER_HALL_RESPONSIVE_LAYOUT_HELPER_READY`; the main playable UI controller is smaller while character flow and reviewed runtime screenshots remain stable.

Current focus update: World HUD responsive panel/top-status/dialogue layout now lives in `RuntimeWorldHudResponsiveLayout` under `LGO_WORLD_HUD_RESPONSIVE_LAYOUT_HELPER_READY`; source-only and runtime screenshot evidence remain stable, and quick dev loop now skips visual capture by default unless forced.

Current focus update: World Hub now has a slightly stronger procedural platform, back-ridge prop layer, and mist depth under `LGO_WORLD_HUB_LIGHTWEIGHT_BACK_RIDGE_DEPTH_READY`; screenshot evidence is improved but still below final reference quality, and local cleanup can now dry-run/remove rebuilt Unity player builds without touching source/evidence, with no `VISUAL_RUNTIME_PASS` claim.

Current focus update: World Hub set-dressing and depth-lighting placement now live in `WorldHubSetDressing` under `LGO_WORLD_HUB_SET_DRESSING_HELPER_READY`; `PlayableWorldController` now only owns the gameplay hook and shared billboard creation, while fresh runtime screenshots confirm World Hub/NPC Dialogue/Session Menu/Login remain stable. No `VISUAL_RUNTIME_PASS` is claimed because World Hub art is still below final reference quality.

Current focus update: Character Hall create-form collapse/expand state and selected-action CTA hierarchy now live in `RuntimeCharacterHallResponsiveLayout` under `LGO_CHARACTER_HALL_STATE_ACTION_HELPER_READY`; `M4PlayableClientController` keeps only flow decisions and delegates reusable presentation state. Source-only gates pass; no visual PASS is claimed from this source-only ownership cleanup.

Current focus update: Local combat HUD cooldown/icon/button/accent presentation now lives in `RuntimeCombatHudPresentation` under `LGO_RUNTIME_COMBAT_HUD_PRESENTATION_HELPER_READY`; `M4PlayableClientController` keeps local combat flow calls while the helper owns text, texture, tooltip, compact status, and accent state. Source-only and refreshed target-dummy runtime evidence pass, with no `VISUAL_RUNTIME_PASS` claim.

Current focus update: World Hub procedural cultivation floor now has stronger lightweight stone seams, rings, guide paths, and platform mist under `LGO_WORLD_GROUND_LIGHTWEIGHT_DEPTH_CUES_READY`; refreshed desktop screenshot is less flat without adding image payload, still below final reference-quality world art, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: compact state brief now suppresses long `PROJECT-STATE.md` body output under `LGO_STATE_BRIEF_PROJECT_STATE_NOISE_REDUCTION_READY`; routine resume output is back under the 90-line target, reducing token/context spend without weakening gates.

Current focus update: Character Hall selected-state proportions now use a wider shell, narrower list, and larger cultivator preview under `LGO_CHARACTER_HALL_SELECTED_HERO_PROPORTION_READY`; refreshed desktop screenshots are less admin-like while still below final reference-quality UI, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: local artifact cleanup now reports `client/Unity/Library` size and adds explicit `--apply-unity-cache` under `LGO_LOCAL_UNITY_CACHE_CLEANUP_MODE_READY`; dry-run confirms the large workspace weight is local Unity cache/player build rather than runtime art source.

Current focus update: Character Hall first-time create state now centers a narrower form and uses `Khai mở tu sĩ` copy under `LGO_CHARACTER_HALL_CREATE_STATE_FORM_POLISH_READY`; refreshed desktop screenshot is cleaner, but the TextField chrome still needs a future reusable form-component pass.

Current focus update: workflow self-review is applied under `LGO_WORKFLOW_SELF_REVIEW_COMPACT_PROGRESS_READY`; state brief no longer prints truncation noise for the core state, commit cadence should group related small batches, and Character Hall first-time `Danh xưng` field alignment is centered while deeper TextField chrome remains a focused future form-component task.

Current focus update: Character Hall first-time input label chrome is cleaned under `LGO_CHARACTER_HALL_INPUT_LABEL_CHROME_CLEANUP_READY`; `Danh xưng` now uses game copy above the field and a label-free input, shared skin label styling stays API-compatible, runtime screenshot evidence was reviewed, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: lobby text field creation now routes through `RuntimeUiFactory.NewLobbyTextField` under `LGO_RUNTIME_UI_LOBBY_TEXT_FIELD_FACTORY_READY`; the playable controller no longer owns the create-form input skin/tooltip details, source gates pass, and visual evidence from the preceding UI change remains current.

Current focus update: Character Hall density-aware status label creation now lives in `RuntimeUiFactory.NewCharacterHallStatusLabel` under `LGO_CHARACTER_HALL_STATUS_LABEL_FACTORY_READY`; the playable controller owns less reusable UI construction while Character Hall validators and quick dev loop remain clean.

Current focus update: World Hub procedural ground contrast is tuned under `LGO_WORLD_GROUND_CONTRAST_READABILITY_TUNE_READY`; the 256x256 runtime-generated texture now gives tile seams, rings, and guide paths more readable contrast without adding image payload, refreshed screenshots were reviewed, and no `VISUAL_RUNTIME_PASS` is claimed.

Current focus update: runtime asset inventory now reports V2 fallback `Resources` payload separately under `LGO_RUNTIME_ASSET_V2_FALLBACK_PAYLOAD_INVENTORY_READY`; current V2 fallback weight is 2389.7 KB across 65 images, kept only while registry/code dependencies still need fallback coverage.

Current focus update: runtime asset inventory now reports referenced V2 registry dependencies under `LGO_RUNTIME_ASSET_V2_DEPENDENCY_SNAPSHOT_READY`; current source references 29 V2 registry properties, 13 of them fallback-only by exact V3B property-name coverage.

Current focus update: Character Hall first-time create form width balance is ready under `LGO_CHARACTER_HALL_CREATE_FORM_WIDTH_BALANCE_READY`; the form panel is narrower, input/CTA cluster is centered, validator ownership follows the current factory/helper split, and refreshed screenshots were reviewed without claiming final visual pass.

Current focus update: secondary button V2 dependency cleanup is ready under `LGO_RUNTIME_UI_SECONDARY_BUTTON_V2_DEPENDENCY_CLEANUP_READY`; `RuntimeUiFactory.NewSecondaryButton` now uses code-side V3B-style framing instead of V2 `ButtonSecondaryTexture`, and runtime screenshots remain readable.

Current focus update: primary button V2 dependency cleanup is ready under `LGO_RUNTIME_UI_PRIMARY_BUTTON_V2_DEPENDENCY_CLEANUP_READY`; primary login buttons use V3B gold texture when available and code-side fallback framing instead of V2 primary/disabled textures.

Current focus update: Character Hall first-time create form readability is polished under `LGO_CHARACTER_HALL_CREATE_HINT_RESPONSIVE_POLISH_READY`; desktop uses a muted cultivation hint instead of a second framed row, tablet/mobile keep the form compact, and refreshed screenshots were reviewed without claiming final visual pass.

Current focus update: World Hub guided-objective readability is improved under `LGO_WORLD_HUB_GUIDED_OBJECTIVE_LABEL_READY`; the current objective object is labeled directly in the scene while existing panel objectives remain unchanged, with refreshed desktop/tablet evidence and no final visual pass claim.

Current focus update: local combat hit feedback now includes a visible reward placeholder under `LGO_LOCAL_COMBAT_REWARD_PLACEHOLDER_FEEDBACK_READY`; `Tinh khí +1` appears during target hit evidence, improving action-result feel without adding economy, inventory, or server reward semantics.

Autopilot operating rule: when a task or phase is truly closed by its required gates, continue to the next roadmap-valid task/phase instead of stopping at the phase boundary. Stop only for a real blocker, unavailable runtime/tooling, required owner decision, or frozen contract/protocol/schema/ADR change.

## Next task

`LGO-RUNTIME-QUALITY-NEXT-COMPACT-BATCH-v1.0`

Continue with one compact runtime quality batch. Pick the next visible issue from latest evidence, prefer layout/scale/hierarchy fixes over new assets, keep file count proportional to player value, and run visual evidence only when the change affects runtime presentation. Marker ready from the previous pass: `LGO_LOCAL_ARTIFACT_CLEANUP_READY`.

## Current blocker

No active blocker for source work. Visual runtime capture is currently available in this environment and has completed multiple consecutive rounds without manual player close; latest focus-robustness run captured 10/10 checkpoints with progress telemetry.

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

Next allowed action: polish Character Hall V3B visual hierarchy while keeping generated captures and package artifacts out of source control. The visual evidence harness records explicit review checklist categories and machine-readable heuristics for every checkpoint under marker `LGO_VISUAL_RUNTIME_REVIEW_HEURISTICS_READY`; combat button mobile evidence is tracked under `LGO_COMBAT_BUTTON_MOBILE_RESPONSIVE_EVIDENCE_READY`; World HUD component boundary cleanup is tracked under `LGO_WORLD_HUD_COMPONENT_BOUNDARY_AUDIT_READY`; runtime UI evidence-state helper is tracked under `LGO_RUNTIME_UI_EVIDENCE_STATE_HELPER_READY`; World HUD header block is tracked under `LGO_WORLD_HUD_HEADER_BLOCK_READY`; World HUD row/helper coverage is tracked under `LGO_WORLD_HUD_ROW_HELPER_COVERAGE_READY`; World HUD row/helper evidence is tracked under `LGO_WORLD_HUD_ROW_HELPER_EVIDENCE_REFRESH_READY`; runtime UI style ownership cleanup is tracked under `LGO_RUNTIME_UI_STYLE_OWNERSHIP_DRIFT_READY`; runtime UI style ownership evidence is tracked under `LGO_RUNTIME_UI_STYLE_OWNERSHIP_EVIDENCE_REFRESH_READY`; runtime UI controller style constants cleanup is tracked under `LGO_RUNTIME_UI_CONTROLLER_STYLE_CONSTANTS_READY`; runtime UI controller style constants evidence is tracked under `LGO_RUNTIME_UI_CONTROLLER_STYLE_CONSTANTS_EVIDENCE_REFRESH_READY`; runtime UI responsive padding profile audit is tracked under `LGO_RUNTIME_UI_RESPONSIVE_PADDING_PROFILE_AUDIT_READY`; runtime UI responsive padding profile evidence is tracked under `LGO_RUNTIME_UI_RESPONSIVE_PADDING_PROFILE_EVIDENCE_REFRESH_READY`; runtime UI session-menu padding profile audit is tracked under `LGO_RUNTIME_UI_SESSION_MENU_PADDING_PROFILE_AUDIT_READY`; runtime UI session-menu padding evidence is tracked under `LGO_RUNTIME_UI_SESSION_MENU_PADDING_PROFILE_EVIDENCE_REFRESH_READY`; runtime UI factory padding helper coverage is tracked under `LGO_RUNTIME_UI_FACTORY_PADDING_HELPER_COVERAGE_READY`; runtime UI factory padding helper evidence is tracked under `LGO_RUNTIME_UI_FACTORY_PADDING_HELPER_EVIDENCE_REFRESH_READY`; runtime UI controller padding profile candidates are tracked under `LGO_RUNTIME_UI_CONTROLLER_PADDING_PROFILE_CANDIDATE_READY`; post-login Character Hall UI reuse cleanup is tracked under `LGO_POST_LOGIN_RUNTIME_UI_REUSE_CLEANUP_READY`; world-hub label responsiveness is tracked under `LGO_WORLD_HUB_PROP_LABEL_RESPONSIVE_READY`; world hub visual staging is tracked under `LGO_WORLD_HUB_VISUAL_READABILITY_CLEANUP_READY`; world hub interaction readability is tracked under `LGO_WORLD_HUB_INTERACTION_READABILITY_READY`; world hub interaction evidence refresh is tracked under `LGO_WORLD_HUB_INTERACTION_EVIDENCE_REFRESH_READY`; near-interaction capture coverage is tracked under `LGO_NEAR_INTERACTION_CHECKPOINT_CAPTURE_READY`; near-interaction evidence refresh is tracked under `LGO_NEAR_INTERACTION_EVIDENCE_REFRESH_READY`; visual evidence upload packaging is tracked under `LGO_POST_LOGIN_VISUAL_EVIDENCE_UPLOAD_READY`; runtime asset budget refresh is tracked under `LGO_RUNTIME_ASSET_WEIGHT_BUDGET_REFRESH_READY`; runtime asset watch queue/profile polish is tracked under `LGO_RUNTIME_ASSET_WATCH_QUEUE_IMPORT_PROFILE_READY`; visual debt triage is tracked under `LGO_WORLD_HUB_VISUAL_DEBT_TRIAGE_READY`; session-menu focus evidence is tracked under `LGO_SESSION_MENU_FOCUS_EVIDENCE_REFRESH_READY`; Character Hall mobile density is tracked under `LGO_CHARACTER_HALL_MOBILE_COPY_DENSITY_READY`; Character Hall mobile evidence is tracked under `LGO_CHARACTER_HALL_MOBILE_COPY_EVIDENCE_REFRESH_READY`; Character Hall mobile selected CTA hierarchy is tracked under `LGO_CHARACTER_HALL_MOBILE_SELECTED_CTA_HIERARCHY_READY`; Character Hall selected CTA evidence is tracked under `LGO_CHARACTER_HALL_MOBILE_SELECTED_CTA_EVIDENCE_REFRESH_READY`; Character Hall style adoption is tracked under `LGO_CHARACTER_HALL_STYLE_ADOPTION_READY`; World HUD style adoption is tracked under `LGO_WORLD_HUD_STYLE_ADOPTION_READY`; runtime UI skin adoption evidence refresh is tracked under `LGO_RUNTIME_UI_SKIN_ADOPTION_EVIDENCE_REFRESH_READY`; runtime UI skin usage guide is tracked under `LGO_RUNTIME_UI_SKIN_USAGE_GUIDE_READY`; runtime UI style duplication audit is tracked under `LGO_RUNTIME_UI_STYLE_DUPLICATION_AUDIT_READY`; runtime UI factory split review is tracked under `LGO_RUNTIME_UI_FACTORY_SPLIT_REVIEW_READY`; runtime UI primitive factory is tracked under `LGO_RUNTIME_UI_PRIMITIVE_FACTORY_READY`; runtime UI button factory adoption is tracked under `LGO_RUNTIME_UI_BUTTON_FACTORY_ADOPTION_READY`; login debug-dot cleanup is tracked under `LGO_LOGIN_CTA_DEBUG_DOT_CLEANUP_READY`; login debug-dot evidence is tracked under `LGO_LOGIN_CTA_DEBUG_DOT_EVIDENCE_REFRESH_READY`; login CTA backing balance is tracked under `LGO_LOGIN_CTA_BACKING_BALANCE_READY`; login CTA backing evidence is tracked under `LGO_LOGIN_CTA_BACKING_EVIDENCE_REFRESH_READY`; runtime UI skin foundation is tracked under `LGO_RUNTIME_UI_SKIN_FOUNDATION_READY`; runtime UI skin adoption audit is tracked under `LGO_RUNTIME_UI_SKIN_ADOPTION_AUDIT_READY`; login NPC grounding shadow balance is tracked under `LGO_LOGIN_NPC_GROUNDING_SHADOW_BALANCE_READY`; login NPC grounding evidence is tracked under `LGO_LOGIN_NPC_GROUNDING_SHADOW_EVIDENCE_REFRESH_READY`; the project still refuses to claim visual PASS from capture/build alone.

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
- `LGO-POST-LOGIN-RUNTIME-UI-REUSE-CLEANUP-v1.0` / `LGO_POST_LOGIN_RUNTIME_UI_REUSE_CLEANUP_READY`
- `LGO-POST-LOGIN-RUNTIME-UI-REUSE-EVIDENCE-REFRESH-v1.0` / `LGO_POST_LOGIN_RUNTIME_UI_REUSE_EVIDENCE_REFRESH_READY`
- `LGO-CHARACTER-HALL-V3B-VISUAL-POLISH-v1.0` / `LGO_CHARACTER_HALL_V3B_VISUAL_POLISH_READY`
- `LGO-CHARACTER-HALL-V3B-VISUAL-POLISH-EVIDENCE-REFRESH-v1.0` / `LGO_CHARACTER_HALL_V3B_VISUAL_POLISH_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-FACTORY-CHARACTER-HALL-CLEANUP-FOLLOWUP-v1.0` / `LGO_RUNTIME_UI_FACTORY_CHARACTER_HALL_CLEANUP_FOLLOWUP_READY`
- `LGO-WORLD-HUD-RUNTIME-UI-REUSE-AUDIT-v1.0` / `LGO_WORLD_HUD_RUNTIME_UI_REUSE_AUDIT_READY`
- `LGO-WORLD-HUD-RUNTIME-UI-REUSE-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_HUD_RUNTIME_UI_REUSE_EVIDENCE_REFRESH_READY`
- `LGO-WORLD-HUD-FANTASY-PANEL-HIERARCHY-POLISH-v1.0` / `LGO_WORLD_HUD_FANTASY_PANEL_HIERARCHY_POLISH_READY`
- `LGO-WORLD-HUD-FANTASY-PANEL-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_HUD_FANTASY_PANEL_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-STATUS-COMPOSITION-CLEANUP-v1.0` / `LGO_RUNTIME_UI_STATUS_COMPOSITION_CLEANUP_READY`
- `LGO-RUNTIME-UI-STATUS-COMPOSITION-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_STATUS_COMPOSITION_EVIDENCE_REFRESH_READY`
- `LGO-COMBAT-BUTTON-COOLDOWN-VISUAL-LIGHTNESS-PASS-v1.0` / `LGO_COMBAT_BUTTON_COOLDOWN_VISUAL_LIGHTNESS_READY`
- `LGO-COMBAT-BUTTON-COOLDOWN-VISUAL-EVIDENCE-REFRESH-v1.0` / `LGO_COMBAT_BUTTON_COOLDOWN_VISUAL_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-STATE-DOC-COMPACTION-AUDIT-v1.0` / `LGO_RUNTIME_UI_STATE_DOC_COMPACTION_AUDIT_READY`
- `LGO-EXECUTION-LEDGER-ROLLUP-VIEW-v1.0` / `LGO_EXECUTION_LEDGER_ROLLUP_VIEW_READY`
- `LGO-COMMIT-CADENCE-POLICY-HARDENING-v1.0` / `LGO_COMMIT_CADENCE_POLICY_HARDENING_READY`
- `LGO-RUNTIME-UI-QUALITY-DEBT-TRIAGE-v1.0` / `LGO_RUNTIME_UI_QUALITY_DEBT_TRIAGE_READY`
- `LGO-LOGIN-NPC-GROUNDING-AND-CTA-PANEL-POLISH-v1.0` / `LGO_LOGIN_NPC_GROUNDING_CTA_PANEL_POLISH_READY`
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
- `LGO-RUNTIME-UI-PRIMITIVE-FACTORY-PASS-v1.0` / `LGO_RUNTIME_UI_PRIMITIVE_FACTORY_READY`
- `LGO-RUNTIME-UI-BUTTON-FACTORY-ADOPTION-PASS-v1.0` / `LGO_RUNTIME_UI_BUTTON_FACTORY_ADOPTION_READY`
- `LGO-RUNTIME-UI-CONTROLLER-RESPONSIBILITY-MAP-v1.0` / `LGO_RUNTIME_UI_CONTROLLER_RESPONSIBILITY_MAP_READY`
- `LGO-RUNTIME-UI-RESPONSIVE-LAYOUT-HELPER-REVIEW-v1.0` / `LGO_RUNTIME_UI_RESPONSIVE_LAYOUT_HELPER_REVIEW_READY`
- `LGO-RUNTIME-UI-RESPONSIVE-CONSTANTS-AUDIT-v1.0` / `LGO_RUNTIME_UI_RESPONSIVE_CONSTANTS_AUDIT_READY`
- `LGO-RUNTIME-UI-RESPONSIVE-SESSION-SHELL-HELPER-REVIEW-v1.0` / `LGO_RUNTIME_UI_RESPONSIVE_SESSION_SHELL_HELPER_REVIEW_READY`
- `LGO-RUNTIME-UI-FACTORY-ADOPTION-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_FACTORY_ADOPTION_EVIDENCE_REFRESH_READY`
- `LGO-SESSION-MENU-SETTING-ROW-VISUAL-POLISH-v1.0` / `LGO_SESSION_MENU_SETTING_ROW_VISUAL_POLISH_READY`
- `LGO-SESSION-MENU-SETTING-ROW-EVIDENCE-REFRESH-v1.0` / `LGO_SESSION_MENU_SETTING_ROW_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-SCREEN-SHELL-COMPONENT-REVIEW-v1.0` / `LGO_RUNTIME_UI_SCREEN_SHELL_COMPONENT_REVIEW_READY`
- `LGO-WORLD-POSE-PULSE-VISUAL-CLEANUP-v1.0` / `LGO_WORLD_POSE_PULSE_VISUAL_CLEANUP_READY`
- `LGO-RUNTIME-UI-SCREEN-SHELL-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_SCREEN_SHELL_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-ACTION-ROW-COMPONENT-REVIEW-v1.0` / `LGO_RUNTIME_UI_ACTION_ROW_COMPONENT_REVIEW_READY`
- `LGO-RUNTIME-UI-ACTION-ROW-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_ACTION_ROW_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-INPUT-FIELD-BASE-AUDIT-v1.0` / `LGO_RUNTIME_UI_INPUT_FIELD_BASE_READY`
- `LGO-RUNTIME-UI-INPUT-FIELD-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_INPUT_FIELD_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-FORM-SECTION-BASE-AUDIT-v1.0` / `LGO_RUNTIME_UI_FORM_SECTION_BASE_READY`
- `LGO-RUNTIME-UI-FORM-SECTION-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_FORM_SECTION_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-LIST-CARD-BASE-AUDIT-v1.0` / `LGO_RUNTIME_UI_LIST_CARD_BASE_READY`
- `LGO-RUNTIME-UI-LIST-CARD-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_LIST_CARD_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-STATUS-CHIP-BASE-AUDIT-v1.0` / `LGO_RUNTIME_UI_STATUS_CHIP_BASE_READY`
- `LGO-RUNTIME-UI-STATUS-CHIP-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_STATUS_CHIP_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-TOGGLE-BASE-AUDIT-v1.0` / `LGO_RUNTIME_UI_TOGGLE_BASE_READY`
- `LGO-RUNTIME-UI-TOGGLE-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_TOGGLE_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-ICON-STATUS-ROW-BASE-AUDIT-v1.0` / `LGO_RUNTIME_UI_ICON_STATUS_ROW_BASE_READY`
- `LGO-RUNTIME-UI-ICON-STATUS-ROW-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_ICON_STATUS_ROW_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-COMBAT-BUTTON-METRICS-AUDIT-v1.0` / `LGO_RUNTIME_UI_COMBAT_BUTTON_METRICS_READY`
- `LGO-RUNTIME-UI-COMBAT-BUTTON-METRICS-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_COMBAT_BUTTON_METRICS_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-COMBAT-HUD-SPACING-AUDIT-v1.0` / `LGO_RUNTIME_UI_COMBAT_HUD_SPACING_READY`
- `LGO-RUNTIME-UI-COMBAT-HUD-SPACING-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_COMBAT_HUD_SPACING_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-RESPONSIVE-BUTTON-METRICS-AUDIT-v1.0` / `LGO_RUNTIME_UI_RESPONSIVE_BUTTON_METRICS_READY`
- `LGO-RUNTIME-UI-RESPONSIVE-BUTTON-METRICS-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_RESPONSIVE_BUTTON_METRICS_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-HEADER-DIALOGUE-BUTTON-METRICS-AUDIT-v1.0` / `LGO_RUNTIME_UI_HEADER_DIALOGUE_BUTTON_METRICS_READY`
- `LGO-RUNTIME-UI-HEADER-DIALOGUE-BUTTON-METRICS-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_HEADER_DIALOGUE_BUTTON_METRICS_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-LABEL-FONT-METRICS-AUDIT-v1.0` / `LGO_RUNTIME_UI_LABEL_FONT_METRICS_READY`
- `LGO-RUNTIME-UI-LABEL-FONT-METRICS-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_LABEL_FONT_METRICS_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-TYPOGRAPHY-OWNERSHIP-SPLIT-REVIEW-v1.0` / `LGO_RUNTIME_UI_TYPOGRAPHY_OWNERSHIP_SPLIT_READY`
- `LGO-RUNTIME-UI-TYPOGRAPHY-OWNERSHIP-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_TYPOGRAPHY_OWNERSHIP_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-COMPONENT-METRIC-OWNERSHIP-DRIFT-SCAN-v1.0` / `LGO_RUNTIME_UI_COMPONENT_METRIC_OWNERSHIP_DRIFT_SCAN_READY`
- `LGO-RUNTIME-UI-COMPONENT-METRIC-OWNERSHIP-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_COMPONENT_METRIC_OWNERSHIP_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-PANEL-HIERARCHY-SIMPLIFICATION-PASS-v1.0` / `LGO_RUNTIME_UI_PANEL_HIERARCHY_SIMPLIFICATION_READY`
- `LGO-RUNTIME-UI-PANEL-HIERARCHY-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_PANEL_HIERARCHY_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-CHARACTER-HALL-CONTENT-DENSITY-POLISH-v1.0` / `LGO_RUNTIME_UI_CHARACTER_HALL_CONTENT_DENSITY_READY`
- `LGO-RUNTIME-UI-CHARACTER-HALL-CONTENT-DENSITY-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_CHARACTER_HALL_CONTENT_DENSITY_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-COMPONENT-DENSITY-BASE-AUDIT-v1.0` / `LGO_RUNTIME_UI_COMPONENT_DENSITY_BASE_READY`
- `LGO-RUNTIME-UI-COMPONENT-DENSITY-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_COMPONENT_DENSITY_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-DENSITY-ADOPTION-SCAN-v1.0` / `LGO_RUNTIME_UI_DENSITY_ADOPTION_SCAN_READY`
- `LGO-M5-VISUAL-EVIDENCE-RUNNER-SKIN-ADOPTION-EVIDENCE-v1.0` / `LGO_M5_VISUAL_EVIDENCE_RUNNER_SKIN_ADOPTION_EVIDENCE_READY`
- `LGO-VISUAL-EVIDENCE-OUTPUT-ISOLATION-AUDIT-v1.0` / `LGO_VISUAL_EVIDENCE_OUTPUT_ISOLATION_READY`
- `LGO-QUICK-FULL-GATE-STRATEGY-v1.0` / `LGO_QUICK_FULL_GATE_STRATEGY_READY`
- `LGO-VISUAL-EVIDENCE-BLANK-SCREEN-DETECTION-v1.0` / `LGO_VISUAL_EVIDENCE_BLANK_SCREEN_DETECTION_READY`
- `LGO-VISUAL-EVIDENCE-REVIEW-SUMMARY-VI-v1.0` / `LGO_VISUAL_EVIDENCE_REVIEW_SUMMARY_VI_READY`
- `LGO-RUNTIME-ASSET-WEIGHT-ACTIONABLE-BUDGET-v1.0` / `LGO_RUNTIME_ASSET_WEIGHT_ACTIONABLE_BUDGET_READY`
- `LGO-VISUAL-EVIDENCE-REVIEW-SUMMARY-VI-EVIDENCE-v1.0` / `LGO_VISUAL_EVIDENCE_REVIEW_SUMMARY_VI_EVIDENCE_READY`
- `LGO-RUNTIME-ASSET-WATCH-QUEUE-PRIORITIZATION-v1.0` / `LGO_RUNTIME_ASSET_WATCH_QUEUE_PRIORITY_READY`
- `LGO-RUNTIME-ASSET-WATCH-QUEUE-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_ASSET_WATCH_QUEUE_EVIDENCE_REFRESH_READY`
- `LGO-EVIDENCE-GATE-SEQUENTIAL-RUN-POLICY-v1.0` / `LGO_EVIDENCE_GATE_SEQUENTIAL_RUN_POLICY_READY`
- `LGO-RUNTIME-UI-RESPONSIVE-STYLE-APPLICATION-AUDIT-v1.0` / `LGO_RUNTIME_UI_RESPONSIVE_STYLE_APPLICATION_AUDIT_READY`
- `LGO-RUNTIME-UI-RESPONSIVE-STYLE-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_RESPONSIVE_STYLE_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-FACTORY-COVERAGE-AUDIT-v1.0` / `LGO_RUNTIME_UI_FACTORY_COVERAGE_AUDIT_READY`
- `LGO-RUNTIME-UI-IMAGE-LAYER-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_IMAGE_LAYER_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-STYLE-DEBT-FOLLOWUP-AUDIT-v1.0` / `LGO_RUNTIME_UI_STYLE_DEBT_FOLLOWUP_AUDIT_READY`
- `LGO-RUNTIME-UI-COMPACT-STATUS-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_COMPACT_STATUS_EVIDENCE_REFRESH_READY`
- `LGO-COMBAT-BUTTON-STATE-READABILITY-POLISH-v1.0` / `LGO_COMBAT_BUTTON_STATE_READABILITY_POLISH_READY`
- `LGO-COMBAT-BUTTON-STATE-EVIDENCE-REFRESH-v1.0` / `LGO_COMBAT_BUTTON_STATE_EVIDENCE_REFRESH_READY`
- `LGO-COMBAT-BUTTON-MOBILE-RESPONSIVE-EVIDENCE-v1.0` / `LGO_COMBAT_BUTTON_MOBILE_RESPONSIVE_EVIDENCE_READY`
- `LGO-WORLD-HUD-COMPONENT-BOUNDARY-AUDIT-v1.0` / `LGO_WORLD_HUD_COMPONENT_BOUNDARY_AUDIT_READY`
- `LGO-RUNTIME-UI-EVIDENCE-STATE-HELPER-REVIEW-v1.0` / `LGO_RUNTIME_UI_EVIDENCE_STATE_HELPER_READY`
- `LGO-WORLD-HUD-HEADER-BLOCK-REVIEW-v1.0` / `LGO_WORLD_HUD_HEADER_BLOCK_READY`
- `LGO-WORLD-HUD-ROW-HELPER-COVERAGE-AUDIT-v1.0` / `LGO_WORLD_HUD_ROW_HELPER_COVERAGE_READY`
- `LGO-WORLD-HUD-ROW-HELPER-EVIDENCE-REFRESH-v1.0` / `LGO_WORLD_HUD_ROW_HELPER_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-STYLE-OWNERSHIP-DRIFT-AUDIT-v1.0` / `LGO_RUNTIME_UI_STYLE_OWNERSHIP_DRIFT_READY`
- `LGO-RUNTIME-UI-STYLE-OWNERSHIP-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_STYLE_OWNERSHIP_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-CONTROLLER-STYLE-CONSTANTS-AUDIT-v1.0` / `LGO_RUNTIME_UI_CONTROLLER_STYLE_CONSTANTS_READY`
- `LGO-RUNTIME-UI-CONTROLLER-STYLE-CONSTANTS-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_CONTROLLER_STYLE_CONSTANTS_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-RESPONSIVE-PADDING-PROFILE-AUDIT-v1.0` / `LGO_RUNTIME_UI_RESPONSIVE_PADDING_PROFILE_AUDIT_READY`
- `LGO-RUNTIME-UI-RESPONSIVE-PADDING-PROFILE-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_RESPONSIVE_PADDING_PROFILE_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-SESSION-MENU-PADDING-PROFILE-AUDIT-v1.0` / `LGO_RUNTIME_UI_SESSION_MENU_PADDING_PROFILE_AUDIT_READY`
- `LGO-RUNTIME-UI-SESSION-MENU-PADDING-PROFILE-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_SESSION_MENU_PADDING_PROFILE_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-FACTORY-PADDING-HELPER-COVERAGE-AUDIT-v1.0` / `LGO_RUNTIME_UI_FACTORY_PADDING_HELPER_COVERAGE_READY`
- `LGO-RUNTIME-UI-FACTORY-PADDING-HELPER-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_FACTORY_PADDING_HELPER_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-CONTROLLER-PADDING-PROFILE-CANDIDATE-AUDIT-v1.0` / `LGO_RUNTIME_UI_CONTROLLER_PADDING_PROFILE_CANDIDATE_READY`
- `LGO-RUNTIME-UI-CONTROLLER-PADDING-PROFILE-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_CONTROLLER_PADDING_PROFILE_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-ONE-EDGE-LAYOUT-HELPER-AUDIT-v1.0` / `LGO_RUNTIME_UI_ONE_EDGE_LAYOUT_HELPER_READY`
- `LGO-RUNTIME-UI-ONE-EDGE-LAYOUT-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_ONE_EDGE_LAYOUT_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-COMPONENT-MARGIN-TOKEN-AUDIT-v1.0` / `LGO_RUNTIME_UI_COMPONENT_MARGIN_TOKEN_READY`
- `LGO-RUNTIME-UI-COMPONENT-MARGIN-TOKEN-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_COMPONENT_MARGIN_TOKEN_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-PRIMITIVE-THEME-SPACING-BRIDGE-AUDIT-v1.0` / `LGO_RUNTIME_UI_PRIMITIVE_THEME_SPACING_BRIDGE_READY`
- `LGO-RUNTIME-UI-PRIMITIVE-THEME-SPACING-BRIDGE-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_PRIMITIVE_THEME_SPACING_BRIDGE_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-PRIMITIVE-SIZE-TOKEN-AUDIT-v1.0` / `LGO_RUNTIME_UI_PRIMITIVE_SIZE_TOKEN_READY`
- `LGO-RUNTIME-UI-PRIMITIVE-SIZE-TOKEN-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_PRIMITIVE_SIZE_TOKEN_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-PRIMITIVE-STYLE-BOUNDARY-GUIDE-v1.0` / `LGO_RUNTIME_UI_PRIMITIVE_STYLE_BOUNDARY_GUIDE_READY`
- `LGO-RUNTIME-UI-CONTROLLER-LOCAL-STYLE-DRIFT-SCAN-v1.0` / `LGO_RUNTIME_UI_CONTROLLER_LOCAL_STYLE_DRIFT_SCAN_READY`
- `LGO-RUNTIME-UI-CONTROLLER-LOCAL-STYLE-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_CONTROLLER_LOCAL_STYLE_EVIDENCE_REFRESH_READY`
- `LGO-LOGIN-CTA-COMPONENT-VISUAL-POLISH-v1.0` / `LGO_LOGIN_CTA_COMPONENT_VISUAL_POLISH_READY`
- `LGO-LOGIN-CTA-COMPONENT-EVIDENCE-REFRESH-v1.0` / `LGO_LOGIN_CTA_COMPONENT_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-COMPONENT-BASE-REUSE-AUDIT-v1.0` / `LGO_RUNTIME_UI_COMPONENT_BASE_REUSE_READY`
- `LGO-RUNTIME-UI-COMPONENT-BASE-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_COMPONENT_BASE_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-SCREEN-SHELL-BASE-AUDIT-v1.0` / `LGO_RUNTIME_UI_SCREEN_SHELL_BASE_READY`
- `LGO-RUNTIME-UI-SCREEN-SHELL-EVIDENCE-REFRESH-v1.0` / `LGO_RUNTIME_UI_SCREEN_SHELL_EVIDENCE_REFRESH_READY`
- `LGO-RUNTIME-UI-ACTION-ROW-BASE-AUDIT-v1.0` / `LGO_RUNTIME_UI_ACTION_ROW_BASE_READY`
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
python3.12 tools/validate_lgo_runtime_ui_primitive_factory.py
python3.12 tools/validate_lgo_runtime_ui_button_factory_adoption.py
python3.12 tools/validate_lgo_runtime_ui_controller_responsibility_map.py
python3.12 tools/validate_lgo_runtime_ui_responsive_layout_helper_review.py
python3.12 tools/validate_lgo_runtime_ui_responsive_constants_audit.py
python3.12 tools/validate_lgo_runtime_ui_responsive_session_shell_helper_review.py
python3.12 tools/validate_lgo_runtime_ui_factory_adoption_evidence_refresh.py
python3.12 tools/validate_lgo_session_menu_setting_row_visual_polish.py
python3.12 tools/validate_lgo_session_menu_setting_row_evidence_refresh.py
python3.12 tools/validate_lgo_runtime_ui_screen_shell_component_review.py
python3.12 tools/validate_lgo_world_pose_pulse_visual_cleanup.py
python3.12 tools/validate_lgo_runtime_ui_screen_shell_evidence_refresh.py
python3.12 tools/validate_lgo_runtime_ui_action_row_component_review.py
python3.12 tools/validate_lgo_runtime_ui_action_row_evidence_refresh.py
python3.12 tools/validate_lgo_runtime_ui_responsive_style_application_audit.py
python3.12 tools/validate_lgo_runtime_ui_responsive_style_evidence_refresh.py
python3.12 tools/validate_lgo_runtime_ui_factory_coverage_audit.py
python3.12 tools/validate_lgo_runtime_ui_image_layer_evidence_refresh.py
python3.12 tools/validate_lgo_runtime_ui_style_debt_followup_audit.py
python3.12 tools/validate_lgo_runtime_ui_compact_status_evidence_refresh.py
python3.12 tools/validate_lgo_combat_button_state_readability_polish.py
python3.12 tools/validate_lgo_combat_button_state_evidence_refresh.py
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

`LGO-LOGIN-NPC-GROUNDING-AND-CTA-PANEL-POLISH-v1.0` has refreshed visual evidence and manual review notes in this session. Continue with `LGO-RUNTIME-UI-QUALITY-DEBT-FIRST-FIX-v1.0`, using existing validators and avoiding new micro-task docs unless a reusable gate is genuinely needed.

Recent visual passes improved scene depth, NPC staging, responsive HUD behavior, world staging density, label readability, and evidence review scoring without new gameplay or frozen-surface changes.
