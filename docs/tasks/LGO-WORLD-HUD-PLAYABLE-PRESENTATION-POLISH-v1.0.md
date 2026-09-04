# Linh Giới Online — World HUD Playable Presentation Polish v1.0

Date: `2026-09-05`

Marker: `LGO_WORLD_HUD_PLAYABLE_PRESENTATION_POLISH_READY`

## Scope

This task polishes the in-world HUD presentation layer after the V3B login and character lobby passes. It keeps the existing playable behavior, local-only combat prototype semantics, account/character flow, protocol, GameData schemas, ADRs, and design tokens unchanged.

## Changes

- Narrowed and left-aligned the in-world HUD so the world background and scene composition remain visible at 1920x1080.
- Replaced oversized image-backed secondary actions with compact UI Toolkit buttons where the V3B/V2 button art would be squeezed and unreadable.
- Improved Vietnamese world and character copy so player-facing text no longer exposes raw `class.sword` identifiers.
- Kept debug/status details available behind local settings instead of making them dominate the default world HUD.
- Confirmed the visual evidence runner can run in background and auto-close the Unity Player after all expected screenshots are written.

## Evidence

Fresh visual evidence is expected in:

- `build/visual-evidence/latest/world-hub.png`
- `build/visual-evidence/latest/enter-world.png`
- `build/visual-evidence/latest/npc-dialogue.png`
- `build/visual-evidence/latest/session-menu.png`
- `build/visual-evidence/latest/visual-runtime-evidence-manifest.json`
- `build/visual-evidence/latest/player.log`

The harness still records that screenshot capture is not a visual pass by itself. Codex or a human must review layout, scale, spacing, sharpness, hierarchy, readability, and reference similarity before claiming visual acceptance.

## Validation

Required validation set:

```bash
git --no-pager diff --check
bash -n tools/lgo_visual_runtime_review.sh
python3.12 tools/validate_m4_2_playable_ui.py
python3.12 tools/validate_m4_visible_ui.py
python3.12 tools/validate_m5_session_menu.py
python3.12 tools/validate_m6_combat_visual_readability.py
python3.12 tools/validate_lgo_login_gate_entry_visual_v1.py
python3.12 tools/validate_lgo_runtime_asset_weight.py
python3.12 tools/validate_package_hygiene.py
```

Runtime evidence command:

```bash
LGO_VISUAL_RUNTIME_TIMEOUT_SECONDS=360 ./tools/lgo_visual_runtime_review.sh
```

## Non-Claims

- No gameplay, combat, protocol, GameData schema, ADR, production auth, DB, economy, social, guild, or liveops expansion.
- No production art claim.
- No `VISUAL_RUNTIME_PASS` claim from source inspection, build success, or screenshot capture alone.
- World hub scene composition still needs a dedicated pass after the HUD is less intrusive.

## Next Step

Continue with `LGO-WORLD-HUB-SCENE-PRESENTATION-POLISH-v1.0`: improve scene-space composition, target/NPC/world readability, and visual hierarchy using existing allowed runtime assets and code paths.
