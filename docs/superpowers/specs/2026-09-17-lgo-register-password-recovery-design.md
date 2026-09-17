# LGO Register + Password Recovery 3-step — Design

Date: 2026-09-17
Status: VISUAL_REVIEW_PENDING

## Purpose

Add real product account registration and a three-stage password-recovery lifecycle on top of the verified Product Auth Foundation, without folding Character persistence into this slice.

The product flow is `Register → Login` and `Recovery Request → Verify Code → New Password → Login`. Successful registration/reset does not auto-login.

## Existing visual authority

The authoritative visual source is the owner-design pack under `/Users/minhdc/Projects/Design/LGO-2D-UI-Owner-Demos-2026-09-13/`, not the simplified runtime screenshots. Runtime evidence proves behavior/layout only and must be brought closer to owner-design fidelity rather than treated as a replacement art direction.

- Entry/Login: `redesign-v5-entry/01-entry-login-CANONICAL.png`.
- Character Select: `redesign-v6-character-select/01-character-select-CANONICAL.png`.
- Server Select: `redesign-v7-server-select/01-server-select-CANONICAL.png`.
- Register: `redesign-v8-register/01-register-account-CANONICAL.png`.
- Recovery Request: `redesign-v9-password-recovery-request/01-password-recovery-request-CANONICAL-CANDIDATE.png`.
- Historical visual reference: `preferred-v2/04-dang-nhap-may-chu-bat-dau-linh-gioi.png`.

A repository/Design-root search and prior project-state audit found no separate owner artwork for Verify Code or New Password; historical docs explicitly state those gates had not been opened. Those two screens may therefore be newly designed, but only as direct extensions of v9's visual DNA and only after owner review.

Any generated comparison/mockup that is not visibly derived from this owner pack is rejected and has no canonical/runtime authority.
## Product identity decision

The first product-registration slice uses **email as the product login identifier and recovery contact**. This avoids creating accounts that cannot actually recover their password.

The existing generic Login field remains compatible with future account-name/phone aliases. Register proposes a copy-only change from `Tài khoản / Email` to `Email đăng nhập`; Recovery Request proposes `Email đăng ký` and `Gửi mã xác minh` so the UI does not claim capabilities the backend cannot honor.

Email canonicalization is conservative: trim + lowercase with `Locale.ROOT`; no provider-specific dot/plus rewriting. Validation requires one `@`, non-empty local/domain parts, total length 3–254. Delivery address is the normalized identifier for this slice.

## Neutral account authority v3

Product registration must not fabricate a dev key. Player persistence therefore moves to a neutral account model:

- `AccountProfile`: `accountId`, `displayName`, `createdAtUnixMs`, `updatedAtUnixMs` only.
- Dev credentials remain solely in `accountIdByDevKeyHash`; `/dev/auth/login` continues hashing/looking up that index.
- Product accounts use IDs `account.product.<uuid32>` and receive no dev-key index entry.
- Current data moves to `players-v3.json`, schema version 3.
Migration rules:

- If `players-v3.json` exists, validate/load it normally.
- Otherwise migrate from `players-v2.json` without modifying or deleting the v2 file. Account IDs, display names, timestamps, characters, slots and dev-key index are preserved; `devKeyHash` is removed from the neutral account record after cross-checking it against the v2 dev index.
- If only v1 exists, apply the existing legacy slot migration while producing v3 directly; v1 remains untouched.
- Future/invalid schema versions fail closed. No automatic downgrade or destructive rewrite is permitted.

`PlayerProfileStore` adds product-account creation and read-only account lookup. Character create/list/load semantics remain unchanged.

## Registration service

`ProductRegistrationService` owns product account + credential provisioning; controllers never mutate stores directly.

Registration validates normalized email, password 8–128 characters, confirmation on the client, and an accepted-terms flag on the request. Server-side terms acceptance is mandatory even if the client already validated it.

The service preflights duplicate identifier, creates a neutral product account, then creates the BCrypt-12 credential. Same-process credential failure must roll back the newly created empty product account. The coordinator is synchronized to prevent duplicate registration races in the current single-process JSON runtime.

Separate JSON files are not process-crash atomic. That limitation is explicit for this foundation; transactional durability across account + credential records moves to the later database-migration task rather than being hidden behind a false production claim.
## Recovery security model

`PasswordRecoveryService` uses the existing product credential lookup but never reveals whether an email exists.

Request behavior:

- Client sends normalized email.
- HTTP response is the same generic accepted shape for existing and unknown addresses when the delivery subsystem is available.
- Existing accounts receive a 6-digit cryptographically random verification code through `RecoveryDelivery`; unknown addresses perform equivalent local work but do not deliver.
- The server stores only a BCrypt hash of the code in an in-memory challenge registry, never the raw code.
- Challenge lifetime: 10 minutes. Maximum verification attempts: 5. Resend cooldown: 60 seconds. New resend invalidates the previous challenge.

The delivery boundary is an interface. Production uses SMTP/configured mail delivery with secrets supplied only through environment/deployment configuration; tests use a deterministic in-memory delivery fake. No recovery code is returned by product HTTP APIs.
Verify/reset behavior:

- Verify accepts challenge ID + six-digit code. Wrong/expired/exhausted codes use one generic failure; attempts are decremented server-side.
- Successful verification consumes the challenge and issues a 32-byte opaque reset token valid for 10 minutes. Only its SHA-256 hash is retained in memory.
- Reset accepts the raw one-time reset token plus a new 8–128 character password. Success replaces the BCrypt-12 credential hash, consumes the reset token, clears remaining recovery challenges for the account and invalidates every active auth session for that account.
- Password reset returns the user to Login with the identifier retained and password blank; it does not auto-login.

Raw password, verification code, bearer session token and reset token are forbidden from persistence, logs, exception messages, screenshots, manifests and committed fixtures.

## Product HTTP contract

- `POST /auth/register` → `201` account summary; duplicate email → generic `409` without returning stored account data.
- `POST /auth/recovery/request` → `202` generic accepted response with challenge metadata needed by the client (`challengeId`, `expiresAtUnixMs`, `resendAvailableAtUnixMs`); response semantics do not disclose account existence.
- `POST /auth/recovery/verify` → `200` reset grant (`resetToken`, `expiresAtUnixMs`) only after a valid code.
- `POST /auth/recovery/reset` → `204` after password replacement/session invalidation.

Malformed request → `400`; duplicate registration → `409`; resend cooldown → `429`; invalid, expired, exhausted or consumed verification/reset grant → one generic `401`, with no secret/token echo. A globally unavailable delivery provider returns `503`; the server never claims that a code was sent when delivery failed.
## Visual proposal — Register and Request

Register must preserve the actual v8 owner composition, ornament, typography, panel treatment, background/character presentation, notice card and utility rail; the current simplified runtime is not the visual target. Geometry/state code may reuse the current implementation, but final Player fidelity is judged against the v8 owner PNG. Proposed product-capability changes are semantic only:

- field 1: `Email đăng nhập`;
- subtitle remains `Bắt đầu hành trình tại Đông Lâm`;
- success status: `Tạo tài khoản thành công. Hãy đăng nhập.` and route back to Entry;
- duplicate/validation/service errors stay in the existing reserved status line.

Recovery Request must preserve and converge toward the actual v9 owner candidate, including the rich Entry scene, blue/gold ornament language, typography and single centered auth panel. Proposed capability copy changes are:

- guidance: `Nhập email đã dùng để đăng ký tài khoản.`;
- field: `Email đăng ký`;
- primary CTA: `Gửi mã xác minh`;
- generic accepted status: `Nếu email hợp lệ, mã xác minh đã được gửi.`;
- successful request advances to Verify Code using the returned challenge metadata without exposing account existence.
## Visual proposal — Verify Code

New screen must be designed as a direct v9 owner-design continuation, reusing the same Entry artwork/composition, blue/gold ornament system, title typography, field treatment, notification card and utility rail instead of creating a second visual system or using simplified runtime styling.

Hierarchy:

1. Title `XÁC MINH MÃ`.
2. Subtitle `Nhập mã 6 số đã gửi tới email của bạn`.
3. A quiet destination line using a locally masked version of the user-entered email; server response is not used to reveal account existence.
4. One real numeric text field `Mã xác minh 6 số`; no six separate fake fields are required.
5. Reserved status/countdown line.
6. Full-width primary CTA `Xác minh`.
7. Quiet resend action `Gửi lại mã` with 60-second cooldown and a `Quay lại` action to Request.

The panel remains one-column and approximately the same height as Recovery Request. PC 1600×900, tablet 1024×768 and mobile 1600×720 preserve one landscape composition with no reflow/stack variant.
## Visual proposal — New Password

New Password must be visually derived from the v8/v9 owner artwork: v9 recovery panel/scene composition plus v8 password field/reveal treatment and gold primary CTA. Shared runtime helpers are implementation details, not substitutes for owner-design fidelity.

Hierarchy:

1. Title `ĐẶT MẬT KHẨU MỚI`.
2. Subtitle `Tạo mật khẩu mới cho tài khoản của bạn`.
3. Password field `Mật khẩu mới` with reveal action.
4. Confirmation field `Nhập lại mật khẩu mới` with independent reveal action.
5. Quiet rule line `Từ 8 đến 128 ký tự` plus reserved validation/status area.
6. Full-width primary CTA `Cập nhật mật khẩu`.
7. Quiet action `Quay lại đăng nhập`.

Mismatch and policy failures keep both values for correction. Successful reset clears both fields/reset token, invalidates previous sessions and returns to Entry with `Mật khẩu đã cập nhật. Hãy đăng nhập.`.

No visual step indicator is added in v1: the title/subtitle make the current recovery stage explicit while keeping the existing compact auth hierarchy.
## Unity flow ownership

One `ProductAccountRecoveryState` (or equivalently cohesive auth-flow state object) owns only transient Register/Recovery navigation data: normalized email, challenge ID/expiry/resend time, verified reset token/expiry and current stage. It is memory-only and cleared on successful reset, explicit cancel/back-to-login, or HUD destruction.

`IProductAccountClient` extends product-account operations without coupling UI directly to `AccountApiClient`: register, request recovery, verify code and reset password. Product Login remains on `IProductAuthClient`.

Register and Recovery partials own hierarchy/state binding only; HTTP/status classification stays in the Account client layer. A 401/410 recovery error never clears an authenticated product session unless it is an authenticated session endpoint response.

## Error and enumeration rules

- Register may state that an email is already registered only after a real registration attempt; it must not expose account metadata.
- Recovery Request always displays the same accepted copy for existing and unknown syntactically valid emails.
- Verify does not distinguish wrong code, expired code, unknown challenge or exhausted attempts in Player-facing copy beyond a generic invalid/expired message.
- Resend while cooldown is active is disabled and shows remaining seconds; it does not create parallel challenges.
- Network/server errors keep the current screen and non-secret input so the user can retry.
- Never fall back to dev auth or fabricate recovery success locally.
## Testing and evidence

Server TDD must cover v2→v3 migration with original v2 bytes preserved, dev login compatibility, product-account creation with no dev index entry, duplicate-email rollback, credential BCrypt hygiene, registration validation and full recovery challenge lifecycle including cooldown, attempt limit, expiry, reset-token one-time use and session invalidation.

Controller tests cover 201/202/success paths plus generic enumeration-safe recovery responses and non-secret 4xx errors. SMTP delivery is exercised through the delivery interface in deterministic tests; no real external mailbox is required for unit closure.

Unity EditMode tests cover Register local validation + async submit state, Request/Verify/New Password stage transitions, resend cooldown, generic errors, mismatch handling, success return to Login and complete transient-secret clearing.

Player evidence must include before/after for every visually changed screen at PC 1600×900, tablet 1024×768 and mobile 1600×720. Evidence manifests must state `usesOsMouseOrKeyboard=false` and contain no email fixture beyond a non-sensitive test address, verification code, raw password, session token or reset token.

Full server tests, full Unity EditMode, shared UI/skin, no-source-image, no-3D, package hygiene, frozen protocol/GameData/ADR/design-token diff and scoped secret scan are required before closure.
## Explicit non-goals

This subproject does not wire persisted Character Select/list/create/load, add social login/phone aliases, create visible logout/settings UI, persist recovery/session tokens, implement multi-node session replication, or claim cross-file JSON crash transactions equivalent to a database.

It does not redesign the Entry scene, Character Hub, class artwork, world HUD, combat controls, or Character Select. The known Character Select mobile motto/stage debt remains assigned to the later Character persistence/create-character slice.

## Approval and implementation gate

Architecture in this spec is ready for owner review, but runtime implementation must not start until the visual proposal is approved because two required screens have no prior canonical design and two existing screens need capability-copy adjustments.

The review package must show:

- v8 owner Register source versus proposed email-specific variant that preserves the same art direction;
- v9 owner Recovery Request source versus proposed `Gửi mã xác minh` variant preserving the same art direction;
- proposed Verify Code screen;
- proposed New Password screen;
- at least one mobile-landscape check proving the new screens keep the Entry composition.

After owner approval, mark this spec `APPROVED`, write a separate TDD implementation plan, and execute without another routine confirmation unless a new design change or genuine blocker appears.
