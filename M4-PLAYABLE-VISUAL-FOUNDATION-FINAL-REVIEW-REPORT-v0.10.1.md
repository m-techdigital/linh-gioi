# M4 Playable Visual Foundation Final Review Report v0.10.1

## Decision

`LGO_M4_PLAYABLE_VISUAL_FOUNDATION_CLOSED_LOCAL_v0.10.1`

## Provenance

- Current commit hash: `4541f00`
- Current tag: `lgo-m4-playable-visual-closed-local-v0.10.1`
- Previous baseline tag: `lgo-m3b-runtime-closed-local-v0.8.5`

## Requirement Summary

Package the verified M4-0 playable client vertical slice and M4-1 visual placeholder foundation for external review. This does not open a new milestone and does not add gameplay beyond M4-0/M4-1.

## Implementation Summary

- M4-0 playable login/lobby/world shell connects Unity to the existing M3-B account/character API, creates/selects characters, enters a local world shell, moves a placeholder marker, saves position, and verifies restart persistence through the M4 smoke helper.
- M4-1 visual placeholder foundation adds original SVG source placeholders, runtime art catalog, world placeholder markers, UI palette integration, visual identity guide, art manifest, validator, and visual smoke helper.

## Runtime Evidence Summary

Local runtime closure was reported by the user-provided/local final state for:

- `M3B_UNITY_ACCOUNT_CHARACTER_RUNTIME_SMOKE_PASS`
- `M4_PLAYABLE_VERTICAL_SLICE_RUNTIME_SMOKE_PASS`
- `M4_VISUAL_PLACEHOLDER_FOUNDATION_SMOKE_PASS`

This report packages source for review and does not expand the runtime claims beyond those markers.

## Inventory

Changed files list: `LGO-M4-PLAYABLE-VISUAL-FOUNDATION-v0.10.1-CHANGED-FILES.txt`

Deleted files list: `LGO-M4-PLAYABLE-VISUAL-FOUNDATION-v0.10.1-DELETIONS.txt`

```text
A	.github/workflows/m0-foundation.yml
A	.github/workflows/m0-unity-self-hosted.yml
M	.gitignore
M	README.md
M	START-HERE.md
M	VERSIONING.md
A	admin/.gitkeep
A	client/Unity/Assets/Game/Art.meta
A	client/Unity/Assets/Game/Art/Characters.meta
A	client/Unity/Assets/Game/Art/Characters/lgo_character_hero_sword_placeholder.svg
A	client/Unity/Assets/Game/Art/Characters/lgo_character_hero_sword_placeholder.svg.meta
A	client/Unity/Assets/Game/Art/Items.meta
A	client/Unity/Assets/Game/Art/Items/lgo_item_healing_gourd_icon.svg
A	client/Unity/Assets/Game/Art/Items/lgo_item_healing_gourd_icon.svg.meta
A	client/Unity/Assets/Game/Art/Items/lgo_item_iron_sword_icon.svg
A	client/Unity/Assets/Game/Art/Items/lgo_item_iron_sword_icon.svg.meta
A	client/Unity/Assets/Game/Art/Items/lgo_item_spirit_stone_icon.svg
A	client/Unity/Assets/Game/Art/Items/lgo_item_spirit_stone_icon.svg.meta
A	client/Unity/Assets/Game/Art/LinhGioi.Art.asmdef
A	client/Unity/Assets/Game/Art/LinhGioi.Art.asmdef.meta
A	client/Unity/Assets/Game/Art/Maps.meta
A	client/Unity/Assets/Game/Art/Maps/lgo_map_training_ground_tile.svg
A	client/Unity/Assets/Game/Art/Maps/lgo_map_training_ground_tile.svg.meta
A	client/Unity/Assets/Game/Art/Monsters.meta
A	client/Unity/Assets/Game/Art/Monsters/lgo_monster_shadow_slime_placeholder.svg
A	client/Unity/Assets/Game/Art/Monsters/lgo_monster_shadow_slime_placeholder.svg.meta
A	client/Unity/Assets/Game/Art/NPCs.meta
A	client/Unity/Assets/Game/Art/NPCs/lgo_npc_keeper_placeholder.svg
A	client/Unity/Assets/Game/Art/NPCs/lgo_npc_keeper_placeholder.svg.meta
A	client/Unity/Assets/Game/Art/README.md
A	client/Unity/Assets/Game/Art/README.md.meta
A	client/Unity/Assets/Game/Art/Runtime.meta
A	client/Unity/Assets/Game/Art/Runtime/M4VisualFoundationSmokeRunner.cs
A	client/Unity/Assets/Game/Art/Runtime/M4VisualFoundationSmokeRunner.cs.meta
A	client/Unity/Assets/Game/Art/Runtime/RuntimeArtCatalog.cs
A	client/Unity/Assets/Game/Art/Runtime/RuntimeArtCatalog.cs.meta
A	client/Unity/Assets/Game/Art/Shared.meta
A	client/Unity/Assets/Game/Art/Shared/lgo_shared_palette_motifs.svg
A	client/Unity/Assets/Game/Art/Shared/lgo_shared_palette_motifs.svg.meta
A	client/Unity/Assets/Game/Art/Skills.meta
A	client/Unity/Assets/Game/Art/Skills/lgo_skill_shadow_bind_icon.svg
A	client/Unity/Assets/Game/Art/Skills/lgo_skill_shadow_bind_icon.svg.meta
A	client/Unity/Assets/Game/Art/Skills/lgo_skill_spirit_guard_icon.svg
A	client/Unity/Assets/Game/Art/Skills/lgo_skill_spirit_guard_icon.svg.meta
A	client/Unity/Assets/Game/Art/Skills/lgo_skill_wind_slash_icon.svg
A	client/Unity/Assets/Game/Art/Skills/lgo_skill_wind_slash_icon.svg.meta
A	client/Unity/Assets/Game/Art/UI.meta
A	client/Unity/Assets/Game/Art/UI/lgo_ui_button_rune.svg
A	client/Unity/Assets/Game/Art/UI/lgo_ui_button_rune.svg.meta
A	client/Unity/Assets/Game/Art/UI/lgo_ui_frame_panel.svg
A	client/Unity/Assets/Game/Art/UI/lgo_ui_frame_panel.svg.meta
A	client/Unity/Assets/Game/Art/UI/lgo_ui_status_api_icon.svg
A	client/Unity/Assets/Game/Art/UI/lgo_ui_status_api_icon.svg.meta
A	client/Unity/Assets/Game/Art/VFX.meta
A	client/Unity/Assets/Game/Art/VFX/lgo_vfx_spirit_burst_marker.svg
A	client/Unity/Assets/Game/Art/VFX/lgo_vfx_spirit_burst_marker.svg.meta
M	client/Unity/Assets/Game/Bootstrap/LinhGioi.Bootstrap.asmdef
M	client/Unity/Assets/Game/Bootstrap/Runtime/GameBootstrap.cs
M	client/Unity/Assets/Game/Foundation/Runtime/ClientRuntimeConfig.cs
M	client/Unity/Assets/Game/UI/LinhGioi.UI.asmdef
A	client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs
A	client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs.meta
M	client/Unity/Assets/Game/World/LinhGioi.World.asmdef
A	client/Unity/Assets/Game/World/Runtime/M4PlayableVerticalSliceSmokeRunner.cs
A	client/Unity/Assets/Game/World/Runtime/M4PlayableVerticalSliceSmokeRunner.cs.meta
A	client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs
A	client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs.meta
M	client/Unity/Assets/StreamingAssets/linhgioi-client.json
A	docs/art/LGO-RUNTIME-ART-MANIFEST-v0.10.0.md
A	docs/art/LGO-VISUAL-IDENTITY-GUIDE-v0.10.0.md
A	docs/tasks/M4-PLAYABLE-VERTICAL-SLICE.md
A	infra/.gitkeep
A	m0-manifest.json
A	m1-manifest.json
A	m2-manifest.json
A	m3-manifest.json
A	m4-manifest.json
A	m4-visual-manifest.json
A	tools/m4_playable_vertical_slice_runtime.py
A	tools/m4_visual_foundation_runtime.py
A	tools/run_m4_playable_vertical_slice_once.sh
A	tools/run_m4_visual_foundation_once.sh
M	tools/validate_m3b_unity_integration.py
A	tools/validate_m4_playable_source.py
A	tools/validate_m4_source.sh
A	tools/validate_m4_visual_foundation.py
```

## Frozen Surfaces

Confirmed unchanged by this packaging task:

- `protocol/**`
- `gamedata/schemas/**`
- `docs/adr/**`
- `client/Unity/Assets/Game/UI/design-tokens.json`

## Known Limitations

- Placeholder art only; not final production art.
- Not final UI/art direction.
- No production authentication.
- No database persistence.
- No full MMO gameplay.
- No full combat.

## Review Focus For ChatGPT

- Source hygiene.
- Task scope drift.
- Unity runtime command correctness.
- Design/art quality gap.
- Next prompt planning.
