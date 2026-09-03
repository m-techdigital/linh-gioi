# Versioning

Three runtime compatibility versions are intentionally separate:

- `client_version`: shipped client build identity.
- `protocol_version`: wire compatibility generation.
- `gamedata_version`: compiled content generation.

Current source package identity:

```text
source_package_version = 0.10.0
milestone = M4-1 Visual Identity + Runtime Art Placeholder Foundation
m0_status = M0_RUNTIME_CLOSED
m1_status = M1_OFFLINE_COMBAT_RUNTIME_CLOSED
m2_status = M2_RUNTIME_CANDIDATE_HARDENED_READY_FOR_LOCAL_EVIDENCE
```

Current runtime placeholders:

```text
client_version = 0.4.0-m4
protocol_version = 1
gamedata_version = 1
```

Rules:

- M2 consumes existing handshake and movement protocol messages and does not change `protocol_version` or `gamedata_version`.
- Any protocol/schema/version bump requires S0 contract-change approval and a regeneration task.
- M2 runtime closure must record the exact source package version, Unity version, Java version, Maven version, runtime evidence batch, and artifact SHA256 values.
- M3 must not start until `M2_ONLINE_SESSION_RUNTIME_CLOSED` is accepted.

## M3 Account / Character Persistence v0.7.0

Current M3 source candidate: `M3_ACCOUNT_CHARACTER_PERSISTENCE_SOURCE_READY`. This was opened by explicit project-owner override from the M2 runtime candidate state; it does not claim `M2_ONLINE_SESSION_RUNTIME_CLOSED`.

Key commands:

```bash
./tools/validate_m3_source.sh
./server/build.sh
./server/test.sh
./tools/run_m3_api_persistence_once.sh
```

Main persistence artifact at runtime: `players-v1.json` under `LG_API_PERSISTENCE_DIR`. Raw dev keys must not be persisted.


m3_status = M3_ACCOUNT_CHARACTER_PERSISTENCE_RUNTIME_SMOKE_CLOSED
m3b_status = M3B_UNITY_ACCOUNT_CHARACTER_SOURCE_READY_FOR_VERIFY


## M3-B Unity Account / Character Integration v0.8.0

M3-B consumes the closed M3 server/API persistence prototype without changing protocol or GameData schema versions. It adds Unity HTTP client integration and a restart-aware Unity smoke command.

Current M3-B source status:

```text
M3B_UNITY_ACCOUNT_CHARACTER_SOURCE_READY_FOR_VERIFY
```

Runtime closure requires `M3B_UNITY_ACCOUNT_CHARACTER_RUNTIME_SMOKE_PASS` from a current player built from this source.

M3-B hotfix v0.8.1: Unity local compile support adds UnityWebRequest module dependency and generated protocol asmdef reference to Google.Protobuf.dll. No frozen protocol/schema changes.

## M4-0 Playable Client Vertical Slice v0.9.0

Current M4 source status:

```text
M4_PLAYABLE_VERTICAL_SLICE_FOUNDATION_SOURCE_READY
```

M4 consumes the existing M3-B account/character API client and persistence endpoints. It does not change protocol or GameData versions.

## M4-1 Visual Placeholder Foundation v0.10.0

Current M4 visual source status:

```text
M4_VISUAL_PLACEHOLDER_FOUNDATION_SOURCE_READY
```

M4-1 adds original placeholder art source assets and runtime catalog references. It does not change protocol, GameData, production authentication, database persistence, or MMO networking versions.
