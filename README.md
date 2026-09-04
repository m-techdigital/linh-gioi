# LINH GIỚI ONLINE — Governance Baseline v1.0

**Current status:** `M6_COMBAT_READINESS_SPEC_CLOSED_v0.32.0` with governance work in progress.

**Code governance:** read `docs/execution/CODE-GOVERNANCE-CONTRACT.md`, `docs/execution/CODE-OWNERSHIP-MAP.md`, and `docs/execution/CODE-QUALITY-GATES.md` before future implementation handoff.

**Non-claims:** no new gameplay, combat, production auth, DB implementation, production admin/player portal, production art, or runtime progress is claimed by governance docs.

## Historical Playable Baseline

**Current status:** `M5_GUIDED_TRAINING_LOOP_SOURCE_READY` after accepting the visual reference pack `LGO_VISUAL_REFERENCE_PACK_ACCEPTED_v0.16.5` and hardening the local non-combat loop into Gate Keeper -> Training Stone guidance. Previous visual evidence status is `M5_VISUAL_EVIDENCE_UX_REVIEW_READY`; previous M5 first playable loop status is `M5_FIRST_PLAYABLE_LOOP_SOURCE_READY`; previous M4 visible UI status is `M4_VISIBLE_UI_USABILITY_SOURCE_READY`; previous M4 closure automation status is `M4_PLAYABLE_SLICE_STABILIZATION_SOURCE_READY`; previous M4 UI/art status is `M4_PLAYABLE_UI_ART_QUALITY_SOURCE_READY`; previous M3 server/API status is `M3_ACCOUNT_CHARACTER_PERSISTENCE_RUNTIME_SMOKE_CLOSED`; previous M2 status remains `M2_RUNTIME_CANDIDATE_HARDENED_READY_FOR_LOCAL_EVIDENCE` pending local Unity evidence.

The accepted foundation remains `M0_RUNTIME_CLOSED`. The accepted base is `M1_OFFLINE_COMBAT_RUNTIME_CLOSED` from `linh-gioi-m1-offline-combat-runtime-closed-v0.5.3-full-source.zip`. M2 opens the first online session scaffold: Unity can keep a realtime connection after the accepted handshake, send a movement intent, and receive an authoritative server transform snapshot from Java Netty using existing protobuf messages.

This is intentionally still a prototype. M3-B connects Unity to the closed M3 dev account/character API, but it does not add production auth, zones, economy, guilds, marketplace, PvP ranking, or MMO-scale AOI.

## What this source contains

- M0 runtime-closed foundation: Unity `6000.3.2f1`, Java `25`, Maven `3.9.16`, Protobuf `3.13.0`, GameData v1, Java API/realtime handshake, Unity-built Linux player smoke evidence.
- M1 runtime-closed offline deterministic combat slice: GameData-driven `skill.sword.wind_slash`, `monster.shadow.slime`, deterministic duel, HUD prototype, offline smoke command, and accepted runtime evidence.
- M2 source-ready online session scaffold:
  - Java `OnlineSession` and `OnlineSessionHandler` after accepted `ClientHello`.
  - One-session movement loop using existing `MoveIntent` and `PlayerTransformSnapshot` protobuf contract.
  - Sequence acknowledgement and idempotent duplicate/late sequence handling.
  - Invalid movement failure path that closes only the offending session.
  - Server reconnect/survival integration tests.
  - Python TCP `online-session-smoke.py` for server runtime smoke.
  - Unity `TcpRealtimeClient.SendMoveIntentAsync(...)` and `--lgo-m2-online-session-smoke` runner.
  - M2 static validator, checklist, prompt, runtime evidence plan, and manifest.

## Validate source

```bash
./tools/local_macos_setup.sh
```

or, on a prepared environment:

```bash
./tools/lgo_m4_closure_check.sh --source-only
```

For the current broader playable source/package gates, use:

```bash
./tools/lgo_playable_closure_check.sh --source-only
./tools/lgo_playable_closure_check.sh --package-ready
```

Expected classification includes:

```text
PROJECT STATE VALIDATION PASS
M4 STABILIZATION VALIDATION PASS
M4 SOURCE VALIDATION PASS
LGO_M4_CLOSURE_SOURCE_GATES_PASS
```

## Server online-session smoke

After server build is available with Java `25` and Maven `3.9.16`, start realtime and run:

```bash
./server/scripts/online-session-smoke.py --host 127.0.0.1 --port 17777
```

Expected marker:

```text
M2_ONLINE_SESSION_SMOKE_PASS
```


## One-command local runtime candidate

For M2 v0.6.2, prefer one local command instead of many manual steps:

```bash
./tools/run_m2_local_runtime_once.sh
```

This command validates source, prepares disposable Unity generated assets, runs server smoke when Java 25/Maven are available, builds Unity evidence, and writes `UPLOAD-THESE-FILES-M2-RUNTIME-CANDIDATE.txt`. The Linux player still needs final sandbox replay before `M2_ONLINE_SESSION_RUNTIME_CLOSED` can be claimed.


## M2 v0.6.2 hardening

The v0.6.2 candidate tightens runtime behavior without opening M3:

- Server and Unity now both reject non-finite or diagonal-over-speed `MoveIntent.move_axis` values.
- Unity M2 smoke proves first movement, duplicate sequence idempotence, and a second authoritative movement snapshot.
- The one-command runner no longer prints `M2_LOCAL_RUNTIME_CANDIDATE_READY` when server or Unity evidence was skipped or missing. Skipped diagnostic runs are marked `M2_LOCAL_RUNTIME_CANDIDATE_PARTIAL`.

## M2 runtime evidence

M2 runtime closure is not claimed by source validation alone. To close M2, build a Unity Linux player with Unity `6000.3.2f1`, start the Java realtime server, run the player with:

```text
--lgo-m2-online-session-smoke --lgo-m2-host 127.0.0.1 --lgo-m2-port <port>
```

The player JSON must contain `status=PASS`, `handshakeAccepted=true`, `acknowledgedSequence=1`, `entityId=1001`, and position `x≈0.4` after one movement intent.

## Authoritative technology baseline

- Client: Unity `6000.3.2f1` + C#.
- Backend: Java `25` + Spring Boot `4.1.1` for API/business backend.
- Realtime: Java + Netty `4.2.17.Final`, authoritative session scaffold.
- Wire contract: Protocol Buffers `3.13.0`; M2 consumes existing `ClientHello`, `ServerHello`, `MoveIntent`, and `PlayerTransformSnapshot` only.
- GameData: compiled manifest version `1`.
- Initial platforms: Android + Windows; Linux player is used for sandbox evidence.

## Directory ownership

| Path | Canonical owner |
|---|---|
| `protocol/**` | S0 Architect |
| `docs/**` contracts / ADR | S0 Architect |
| `client/Unity/Assets/Game/**` | S1 Client, except UI lane-owned paths |
| `client/Unity/Assets/Game/UI/**` | S3 UI/UX |
| `client/Unity/Assets/Game/Combat/**` | M1 Offline Combat slice |
| `client/Unity/Assets/Game/Networking/**` | M2 Online Session client slice |
| `server/realtime/**` | S2/M2 Realtime session slice |
| `server/api/**`, `server/shared/**` | S2 Server foundation |
| `gamedata/**` | S4 Content |
| `tools/**`, `tests/**`, CI | S5 QA/Tools |

No sandbox may casually edit another owner surface. Protocol/schema/design contract changes require an explicit S0 contract-change request.

## Current next step

Allowed next verification step: M4-0 playable vertical slice runtime verification on a host with pinned Protobuf `3.13.0` tooling and Unity `6000.3.2f1`. M2 runtime evidence remains pending; do not claim `M2_ONLINE_SESSION_RUNTIME_CLOSED` from this M4 source work.

## M3 Account / Character Persistence v0.7.0

Current M3 source candidate: `M3_ACCOUNT_CHARACTER_PERSISTENCE_SOURCE_READY`. This was opened by explicit project-owner override from the M2 runtime candidate state; it does not claim `M2_ONLINE_SESSION_RUNTIME_CLOSED`.

Key commands:

```bash
./tools/validate_m3_source.sh
./server/build.sh
./server/test.sh
./tools/run_m3_api_persistence_once.sh
```

Main persistence artifact at runtime: `players-v1.json` under `LG_API_PERSISTENCE_DIR`. Raw dev keys must not be persisted.


## M3-B Unity Account / Character Integration v0.8.0

M3 server/API persistence is accepted as `M3_ACCOUNT_CHARACTER_PERSISTENCE_RUNTIME_SMOKE_CLOSED`. M3-B adds the Unity client integration layer on top of that closed API slice:

- `ClientRuntimeConfig.apiBaseUrl` and `apiTimeoutSeconds`.
- `LinhGioi.Account` Unity assembly.
- `AccountApiClient` using real `UnityWebRequest` calls to the M3 API.
- Runtime models for dev login, account, character, list, create, load, and save position.
- `--lgo-m3b-account-character-smoke` command path.
- `--lgo-m3b-expect-existing` restart persistence assertion path.

Runtime closure still requires a current Unity player built from this exact source and executed through `./tools/run_m3b_unity_account_character_once.sh --unity-player "$PWD/build/unity-player-macos/LinhGioiOnline.app/Contents/MacOS/Unity"`. Missing Unity player/editor is `UNVERIFIED_ENVIRONMENT`, not PASS.

## Current M4-0 gate

M3-B is locally closed and M4-0 source work is present. Runtime closure still requires a current macOS Unity player and the `M4_PLAYABLE_VERTICAL_SLICE_RUNTIME_SMOKE_PASS` marker.

M3-B hotfix v0.8.1: Unity local compile support adds UnityWebRequest module dependency and generated protocol asmdef reference to Google.Protobuf.dll. No frozen protocol/schema changes.


## Local sandbox command discipline v0.8.5

Future sandbox commands must follow `docs/execution/LOCAL-SANDBOX-COMMAND-DISCIPLINE.md` before local validation/build/runtime work. The operating assumption is that commands are run from repo root `LinhGioiOnline`; commands must avoid placeholder executable paths, `...`, and `|| true` in gates.

For M3-B macOS runtime smoke, the Unity player executable is:

```text
build/unity-player-macos/LinhGioiOnline.app/Contents/MacOS/Unity
```

For Unity protocol compilation, generate local disposable C# protocol files into:

```text
client/Unity/Assets/Game/Protocol/Generated
```

M3-B local closure marker is:

```text
M3B_UNITY_ACCOUNT_CHARACTER_RUNTIME_SMOKE_PASS
```

Do not infer broader M0 runtime closure or M4 readiness from this slice.

## M4-0 Playable Client Vertical Slice v0.9.0

M4-0 adds a Unity playable shell that reuses the M3-B `AccountApiClient`:

- dev login UI with default local dev key
- account/profile status display
- character lobby list/create/select flow
- world entry with a placeholder player marker at persisted position
- WASD/arrow movement, Q/E rotation
- save position to the existing API
- restart-aware Unity player smoke marker: `M4_PLAYABLE_VERTICAL_SLICE_RUNTIME_SMOKE_PASS`

Source validation:

```bash
./tools/validate_m4_source.sh
```

Runtime smoke after building a current macOS player:

```bash
./tools/run_m4_playable_vertical_slice_once.sh --unity-player "$PWD/build/unity-player-macos/LinhGioiOnline.app/Contents/MacOS/Unity"
```

## M4-1 Visual Placeholder Foundation v0.10.0

M4-1 adds original placeholder art sources under `client/Unity/Assets/Game/Art/**`, a concise visual identity guide in `docs/art/`, and a runtime art catalog used by the playable UI/world shell.

Validation:

```bash
python3.12 tools/validate_m4_visual_foundation.py
```

Runtime visual smoke after building a current macOS player:

```bash
./tools/run_m4_visual_foundation_once.sh --unity-player "$PWD/build/unity-player-macos/LinhGioiOnline.app/Contents/MacOS/Unity"
```

## M4-2/M4-3 Playable UI And Art Quality Pass v0.12.0

M4-2 turns the playable UI into a clearer game shell with Auth / Gate Entry, Character Hall, selected character preview, and World HUD panels. M4-3 upgrades the original placeholder SVGs in place while preserving their paths and `.meta` files.

## M5 First Playable Loop Foundation v0.15.0

M5 v0.15.0 adds a first local-only playable loop foundation to the existing world shell:

- enter world through the existing account/character flow;
- see player, Gate Keeper, Training Stone, and non-combat Shadow Slime markers;
- move near an interactable;
- press F or Space to receive objective and interaction feedback;
- keep Save Position and Back to Lobby available.

Runtime marker:

```text
M5_FIRST_PLAYABLE_LOOP_RUNTIME_SMOKE_PASS
```

This does not implement combat, damage, loot, inventory, economy, guild, chat, market, party, live ops, production auth, database persistence, protocol changes, GameData schema changes, or final production art.

## M5 Visual Evidence UX Acceptance v0.16.0

Visual evidence review:

```bash
./tools/lgo_playable_closure_check.sh --visual-evidence
./tools/run_m5_visual_evidence_review.sh --rebuild
```

Expected output:

```text
build/visual-evidence/gate-entry.png
build/visual-evidence/character-hall.png
build/visual-evidence/world-hud.png
build/visual-evidence/first-playable-loop-feedback.png
build/visual-evidence/visual-evidence-summary.json
build/visual-evidence/visual-evidence-summary.txt
```

Visual evidence does not equal human visual acceptance by itself.

Validation:

```bash
python3.12 tools/validate_m4_2_playable_ui.py
python3.12 tools/validate_m4_visual_foundation.py
```

## M4 Playable Slice Stabilization v0.13.0

M4 v0.13.0 adds a one-command closure workflow and stabilization validator for the current playable slice. It also bundles checksum-pinned macOS `protoc` `3.13.0` so inherited source validation can run on this host without changing protocol files.

Source/package gates:

```bash
./tools/lgo_m4_closure_check.sh --source-only
./tools/lgo_m4_closure_check.sh --package-ready
```

Runtime gate, only when Unity `6000.3.2f1` is configured:

```bash
./tools/lgo_m4_closure_check.sh --runtime
```

## M4 Visible UI Usability v0.14.0

M4 v0.14.0 targets the visible macOS player usability gap found during manual review. The playable shell keeps the existing account/character/world behavior but uses a narrower state-focused layout, clearer API/status text, visible Save/Back actions, and a Quit/Escape exit affordance.

Manual visible review harness:

```bash
./tools/run_m4_visible_ui_review.sh --rebuild
./tools/run_m4_visible_ui_review.sh --open-existing
./tools/run_m4_visible_ui_review.sh --stop
```

Source validation includes:

```bash
python3.12 tools/validate_m4_visible_ui.py
./tools/lgo_m4_closure_check.sh --source-only
```
