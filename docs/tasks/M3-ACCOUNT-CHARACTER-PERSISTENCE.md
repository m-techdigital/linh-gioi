# M3 — Account / Character Persistence Prototype

Status target: `M3_ACCOUNT_CHARACTER_PERSISTENCE_SOURCE_READY`.

## Entry note

M3 normally starts after `M2_ONLINE_SESSION_RUNTIME_CLOSED`. This branch starts from `M2_RUNTIME_CANDIDATE_HARDENED_READY_FOR_LOCAL_EVIDENCE` by explicit project-owner override. The override does not convert M2 Unity runtime gates into PASS.

## Scope

Allowed:
- development-only account login path;
- character creation/list/load;
- position save/load;
- local JSON persistence schema v1;
- migration/version guard and restart smoke;
- server API tests and smoke tooling.

Forbidden:
- no payment, marketplace, public auth, OAuth, password auth, Redis, PostgreSQL production schema, multi-region infra, guild, party, economy, or protocol changes;
- no mutation of `protocol/**`, `gamedata/schemas/**`, ADR, or design tokens;
- no claim of M2 or M3 Unity runtime closure without local Unity evidence.

## API prototype

- `POST /dev/auth/login`
- `GET /accounts/{accountId}/characters`
- `POST /accounts/{accountId}/characters`
- `GET /characters/{characterId}`
- `POST /characters/{characterId}/position`

## Persistence prototype

Store file:

```text
players-v1.json
```

The prototype stores a SHA-256 hash of the dev key, never the raw dev key. Writes use a temp file and atomic move where supported.

## Required evidence

- `./tools/validate_m3_source.sh`
- `./server/build.sh`
- `./server/test.sh`
- `./tools/run_m3_api_persistence_once.sh`

The runtime smoke must start the API, create/load/save a character, restart the API, and prove persisted reload.
