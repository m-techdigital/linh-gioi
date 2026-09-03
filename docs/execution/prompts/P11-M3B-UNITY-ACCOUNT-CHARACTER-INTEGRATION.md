# P11 — M3-B Unity Account / Character Integration

Use the current authoritative source only. Do not restore old source.

## Goal

Integrate Unity with the M3 account/character persistence API through real HTTP client code.

## Implement

- Unity runtime models matching M3 API response fields.
- Unity `AccountApiClient` using `UnityWebRequest`.
- API config in `ClientRuntimeConfig` and `StreamingAssets/linhgioi-client.json`.
- Unity smoke command path `--lgo-m3b-account-character-smoke`.
- Restart-aware smoke assertion via `--lgo-m3b-expect-existing`.
- M3-B source validator and runtime runner.

## Required commands

```bash
./tools/validate_m3b_source.sh
./server/build.sh
./server/test.sh
./tools/run_m3_api_persistence_once.sh
./tools/run_m3b_unity_account_character_once.sh --unity-player <current-player-built-from-this-source>
```

## Do not implement

- M4 social/party/guild/economy/marketplace.
- Production authentication, password/OAuth, billing/payment.
- PostgreSQL/Redis production infra.
- Protocol or GameData schema mutation.
- Mock API runtime PASS.

## Completion

Return changed files, deletions, report, handoff, evidence, delta ZIP, and SHA256. If the Unity player cannot run in this environment, return the limitation as `UNVERIFIED_ENVIRONMENT` and use final decision `M3B_SERVER_ONLY_SOURCE_CLOSED_UNITY_ENV_LIMITED` rather than claiming Unity runtime closure.
