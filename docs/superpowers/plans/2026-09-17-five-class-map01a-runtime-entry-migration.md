# Five-Class Character Schema + Map01A Runtime Entry Migration Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Migrate character persistence to a backward-compatible five-class/runtime-state contract and apply persisted Map01A lane/facing safely at product world entry.

**Architecture:** Keep raw legacy class/XYZ/yaw untouched, add a v4 runtime-state map and derived canonical runtime class. Product bearer APIs expose/save Map01A state; Unity maps the loaded record into an explicit entry state without using review-only renderer assets.

**Tech Stack:** Java 25, Spring Boot, Jackson, Unity 6000.3.2f1, C# UI Toolkit/runtime, existing product bearer auth.

**Spec:** `docs/superpowers/specs/2026-09-17-five-class-map01a-runtime-entry-migration-design.md`

## Global Constraints

- Preserve legacy `class.sword/class.martial` and raw M3/M4 XYZ/yaw meaning.
- Canonical runtime IDs are exactly `vo`, `kiem`, `phap`, `co`, `linh`.
- Map01A ID is exactly `map-01a-cong-dong-lam`; lane range `[-3.8, 44.4]`; fallback `-3.6/+1`.
- Product account ownership remains bearer-derived; cross-account and missing character stay indistinguishable.
- Do not use `TwoDSourcePoseReview` as product renderer; it remains `REVIEW_ONLY/runtimeEligible=false`.
- No Create Character UI/product create route, class-art redesign, GameData/protocol frozen changes or guessed autosave policy.
- Review/intermediate images stay under ignored `build/`; no PNG/JPG review artifacts are committed.

---
### Task 1: Persistence v4 + class compatibility

**Files:**
- Create: `server/api/src/main/java/com/linhgioi/server/api/persistence/CharacterClassCompatibility.java`
- Modify: `server/api/src/main/java/com/linhgioi/server/api/persistence/CharacterProfile.java`
- Create: `server/api/src/main/java/com/linhgioi/server/api/persistence/CharacterRuntimeState.java`
- Create: `server/api/src/main/java/com/linhgioi/server/api/persistence/SaveMap01AStateCommand.java`
- Modify: `server/api/src/main/java/com/linhgioi/server/api/persistence/PlayerPersistenceSnapshot.java`
- Modify: `server/api/src/main/java/com/linhgioi/server/api/persistence/PlayerProfileStore.java`
- Modify: `server/api/src/main/java/com/linhgioi/server/api/persistence/JsonFilePlayerProfileStore.java`
- Test: `server/api/src/test/java/com/linhgioi/server/api/persistence/JsonFilePlayerProfileStoreTest.java`

**Interfaces:**
- Produces `CharacterClassCompatibility.toRuntimeClassId(String)`.
- Produces `findRuntimeState(characterId)` and `saveMap01AState(SaveMap01AStateCommand)`.
- Persists schema v4 `runtimeStatesByCharacterId` without rewriting raw legacy character coordinates/class IDs.

- [ ] Add RED tests for five canonical IDs, legacy aliases, unknown rejection, v3→v4 byte preservation, empty runtime-state migration, Map01A state reload and invalid lane/facing rejection.
- [ ] Run `cd server/api && mvn -q -Dtest=JsonFilePlayerProfileStoreTest test`; require failures caused by missing v4/runtime-state APIs.
- [ ] Implement schema v4, compatibility helper, migration from v3/v2/v1 and atomic `players-v4.json` writes. Legacy source files remain untouched.
- [ ] Re-run focused persistence tests plus `AccountCharacterControllerTest`; require GREEN.
- [ ] Commit `feat(persistence): add five-class map runtime state v4`.

---
### Task 2: Bearer-scoped product runtime-state API

**Files:**
- Modify: `server/api/src/main/java/com/linhgioi/server/api/account/CharacterResponse.java`
- Create: `server/api/src/main/java/com/linhgioi/server/api/account/CharacterRuntimeStateResponse.java`
- Create: `server/api/src/main/java/com/linhgioi/server/api/account/SaveMap01AStateRequest.java`
- Modify: `server/api/src/main/java/com/linhgioi/server/api/account/ProductCharacterController.java`
- Test: `server/api/src/test/java/com/linhgioi/server/api/account/ProductCharacterControllerTest.java`
- Test: `server/api/src/test/java/com/linhgioi/server/api/account/AccountCharacterControllerTest.java`

**Interfaces:**
- Product reads add `runtimeClassId` and optional `runtimeState` while preserving legacy response fields.
- Adds `POST /auth/characters/{characterId}/map01a-state` with bearer-derived ownership and `{laneX,facing}` body.

- [ ] Add RED controller tests proving derived runtime class, null legacy runtime state, authenticated Map01A save/reload, `401` invalid session and generic `404` for unknown/cross-account IDs.
- [ ] Add a regression proving legacy `/characters/{id}/position` still updates only raw XYZ/yaw and leaves Map01A runtime state untouched.
- [ ] Run `cd server/api && mvn -q -Dtest=ProductCharacterControllerTest,AccountCharacterControllerTest test`; require RED for missing response/save contract.
- [ ] Implement response mapping and bearer-scoped Map01A save through `PlayerProfileStore`; do not change legacy route semantics.
- [ ] Re-run controller tests plus `ProductAuthControllerTest`; require GREEN.
- [ ] Commit `feat(auth): expose scoped map01a character runtime state`.

---
### Task 3: Unity product DTO/client + pure entry mapper

**Files:**
- Modify: `client/Unity/Assets/Game/Account/Runtime/AccountModels.cs`
- Modify: `client/Unity/Assets/Game/Account/Runtime/IProductCharacterClient.cs`
- Modify: `client/Unity/Assets/Game/Account/Runtime/AccountApiClient.cs`
- Create: `client/Unity/Assets/Game/World/Runtime/Map01ACharacterEntryMapper.cs`
- Test: `client/Unity/Assets/Game/Tests/EditMode/ProductAuthClientTests.cs`
- Create test: `client/Unity/Assets/Game/Tests/EditMode/Map01ACharacterEntryMapperTests.cs`

**Interfaces:**
- `CharacterResponse.runtimeClassId` and optional `CharacterRuntimeStateResponse runtimeState`.
- `IProductCharacterClient.SaveMap01AStateAsync(string accessToken, string characterId, float laneX, int facing, CancellationToken)`.
- `Map01ACharacterEntryMapper.Resolve(CharacterResponse)` returns canonical class, lane/facing and legacy-fallback flag.

- [ ] Write RED tests for DTO/route contract, all five classes, legacy alias fallback, valid Map01A state, missing/foreign-map fallback `-3.6/+1`, malformed state/class fail-closed, and proof raw XYZ/yaw do not influence Map01A entry.
- [ ] Run `bash tools/unity_batch_test.sh --filter LinhGioi.Tests.EditMode.Map01ACharacterEntryMapperTests` and `--filter LinhGioi.Tests.EditMode.ProductAuthClientTests`; verify RED for missing types/methods.
- [ ] Implement DTOs, bearer save call and pure mapper. Do not touch renderer/UI yet.
- [ ] Re-run both focused suites and legacy `LinhGioi.Tests.M3BAccountCharacterTests`; require GREEN/non-zero matched tests.
- [ ] Commit `feat(client): map product characters into map01a entry state`.

---
### Task 4: Apply persisted Map01A entry state without renderer impersonation

**Files:**
- Modify: `client/Unity/Assets/Game/World/Runtime/CongDongLamMap01AArtPreview.cs`
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.CharacterSelect.cs`
- Modify: `client/Unity/Assets/Game/Tests/EditMode/CharacterSelectAccountBindingTests.cs`
- Test: `client/Unity/Assets/Game/Tests/EditMode/TwoDCharacterRuntimeStateTests.cs`

**Interfaces:**
- `CongDongLamMap01AArtPreview.ApplyProductCharacterEntryState(Map01ACharacterEntryState)` applies lane/facing and stores selected product runtime class metadata.
- `ActiveEquipmentClassId` remains renderer-owned; non-Võ product class does not silently switch renderer authority.

- [ ] Extend RED tests so Enter Game applies mapper output before closing, valid lane/facing moves the Map01A actor/nearest route state, legacy records start at `-3.6/+1`, and five-class semantic IDs survive without mutating renderer authority.
- [ ] Run focused Character Select and Map01A runtime tests; verify RED is missing entry-application behavior.
- [ ] Implement product entry application after successful bearer load and before Character Select closes. Call existing `Refresh()` / pose application; do not load review-only source-pose art.
- [ ] Keep loaded product name behavior and generic load errors unchanged. Do not expose raw/technical class IDs in UI.
- [ ] Re-run focused binding/runtime/auth-success regressions and shared-UI validator tests; require GREEN.
- [ ] Commit `feat(world): apply authenticated map01a entry state`.

---
### Task 5: Exact-source verification + entry-path evidence + closure

**Files:**
- Modify capture tooling only if existing Map01A/Character Select capture cannot seed a deterministic loaded character runtime state without network.
- Update: `docs/execution/PROJECT-STATE.md`
- Update: `docs/execution/NEXT-ACTION.md`
- Evidence: ignored `build/five-class-map01a-runtime-entry-v1/`

**Interfaces:**
- Produces exact-source server/Unity/evidence closure and a written list of remaining gates.

- [ ] Run full server reactor and full Unity EditMode with graphic device. Require zero failures/skips; do not treat pre-existing CS0618 warnings as this slice's work.
- [ ] Run relevant Python/shared-UI, no-3D, no-source-images, package hygiene, code governance, frozen protocol/GameData/ADR/design-token diff and `git diff --check`.
- [ ] Build one macOS Player from exact runtime HEAD. Capture Character Select → Enter Game → Map01A entry at PC `1600×900`, tablet `1024×768`, mobile `1600×720` with deterministic internal state and no OS input/network.
- [ ] Keep all PNG/JPG/rejected/intermediate evidence under ignored `build/`; verify zero review images are added to Git.
- [ ] Record two explicit remaining gates: production five-class renderer authority for non-Võ classes, and an approved lifecycle/autosave trigger for calling `SaveMap01AStateAsync` during live play.
- [ ] Commit closure docs, fetch remote, verify fast-forward ancestry, push `feature/2d`, then verify local HEAD equals remote and worktree is clean.

---

## Execution

Use inline execution in this session via `superpowers:executing-plans`; do not open Work/Codex. Before every task batch, re-run Manager check and native process scan. Every production task follows RED → verified RED → minimal GREEN → fresh verification → commit.
