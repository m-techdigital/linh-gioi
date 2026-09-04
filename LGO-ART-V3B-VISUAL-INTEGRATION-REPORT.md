# LGO ART V3B Visual Integration Report

Decision: LGO_ART_V3B_RUNTIME_CANDIDATE_INTEGRATED_NOT_PRODUCTION_FINAL

Scope:

- Created dedicated Login/Gate Entry runtime candidates instead of cropping reference posters or composite sheets.
- Wired runtime UI to prefer V3B candidates with V2 fallback.
- Kept account, character, world, and combat gameplay semantics unchanged.
- Added role-based Unity import texture budgets so high-resolution source candidates do not become unnecessarily heavy runtime textures.

Reference used as North Star:

- `/Users/minhdc/Projects/LGO-ArtPacks/LGO-ART-V3-HIGH-RES-VISUAL-TARGET-PACK/images/reference-only/01-login-gate-entry-high-res-target-reference.png`

Created assets:

- `docs/reference-art/v3b/runtime-candidates/login/login_background_spirit_gate_1920x1080_v3b_candidate.png`
- `docs/reference-art/v3b/runtime-candidates/login/panel_main_dark_gold_v3b_candidate.png`
- `docs/reference-art/v3b/runtime-candidates/login/button_enter_world_gold_v3b_candidate.png`
- `docs/reference-art/v3b/runtime-candidates/login/gate_keeper_npc_login_v3b_candidate.png`
- `docs/reference-art/v3b/runtime-candidates/world/gate/spirit_gate_v3b_candidate.png`
- `docs/reference-art/v3b/runtime-candidates/world/training-stone/training_stone_v3b_candidate.png`
- `docs/reference-art/v3b/runtime-candidates/vfx/wind-slash/wind_slash_frame_01_v3b_candidate.png`
- `docs/reference-art/v3b/runtime-candidates/vfx/impact/impact_spark_v3b_candidate.png`
- `docs/reference-art/v3b/runtime-candidates/combat/cooldown/cooldown_ready_v3b_candidate.png`
- `docs/reference-art/v3b/runtime-candidates/combat/cooldown/cooldown_active_v3b_candidate.png`
- `docs/reference-art/v3b/runtime-candidates/combat/target-dummy/target_dummy_idle_v3b_candidate.png`
- matching Unity imports under `client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/Login/**`
- matching Unity imports under `client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/World/**`
- matching Unity imports under `client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/Combat/**`
- matching Unity imports under `client/Unity/Assets/Game/Art/Runtime/V3B/Resources/LGOArtV3B/VFX/**`
- Unity runtime imports are downscaled by role; docs/source candidates remain available for review and future controlled downsampling.

Deleted during cleanup:

- removed the lower-resolution duplicate `login_background_spirit_gate_v3b_candidate.png` from docs and Unity candidate folders after creating the 1920x1080 version.

Non-claims:

- V1 remains reference/mockup only.
- V2 remains `STRUCTURAL_RUNTIME_PLACEHOLDER_V2`.
- V3 remains reference-only visual target.
- V3B candidates are not production art and not final visual quality.
- No production auth, DB, economy, social, liveops, or new gameplay mechanics.

Remaining visual gaps:

- Gate Keeper NPC now has a transparent V3B runtime candidate, but it still needs human edge/scale review in Unity before production-final acceptance.
- Spirit Gate and Training Stone now have transparent V3B runtime candidates wired in world with V2 fallback.
- Player cultivator V3B candidate is imported but not wired over the movement capsule until a pose/animation task handles marker readability.
- Wind Slash, impact spark, and cooldown rings now have transparent V3B runtime candidates wired in combat feedback with V2/v0.45 fallback.
- Target dummy idle now has a transparent V3B runtime candidate wired in combat feedback with V2/v0.45 fallback.
- Target dummy selected/hit/recover V3B trials without real alpha were rejected and not imported.
- Unity import caps now keep login background at 2048, large UI/world sprites at 1024, and cooldown rings at 512.
- Unused player cultivator source candidate is no longer imported into Unity runtime until pose/animation integration needs it.
- Panel and button edge glow need human review in Unity screenshots.
- HUD frames, combat skill icons, target dummy selected/hit/recover states, monsters, and secondary props still need V3B high-resolution separated asset passes.
