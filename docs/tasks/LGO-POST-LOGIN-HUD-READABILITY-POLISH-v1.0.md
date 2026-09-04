# LGO Post-Login HUD Readability Polish v1.0

Status: `LGO_POST_LOGIN_HUD_READABILITY_SOURCE_READY_RUNTIME_BLOCKED_ENV`

Date: `2026-09-05`

## Scope

Improve source-level readability for the existing post-login runtime screens without opening production auth, production DB, economy, social, liveops, or full combat.

Touched runtime surface:

- Character Select / `Điện Nhân Vật`;
- selected character preview;
- World Hub / `Sân Luyện An Toàn`;
- NPC Dialogue;
- Session Menu;
- mobile/tablet layout profile labels.

## Implementation Notes

- Added reusable post-login readability rows for selected-character context.
- Added explicit runtime layout profile copy for desktop, tablet, and mobile HUD captures.
- Strengthened dialogue and session menu panel headings through the existing preview-panel primitive.
- Preserved local-only combat/readability hooks and existing account/character API wiring.
- Did not modify frozen protocol, GameData schemas, ADRs, or design tokens.

## Validation

Passed:

- `git --no-pager diff --check`
- `python3.12 tools/validate_lgo_login_gate_entry_visual_v1.py`
- `python3.12 tools/validate_lgo_runtime_asset_weight.py`
- `python3.12 tools/validate_m4_2_playable_ui.py`
- `python3.12 tools/validate_m4_visible_ui.py`
- `python3.12 tools/validate_m6_combat_visual_readability.py`
- `python3.12 tools/validate_m6_unity_combat_placeholder_asset_import.py`
- `python3.12 tools/validate_package_hygiene.py`
- `./tools/lgo_codex_autopilot.sh --dry-run`

Blocked:

- `./tools/lgo_continue_dev_loop.sh`
- `./tools/lgo_visual_runtime_review.sh`

Blocker: sandbox denied local socket bind during `tests/server/test_handshake_smoke.py`, raising `PermissionError: [Errno 1] Operation not permitted` before the visual runtime screenshot command could complete. This was reproduced on the 2026-09-05 rerun.

Evidence:

- `build/codex-autopilot/continue-dev-loop-post-login-hud-readability.log`
- `build/codex-autopilot/continue-dev-loop-post-login-hud-readability-rerun-2026-09-05.log`
- `build/codex-autopilot/visual-runtime-review-post-login-hud-readability-rerun-2026-09-05.log`

## Decision

`LGO_POST_LOGIN_HUD_READABILITY_SOURCE_READY_RUNTIME_BLOCKED_ENV`

Next allowed action: rerun `./tools/lgo_continue_dev_loop.sh` and `./tools/lgo_visual_runtime_review.sh` from an environment that permits local sockets and Unity/player visual capture.
