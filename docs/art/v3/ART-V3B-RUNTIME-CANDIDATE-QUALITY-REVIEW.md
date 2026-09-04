# ART V3B Runtime Candidate Quality Review

Status: LGO_ART_V3B_RUNTIME_CANDIDATE_NOT_PRODUCTION_FINAL

Created runtime candidates:

- `login_background_spirit_gate_1920x1080_v3b_candidate.png`
- `panel_main_dark_gold_v3b_candidate.png`
- `button_enter_world_gold_v3b_candidate.png`
- `gate_keeper_npc_login_v3b_candidate.png`
- `spirit_gate_v3b_candidate.png`
- `training_stone_v3b_candidate.png`
- `wind_slash_frame_01_v3b_candidate.png`
- `impact_spark_v3b_candidate.png`
- `cooldown_ready_v3b_candidate.png`
- `cooldown_active_v3b_candidate.png`
- `target_dummy_idle_v3b_candidate.png`

Reference standard:

- `/Users/minhdc/Projects/LGO-ArtPacks/LGO-ART-V3-HIGH-RES-VISUAL-TARGET-PACK/images/reference-only/01-login-gate-entry-high-res-target-reference.png`

Quality improvement:

- the Login/Gate Entry background now follows the bright celestial spirit-gate visual language directly instead of the lower-detail V2 placeholder style;
- the panel and button candidates use the same dark glass, gold trim, jade-blue gem, and luminous edge language as the reference UI;
- the Gate Keeper NPC candidate is a high-resolution transparent cutout with teal/white robes, gold trim, and sapphire staff language aligned to the Login/Gate reference;
- the Spirit Gate and Training Stone candidates raise the in-world entry/training landmarks toward the same jade, gold, cyan-spirit material language used by the login reference;
- Wind Slash, impact spark, and cooldown ring candidates improve combat readability while preserving the current M6 local/server-authoritative behavior;
- the target dummy idle candidate replaces the low-detail idle silhouette while selected/hit/recover continue to use existing fallback assets until clean-alpha V3B state sprites are generated;
- Unity import settings now cap runtime texture sizes by role so high-resolution review sources do not automatically become oversized runtime textures;
- all player-facing Vietnamese text remains rendered by Unity code, not baked into generated sprites.

Current acceptance level:

- these files are runtime candidates and may be used for visual integration;
- they are not production art;
- they are not final visual quality;
- human visual review and Unity screenshot evidence are still required before final acceptance.

Known issues:

- an earlier Gate Keeper NPC trial had a painted background and was not imported;
- selected/hit/recover target dummy trials baked checkerboard backgrounds and were not imported;
- the imported Gate Keeper NPC candidate has real alpha transparency, but robe/hair/staff edge quality still needs human review in-engine;
- Spirit Gate and Training Stone are now wired as V3B in-world runtime candidates with V2 fallback;
- player cultivator source candidate is kept for review only and is not imported into Unity runtime until a pose/animation task handles marker readability;
- Wind Slash, impact spark, and cooldown rings are wired as V3B combat feedback candidates with V2/v0.45 fallback;
- target dummy idle is wired as a V3B combat candidate with V2/v0.45 fallback;
- selected, hit, and recover target dummy V3B trials that lacked real alpha were rejected and not imported;
- panel/button alpha and edge glow need human review in-engine;
- more V3B separated assets are still required for HUD frames, combat skill icons, target dummy selected/hit/recover states, monsters, and secondary props.

Runtime weight policy:

- source PNGs may remain larger for visual review and future downsampling;
- Unity Resources should contain optimized runtime copies, not duplicate full-resolution source files for every candidate;
- Unity import caps are part of the runtime contract: 2048 for login background, 1024 for most large UI/world sprites, 512 for cooldown rings and small indicators;
- oversized source candidates must not be interpreted as approval to ship unbounded runtime textures.
