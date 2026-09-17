# Product Character Select Account Binding Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Bind canonical Character Select to the authenticated product account's persisted three-slot list and selected character record through bearer-scoped APIs.

**Architecture:** Add product-only read endpoints that derive account scope from the existing bearer session while preserving legacy M3 routes. Unity adds a separate product-character client contract, then Character Select binds list/load state without changing canonical layout or applying unverified legacy class/position data to Map01A.

**Tech Stack:** Java 25, Spring Boot, Unity 6000.3.2f1, C# UI Toolkit, existing `AccountApiClient` / `ProductAuthService`.

**Spec:** `docs/superpowers/specs/2026-09-17-product-character-select-account-binding-design.md`

## Global Constraints

- Preserve Entry v5→v9 and current Character Select visual composition; no redesign.
- Product character reads must be bearer-scoped and must not accept a product account ID from the client.
- Keep legacy M3/dev character routes intact for existing smoke coverage.
- Do not apply `class.sword/class.martial` or legacy XYZ/yaw into the five-class Map01A runtime in this slice.
- No access token or character identity is persisted to PlayerPrefs.
- Evidence profiles remain PC `1600×900`, tablet `1024×768`, mobile landscape `1600×720`.

---
### Task 1: Bearer-scoped product character read API

**Files:**
- Create: `server/api/src/main/java/com/linhgioi/server/api/account/ProductCharacterController.java`
- Test: `server/api/src/test/java/com/linhgioi/server/api/account/ProductCharacterControllerTest.java`

**Interfaces:**
- Consumes `ProductAuthService.validateSession(String)` and `PlayerProfileStore.listCharacters/findCharacter`.
- Produces `GET /auth/characters` and `GET /auth/characters/{characterId}`.

- [ ] Write RED tests proving own-account list works, missing/invalid bearer returns `401`, own character loads, and unknown/cross-account IDs return the same generic `404 character not found`.
- [ ] Run `cd server/api && mvn -q -Dtest=ProductCharacterControllerTest test`; verify RED is missing controller/product routes.
- [ ] Implement minimal bearer parsing, session validation, account-derived list, ownership-checked load and generic error mapping. Do not modify legacy `AccountCharacterController` behavior.
- [ ] Re-run `ProductCharacterControllerTest` plus `ProductAuthControllerTest` and `AccountCharacterControllerTest`; require GREEN.
- [ ] Commit `feat(auth): scope product character reads to bearer session`.

### Task 2: Unity bearer product-character client

**Files:**
- Create/replace WIP: `client/Unity/Assets/Game/Account/Runtime/IProductCharacterClient.cs`
- Modify: `client/Unity/Assets/Game/Account/Runtime/AccountApiClient.cs`
- Modify: `client/Unity/Assets/Game/Tests/EditMode/CharacterSelectAccountBindingTests.cs`
- Test: `client/Unity/Assets/Game/Tests/EditMode/ProductAuthClientTests.cs`
**Interfaces:**
- Produces `IProductCharacterClient.ListProductCharactersAsync(string accessToken, CancellationToken)` and `LoadProductCharacterAsync(string accessToken, string characterId, CancellationToken)`.
- Existing legacy account/character methods on `AccountApiClient` remain unchanged.

- [ ] Update the current RED UI test fake to target `IProductCharacterClient` and bearer token calls; add a focused client contract test for route names and interface implementation.
- [ ] Run focused Unity tests; verify RED is missing the new interface/methods.
- [ ] Implement `IProductCharacterClient` and add bearer calls to `/auth/characters` and `/auth/characters/{characterId}` using the existing `SendJsonRawAsync` bearer parameter.
- [ ] Re-run `ProductAuthClientTests` and `CharacterSelectAccountBindingTests`; require client contract GREEN before UI binding work.
- [ ] Commit `feat(client): add bearer-scoped product character reads`.

### Task 3: Bind canonical Character Select to three authenticated slots

**Files:**
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.cs`
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.CharacterSelect.cs`
- Modify: `client/Unity/Assets/Game/Tests/EditMode/CharacterSelectAccountBindingTests.cs`
- Modify: `client/Unity/Assets/Game/Tests/EditMode/TwoDCharacterRuntimeStateTests.cs`
- Modify: `tools/validate_lgo_ui_shared_skin.py`

**Interfaces:**
- Consumes `ProductAuthSessionState.AccessToken/Account` and `IProductCharacterClient`.
- Produces three stable slot controls, list/loading/error state, selected `CharacterResponse`, load-before-close Enter Game behavior and loaded display name.
- [ ] Extend RED UI tests to require list call on open, explicit slots `1..3`, first-occupied auto-selection, selection isolation, load-before-close, load failure, and no fake LụcThiên in normal authenticated runtime.
- [ ] Run `CharacterSelectAccountBindingTests`; confirm RED is missing list/load UI behavior.
- [ ] Implement shared product-character client wiring in `CongDongLamArrivalHud`, three-slot binding and capture-only review seed in `CharacterSelect.cs`.
- [ ] Keep the Create/Edit/Delete design gates unchanged; only the existing slot data and Enter Game path become real.
- [ ] Update the legacy canonical Character Select test to use an authenticated fake account and update shared-UI validator structural markers to the new stable slot/list/load contract.
- [ ] Run `CharacterSelectAccountBindingTests`, the existing Character Select regression, shared-UI validator tests and then full Unity EditMode with graphic device; require GREEN.
- [ ] Commit `feat(ui): bind character select to product account records`.

### Task 4: Player BEFORE/AFTER, source gates and closure

**Files:**
- Modify capture tooling only if the current Character Select capture cannot seed deterministic three-slot account data without network.
- Update: `docs/execution/PROJECT-STATE.md`
- Update: `docs/execution/NEXT-ACTION.md`
- Evidence: `build/character-select-account-binding-v1/`

**Interfaces:**
- Produces exact-source Character Select evidence at PC/tablet/mobile and closure provenance.

- [ ] Preserve the existing BEFORE images from whole-flow P0; build one fresh macOS Player from the final runtime HEAD.
- [ ] Capture Character Select on all three profiles using internal capture state only; capture must not call external account services or use OS mouse/keyboard.
- [ ] Produce BEFORE/AFTER boards and visually review stage/name/slot/action readability plus the known mobile motto/stage debt.
- [ ] Run full server reactor, full Unity EditMode, relevant Python/shared-UI, no-3D, no-source-images, package hygiene, code governance, frozen protocol/GameData/ADR/design-token diff, `git diff --check`, and scoped secret/token scan.
- [ ] Commit closure docs and normal-push `feature/2d` only when local HEAD is a fast-forward of remote and source is clean.
- [ ] Record the explicit next subproject: five-class character-schema + Map01A spawn/position migration; Create Character remains design-gated.
