# M3-B — Unity Account / Character Integration

## Task ID

`LG-M3B-UNITY-ACCOUNT-CHARACTER-INTEGRATION-v0.8.0`

## Baseline

`linh-gioi-m3-account-character-persistence-v0.7.0-full-source.zip`

Baseline SHA256:

```text
6110c4d1043eae7650df72b8386b9784bf91f1fd9523490ddd26f5c0308ab968
```

## Goal

Connect the Unity client to the already closed M3 server/API persistence prototype.

The Unity client must exercise the real HTTP API for:

- development-only login;
- account character list;
- character create;
- character load;
- character position save/load;
- restart persistence proof when a current Unity Linux player is available.

## Scope

Allowed:

- Unity account/character API client source;
- Unity smoke entrypoint for M3-B account/character integration;
- Unity EditMode/source tests for client DTO/config/parser behavior;
- M3-B source validator and runtime runner;
- M3-B docs, checklist, report, handoff, manifest;
- server regression only if a real M3 API defect is found.

Forbidden:

- no M4 social/party/guild/economy/marketplace;
- no production authentication, password, OAuth, payment, or billing;
- no PostgreSQL/Redis production persistence;
- no protocol mutation;
- no GameData schema mutation;
- no downgrade of Java, Maven, Unity, URP, or Protobuf pins;
- no mock API or source-inspection-as-runtime-PASS.

## Required verification

```bash
./tools/validate_m3b_source.sh
./server/build.sh
./server/test.sh
./tools/run_m3_api_persistence_once.sh
./tools/run_m3b_unity_account_character_once.sh --unity-player <current-player-built-from-this-source>
```

If a current Unity player cannot be built or provided in the sandbox, the Unity runtime gate must be reported as `UNVERIFIED_ENVIRONMENT`, not as PASS.

## Completion status for this source

`M3B_UNITY_ACCOUNT_CHARACTER_SOURCE_READY_FOR_VERIFY`

This status does not claim M2 Unity runtime closure, does not claim M4 entry, and does not claim M3-B Unity runtime PASS without a current player smoke.
