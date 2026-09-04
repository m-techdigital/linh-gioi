# Linh Giới Online — Next Action

Last updated: `2026-09-05`

## Current focus

Post-login visual runtime hardening plus device-profile asset governance. Login has been upgraded to V3B-aligned runtime presentation, source-level post-login readability polish is implemented, and mobile/tablet/PC runtime asset profile budgets are now documented and validated. The standalone visual evidence harness now captures all seven screenshots, can continue in background, and auto-finishes the Unity Player after manifest completion so the operator should not need to click or close the player by hand. Character lobby usability, in-world HUD presentation, world hub scene readability, and desktop/tablet/mobile responsive evidence are now verified with fresh runtime screenshots. The visual review script supports quick iteration modes so UI/world changes do not need to rerun every full source/server gate after a nearby full pass.

Autopilot operating rule: when a task or phase is truly closed by its required gates, continue to the next roadmap-valid task/phase instead of stopping at the phase boundary. Stop only for a real blocker, unavailable runtime/tooling, required owner decision, or frozen contract/protocol/schema/ADR change.

## Next task

`LGO-WORLD-LABEL-READABILITY-PASS-v1.0`

Improve world label readability and occlusion rules for desktop/tablet/mobile screenshots. Focus on labels above Gate Keeper, Training Stone, Spirit Gate, target dummy, and shadow warning; keep Vietnamese copy and current gameplay semantics.

## Current blocker

No active blocker for source work. Visual runtime capture is currently available in this environment and has completed multiple consecutive rounds without manual player close.

Evidence:

- `tools/validate_lgo_device_profile_ui_budgets.py`
- `docs/tasks/LGO-MOBILE-TABLET-UI-PROFILE-HARDENING-v1.0.md`
- `docs/tasks/LGO-VISUAL-CAPTURE-TIMEOUT-HARDENING-v1.0.md`
- `docs/tasks/LGO-CHARACTER-LOBBY-VISUAL-POLISH-v1.0.md`
- `docs/tasks/LGO-WORLD-HUD-PLAYABLE-PRESENTATION-POLISH-v1.0.md`
- `docs/tasks/LGO-WORLD-HUB-SCENE-PRESENTATION-POLISH-v1.0.md`
- `build/visual-evidence/latest/player.log`
- `build/visual-evidence/latest/unity-build.log`
- `build/codex-autopilot/status.json`

Next allowed action: tune world label readability and occlusion if screenshots still show labels too small or clipped. The scene depth follow-up was pulled forward because runtime review showed the world floor was still too flat; it now uses a lightweight procedural training-ground texture instead of any large image import. The Gate Keeper dialogue checkpoint is repaired and staged with an offset player position. Responsive profile evidence now keeps tablet/mobile HUD compact enough to leave more scene visible. World hub staging now reuses existing V3B props for a denser, lighter scene.

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
python3.12 tools/validate_lgo_device_profile_ui_budgets.py
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
LGO_VISUAL_RUNTIME_SOURCE_GATES=fast \
LGO_VISUAL_RUNTIME_SERVER_BUILD=skip \
LGO_VISUAL_RUNTIME_CLEAR_UNITY_CACHE=0 \
./tools/lgo_visual_runtime_review.sh
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

`LGO-WORLD-SCENE-DEPTH-AND-GROUNDING-PASS-v1.0`, `LGO-WORLD-V3B-NPC-IN-WORLD-QUALITY-PASS-v1.0`, `LGO-WORLD-RESPONSIVE-HUD-VIEWPORT-POLISH-v1.0`, and `LGO-WORLD-HUB-STAGING-DENSITY-PASS-v1.0` are closed. Continue with `LGO-WORLD-LABEL-READABILITY-PASS-v1.0`.

The completed scene-depth pass added procedural ground texture cues without new runtime image weight or gameplay changes. The completed NPC pass fixed dialogue evidence staging and reduced player/NPC overlap. The responsive HUD pass compacted tablet/mobile world HUD behavior and moved the Gate Keeper inward for narrower viewports. The staging density pass reused existing props only, adding no new image files.
