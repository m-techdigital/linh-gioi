# LGO Register + Password Recovery 3-step Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Ship real product registration plus Request → Verify Code → New Password recovery, preserving owner-approved v3 visuals and reusing Character Hub runtime chrome.

**Architecture:** Keep product credential/session/recovery state separate from character persistence, but migrate player persistence to neutral account schema v3 so product accounts never fabricate dev keys. Server services own registration/recovery security; Unity owns transient flow state and calls typed HTTP APIs. UI partials bind state only and reuse existing Hub/Entry skin helpers.

**Tech Stack:** Java 25, Spring Boot, Jackson, BCrypt; Unity 6000.3.2f1, C# UI Toolkit, UnityWebRequest; Python capture/validation tooling.

**Spec:** `docs/superpowers/specs/2026-09-17-lgo-register-password-recovery-design.md`

## Global Constraints

- Proposal v3 is the approved visual authority; do not generate or change art direction.
- Preserve Entry v5→v9 scene/background/frame/layout and reuse shipping Character Hub shell/panel/action/input helpers where compatible.
- Product identifier is normalized email; password length is 8–128; registration requires accepted terms.
- Recovery code lifetime 10 min, max 5 attempts, resend cooldown 60 s; reset token lifetime 10 min and one-time use.
- Never persist/log/capture raw password, code, bearer token or reset token.
- Successful registration/reset does not auto-login; reset invalidates all active sessions for the account.
- PC 1600×900, tablet 1024×768, mobile landscape 1600×720 keep one composition.

---

### Task 1: Neutral player account persistence v3

**Files:**
- Modify: `server/api/src/main/java/com/linhgioi/server/api/persistence/AccountProfile.java`
- Modify: `server/api/src/main/java/com/linhgioi/server/api/persistence/PlayerProfileStore.java`
- Modify: `server/api/src/main/java/com/linhgioi/server/api/persistence/PlayerPersistenceSnapshot.java`
- Modify: `server/api/src/main/java/com/linhgioi/server/api/persistence/JsonFilePlayerProfileStore.java`
- Test: `server/api/src/test/java/com/linhgioi/server/api/persistence/JsonFilePlayerProfileStoreTest.java`

**Interfaces:**
- Produces `AccountProfile createProductAccount(String displayName)`, `boolean deleteEmptyProductAccount(String accountId)`, existing `loginDev` compatibility and `players-v3.json` migration.

- [ ] Write failing tests proving v2→v3 preserves original v2 bytes, dev login/index compatibility, product account has no dev-key index, and rollback deletes only empty product accounts.
- [ ] Run focused `JsonFilePlayerProfileStoreTest`; verify RED is caused by missing v3 API/schema.
- [ ] Implement neutral `AccountProfile`, schema v3 migration and product-account create/delete APIs with atomic-file persistence.
- [ ] Re-run focused persistence test and existing account controller tests; require GREEN.
- [ ] Commit `feat(auth): add neutral product account persistence v3`.

### Task 2: Product registration service and HTTP endpoint

**Files:**
- Modify: `server/api/src/main/java/com/linhgioi/server/api/auth/ProductCredentialStore.java`
- Modify: `server/api/src/main/java/com/linhgioi/server/api/auth/JsonFileProductCredentialStore.java`
- Create: `server/api/src/main/java/com/linhgioi/server/api/auth/ProductRegistrationService.java`
- Create: `server/api/src/main/java/com/linhgioi/server/api/auth/ProductRegisterRequest.java`
- Create: `server/api/src/main/java/com/linhgioi/server/api/auth/ProductRegisterResponse.java`
- Modify: `server/api/src/main/java/com/linhgioi/server/api/auth/ProductAuthController.java`
- Modify: `server/api/src/main/java/com/linhgioi/server/api/auth/ProductAuthConfiguration.java`
- Test: `server/api/src/test/java/com/linhgioi/server/api/auth/ProductRegistrationServiceTest.java`
- Test: `server/api/src/test/java/com/linhgioi/server/api/auth/ProductAuthControllerTest.java`

**Interfaces:**
- Produces `POST /auth/register`, email normalization/validation, BCrypt-12 credential creation, synchronized rollback coordinator and generic duplicate `409`.

- [ ] Write service/controller RED tests for normalization, password/terms validation, duplicate email, rollback after credential failure and `201` response without secrets.
- [ ] Run focused registration tests and confirm expected RED.
- [ ] Implement minimal registration service, request/response records, credential deletion needed only for rollback, controller mapping and Spring beans.
- [ ] Re-run focused tests plus existing Product Auth tests; require GREEN.
- [ ] Commit `feat(auth): add product registration endpoint`.

### Task 3: Password recovery security lifecycle

**Files:**
- Create: `server/api/src/main/java/com/linhgioi/server/api/auth/RecoveryDelivery.java`
- Create: `server/api/src/main/java/com/linhgioi/server/api/auth/PasswordRecoveryService.java`
- Modify: `server/api/src/main/java/com/linhgioi/server/api/auth/ProductCredentialStore.java`
- Modify: `server/api/src/main/java/com/linhgioi/server/api/auth/JsonFileProductCredentialStore.java`
- Modify: `server/api/src/main/java/com/linhgioi/server/api/auth/AuthSessionRegistry.java`
- Test: `server/api/src/test/java/com/linhgioi/server/api/auth/PasswordRecoveryServiceTest.java`

**Interfaces:**
- Produces `request(email)`, `verify(challengeId, code)`, `reset(resetToken, newPassword)`; credential password-hash replacement; `invalidateAccount(accountId)`.

- [ ] Write RED tests for existing/unknown enumeration parity, BCrypt code storage behavior, 10-minute expiry, 5 attempts, 60-second resend cooldown, challenge replacement, one-time reset token and account-wide session invalidation.
- [ ] Run focused recovery service tests and confirm failures are missing behavior.
- [ ] Implement in-memory challenge/reset registries using `SecureRandom`, BCrypt code hash and SHA-256 reset-token hash; add credential replacement/session invalidation APIs.
- [ ] Re-run focused recovery/auth tests; require GREEN.
- [ ] Commit `feat(auth): add secure password recovery lifecycle`.

### Task 4: Recovery HTTP contract and delivery boundary

**Files:**
- Create: `server/api/src/main/java/com/linhgioi/server/api/auth/RecoveryRequest.java`
- Create: `server/api/src/main/java/com/linhgioi/server/api/auth/RecoveryRequestResponse.java`
- Create: `server/api/src/main/java/com/linhgioi/server/api/auth/RecoveryVerifyRequest.java`
- Create: `server/api/src/main/java/com/linhgioi/server/api/auth/RecoveryVerifyResponse.java`
- Create: `server/api/src/main/java/com/linhgioi/server/api/auth/RecoveryResetRequest.java`
- Modify: `server/api/src/main/java/com/linhgioi/server/api/auth/ProductAuthController.java`
- Modify: `server/api/src/main/java/com/linhgioi/server/api/auth/ProductAuthConfiguration.java`
- Test: `server/api/src/test/java/com/linhgioi/server/api/auth/ProductAuthControllerTest.java`

**Interfaces:**
- Produces `POST /auth/recovery/request` 202, `/verify` 200 and `/reset` 204 with generic 400/401/429/503 semantics and no code echo.

- [ ] Add controller RED tests for success, enumeration-safe request, cooldown 429, invalid/expired generic 401, and no secret material in error bodies.
- [ ] Run focused controller test and confirm RED.
- [ ] Implement controller DTOs/status translation and configurable `RecoveryDelivery` bean; tests inject deterministic in-memory delivery.
- [ ] Run full `server/api` tests then reactor server suite; require GREEN.
- [ ] Commit `feat(auth): expose registration recovery http contract`.

### Task 5: Unity product-account client and transient recovery state

**Files:**
- Create: `client/Unity/Assets/Game/Account/Runtime/IProductAccountClient.cs`
- Create: `client/Unity/Assets/Game/Account/Runtime/ProductAccountModels.cs`
- Create: `client/Unity/Assets/Game/Account/Runtime/ProductAccountRecoveryState.cs`
- Modify: `client/Unity/Assets/Game/Account/Runtime/AccountApiClient.cs`
- Test: `client/Unity/Assets/Game/Tests/EditMode/AccountApiClientTests.cs`
- Test: `client/Unity/Assets/Game/Tests/EditMode/TwoDCharacterRuntimeStateTests.cs`

**Interfaces:**
- Produces async register/request/verify/reset methods and memory-only state with `Request`, `Verify`, `NewPassword` stages plus clear-on-cancel/success.

- [ ] Add RED tests for request JSON/endpoints, HTTP status classification, cooldown timestamps, stage transitions and secret clearing.
- [ ] Run focused Unity EditMode tests and confirm RED.
- [ ] Implement typed models/interface/client calls and cohesive transient state; do not write tokens/passwords to PlayerPrefs.
- [ ] Re-run focused tests; require GREEN.
- [ ] Commit `feat(client): add product account recovery client state`.

### Task 6: Register + Recovery 3-step UI using approved v3 and Hub runtime chrome

**Files:**
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Register.cs`
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.PasswordRecovery.cs`
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Entry.cs`
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Skin.cs`
- Test: `client/Unity/Assets/Game/Tests/EditMode/TwoDCharacterRuntimeStateTests.cs`

**Interfaces:**
- Register calls `IProductAccountClient.RegisterAsync`; Recovery owns one overlay tree whose stage binding swaps Request/Verify/New Password content without changing the Entry scene.
- Reuse `character-hub-surface`, `character-hub-panel-surface`, `character-hub-action-gold`, existing input/lock/eye helpers and Hub open-motion timing; no new procedural art direction.

- [ ] Add RED UI tests for email/password/terms validation, loading-disable state, Request→Verify→NewPassword transitions, resend cooldown, generic invalid/expired error, password mismatch/rules, success return to Login and transient-secret clearing.
- [ ] Run focused UI tests and confirm RED.
- [ ] Implement async Register submission and all three recovery stages with approved v3 copy/hierarchy; bind Hub shared chrome through auth-specific wrapper helpers only.
- [ ] Re-run focused UI tests plus full Unity EditMode; require GREEN.
- [ ] Commit `feat(ui): ship register and three-step recovery flow`.

### Task 7: Player evidence, security gates and closure

**Files:**
- Modify/Create capture tooling only as needed under `tools/` without changing visual authority.
- Update: `docs/execution/PROJECT-STATE.md`
- Update: `docs/execution/NEXT-ACTION.md`
- Evidence: `build/register-recovery-product-flow-v1/`

**Interfaces:**
- Produces exact-source BEFORE/AFTER evidence for Register, Request, Verify and New Password across PC/tablet/mobile and closure logs/manifests.

- [ ] Capture BEFORE from current authoritative runtime evidence and AFTER from one Player build at exact source HEAD; include validation/loading/error/resend/expiry/password-rule states without raw secrets.
- [ ] Run full server suite, full Unity EditMode, shared UI/skin, no-source-image, no-3D, package hygiene, frozen protocol/GameData/ADR/design-token diff, `git diff --check` and scoped secret scan.
- [ ] Visually audit all changed screens on PC 1600×900, tablet 1024×768 and mobile 1600×720; mismatch is FIX_REQUIRED, not closure.
- [ ] Update PROJECT-STATE/NEXT-ACTION with exact evidence and remaining product-visible work.
- [ ] Verify remote ancestry, commit closure, push `origin/feature/2d`, register/link evidence and checkpoint with `v3.finish`.
