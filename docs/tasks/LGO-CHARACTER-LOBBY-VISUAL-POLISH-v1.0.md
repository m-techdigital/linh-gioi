# LGO Character Lobby Visual Polish v1.0

Marker: `LGO_CHARACTER_LOBBY_VISUAL_POLISH_READY`

Date: `2026-09-05`

## Scope

Polish the Điện Nhân Vật / character lobby runtime presentation without changing account or character semantics.

## Changes

- Hid the internal class id field from player-facing UI while preserving the existing `class.sword` create-character request value.
- Reworked the selected-character preview into a clearer tu sĩ profile block with icon, status, objective, and cultivation-path summaries.
- Replaced the small enter-world/save buttons with compact readable primary buttons instead of forcing the large login CTA art into small controls.
- Fixed the main shell cross-axis stretch that made lobby/world panels grow into oversized empty blocks.
- Hardened the visual runtime evidence runner so capture can continue while the player is not focused and the harness can auto-close after manifest completion.

## Evidence

- `build/visual-evidence/latest/character-lobby.png`
- `build/visual-evidence/latest/character-select.png`
- `build/visual-evidence/latest/player.log`
- `build/visual-evidence/latest/visual-runtime-evidence-manifest.json`

## Validation

- `git --no-pager diff --check`
- `python3.12 tools/validate_m4_2_playable_ui.py`
- `python3.12 tools/validate_m4_visible_ui.py`
- `python3.12 tools/validate_lgo_login_gate_entry_visual_v1.py`
- `./tools/lgo_visual_runtime_review.sh`

## Non-Claims

- No production auth.
- No protocol, GameData schema, ADR, or design-token changes.
- No `VISUAL_RUNTIME_PASS` claim from capture alone.
- Lobby is improved for usability and hierarchy, but still needs deeper V3B-quality visual art and animation treatment later.
