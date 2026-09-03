# P10 — M3 Account / Character Persistence Prototype

Use the current authoritative source only. Do not restore old source.

## Goal

Implement the first server-side account and character persistence prototype for Linh Giới Online.

## Scope

Implement:
- development-only account login;
- character creation/list/load;
- position save/load;
- local JSON persistence schema v1;
- schema version guard;
- runtime smoke proving restart persistence.

Do not implement:
- production auth;
- password/OAuth/payment;
- PostgreSQL/Redis production infra;
- guild/economy/marketplace;
- protocol mutation;
- GameData schema mutation;
- M4 or later systems.

## Required commands

```bash
./tools/validate_m3_source.sh
./server/build.sh
./server/test.sh
./tools/run_m3_api_persistence_once.sh
```

## Completion

Return a report, handoff, changed-file inventory, delta ZIP, full-source ZIP, evidence ZIP, and SHA256 files. Do not claim `M3_ACCOUNT_CHARACTER_PERSISTENCE_RUNTIME_CLOSED` unless runtime smoke actually passed on the same source/toolchain/provenance.
