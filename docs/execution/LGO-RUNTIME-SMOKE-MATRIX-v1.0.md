# Linh Gioi Runtime Smoke Matrix v1.0

Marker: `LGO_RUNTIME_SMOKE_MATRIX_READY`

## Purpose

This matrix keeps runtime evidence discoverable and prevents repeated stop/start behavior caused by ad hoc command selection.

## Source Gates

| Gate | Command | Claim |
|---|---|---|
| Package hygiene | `python3.12 tools/validate_package_hygiene.py` | source tree has no forbidden cache/package clutter |
| Continuous mode | `python3.12 tools/validate_lgo_continuous_development_mode.py` | continuous workflow docs/tools are present |
| Playable source closure | `./tools/lgo_playable_closure_check.sh --source-only` | source validators pass |
| Package ready closure | `./tools/lgo_playable_closure_check.sh --package-ready` | source can be packaged cleanly |


## 2D Onboarding Gates

These gates validate the current `feature/2d` onboarding evidence without re-running Unity. They are used after Unity compile, Editor smoke, macOS Player build, and visual capture have already produced artifacts. Missing artifacts are `UNVERIFIED_ENVIRONMENT`, not PASS.

| Gate | Evidence | Required proof |
|---|---|---|
| `two_d_onboarding_smoke` | `build/2d-onboarding/twod-onboarding-smoke.json` | `status=PASS`, `finalStep=Complete`, `linhThanhUnlocked=true` |
| `two_d_player_build` | `build/2d-onboarding-player/build-macos-player.log` | `LGO_MACOS_PLAYER_BUILD result=Succeeded`, `errors=0`, `Build Finished, Result: Success` |
| `two_d_visual_capture` | `build/2d-onboarding-visual/twod-onboarding-visual-manifest.json` | `status=PASS`, `screenshotCount>=14`, `finalStep=Complete`, `runtimeMapSnapshot` contains `WorldMapNetwork: hub=linh-thanh`, `LinhThanhHubRuntime:`, `HubShell: linh-thanh`, `district=plaza`, `district=academy`, `district=market`, `PlazaShell: district=plaza`, `AcademyShell: district=academy`, `MarketShell: district=market`, `SpiritTempleShell: district=spirit-temple`, `ResidentialShell: district=residential`, `ForgeShell: district=forge`, `GuildShell: district=guild`, `HarborShell: district=harbor`, `safe-no-trade-backend`, `safe-no-skill-backend`, `safe-no-economy-backend`, `safe-no-buff-backend`, `safe-no-housing-backend`, `safe-no-crafting-backend`, `safe-no-guild-backend`, `safe-no-travel-backend`; `runtimeLinhThanhUnlockSnapshot` contains `LinhThanhUnlock`, `unlocked=True`, `unlock=plaza`, `safe-local-no-teleport`; `runtimeLinhThanhAcademyShellSnapshot` contains `AcademyShell: district=academy`, `skill-hall=preview-only`, `class-trainer=locked`, `safe-no-skill-backend`, `safe-local-no-backend`; `runtimeLinhThanhMarketShellSnapshot` contains `MarketShell: district=market`, `vendor-row=preview-only`, `auction-board=locked`, `safe-no-trade-backend`, `safe-no-economy-backend`, `safe-local-no-backend`; `runtimeLinhThanhSpiritTempleShellSnapshot` contains `SpiritTempleShell: district=spirit-temple`, `blessing-altar=preview-only`, `story-shrine=locked`, `safe-no-buff-backend`, `safe-local-no-backend`; `runtimeLinhThanhResidentialShellSnapshot` contains `ResidentialShell: district=residential`, `npc-home-row=preview-only`, `social-chat-node=locked`, `safe-no-housing-backend`, `safe-local-no-backend`; `runtimeLinhThanhForgeShellSnapshot` contains `ForgeShell: district=forge`, `anvil-row=preview-only`, `craft-board=locked`, `safe-no-crafting-backend`, `safe-local-no-backend`; `runtimeLinhThanhGuildShellSnapshot` contains `GuildShell: district=guild`, `guild-hall=preview-only`, `guild-banner=local-preview`, `safe-no-guild-backend`, `safe-local-no-backend`; `runtimeLinhThanhHarborShellSnapshot` contains `HarborShell: district=harbor`, `spirit-boat=preview-only`, `travel-board=locked`, `safe-no-travel-backend`, `safe-local-no-backend`; `runtimeLinhThanhPlazaHubSnapshot` contains `PlazaHubRuntime`, `unlocked=True`, `npc=gate-guide`, `npc=merchant-preview`, `board=event-local-preview`, `interaction=npc-merchant-preview`, `safe-local-no-backend`, `safe-local-no-shop-backend`; `runtimeHubTransitionSnapshot` contains `HubTransition`, `from=east-gate`, `to=plaza`, `mode=local-route-preview`, `safe-local-no-teleport-backend`; `runtimePlazaHubInputSnapshot` contains `PlazaHubInput`, `selected=merchant-preview`, `layout=spaced-social-triangle`, `controls=P select, E interact`, `interaction=npc-merchant-preview`, `safe-local-no-shop-backend`; `runtimePlazaHubDetailSnapshot` contains `PlazaHubDetail`, `selected=merchant-preview`, `role=starter-gear-preview`, `detail=try-before-shop`, `safe-no-shop-backend`, `safe-local-no-backend`; `runtimePlazaHubLayoutSnapshot` contains `PlazaHubLayout`, `anchors=5`, `anchor=social-spawn@center`, `anchor=event-board@upper-mid`, `anchor=gate-guide@left`, `anchor=merchant-preview@right`, `anchor=guild-locked@far-right`, `safe-local-no-backend`; `runtimePlazaReadabilitySnapshot` contains `PlazaReadability`, `mode=label-rail`, `world-label-density=reduced`, `target-chips=event-board,gate-guide,merchant-preview`, `safe-local-no-backend`; `runtimeDongMonReadabilitySnapshot` contains `DongMonReadability`, `mode=route-label-rail`, `world-label-density=reduced`, `chips=gate,stone,jump,dash,slime`, `avoids-hud-overlap`, `safe-local-no-backend`; `runtimeDongMonUnityTilemapSnapshot` contains `DongMonUnityTilemap`, `renderer=TilemapRenderer`, `grid=Grid`, `source=LGOMaps/DongMonChunkPlacement`, `cells=16`, `safe-no-source-image`, `safe-no-3d`; `runtimeTilemapSnapshot` contains `ChunkFlow`, `chunk_gate_entry`, `chunk_slime_arena`; `runtimeDongMonTilePaletteSnapshot` contains `DongMonTilePalette`, `tile_ground_grass:earth-green:soft-grass-edge`, `tile_dash_lane:spirit-cyan:wind-streak`, `safe-no-source-image`; `runtimeDongMonTilePaletteSourceSnapshot` contains `DongMonTilePaletteSource`, `resource=LGOMaps/DongMonTilePalette`, `tile_dash_lane=True`, `safe-no-source-image=True`, `safe-runtime-resource=True`; `runtimeDongMonChunkPlacementSourceSnapshot` contains `DongMonChunkPlacementSource`, `resource=LGOMaps/DongMonChunkPlacement`, `chunk_gate_entry@-3.70,-2.02x4`, `chunk_dash_lane@1.82,-1.02x4`, `authored-placement=True`, `safe-runtime-resource=True`, `safe-no-3d=True`; `runtimeDongMonAuthoredPassSnapshot` contains `DongMonAuthoredPass`, `route-segments=5`, `detail-density=readable`, `collision-boundaries=from-bands`, `no-random-decoration`; inventory input is applied |

Run:

```bash
python3.12 tools/lgo_runtime_smoke_matrix.py --phase two-d
```

The 2D closure marker is:

```text
LGO_RUNTIME_SMOKE_MATRIX_2D_PASS
```

## Runtime Gates

| Gate | Command | Required marker |
|---|---|---|
| M4 inherited runtime | `./tools/lgo_m4_closure_check.sh --runtime` | `LGO_M4_CLOSURE_RUNTIME_GATES_PASS` |
| M5 first playable loop | `./tools/run_m5_first_playable_loop_once.sh` | `M5_FIRST_PLAYABLE_LOOP_RUNTIME_SMOKE_PASS` |
| M5 guided training loop | `./tools/run_m5_guided_training_loop_once.sh` | `M5_GUIDED_TRAINING_LOOP_RUNTIME_SMOKE_PASS` |
| M5 lightweight dialogue | `./tools/run_m5_lightweight_dialogue_once.sh` | `M5_LIGHTWEIGHT_NPC_DIALOGUE_RUNTIME_SMOKE_PASS` |
| M6 minimal local combat | `./tools/run_m6_minimal_local_combat_once.sh` | `M6_MINIMAL_LOCAL_COMBAT_RUNTIME_SMOKE_PASS` |
| M6 Unity combat intent client | `./tools/run_m6_unity_combat_intent_client_once.sh` | `M6_UNITY_COMBAT_INTENT_CLIENT_RUNTIME_SMOKE_PASS` |
| M6 Unity Java combat smoke | `./tools/run_m6_unity_java_combat_smoke.sh` | `M6_UNITY_JAVA_COMBAT_SMOKE_PASS` |
| M6 server-authoritative pilot | `./tools/run_m6_server_authoritative_combat_pilot.sh` | `M6_SERVER_AUTHORITATIVE_COMBAT_PILOT_RUNTIME_PASS` |
| M6 Unity Java combat E2E | `./tools/run_m6_unity_java_combat_e2e.sh` | `M6_UNITY_JAVA_COMBAT_E2E_PASS_v0.52.0` |

## One-Command Runtime Closure

Use:

```bash
./tools/lgo_playable_closure_check.sh --runtime
```

The final runtime closure marker is:

```text
LGO_PLAYABLE_CLOSURE_RUNTIME_GATES_PASS
```

## Failure Rules

- Source/static failure is `FIX_REQUIRED`.
- Missing Unity/player runtime is `UNVERIFIED_ENVIRONMENT`.
- `executed=0` is not a pass.
- Existing runtime smoke can be hardened, but gameplay semantics must not change inside this task.
- Do not mask failures with `|| true` in evidence commands.
