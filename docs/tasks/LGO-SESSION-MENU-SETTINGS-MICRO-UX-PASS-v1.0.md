# LGO Session Menu Settings Micro UX Pass v1.0

Date: 2026-09-05

## Scope

Polish the in-world session menu and local display settings shell without adding gameplay, persistence, production auth, DB, economy, social, liveops, protocol, GameData, ADR, or design-token changes.

## Changes

- Reworked the session menu into a narrower responsive pause sheet with clearer session context, objective rows, and grouped actions.
- Converted the local settings area from sparse text into framed control rows on desktop.
- Hid detailed settings on tablet/mobile to avoid clipped titles and cramped controls.
- Kept all settings local to the current runtime session.

## Evidence

- `build/visual-evidence/latest/session-menu.png`
- `build/visual-evidence/latest/world-hub.png`
- `build/visual-evidence/profiles/desktop/session-menu.png`
- `build/visual-evidence/profiles/tablet/session-menu.png`
- `build/visual-evidence/profiles/mobile/session-menu.png`

## Validation

- `git --no-pager diff --check`
- `python3.12 tools/validate_m5_session_menu.py`
- `python3.12 tools/validate_m5_local_settings.py`
- `python3.12 tools/validate_m4_visible_ui.py`
- `python3.12 tools/validate_m4_2_playable_ui.py`
- `python3.12 tools/validate_package_hygiene.py`
- `LGO_VISUAL_RUNTIME_SOURCE_GATES=fast LGO_VISUAL_RUNTIME_SERVER_BUILD=skip LGO_VISUAL_RUNTIME_CLEAR_UNITY_CACHE=0 LGO_VISUAL_RUNTIME_TIMEOUT_SECONDS=420 ./tools/lgo_visual_runtime_review.sh`
- `LGO_VISUAL_RUNTIME_SOURCE_GATES=fast LGO_VISUAL_RUNTIME_SERVER_BUILD=skip LGO_VISUAL_RUNTIME_CLEAR_UNITY_CACHE=0 LGO_VISUAL_RUNTIME_TIMEOUT_SECONDS=420 LGO_VISUAL_RUNTIME_PROFILES_FIRST_PLAYER_BUILD=skip ./tools/lgo_visual_runtime_review_profiles.sh`

## Decision

`LGO_SESSION_MENU_SETTINGS_MICRO_UX_READY`

No `VISUAL_RUNTIME_PASS` is claimed from capture alone.
