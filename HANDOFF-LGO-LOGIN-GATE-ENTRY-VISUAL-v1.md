# Handoff: Login Gate Entry Visual v1

Decision: `LGO_LOGIN_GATE_ENTRY_VISUAL_CLOSED_v1`

The login/gate-entry presentation now prioritizes `LGOFinalLogin` for the full-screen background, logo, and primary enter-world button. `LGOArtV3BA` remains fallback/source material for the Gate Keeper NPC, panels, server selector, icons, and disabled state. The old V2/V3B assets remain deeper fallback. The screen is structured as a full login scene with background, logo, hero copy, Gate Keeper visual, server selector, and a large Vietnamese enter-world CTA.

Validation must include:

- `git --no-pager diff --check`
- `python3.12 tools/validate_lgo_login_gate_entry_visual_v1.py`
- `python3.12 tools/validate_package_hygiene.py`
- `./tools/lgo_playable_closure_check.sh --source-only`
- `./tools/lgo_playable_closure_check.sh --package-ready`
- `./tools/lgo_playable_closure_check.sh --runtime` when Unity is available

Reference-only pack images were not imported. The imported assets are runtime candidates, not final production art.
