# Five-Class Character Schema + Map01A Runtime Entry Migration Design

Status: implementation-authorized by owner MCP intent D-0885e1e6e929 after source audit.

## Goal

Make persisted characters compatible with canonical runtime classes `vo`, `kiem`, `phap`, `co`, `linh`, while preserving legacy M3/M4 records and defining an unambiguous Map01A lane-entry state.

This slice carries class/runtime-position data safely from server persistence through product bearer APIs and Unity account models into Map01A entry state. It does not claim that all five class renderers are production-ready.

## Existing authorities

- Legacy persistence class IDs: `class.sword`, `class.martial`.
- Canonical 2D runtime IDs: `vo`, `kiem`, `phap`, `co`, `linh`.
- Existing accepted display compatibility: `class.sword → Kiếm`, `class.martial → Võ`.
- Map authority ID: `map-01a-cong-dong-lam`.
- Map01A lane bounds: `[-3.8, 44.4]`; current safe start: `TwoDOnboardingState.PlayerStart.x == -3.6`.
- Map01A facing authority is integer direction `-1` or `+1`; default is `+1`.

## Class compatibility contract

`CharacterProfile.classId` remains the raw stored identifier for backward compatibility. Historical rows are not rewritten merely to satisfy the new runtime vocabulary.

A new server compatibility helper resolves a canonical runtime class:

- `class.martial` and `vo` → `vo`
- `class.sword` and `kiem` → `kiem`
- `phap` → `phap`
- `co` → `co`
- `linh` → `linh`
- any other value is rejected for new writes and fails validation when used as a runtime class.

The legacy create path may continue accepting `class.sword` / `class.martial`; it also accepts the five canonical IDs. Product responses add `runtimeClassId` so Unity never needs to infer a display/runtime class from raw legacy IDs.

## Persistence v4

Create `players-v4.json`; preserve `players-v3.json` byte-for-byte as the rollback source.

`PlayerPersistenceSnapshot` gains `runtimeStatesByCharacterId`, keyed by character ID. Runtime state is separate from `CharacterProfile` so old XYZ/yaw meaning remains intact.

Each runtime state contains:

- `mapId`
- `laneX`
- `facing`
- `updatedAtUnixMs`

For this slice the only valid map state is `map-01a-cong-dong-lam`, with finite `laneX` inside `[-3.8, 44.4]` and `facing` exactly `-1` or `+1`.

Migration v3→v4 copies accounts, dev-key index and characters without rewriting `classId`, raw XYZ or yaw. It initializes no Map01A state for legacy rows, validates references, writes v4 atomically and leaves v3 untouched.

Legacy raw `positionX/Y/Z/yawDegrees` remain the M3/M4 world-coordinate contract. They are not Map01A coordinates.

## Map01A entry mapping

Product character responses expose both `runtimeClassId` and optional runtime state.

When entering Map01A:

1. If runtime state exists for `map-01a-cong-dong-lam`, apply its `laneX` and `facing` exactly after validation.
2. If runtime state is absent or belongs to another map, enter at `laneX = -3.6`, `facing = +1`.
3. Never derive Map01A lane/facing from legacy `x/y/z/yawDegrees`.
4. The nearest route node continues to be recomputed by existing Map01A `Refresh()` from the applied lane position.

## Product API contract

Existing bearer-scoped reads remain authoritative:

- `GET /auth/characters`
- `GET /auth/characters/{characterId}`

Their response adds canonical `runtimeClassId` and optional `runtimeState` without removing legacy fields.

Add bearer-scoped `POST /auth/characters/{characterId}/map01a-state` with body `{ laneX, facing }`. The server derives account ownership from the bearer token, verifies the character belongs to that account, fixes `mapId` to `map-01a-cong-dong-lam`, validates lane/facing, and persists only the v4 runtime state.

Unknown and cross-account character IDs return the same generic `404 character not found`. Missing/invalid bearer remains generic `401`.

Legacy `/characters/{characterId}/position` remains unchanged for M3/M4 smoke compatibility and never writes the Map01A state.

## Unity client and entry adapter

`CharacterResponse` adds `runtimeClassId` and optional `CharacterRuntimeStateResponse`.

`IProductCharacterClient` adds a bearer-scoped `SaveMap01AStateAsync(accessToken, characterId, laneX, facing, ct)` method.

A pure `Map01ACharacterEntryMapper` converts a loaded product character into `Map01ACharacterEntryState` with canonical class, lane, facing and `usedLegacySpawnFallback` flag. Invalid canonical class or malformed Map01A state is rejected rather than silently repaired.

`CongDongLamMap01AArtPreview` gains an explicit product-entry application method that sets only `_routeX`, `_sourcePoseFacing`, and loaded product runtime-class metadata, then calls existing refresh/pose logic.

It must not turn `TwoDSourcePoseReview` into a product renderer and must not rewrite `ActiveEquipmentClassId` to a class whose renderer is unavailable.

## Five-class renderer gate

The data path is five-class compatible, but current product Map01A renderer authority is still the Võ runtime pack. `TwoDSourcePoseReview` is explicitly `REVIEW_ONLY` with `runtimeEligible=false`.

Therefore this migration records and carries the canonical selected class without pretending the corresponding actor art/gameplay renderer exists. Review must report this as a remaining renderer gate for `kiem/phap/co/linh`; it is not a failure of schema migration.

No source-pose, class art, wardrobe or combat redesign is part of this slice.

## Save-trigger boundary

This slice provides the product-safe save API/client contract and unit/integration coverage. It does not invent a new visible Save button or an unreviewed autosave cadence because the current Map01A product HUD has no authoritative save lifecycle trigger.

A later lifecycle slice may bind `SaveMap01AStateAsync` to an approved checkpoint/autosave/logout/map-transition trigger. Until then, runtime entry can consume persisted Map01A state written through the authenticated API in integration tests/tools.

## Tests and evidence

Server RED/GREEN coverage must prove:

- v3→v4 migration preserves v3 bytes and legacy raw class/XYZ/yaw;
- all five canonical classes are accepted for new storage while legacy aliases remain readable;
- canonical runtime-class resolution is deterministic;
- Map01A state validates map/lane/facing and survives reload;
- product save is bearer-scoped and cross-account indistinguishable from missing character;
- legacy position endpoint does not mutate Map01A runtime state.

Unity RED/GREEN coverage must prove:

- product DTO/client contract parses canonical runtime class + optional Map01A state;
- legacy no-runtime-state entry uses exactly `-3.6 / +1`, ignoring raw XYZ/yaw;
- valid Map01A state applies lane/facing and existing route-node refresh follows the new lane;
- all five canonical class IDs survive the mapper without changing renderer authority;
- malformed map state/class fails closed.

Player evidence is narrow: Character Select → Enter Game → Map01A entry position/facing on PC/tablet/mobile using deterministic capture state. No review/rejected PNG/JPG enters Git; all visual evidence stays under ignored `build/`.

## Non-goals

- No Create Character UI or product create endpoint.
- No five-class actor-art/pose/wardrobe renderer integration.
- No combat-class implementation.
- No GameData/protocol frozen-surface changes.
- No reinterpretation or deletion of legacy M3/M4 XYZ/yaw.
- No new save button or guessed autosave policy.
