# 07 — Phase Gates and Acceptance Conditions

This document defines what must be true before a milestone can start, what is allowed inside it, and what proves it is done. A sandbox may add implementation only inside the active milestone and must not promote source inspection into runtime evidence.

## Code governance

Every future milestone must read and obey `docs/execution/CODE-GOVERNANCE-CONTRACT.md`, `docs/execution/CODE-OWNERSHIP-MAP.md`, and `docs/execution/CODE-QUALITY-GATES.md` before final handoff. Code governance requires anti-duplication review, ownership audit, validator non-weakening, package hygiene, frozen surface audit, runtime evidence classification, and explicit technical-debt follow-up.

## Gate vocabulary

| Term | Meaning |
|---|---|
| Entry gate | Conditions required before starting a milestone/task. |
| Work gate | Constraints while implementation is in progress. |
| Verification gate | Tests/evidence that must pass before handoff. |
| Acceptance gate | Owner/control-tower decision that promotes the result to the next baseline. |
| Override | Written owner exception. Must record risk and rollback path. |

## Universal milestone states

```text
BACKLOG -> READY -> IN_PROGRESS -> VERIFY -> INTEGRATE -> DONE
```

`DONE` means accepted into the authoritative source baseline. `VERIFY` means ready for review only.

## M0 — Foundation Runtime Closure

M0 final state: `M0_RUNTIME_CLOSED`.

### Entry gate

- Authoritative M0 source ZIP and SHA are known.
- Frozen contracts are locked: `protocol/**`, ADRs, schema contracts, GameData contracts, network contracts, design tokens.
- Java/Unity target versions are known.

### Allowed work

- Validation tooling, runtime closure scripts, source fixes required to make foundation runtime verifiable.
- No gameplay expansion.
- No new maps/classes/skills/monsters except existing M0 fixtures.

### Exit gate

| Gate | Required evidence | Current decision |
|---|---|---|
| Source validation | `./tools/validate_m0_source.sh` PASS | PASS |
| Protocol tooling | descriptor compile + C#/Java generation + determinism + negative compile | PASS |
| GameData tooling | positive/negative tests + deterministic manifest | PASS |
| Java 25/Maven | actual versions from runtime host | PASS on uploaded server kit |
| Java server tests | `server/test.sh` executed with nonzero test count | PASS |
| API runtime | real Spring Boot boot + `/health` response | PASS |
| Realtime runtime | real Netty bind + TCP handshake | PASS |
| Bad-client survival | unsupported/malformed client then valid client accepted | PASS |
| Unity import/compile | Unity `6000.3.2f1` import/compile logs | PASS |
| Unity EditMode | executed EditMode result XML/log | PASS |
| Unity player ↔ Java | Linux player smoke against real Java realtime server | PASS |

M0 is closed. Further M0 work requires an explicit regression/fix task and must not reopen M1 scope by accident.

## M1 — Offline Combat Prototype

M1 final accepted state: `M1_OFFLINE_COMBAT_RUNTIME_CLOSED`.

### Entry gate

- Preferred and current path: `M0_RUNTIME_CLOSED`.
- Historical override path only: `M0_SERVER_RUNTIME_CLOSED_UNITY_ENV_LIMITED` plus owner accepts Unity risk. Do not use this path now because M0 is already closed.
- `protocol/**` and `gamedata/**` remain frozen unless S0 approves a contract-change task first.

### Allowed work

- Local/offline combat loop prototype.
- GameData-driven character/enemy/skill read-only consumption.
- Combat HUD prototype consuming existing design tokens.
- Deterministic local simulation tests.

### Forbidden work

- Online combat authority, persistence, account systems, monetization, marketplace, guild, PvP ranking, large content expansion.

### Exit gate

- Unity compile/import PASS on the M1 source.
- At least one deterministic combat fixture PASS.
- Player can enter prototype scene, attack, receive damage, finish a local encounter, and return to safe state.
- HUD shows HP/resource/cooldown states without hard-coded production balancing.
- M0 source/runtime regression still PASS for touched slices.

## M2 — Online Session Prototype

Current state: `M5_GUIDED_TRAINING_LOOP_SOURCE_READY`.

M1 final state: `M1_OFFLINE_COMBAT_RUNTIME_CLOSED`.

Previous M2 source-only marker: `M2_ONLINE_SESSION_SOURCE_READY` (superseded by v0.6.1 runtime candidate tooling).

### Entry gate

- M1 offline combat accepted.
- M0/M1 protocol compatibility verified.
- Java realtime server baseline still boots and handshakes.

### Allowed work

- Single-session online loop scaffold.
- Client connection lifecycle, server-side session state, latency-safe input/state acknowledgement.
- Existing `MoveIntent -> PlayerTransformSnapshot` consumption without protocol mutation.
- Runtime evidence scripts that start Java realtime and run Unity player session smoke.

### Forbidden work

- No account persistence, character database, Redis routing, economy, guild, marketplace, PvP ranking, or large-scale MMO AOI.
- No `protocol/**` or `gamedata/schemas/**` change without S0 contract-change task.

### Exit gate

- Java server build/test PASS with M2 session tests.
- Live Java Netty `online-session-smoke.py` PASS.
- Unity client connects to Java realtime after accepted handshake.
- Unity sends one movement intent and receives an authoritative `PlayerTransformSnapshot`.
- Reconnect/failure path is tested.
- Server survives malformed/invalid/disconnect conditions.
- Final decision may become `M2_ONLINE_SESSION_RUNTIME_CLOSED` only after runtime evidence is accepted.

## M3 — Account and Character Persistence

### Entry gate

- M2 online session accepted.
- Persistence schema ownership defined.
- Migration discipline agreed.

### Allowed work

- Dev auth/login path, account/character creation, load/save, basic inventory placeholder if needed for character state.

### Exit gate

- Migration up/down or rollback plan validated.
- Character create/load/save tests PASS.
- API/runtime smoke PASS.
- No secret/config leakage.

## M4 — Progression, Economy Skeleton, and Inventory Foundation

Current state: `M5_FIRST_PLAYABLE_LOOP_SOURCE_READY`.

The current M4 source is limited to playable vertical slice presentation work, visual placeholder foundation, playable UI redesign, placeholder art quality pass, closure automation, source-gate stabilization, visible UI usability, and manual review harness support. It does not open progression, economy, inventory, social, production auth, protocol, or GameData schema scope.

## M5-0 — First Playable Loop Foundation

Current state: `M5_FIRST_PLAYABLE_LOOP_SOURCE_READY`.

This owner-approved M5-0 foundation is intentionally narrower than the full M5 social roadmap. It adds local-only interaction feedback inside the playable world shell: Gate Keeper, Training Stone, non-combat Shadow Slime marker, proximity prompt, F/Space acknowledgement, objective completion text, and existing Save Position / Back to Lobby preservation.

Forbidden work remains full combat, damage, HP balancing, loot, inventory, economy, guild, chat, market, party, live ops, production auth, database persistence, protocol mutation, GameData schema mutation, final production art, and MMO-scale gameplay.

Runtime closure requires the inherited M3-B/M4 runtime smoke markers plus:

```text
M5_FIRST_PLAYABLE_LOOP_RUNTIME_SMOKE_PASS
```

## M5-1 — Visual Evidence UX Acceptance

Current state: `M5_VISUAL_EVIDENCE_UX_REVIEW_READY`.

This review gate adds deterministic Unity-side visual evidence for Gate Entry, Character Hall, World HUD, and First Playable Loop Feedback. Human visual acceptance remains separate from file generation unless explicitly accepted by the owner.

Runtime/review evidence requires:

```text
LGO_PLAYABLE_VISUAL_EVIDENCE_READY
```

## M5-2 — Guided Training Loop

Current state: `M5_GUIDED_TRAINING_LOOP_SOURCE_READY`.

This gate uses the accepted visual reference pack `LGO_VISUAL_REFERENCE_PACK_ACCEPTED_v0.16.5` to clarify the local non-combat loop: Enter World, talk to Gate Keeper, move to Training Stone, stabilize the spirit pulse, complete objective, and preserve Save Position / Back to Lobby.

Runtime evidence requires:

```text
M5_GUIDED_TRAINING_LOOP_RUNTIME_SMOKE_PASS
```

### Entry gate

- M3 accepted.
- GameData item/skill/class identity rules stable.

### Allowed work

- XP/level skeleton, item acquisition/consumption, inventory state, non-monetized currency placeholder.

### Exit gate

- Transaction/inventory consistency tests PASS.
- Duplicate/overflow/invalid item paths rejected.
- Client display and server authority align.

## M5 — Social, Party, Guild Foundation

### Entry gate

- M3 persistence and M2 session lifecycle accepted.

### Allowed work

- Friends, party invite lifecycle, guild shell, chat-safe message envelope placeholder.

### Exit gate

- Invite/accept/reject/leave flows PASS.
- Permissions and membership state tests PASS.
- Abuse/safety hooks documented, even if moderation implementation is deferred.

## M6 — Content Expansion and LiveOps Preparation

### Entry gate

- Core loop and data ownership stable.

### Allowed work

- Content authoring workflow, validation, localization hooks, event schedule model, admin/liveops planning.

### Exit gate

- Content packs compile deterministically.
- Invalid content rejected.
- Client/server consume the same compiled content version.

## M7 — Alpha Readiness and Operations

### Entry gate

- Minimum playable loop accepted.
- Account, content, server runtime, and client build pipeline stable.

### Exit gate

- Release candidate package created.
- Install smoke PASS.
- Observability, rollback, support, and incident process documented.
- Legal/publishing checklist reviewed by owner.

## Gate-change rule

Any sandbox that believes a gate is wrong must create a gate-change request instead of silently bypassing it. The request must include reason, impacted files, risk, replacement gate, and rollback decision.


## M3-B — Unity Account / Character Integration

Current state: `M3B_UNITY_ACCOUNT_CHARACTER_SOURCE_READY_FOR_VERIFY` for the historical M3-B source gate.

### Entry gate

- M3 server/API persistence final decision is `M3_ACCOUNT_CHARACTER_PERSISTENCE_RUNTIME_SMOKE_CLOSED`.
- Owner explicitly allows Unity client integration before reopening broader M4 scope.

### Allowed work

- Unity account/character API client consuming the M3 HTTP API.
- Runtime smoke command proving login, list/create/load, save/load position, and restart persistence.
- Validation/report/handoff tooling for this slice.

### Forbidden work

- M4 social, party, guild, economy, marketplace, payment, production authentication, PostgreSQL/Redis migration, protocol mutation, and GameData schema mutation.

### Exit gate

- `./tools/validate_m3b_source.sh` PASS.
- Java 25/Maven server build/test PASS.
- M3 API persistence smoke PASS on the same source/toolchain/provenance.
- Current Unity player built from this source runs `--lgo-m3b-account-character-smoke` first pass and restart pass.
- Raw `m3b-unity-dev-key` is not persisted.
- Frozen contract audit PASS.

If the current Unity player cannot be built or executed in the environment, the Unity runtime gate remains `UNVERIFIED_ENVIRONMENT` and the final decision cannot be `M3B_UNITY_ACCOUNT_CHARACTER_RUNTIME_CLOSED`.
