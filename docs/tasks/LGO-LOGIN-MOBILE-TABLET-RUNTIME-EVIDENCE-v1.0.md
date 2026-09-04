# LGO Login / Mobile / Tablet Runtime Evidence v1.0

Status: `DONE`

Decision: `LGO_LOGIN_MOBILE_TABLET_RUNTIME_EVIDENCE_READY`

## Scope

Validate the current login, character lobby, world HUD, dialogue, and session menu presentation across desktop, tablet, and mobile visual runtime profiles.

This task does not add gameplay, change protocol, change GameData schemas, or import new production art.

## Changes

- Added viewport-driven responsive sizing for the login logo, CTA panel, server row, and primary button.
- Kept pixel values as min/max safety rails while deriving mobile layout from screen width and short-side budget.
- Compacted mobile world HUD by hiding secondary metadata, footer actions, toast, and long debug blocks.
- Moved the session menu from HUD-local layout into a root-level overlay so it is not clipped by the in-world panel.
- Added quick profile review flow for desktop, tablet, and mobile evidence.

## Evidence

- `build/visual-evidence/profiles/desktop/login.png`
- `build/visual-evidence/profiles/desktop/world-hub.png`
- `build/visual-evidence/profiles/tablet/login.png`
- `build/visual-evidence/profiles/tablet/world-hub.png`
- `build/visual-evidence/profiles/mobile/login.png`
- `build/visual-evidence/profiles/mobile/world-hub.png`
- `build/visual-evidence/profiles/mobile/session-menu.png`

## Validation

- `git --no-pager diff --check`
- `python3.12 tools/validate_m4_2_playable_ui.py`
- `python3.12 tools/validate_m4_visible_ui.py`
- `python3.12 tools/validate_m5_session_menu.py`
- `python3.12 tools/validate_m5_world_hub_readability.py`
- `python3.12 tools/validate_lgo_device_profile_ui_budgets.py`
- `LGO_VISUAL_RUNTIME_PROFILES_FIRST_PLAYER_BUILD=skip LGO_VISUAL_RUNTIME_SERVER_BUILD=skip LGO_VISUAL_RUNTIME_CLEAR_UNITY_CACHE=0 LGO_VISUAL_RUNTIME_TIMEOUT_SECONDS=360 ./tools/lgo_visual_runtime_review_profiles.sh`

## Visual Review

- Desktop login remains V3B-aligned and readable.
- Tablet login remains readable after the responsive patch.
- Mobile login now fits logo, server selector, and CTA in the 844x390 profile without requiring manual click or player close.
- Mobile world HUD is compact enough for the current placeholder scene and no longer clips the session menu.

## Non-Claims

- No `VISUAL_RUNTIME_PASS` is claimed from these source/runtime checks alone.
- Remaining V2 world sprites are structural placeholders, not final production art.
- This does not close final art quality for world hub, NPCs, props, combat, or animation.
