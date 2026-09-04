# START HERE — Linh Giới Online Current Source

**Current milestone:** `M6 combat foundation v0.55.0`.

**Current status:** `M6_COMBAT_FOUNDATION_CLOSED_LOCAL_v0.55.0`.

**Historical governance baseline:** `M6_COMBAT_READINESS_SPEC_CLOSED_v0.32.0`.

**Accepted visual reference:** `LGO_VISUAL_REFERENCE_PACK_ACCEPTED_v0.16.5`.

**Previous visual evidence status:** `M5_VISUAL_EVIDENCE_UX_REVIEW_READY`.

**Accepted base:** `M1_OFFLINE_COMBAT_RUNTIME_CLOSED` from `linh-gioi-m1-offline-combat-runtime-closed-v0.5.3-full-source.zip`.

**Current source successor:** `linh-gioi-governance-roadmap-queue-v1.0`.

**Historical playable marker:** `M5_GUIDED_TRAINING_LOOP_SOURCE_READY`.

## 1. Read order

1. `README.md`
2. `docs/execution/CODE-GOVERNANCE-CONTRACT.md`
3. `docs/execution/CODE-OWNERSHIP-MAP.md`
4. `docs/execution/CODE-QUALITY-GATES.md`
5. `docs/execution/LOCAL-SANDBOX-COMMAND-DISCIPLINE.md`
6. `docs/execution/PROJECT-STATE.md`
7. `docs/execution/MILESTONE-ROADMAP.md`
8. `docs/execution/07-PHASE-GATES.md`
9. `docs/execution/03-HANDOFF-CONTRACT.md`

## 2. Environment

Authoritative targets:

- Unity `6000.3.2f1`
- URP `17.3.0`
- Java `25`
- Maven `3.9.16`
- Protobuf compiler/runtime `3.13.0`
- GameData compiled manifest version `1`

## 3. Source validation

Recommended local macOS setup:

```bash
./tools/local_macos_setup.sh
```

Direct validation on a prepared environment:

```bash
./tools/lgo_m4_closure_check.sh --source-only
```

Expected classification:

```text
PROJECT STATE VALIDATION PASS
M4 SOURCE VALIDATION PASS
LGO_M4_CLOSURE_SOURCE_GATES_PASS
```

## 4. Unity preparation

Before opening Unity manually on a fresh checkout:

```bash
./tools/local_macos_setup.sh
./tools/prepare_unity_local_assets.sh
```

Then open `client/Unity` with Unity `6000.3.2f1`. Generated Unity assets belong under `client/Unity/Assets/Game/Generated/**` and must not be packaged into source deltas.

## 4A. One-command M2 runtime candidate

When ready to produce the final local evidence batch for sandbox replay:

```bash
./tools/run_m3_api_persistence_once.sh
```

Wait for `M2_LOCAL_RUNTIME_CANDIDATE_READY`, then upload the files listed in `build/m2-local-runtime-candidate/UPLOAD-THESE-FILES-M2-RUNTIME-CANDIDATE.txt`.

## 5. M2 runtime evidence

M2 runtime is not closed until Unity and Java are exercised together. Runtime closure must run a real Java realtime server and a Unity-built Linux player with:

```text
--lgo-m2-online-session-smoke
```

The smoke must prove handshake accepted, one movement intent sent, `PlayerTransformSnapshot` received, acknowledged sequence `1`, entity `1001`, and position `x≈0.4`.

## 6. Current M4-0 validation

M4-0 is the active playable client vertical slice foundation. It is source-ready until the macOS Unity player build and runtime smokes are rerun on a host with the pinned protobuf toolchain and Unity editor configured.

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


M2 runtime evidence remains pending local Unity execution; this M3 source work is an owner override, not an M2 runtime closure.


## M3-B Unity Account / Character Integration v0.8.0

Use this command for source validation:

```bash
./tools/validate_m3b_source.sh
```

Use this command for runtime closure only after building or providing a current Linux player from this exact source:

```bash
./tools/run_m3b_unity_account_character_once.sh --unity-player "$PWD/build/unity-player-macos/LinhGioiOnline.app/Contents/MacOS/Unity"
```

The Unity smoke path is:

```text
--lgo-m3b-account-character-smoke
```

It must login, list/create/load character, save/load position, restart API, and rerun with `--lgo-m3b-expect-existing`.

M3-B hotfix v0.8.1: Unity local compile support adds UnityWebRequest module dependency and generated protocol asmdef reference to Google.Protobuf.dll. No frozen protocol/schema changes.


## Local sandbox command discipline v0.8.5

Before giving or running local commands, read `docs/execution/LOCAL-SANDBOX-COMMAND-DISCIPLINE.md`. Commands are expected to be issued from repo root `LinhGioiOnline`, without placeholder executable paths, without `...`, and without `|| true` on gates.

For macOS Unity player smoke, the executable path is:

```text
build/unity-player-macos/LinhGioiOnline.app/Contents/MacOS/Unity
```

For Unity protocol compile/build gates, generate C# protocol files into:

```text
client/Unity/Assets/Game/Protocol/Generated
```

M3-B is locally closed only when `M3B_UNITY_ACCOUNT_CHARACTER_RUNTIME_SMOKE_PASS` is observed on the current source. This does not claim all M0 runtime closed and does not open M4.

## M4-0 Playable Client Vertical Slice v0.9.0

Use this command for M4 source validation after local toolchain setup:

```bash
./tools/validate_m4_source.sh
```

Use this command for M4 runtime verification after building a current macOS player:

```bash
./tools/run_m4_playable_vertical_slice_once.sh --unity-player "$PWD/build/unity-player-macos/LinhGioiOnline.app/Contents/MacOS/Unity"
```

The M4 smoke marker is:

```text
M4_PLAYABLE_VERTICAL_SLICE_RUNTIME_SMOKE_PASS
```

## M4-1 Visual Placeholder Foundation v0.10.0

Read:

- `docs/art/LGO-VISUAL-IDENTITY-GUIDE-v0.10.0.md`
- `docs/art/LGO-RUNTIME-ART-MANIFEST-v0.10.0.md`
- `client/Unity/Assets/Game/Art/README.md`

Validate:

```bash
python3.12 tools/validate_m4_visual_foundation.py
```

Runtime smoke after building a current macOS player:

```bash
./tools/run_m4_visual_foundation_once.sh --unity-player "$PWD/build/unity-player-macos/LinhGioiOnline.app/Contents/MacOS/Unity"
```

## M4-2/M4-3 Playable UI And Art Quality Pass v0.12.0

M4-2 upgrades the playable UI shell from the accepted v0.11.0 design lock. M4-3 improves the committed placeholder SVGs in place while keeping them source-controlled placeholders, not final production art.

Validate:

```bash
python3.12 tools/validate_m4_2_playable_ui.py
python3.12 tools/validate_m4_visual_foundation.py
```

## M4 Playable Slice Stabilization v0.13.0

Use one command for current M4 source gates:

```bash
./tools/lgo_m4_closure_check.sh --source-only
```

Use this before packaging source handoff artifacts:

```bash
./tools/lgo_m4_closure_check.sh --package-ready
```

Runtime closure still requires Unity `6000.3.2f1`, Java `25`, Maven `3.9.16`, and observed M3-B/M4 smoke markers on the current source.

## M4 Visible UI Usability v0.14.0

Visible UI review uses:

```bash
./tools/run_m4_visible_ui_review.sh --rebuild
```

## M5 First Playable Loop Foundation v0.15.0

M5 v0.15.0 keeps the existing account, character, world entry, movement, save position, and back-to-lobby semantics, then adds a local-only first interaction loop: approach the Gate Keeper or Training Stone, press F or Space, and receive objective/interaction feedback.

Historical status: `M5_FIRST_PLAYABLE_LOOP_SOURCE_READY`.

Validate source/package readiness:

```bash
./tools/lgo_playable_closure_check.sh --source-only
./tools/lgo_playable_closure_check.sh --package-ready
```

Runtime smoke after building a current macOS player:

```bash
./tools/lgo_playable_closure_check.sh --runtime
```

## M5 Visual Evidence UX Acceptance v0.16.0

Generate review artifacts:

```bash
./tools/lgo_playable_closure_check.sh --visual-evidence
```

Direct review command:

```bash
./tools/run_m5_visual_evidence_review.sh --rebuild
```

Manual visible review command:

```bash
./tools/run_m4_visible_ui_review.sh --rebuild
```

Open an existing built player:

```bash
./tools/run_m4_visible_ui_review.sh --open-existing
```

Stop the local review API/player:

```bash
./tools/run_m4_visible_ui_review.sh --stop
```
