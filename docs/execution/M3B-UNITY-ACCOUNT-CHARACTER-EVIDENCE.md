# M3-B Unity Account / Character Integration Evidence

## Purpose

Prove that Unity consumes the M3 server/API persistence slice through the real HTTP API instead of duplicating account or character state locally.

## Required command evidence

| Gate | Command | Required marker |
|---|---|---|
| Source validation | `./tools/validate_m3b_source.sh` | `M3B SOURCE VALIDATION PASS` |
| Server build | `./server/build.sh` | `BUILD SUCCESS` |
| Server tests | `./server/test.sh` | `TEST_EVIDENCE_PASS executed>0 skipped=0` |
| M3 API persistence smoke | `./tools/run_m3_api_persistence_once.sh` | `M3_API_PERSISTENCE_RUNTIME_SMOKE_PASS` |
| M3-B Unity client smoke | `./tools/run_m3b_unity_account_character_once.sh --unity-player <path>` | `M3B_UNITY_ACCOUNT_CHARACTER_RUNTIME_SMOKE_PASS` |

## Runtime smoke expectations

The current Unity Linux player built from this source must run with:

```text
--lgo-m3b-account-character-smoke
--lgo-m3b-api-url http://127.0.0.1:<port>
--lgo-m3b-result <json>
```

The runner must complete:

1. `POST /dev/auth/login`.
2. `GET /accounts/{accountId}/characters`.
3. `POST /accounts/{accountId}/characters` if the named smoke character does not already exist.
4. `POST /characters/{characterId}/position`.
5. `GET /characters/{characterId}`.
6. API restart.
7. Repeat Unity smoke with `--lgo-m3b-expect-existing` and the same dev key/name.

Expected result JSON:

```text
status = PASS
accountId starts with account.dev.
characterId starts with character.
characterName = M3BHero
classId = class.sword
x = 3.25
y = 0.5
z = -7.75
yawDegrees = 270.0
expectExisting = true on restart pass
reusedExistingCharacter = true on restart pass
```

## Anti-overclaim rules

- Missing Unity player or Unity Editor is `UNVERIFIED_ENVIRONMENT`.
- A player built from an older source is not valid M3-B runtime evidence.
- HTTP smoke from Python alone does not prove Unity client integration.
- Do not claim M4 readiness from this evidence.
