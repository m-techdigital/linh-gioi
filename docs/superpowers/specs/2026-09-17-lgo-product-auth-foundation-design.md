# LGO Product Auth Foundation — Design

Date: 2026-09-17
Status: OWNER_CONTINUATION_APPROVED

## Purpose

Replace the presentation-only Login callback with a real product authentication boundary while preserving the canonical Entry/Login visual design.

This subproject establishes credential verification, short-lived authenticated session state, token validation, logout/expiry handling, and a Unity client contract that later Character persistence, Register, Recovery and social systems can reuse.

## Visual design decision

The current Entry/Login screen remains canonical. P0 Player evidence shows the existing hierarchy is usable at PC 1600×900, tablet 1024×768 and mobile landscape 1600×720.

No visual redesign is required for this auth slice. Only runtime states may change inside the existing reserved status line and existing Login CTA: default, validating, authenticating, invalid credentials, network/server error and success transition.

If implementation reveals a visual defect that cannot be fixed within the locked design, stop replacement work and prepare old-vs-new screenshots for owner approval before changing canonical layout.
## Considered architectures

1. Extend `PlayerProfileStore` with credentials and sessions. Smallest file count, but it mixes character persistence with security lifecycle and makes future DB migration harder.
2. Keep one shared persistence file but introduce an auth service facade. Better API ownership, but sensitive credential records still share migration/versioning with player data.
3. **Chosen:** separate credential persistence plus an in-memory session registry, orchestrated by `ProductAuthService`. This isolates security state, leaves M3 character persistence stable, and gives Register a reusable internal provisioning API later.

The chosen design deliberately does not persist active session tokens. API restart invalidates sessions, which is acceptable for the current single-node foundation and safer than writing bearer material to disk.

## Server components

- `ProductCredentialStore`: create/find credential records by normalized login identifier; no HTTP concerns.
- `JsonFileProductCredentialStore`: local persistence in a dedicated versioned auth JSON file, separate from `players-v2.json`.
- `ProductAuthService`: password verification, account lookup, session issue/validate/logout and expiry.
- `AuthSessionRegistry`: thread-safe in-memory session registry keyed by SHA-256 of opaque bearer tokens; raw token is never retained server-side after issuance.
- `ProductAuthController`: product HTTP contract only. `/dev/auth/login` remains untouched and development-only.

`PlayerProfileStore` gains only a read-only `findAccount(accountId)` method so auth can resolve authoritative account display data without duplicating it in the credential store.
## Credential contract

A credential record contains: `accountId`, normalized identifier lookup key, BCrypt password hash, created timestamp and updated timestamp. The credential store must never persist raw passwords.

Identifier normalization for this foundation is intentionally generic: trim, lowercase with `Locale.ROOT`, length 3–254. Register will later decide whether a new identifier is an account name, email or phone and will add contact/recovery metadata without changing Login's lookup contract.

Password input is accepted only as a transient request value. Foundation validation accepts 8–128 characters; Register may impose stricter UX policy later but must stay compatible with this server boundary.

Password hashing uses Spring Security Crypto `BCryptPasswordEncoder` with strength 12. No password, raw bearer token or Authorization header may be logged, serialized into PlayerPrefs, included in evidence manifests, or echoed in error text.

Internal `ProductAuthService.provisionCredential(accountId, identifier, rawPassword)` is allowed for deterministic tests and becomes the service API used by the later Register subproject. This slice exposes no product Register endpoint.

## Product HTTP API

- `POST /auth/login` body `{identifier,password}` → HTTP 200 `{account,accessToken,expiresAtUnixMs}`.
- `GET /auth/session` with `Authorization: Bearer <token>` → HTTP 200 `{account,expiresAtUnixMs}`.
- `POST /auth/logout` with bearer token → HTTP 204 and immediate token invalidation.
- Invalid credentials and missing/expired/invalid bearer tokens return HTTP 401 with no account-existence disclosure.
- Malformed requests return HTTP 400. Rate limiting is explicitly deferred to the later auth-hardening/recovery slice.
## Session lifecycle

Each successful login creates a cryptographically random 32-byte opaque token encoded base64url without padding. The server stores only its SHA-256 lookup hash plus `accountId`, creation time and expiry.

Default session TTL is 12 hours and is configuration-backed. Validation rejects expired sessions and removes them from the registry. Logout is idempotent for a valid token and clears it immediately.

This foundation uses absolute expiry only; refresh tokens, sliding expiry, multi-device session management and remote session revocation UI are future hardening work.

API restart invalidates all active sessions because the registry is intentionally in memory. The Unity client must treat resulting 401 responses as signed-out state, clear its local session, and return to Entry/Login without destroying remembered account text.

## Unity client contract

`AccountApiClient` gains product methods for login, session validation and logout while preserving the existing dev-login/character methods used by earlier milestone smoke tests.

The HTTP layer gains an `AccountApiException` carrying response status without requiring Player-facing code to parse raw server error bodies. Bearer requests use an Authorization header supplied only for the request lifetime.

`ProductAuthSessionState` owns the current account, bearer token and expiry in memory only. It provides `Set`, `Clear`, `IsAuthenticated` and `IsExpired(nowUnixMs)` behavior and never writes token/password data to disk or PlayerPrefs.
## Login UI behavior

The current Login button remains the only primary CTA. Missing fields keep current local validation. With both fields present:

1. Disable the Login CTA and show `Đang xác thực…` in the existing status line.
2. Call product `/auth/login` asynchronously with cancellation bound to the HUD lifetime.
3. On HTTP 200, set `ProductAuthSessionState`, clear the password field, keep remembered identifier policy unchanged, and open Character Select.
4. On HTTP 401, keep Entry open and show the generic `Tài khoản hoặc mật khẩu không đúng.` message.
5. On timeout/transport/server failure, keep Entry open and show a non-sensitive connection/service message; do not clear identifier/password automatically.
6. Re-enable the Login CTA after every non-success completion.

The login callback must never fall back to `/dev/auth/login` or create an account implicitly.

## Logout and unauthorized handling

The service/client logout contract is implemented in this foundation even though a new visible Logout button is not introduced into the locked Menu design. A later System Screens task may expose it after its own design review.

`ProductAuthSessionState.Clear()` is the single client-side sign-out action. Explicit logout calls the server first when possible, then clears locally. Any product API 401 tied to an authenticated request must also clear local session before returning the user to Entry.

Because Character persistence wiring is the next separate subproject, this slice only routes successful Login to the existing Character Select screen; it does not yet replace its hard-coded character list.
## Testing strategy

Server TDD must prove: credential provisioning hashes the password; login accepts correct credentials and rejects wrong/unknown credentials identically; raw password/token never appears in persisted auth JSON; issued token validates before expiry; expiry invalidates; logout invalidates; account lookup is authoritative; `/dev/auth/login` remains operational and separate.

Controller tests must cover HTTP-equivalent status behavior for login/session/logout without depending on a real network listener. Maven module tests must remain green.

Unity TDD must prove: product auth DTO parsing; session state set/clear/expiry; bearer header path does not mutate dev methods; Login UI local validation; authenticating state disables duplicate submit; 401/error copy; success stores session, clears password and routes to Character Select.

Runtime evidence uses the existing Entry design and must capture before/after at PC/tablet/mobile. The after pack must include default, authenticating or deterministic in-flight state, invalid-credentials state, and success transition evidence when a local API fixture is available.

No password or access token may appear in logs, screenshots, test-result XML, capture manifests or committed fixtures.

## Completion gate

Auth Foundation is ready for closure when server + Unity tests are green, product Login no longer reports `Dịch vụ đăng nhập chưa kết nối` for a reachable API, successful auth opens Character Select, wrong credentials remain on Entry with generic copy, expiry/logout clear the session, and all three Entry viewports preserve the canonical layout.

Frozen protocol/GameData/design-token surfaces remain unchanged. Register, Recovery, persisted Character Select, refresh tokens, rate limiting and visible logout/settings UI are explicit follow-up scopes.
