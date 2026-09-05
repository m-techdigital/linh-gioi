# LGO Runtime UI Session Menu Padding Profile Evidence Refresh v1.0

Status: `LGO_RUNTIME_UI_SESSION_MENU_PADDING_PROFILE_EVIDENCE_REFRESH_READY`

## Scope

Refresh runtime screenshots after session-menu responsive padding moved into `RuntimeUiLayoutProfile`.

## Evidence

- `build/visual-evidence/latest/session-menu.png`
- `build/visual-evidence/latest/world-hub.png`
- `build/visual-evidence/latest/npc-dialogue.png`
- `build/visual-evidence/latest/target-dummy-state.png`
- `build/visual-evidence/latest/visual-runtime-evidence-manifest.json`
- `build/visual-evidence/latest/visual-runtime-evidence-review.md`

## Review Notes

- Session menu keeps the main pause panel, action row, and local settings rows readable.
- World HUD and dialogue side panels remain stable around the changed session profile ownership.
- Target dummy combat panel keeps cooldown button fit.

## Validation

- `./tools/lgo_visual_runtime_review.sh`
- `python3.12 tools/validate_lgo_runtime_ui_session_menu_padding_profile_evidence_refresh.py`
- `./tools/lgo_playable_closure_check.sh --source-only`

## Non-Claims

- No gameplay change.
- No visual asset payload change.
- No `VISUAL_RUNTIME_PASS` claim.
