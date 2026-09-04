# Linh Giới Online — World Hub Scene Presentation Polish v1.0

Date: `2026-09-05`

Marker: `LGO_WORLD_HUB_SCENE_PRESENTATION_POLISH_READY`

## Scope

This task improves the visible World Hub presentation without adding gameplay systems or changing combat semantics. It uses existing runtime assets and UI/runtime code only.

## Changes

- Replaced the visible capsule player marker with the existing runtime cultivator sprite when available.
- Added lightweight scene dressing from existing runtime placeholder props so the world hub no longer appears as an empty debug plane.
- Kept legacy cube markers as fallback/source validation markers but prevented them from showing when matching runtime sprites are available.
- Widened the in-world camera frame and moved the HUD shell to the left edge instead of the centered content column.
- Added fast visual-review modes so screenshot iteration can avoid full source gates, full server tests, duplicate protocol generation, and Unity cache clearing when a nearby full pass already exists.

## Evidence

Latest runtime screenshots:

- `build/visual-evidence/latest/world-hub.png`
- `build/visual-evidence/latest/enter-world.png`
- `build/visual-evidence/latest/npc-dialogue.png`
- `build/visual-evidence/latest/session-menu.png`
- `build/visual-evidence/latest/player.log`

The latest harness run captured all seven checkpoints and self-closed the Unity Player after the manifest was complete.

## Quality Assessment

World Hub is cleaner and more readable than the previous debug-style view, but it is not final visual quality. Several world assets still come from `STRUCTURAL_RUNTIME_PLACEHOLDER_V2`, so this task does not claim production art or parity with V3B login/reference quality.

## Validation

Validated with:

```bash
git --no-pager diff --check
bash -n tools/prepare_unity_local_assets.sh
bash -n tools/lgo_visual_runtime_review.sh
bash -n tools/lgo_continue_dev_loop.sh
python3.12 tools/validate_m4_2_playable_ui.py
python3.12 tools/validate_m4_visible_ui.py
python3.12 tools/validate_m5_input_camera_polish.py
python3.12 tools/validate_m5_world_hub_readability.py
python3.12 tools/validate_m5_session_menu.py
python3.12 tools/validate_m6_combat_visual_readability.py
python3.12 tools/validate_m6_combat_ux_feedback.py
python3.12 tools/validate_m6_unity_combat_placeholder_asset_import.py
python3.12 tools/validate_lgo_login_gate_entry_visual_v1.py
python3.12 tools/validate_lgo_runtime_asset_weight.py
python3.12 tools/validate_lgo_device_profile_ui_budgets.py
python3.12 tools/validate_package_hygiene.py
LGO_VISUAL_RUNTIME_SOURCE_GATES=fast LGO_VISUAL_RUNTIME_SERVER_BUILD=skip LGO_VISUAL_RUNTIME_CLEAR_UNITY_CACHE=0 ./tools/lgo_visual_runtime_review.sh
```

## Non-Claims

- No protocol, GameData schema, ADR, design token, production auth, DB, economy, social, guild, liveops, or new gameplay scope.
- No production art claim.
- No `VISUAL_RUNTIME_PASS` claim from screenshot capture alone.
- V2 assets remain structural placeholders, not final visual quality.

## Next Step

Continue with `LGO-LOGIN-MOBILE-TABLET-RUNTIME-EVIDENCE-v1.0`: capture and review login/lobby/world at mobile, tablet, and desktop profiles, then adjust responsive layout and asset budgets.
