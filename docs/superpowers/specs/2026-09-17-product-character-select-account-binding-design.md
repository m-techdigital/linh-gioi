# Product Character Select Account Binding Design

Status: `ARCHITECTURAL_SECURITY_UPGRADE / IMPLEMENTATION_AUTHORIZED_BY_STANDING_OWNER_DIRECTIVE`

## Goal

Bind the existing canonical Map01A Character Select screen to the authenticated product account's persisted character list and selected character record without weakening account scope, changing the approved visual composition, or pretending legacy character class/position data already maps to the five-class Map01A runtime.

## Current facts

- Product Auth is closed and provides opaque bearer sessions through `ProductAuthService` / `ProductAuthSessionState`.
- `AccountCharacterController` currently exposes legacy M3 routes by raw `accountId` / `characterId` and does not require bearer authentication.
- Product Character Select currently renders hard-coded `LụcThiên` plus two empty slots.
- Persistence already stores three slots per account and exposes list/load methods.
- Persisted class IDs are currently limited to `class.sword` / `class.martial`; Map01A runtime uses Võ/Kiếm/Pháp/Cơ/Linh. Do not invent a mapping beyond display labels in this slice.
## Security architecture

Product runtime must not call the raw legacy account routes.

Add product-scoped read routes:

- `GET /auth/characters`
- `GET /auth/characters/{characterId}`

Both require `Authorization: Bearer <token>`.
The controller validates the token through `ProductAuthService.validateSession`, derives the account ID from the authenticated session, and never accepts a product account ID from the client.

Character load must verify `character.accountId == authenticatedAccountId`. A character owned by another account is returned as the same generic `404 character not found` as an unknown ID to avoid cross-account existence disclosure.

Keep legacy `/accounts/{accountId}/characters` and `/characters/{characterId}` unchanged for M3/dev smoke compatibility; they are not product endpoints and the product UI must not call them.
## Unity client contract

Add a bearer-scoped `IProductCharacterClient` implemented by `AccountApiClient`:

```csharp
Task<CharacterResponse[]> ListProductCharactersAsync(string accessToken, CancellationToken cancellationToken);
Task<CharacterResponse> LoadProductCharacterAsync(string accessToken, string characterId, CancellationToken cancellationToken);
```

The methods call `/auth/characters` and `/auth/characters/{characterId}` and send the existing bearer header. Existing legacy overloads remain available for M3/dev runners.

`CongDongLamArrivalHud.Attach` reuses the injected `AccountApiClient` when it implements the new interface. If no client was injected, the existing lazy client construction path creates one shared `AccountApiClient`; do not create a second network client for Character Select.

No access token, character ID, or password is written to PlayerPrefs. Character list/load state stays in memory.
## Character Select behavior

The existing canonical Character Select geometry/chrome remains unchanged.

- Render three stable slot buttons: `Map01A Character Slot 1`, `2`, `3`.
- While listing, show `Đang tải danh sách nhân vật…` and disable Enter Game.
- Bind persisted records by their explicit `slot` field. Empty slots remain visually empty and keep the existing Create Character gate.
- Auto-select the first occupied slot in slot order; selecting another occupied slot changes only Character Select state.
- Display legacy class IDs as `Kiếm` for `class.sword` and `Võ` for `class.martial`; this is display-only and must not mutate Map01A runtime class state.
- If list loading fails, keep Character Select open and show `Không thể tải danh sách nhân vật. Vui lòng thử lại.` without inventing local profiles.
- Direct capture mode may seed the existing deterministic LụcThiên review profile so canonical screenshot tooling stays network-independent. Normal product runtime must never use that seed.
## Enter Game behavior

`Vào game` is enabled only for an occupied selected slot and after list loading has settled.

On click:

1. disable the action and show `Đang tải nhân vật…`;
2. call the bearer-scoped load endpoint;
3. verify the returned character ID and account ownership against the selected record/session;
4. store the loaded `CharacterResponse` in HUD memory;
5. bind the product-visible player name to the loaded character name;
6. close Character Select only after a successful load.

On load failure, keep Character Select open and show `Không thể tải nhân vật. Vui lòng thử lại.`.

This slice does **not** apply persisted class or XYZ/yaw into Map01A. The current persisted class schema and the 2D five-class runtime are not yet compatible, and legacy XYZ coordinates are not yet defined as Map01A lane coordinates.
## Error and loading semantics

- Missing/invalid bearer: `401 invalid or expired session`.
- Unknown or cross-account character: generic `404 character not found`.
- Character list network/server failure: generic product-facing retry copy; no raw response body in UI.
- Duplicate list requests are suppressed while one request is in flight.
- Duplicate character load callbacks are suppressed while a load is in flight.
- Back navigation remains available only when no list/load request is actively mutating Character Select state.

## Testing and evidence

TDD must cover server authorization scope, cross-account non-disclosure, bearer HTTP routes, three-slot binding, list loading/error, selection isolation, load-before-close, and load failure.

Player evidence must preserve the current Character Select composition on PC `1600×900`, tablet `1024×768`, and mobile landscape `1600×720`, with BEFORE/AFTER boards. No Create Character UI is added in this slice.

## Explicit follow-up

The next subproject after this slice is character schema/runtime entry migration: replace `class.sword/class.martial` with an explicit compatibility/migration contract for Võ/Kiếm/Pháp/Cơ/Linh, then define how persisted position maps into Map01A lane/world state. Create Character remains design-gated until a current canonical product screen is approved.
