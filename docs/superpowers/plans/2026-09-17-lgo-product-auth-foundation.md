# LGO Product Auth Foundation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add real product login/session/logout behavior from Unity Entry to Java API without changing the locked Login layout or turning `/dev/auth/login` into production auth.

**Architecture:** Credentials live in a dedicated versioned JSON auth store; `ProductAuthService` owns BCrypt verification and an in-memory opaque-token session registry. Unity uses `IProductAuthClient` + `ProductAuthSessionState`, keeps token/password memory-only, and routes successful Login into the existing Character Select screen.

**Tech Stack:** Java 25, Spring Boot 4.1.1, Spring Security Crypto BCrypt, Jackson, NUnit/EditMode, Unity 6000.3.2f1 UI Toolkit.

**Spec:** `docs/superpowers/specs/2026-09-17-lgo-product-auth-foundation-design.md`

## Global Constraints

- Preserve canonical Entry/Login layout; no visual redesign in this slice.
- Never persist or log raw password, bearer token, or Authorization header.
- `/dev/auth/login` remains development-only and unchanged in behavior.
- Register, Recovery, persisted Character Select, refresh tokens and rate limiting are out of scope.
- Every visible Login-state change needs current-run before/after Player evidence at PC/tablet/mobile.
- Protocol/GameData/design-token frozen surfaces remain unchanged.

---
### Task 1: Credential persistence boundary

**Files:**
- Modify: `server/api/pom.xml`
- Modify: `server/api/src/main/java/com/linhgioi/server/api/persistence/PlayerProfileStore.java`
- Modify: `server/api/src/main/java/com/linhgioi/server/api/persistence/JsonFilePlayerProfileStore.java`
- Create: `server/api/src/main/java/com/linhgioi/server/api/auth/ProductCredential.java`
- Create: `server/api/src/main/java/com/linhgioi/server/api/auth/ProductCredentialStore.java`
- Create: `server/api/src/main/java/com/linhgioi/server/api/auth/JsonFileProductCredentialStore.java`
- Test: `server/api/src/test/java/com/linhgioi/server/api/auth/JsonFileProductCredentialStoreTest.java`

**Interfaces:**
- `PlayerProfileStore.findAccount(String accountId) -> Optional<AccountProfile>`.
- `ProductCredentialStore.create(String accountId, String normalizedIdentifier, String passwordHash, long nowUnixMs) -> ProductCredential`.
- `ProductCredentialStore.findByIdentifier(String normalizedIdentifier) -> Optional<ProductCredential>`.

- [ ] Write RED tests proving create/find persistence, duplicate identifier rejection, and no raw password fixture value in `auth-credentials-v1.json`.
- [ ] Run `tools/with_m0_server_toolchain.sh mvn -B -ntp -f server/pom.xml -pl api -am -Dtest=JsonFileProductCredentialStoreTest test` and verify RED because auth store types do not exist.
- [ ] Add `spring-security-crypto`, read-only account lookup, immutable credential record and atomic JSON store implementation.
- [ ] Run focused Maven test GREEN, then existing `AccountCharacterControllerTest` GREEN to prove M3 character persistence did not regress.
- [ ] Commit as `feat(auth): add credential persistence boundary`.
### Task 2: Product auth service and session lifecycle

**Files:**
- Create: `server/api/src/main/java/com/linhgioi/server/api/auth/AuthSessionRegistry.java`
- Create: `server/api/src/main/java/com/linhgioi/server/api/auth/ProductAuthService.java`
- Create: `server/api/src/main/java/com/linhgioi/server/api/auth/ProductAuthConfiguration.java`
- Test: `server/api/src/test/java/com/linhgioi/server/api/auth/ProductAuthServiceTest.java`

**Interfaces:**
- `ProductAuthService.provisionCredential(String accountId, String identifier, String rawPassword)`.
- `ProductAuthService.login(String identifier, String rawPassword) -> LoginResult`.
- `ProductAuthService.validateSession(String rawToken) -> SessionResult`.
- `ProductAuthService.logout(String rawToken)`.

- [ ] Write RED tests for BCrypt provisioning, correct/wrong/unknown login, 32-byte opaque token issue, validation before expiry, rejection/removal after expiry, and logout invalidation.
- [ ] Verify RED with focused Maven test because service/session registry do not exist.
- [ ] Implement identifier normalization, BCrypt strength 12, `SecureRandom` token issue, SHA-256 token lookup keys and 12-hour configuration-backed absolute expiry.
- [ ] Ensure test assertions inspect persisted auth JSON and confirm neither raw password nor raw token occurs in it.
- [ ] Run focused GREEN plus all `server/api` tests.
- [ ] Commit as `feat(auth): add product session lifecycle`.

### Task 3: Product auth HTTP contract

**Files:**
- Create: `server/api/src/main/java/com/linhgioi/server/api/auth/ProductLoginRequest.java`
- Create: `server/api/src/main/java/com/linhgioi/server/api/auth/ProductLoginResponse.java`
- Create: `server/api/src/main/java/com/linhgioi/server/api/auth/ProductSessionResponse.java`
- Create: `server/api/src/main/java/com/linhgioi/server/api/auth/ProductAuthController.java`
- Test: `server/api/src/test/java/com/linhgioi/server/api/auth/ProductAuthControllerTest.java`
**Interfaces:**
- `POST /auth/login` → 200 `{account,accessToken,expiresAtUnixMs}`.
- `GET /auth/session` + bearer → 200 `{account,expiresAtUnixMs}`.
- `POST /auth/logout` + bearer → 204.
- Invalid credentials/token/expiry → 401; malformed request → 400.

- [ ] Write controller RED tests that provision a fixture credential directly through `ProductAuthService`, then assert login/session/logout and identical 401 behavior for wrong password versus unknown identifier.
- [ ] Run focused Maven test and verify RED because controller/DTOs do not exist.
- [ ] Implement bearer parsing locally in the auth controller; never log or echo the header/token.
- [ ] Map service validation failures to generic HTTP 401 and request validation to HTTP 400.
- [ ] Run focused GREEN, `AccountCharacterControllerTest`, then `server/test.sh` if disk/runtime permits.
- [ ] Commit as `feat(auth): expose product auth endpoints`.

### Task 4: Unity product auth client/session state

**Files:**
- Create: `client/Unity/Assets/Game/Account/Runtime/IProductAuthClient.cs`
- Create: `client/Unity/Assets/Game/Account/Runtime/ProductAuthModels.cs`
- Create: `client/Unity/Assets/Game/Account/Runtime/ProductAuthSessionState.cs`
- Modify: `client/Unity/Assets/Game/Account/Runtime/AccountApiClient.cs`
- Test: `client/Unity/Assets/Game/Tests/EditMode/ProductAuthClientTests.cs`

**Interfaces:**
- `IProductAuthClient.LoginAsync(identifier,password,cancellationToken)`.
- `IProductAuthClient.ValidateSessionAsync(accessToken,cancellationToken)`.
- `IProductAuthClient.LogoutAsync(accessToken,cancellationToken)`.
- `ProductAuthSessionState.Set(ProductLoginResponse)`, `Clear()`, `IsAuthenticated`, `IsExpired(long nowUnixMs)`.
- [ ] Write RED tests for DTO parsing, session set/clear/expiry and API exception status handling; test that dev-login methods remain available and product bearer methods use separate paths.
- [ ] Run focused Unity EditMode test and verify RED because product auth client/session types do not exist.
- [ ] Implement product DTOs, `AccountApiException`, bearer-capable request helper and product auth methods without altering existing dev/character calls.
- [ ] Keep access token/password in memory only; do not add PlayerPrefs or file persistence.
- [ ] Run focused GREEN plus existing `M3BAccountCharacterTests`.
- [ ] Commit as `feat(auth): add unity product auth client`.

### Task 5: Wire canonical Login to product auth

**Files:**
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.cs`
- Modify: `client/Unity/Assets/Game/UI/Runtime/CongDongLamArrivalHud.Entry.cs`
- Test: `client/Unity/Assets/Game/Tests/EditMode/TwoDCharacterRuntimeStateTests.cs`

**Interfaces:**
- `CongDongLamArrivalHud.Attach(scene, IProductAuthClient productAuthClient = null, ProductAuthSessionState authSession = null)` preserves existing one-argument call sites.
- Existing `Map01A Entry Login Button`, fields and status line remain canonical.

- [ ] Write RED UI tests with a deterministic fake client: missing fields stay local; first submit enters authenticating state and disables duplicate submit; 401 shows generic invalid-credential copy; success stores session, clears password and opens Character Select.
- [ ] Run focused EditMode filters and verify RED before modifying Entry runtime behavior.
- [ ] Add lazy default `AccountApiClient` creation from `ClientRuntimeConfig`, cancellation tied to HUD destroy, and one in-flight login guard.
- [ ] Preserve remember-account username-only behavior and exact Entry hierarchy/style classes; do not add/remove layout elements.
- [ ] Run focused GREEN and full `TwoDCharacterRuntimeStateTests`.
- [ ] Commit as `feat(auth): connect entry login to product auth`.
### Task 6: Runtime evidence, security audit and closure

**Files/Evidence:**
- Generate: `build/product-auth-foundation-v1/` logs, Player and task evidence.
- Update: `docs/execution/PROJECT-STATE.md`
- Update: `docs/execution/NEXT-ACTION.md`

- [ ] Run full server tests and full Unity EditMode; report exact pass/fail/skip counts.
- [ ] Run shared-skin, no-source-image, no-3D, package hygiene, frozen-surface and change-budget checks; scan generated logs/evidence for raw fixture password/token/Authorization values.
- [ ] Build one fresh macOS Player. Preserve the first failed build log if environment/disk fails; do not overwrite evidence history.
- [ ] Capture Entry/Login after states at PC/tablet/mobile using the canonical layout. Save before/after pairs under `build/product-auth-foundation-v1/task-evidence/login/`.
- [ ] Directly inspect all before/after Login screenshots. If layout remains canonical, continue; if a redesign is required, produce old-vs-proposed-new screenshots and stop visual replacement until owner approval.
- [ ] Record any server/client limitation honestly. Product Login success must be proven by deterministic server/controller + Unity fake-client routing tests even if a live API fixture is not yet available; do not claim network E2E that was not run.
- [ ] Update project state and next action to Register + three-stage Password Recovery after Auth Foundation closes.
- [ ] Verify remote is fast-forward safe, push commits, register immutable evidence/checkpoint when MCP Session Manager API is available, and release only this session's claims at batch boundary.

## Expected closure markers

- `LGO_PRODUCT_AUTH_SERVER_PASS`
- `LGO_PRODUCT_AUTH_UNITY_PASS`
- `LGO_PRODUCT_AUTH_SECURITY_SCAN_PASS`
- `LGO_PRODUCT_AUTH_ENTRY_VISUAL_PASS`

After this plan closes, do not expand directly into persisted Character Select. The next approved subproject is Register + Password Recovery, followed by Character persistence flow as recorded in `NEXT-ACTION.md`.
