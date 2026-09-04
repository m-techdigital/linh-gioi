# Versioning

Three runtime compatibility versions are intentionally separate:

- `client_version`: shipped client build identity.
- `protocol_version`: wire compatibility generation.
- `gamedata_version`: compiled content generation.

Current source package identity:

```text
source_package_version = governance-v1.0
milestone = Governance Roadmap Queue
m0_status = M0_RUNTIME_CLOSED
m1_status = M1_OFFLINE_COMBAT_RUNTIME_CLOSED
m2_status = M2_RUNTIME_CANDIDATE_HARDENED_READY_FOR_LOCAL_EVIDENCE
m6_status = M6_COMBAT_READINESS_SPEC_CLOSED_v0.32.0
governance_status = LGO_CODE_GOVERNANCE_CONTRACT_ACCEPTED_v1.0
```

Current runtime placeholders:

```text
client_version = 0.6.0-governance
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

## M4-2/M4-3 Playable UI And Art Quality Pass v0.12.0

Current source status:

```text
M4_PLAYABLE_UI_ART_QUALITY_SOURCE_READY
```

M4-2 improves the playable UI shell from the accepted design lock. M4-3 upgrades committed placeholder SVG quality in place while preserving runtime catalog paths. This does not change protocol, GameData, production authentication, database persistence, final production art status, or MMO networking versions.

## M4 Playable Slice Stabilization v0.13.0

Current source status:

```text
M4_PLAYABLE_SLICE_STABILIZATION_SOURCE_READY
```

M4 v0.13.0 adds checksum-pinned macOS protobuf tooling, a one-command M4 closure check, and a stabilization validator. It does not change protocol, GameData, production authentication, database persistence, combat, economy, final production art, or MMO gameplay scope.

## M4 Visible UI Usability v0.14.0

Current source status:

```text
M4_VISIBLE_UI_USABILITY_SOURCE_READY
```

M4 v0.14.0 makes the visible 1280x720 playable UI reviewable, adds a manual visible review harness, and hardens source validation for visible UI affordances. It does not change protocol, GameData, production authentication, database persistence, combat, economy, final production art, or MMO gameplay scope.

## M5 First Playable Loop Foundation v0.15.0

Current source status:

```text
M5_FIRST_PLAYABLE_LOOP_SOURCE_READY
```

M5 v0.15.0 adds a controlled local-only first playable loop foundation inside the existing playable world shell: enter world, approach the Gate Keeper or Training Stone, trigger a concise F/Space interaction, receive objective feedback, and keep Save Position / Back to Lobby behavior. It does not change protocol, GameData, production authentication, database persistence, combat, inventory, economy, guild, chat, market, party, live ops, final production art, or MMO-scale gameplay scope.

## M5 Visual Evidence UX Acceptance v0.16.0

Current source status:

```text
M5_VISUAL_EVIDENCE_UX_REVIEW_READY
```

M5 v0.16.0 adds a Unity-side visual evidence review path for Gate Entry, Character Hall, World HUD, and First Playable Loop Feedback screenshots plus summary metadata. It does not add gameplay, combat, inventory, quests, economy, guild, chat, market, party, live ops, production auth, database persistence, protocol changes, GameData schema changes, final production UI, or final production art.

## Visual Reference Pack v0.16.5

Current reference status:

```text
LGO_VISUAL_REFERENCE_PACK_ACCEPTED_v0.16.5
```

The v0.16.5 pack records seven owner-provided visual reference PNGs and translation rules. It is reference-only and does not change client runtime, protocol, GameData, production authentication, database persistence, final production UI, or final production art status.

## M5 Guided Training Loop v0.17.0

Current source status:

```text
M5_GUIDED_TRAINING_LOOP_SOURCE_READY
```

M5 v0.17.0 hardens the local non-combat first playable loop into a clearer sequence: Enter World, talk to Gate Keeper, follow the cyan spirit pulse to the Training Stone, stabilize it with F/Space, complete the objective, and preserve Save Position / Back to Lobby. It does not add combat, damage, HP balancing, loot, inventory, economy, guild, chat, market, party, live ops, production auth, database persistence, protocol changes, GameData schema changes, final production UI, or final production art.
