# M3 Persistence Evidence Plan

M3 is a server/API persistence prototype. It does not require Unity runtime evidence unless a later M3 slice adds Unity UI or client-side account flow.

## Source validation

```bash
./tools/validate_m3_source.sh
```

Expected marker:

```text
M3 SOURCE VALIDATION PASS
```

## Server build/test

```bash
./server/build.sh
./server/test.sh
```

Expected: Java 25, Maven 3.9.16, no skipped test-as-pass, and test reports with executed count greater than zero.

## Runtime smoke

```bash
./tools/run_m3_api_persistence_once.sh
```

The smoke must prove:

- Spring Boot API starts with a dedicated `LG_API_PERSISTENCE_DIR`;
- `/health` returns UP;
- `POST /dev/auth/login` creates or reuses a dev account;
- raw dev key is not present in `players-v1.json`;
- character create/list/load succeeds;
- position save/load succeeds;
- invalid create and missing character return client errors;
- API restart reloads persisted account/character;
- no API process remains orphaned after the script exits.

## Closure decision labels

- `M3_ACCOUNT_CHARACTER_PERSISTENCE_SOURCE_READY`: source/test/smoke are ready, but full runtime closure has not been accepted.
- `M3_ACCOUNT_CHARACTER_PERSISTENCE_RUNTIME_CLOSED`: source and API runtime persistence smoke are accepted.
- `FIX_REQUIRED`: actionable defect remains.
- `BLOCKED_CONTRACT`: M3 requires a frozen contract/schema/protocol change.

Expected runtime smoke marker:

```text
M3_PERSISTENCE_SMOKE_PASS
```
