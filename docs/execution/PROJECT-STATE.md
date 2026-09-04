# Linh Giới Online — Project State

Last updated: `2026-09-04`

## Current milestone

`M5 Guided Training Loop`

## Current decision

`M5_GUIDED_TRAINING_LOOP_SOURCE_READY`

M0 final decision: `M0_RUNTIME_CLOSED`.

M1 final decision: `M1_OFFLINE_COMBAT_RUNTIME_CLOSED` (`M1 Offline Combat Prototype` closed).

M2 has source-level implementation for the first online session scaffold. Runtime closure is still pending because this sandbox does not run Unity Editor and Java 25/Maven runtime together for M2 evidence.

## Authoritative source baseline

`linh-gioi-m5-guided-training-loop-v0.17.0`

Baseline ancestry:

- accepted M0 runtime closure;
- accepted M1 offline combat runtime closure;
- M3 server/API persistence runtime closure;
- M3-B Unity account/character source integration;
- M4-0 playable vertical slice foundation;
- M4-1 visual placeholder foundation;
- M4-2 playable UI redesign;
- M4-3 placeholder art quality pass;
- M4 closure automation and stabilization validation;
- M4 visible UI usability and manual review harness;
- M5 first playable loop foundation with local-only interaction feedback;
- M5 visual evidence UX review path.
- accepted visual reference pack v0.16.5.
- M5 guided training loop source hardening.

## Current source successor

`linh-gioi-m5-guided-training-loop-v0.17.0`

This source includes:

- existing M2 online session runtime candidate tooling;
- closed M3 server/API persistence;
- M3-B Unity account/character client integration source;
- M4 source status `M4_PLAYABLE_VERTICAL_SLICE_FOUNDATION_SOURCE_READY`;
- M4 visual status `M4_VISUAL_PLACEHOLDER_FOUNDATION_SOURCE_READY`;
- M4 UI/art quality status `M4_PLAYABLE_UI_ART_QUALITY_SOURCE_READY`;
- M4-2/M4-3 Playable UI And Art Quality Pass v0.12.0;
- M4 stabilization status `M4_PLAYABLE_SLICE_STABILIZATION_SOURCE_READY`;
- M4 visible UI status `M4_VISIBLE_UI_USABILITY_SOURCE_READY`;
- M4 playable UI shell, runtime art catalog, and in-world HUD shell;
- upgraded original placeholder SVGs under `client/Unity/Assets/Game/Art/**`;
- M4 source validators, runtime smoke command, closure automation, handoff, report, and manifest.
- M4 visible UI review harness and usability validator.
- M5 first playable loop source status `M5_FIRST_PLAYABLE_LOOP_SOURCE_READY`.
- M5 local-only interaction loop with Gate Keeper, Training Stone, non-combat Shadow Slime marker, proximity prompt, F/Space acknowledgement, and existing save/back flow preserved.
- M5 visual evidence source status `M5_VISUAL_EVIDENCE_UX_REVIEW_READY`.
- Unity-side visual evidence runner for Gate Entry, Character Hall, World HUD, and First Playable Loop Feedback.
- Visual reference pack status `LGO_VISUAL_REFERENCE_PACK_ACCEPTED_v0.16.5`.
- M5 guided training loop source status `M5_GUIDED_TRAINING_LOOP_SOURCE_READY`.
- Guided local sequence: talk to Gate Keeper, stabilize Training Stone, preserve Save Position and Back to Lobby.

## Closed M0 gates

- Source validation: PASS.
- Protocol codegen and tests: PASS.
- GameData tests and compiled manifest: PASS.
- Java 25 runtime: PASS.
- Maven 3.9.16 runtime: PASS.
- Server build/test: PASS, 25 executed / 0 skipped.
- Spring Boot `/health`: PASS.
- Netty TCP bind: PASS.
- Real TCP `ClientHello -> ServerHello`: PASS.
- Unsupported protocol / malformed payload survival: PASS.
- Unity Editor evidence: PASS, 6000.3.2f1.
- Unity project import/generate: PASS.
- Unity EditMode tests: PASS.
- Unity Linux player build: PASS.
- Unity Player -> Java Netty handshake: PASS.
- Graceful shutdown / no orphan server process: PASS.
- Frozen contract audit: PASS.

## Closed M1 gates

- M1 source implementation: PASS.
- M1 GameData-driven catalog mapping: PASS.
- M1 stricter catalog validation source: PASS.
- M1 invalid request rejection source: PASS.
- M1 deterministic combat test source: PASS.
- M1 HUD prototype source: PASS.
- M1 default camera/light scene generation: PASS.
- M1 offline smoke command: PASS.
- M1 runtime evidence scripts: PASS.
- Unity `6000.3.2f1` M1 EditMode runtime: PASS.
- Unity M1 player build: PASS.
- Sandbox M1 offline combat Linux player replay: PASS.

## Current M2 source gates

- Existing protocol consumed without mutation: PASS source audit.
- Java online session source: PASS static validation.
- Java online session unit/integration tests added: PASS static validation.
- Unity online session client source: PASS static validation.
- Unity online smoke command source: PASS static validation.
- M2 runtime evidence tooling/docs: PASS static validation.

## M2 runtime gates still pending

- Java 25/Maven server build and tests with new M2 session classes: pending runtime verification.
- Server `online-session-smoke.py` against live Java Netty: pending runtime verification.
- Unity EditMode tests with M2 client serialization: pending runtime verification.
- Unity-built Linux player `--lgo-m2-online-session-smoke` against live Java Netty: pending runtime verification.
- Reconnect/failure path in runtime smoke evidence: pending runtime verification.

## Next allowed step

Run targeted playable closure verification on the current source. M2 runtime closure remains pending local Unity evidence and must not be inferred from M4/M5 work.

## Forbidden next step

Do not start full M5 social/guild, payment, marketplace, PvP ranking, production auth, production DB infra, progression/economy expansion, inventory expansion, combat expansion, or broad content expansion. Do not mutate `protocol/**`, `gamedata/schemas/**`, ADR, or design tokens without an S0 contract-change task.

## M5 First Playable Loop Foundation v0.15.0

Current M5 source status: `M5_FIRST_PLAYABLE_LOOP_SOURCE_READY`.

This owner-approved foundation adds only a lightweight local interaction loop to the existing M4 playable shell:

- enter world through the existing account/character flow;
- see player, Gate Keeper, Training Stone, and non-combat Shadow Slime markers;
- approach an interactable and press F or Space;
- receive concise objective/interaction feedback;
- preserve Save Position and Back to Lobby.

It does not claim full M5 social scope, full combat, economy, guild, chat, market, party, live ops, production auth, DB persistence, final production art, protocol changes, or GameData schema changes.

## M5 Visual Evidence UX Acceptance v0.16.0

Current source status: `M5_VISUAL_EVIDENCE_UX_REVIEW_READY`.

This review path adds Unity-side screenshot capture and deterministic metadata for:

- Gate Entry;
- Character Hall;
- World HUD;
- First Playable Loop Feedback.

It writes `build/visual-evidence/visual-evidence-summary.json` and `build/visual-evidence/visual-evidence-summary.txt`. Visual evidence remains review-ready and does not by itself claim explicit human visual acceptance.


## M3 owner override note

`OWNER_OVERRIDE_FROM_M2_RUNTIME_CANDIDATE` is recorded because M3 source work was explicitly opened by the project owner before `M2_ONLINE_SESSION_RUNTIME_CLOSED`. This does not claim the pending M2 Unity local evidence gates as PASS.

## Current M3 source gates

- Dev account login API source: PASS static/source validation.
- Dev key SHA-256 persistence hygiene: PASS static/source validation.
- Character create/list/load API source: PASS static/source validation.
- Character position save/load API source: PASS static/source validation.
- Local JSON persistence schema v1: PASS static/source validation.
- Unsupported future schema guard: PASS static/source validation.
- API persistence smoke tooling: PASS static/source validation.

## M3 runtime gates pending until execution

- Java 25 + Maven server build/test on M3 source.
- Spring Boot API runtime with `LG_API_PERSISTENCE_DIR`.
- Dev login/create/load/save HTTP smoke.
- API restart reload smoke.
- No raw dev key persisted.
- No orphan API process after smoke.

## M2 v0.6.2 hardening note

`M3_ACCOUNT_CHARACTER_PERSISTENCE_SOURCE_READY` supersedes v0.6.1 while keeping M2 locked to the same protocol and scope. v0.6.2 retains the one-command local runner and dependency-free GameData fallback, then adds server/client movement validation parity, multi-snapshot Unity smoke assertions, and no-false-ready local evidence classification.


## Previous M2 state marker for inherited M2 validators

`M2 Online Session Prototype` remains at `M2_RUNTIME_CANDIDATE_HARDENED_READY_FOR_LOCAL_EVIDENCE`; M2 runtime evidence remains pending local Unity execution.


## Closed M3 server/API gates

- M3 final decision: `M3_ACCOUNT_CHARACTER_PERSISTENCE_RUNTIME_SMOKE_CLOSED`.
- Dev login API runtime smoke: PASS.
- Character create/list/load runtime smoke: PASS.
- Character position save/load runtime smoke: PASS.
- API restart/reload persistence smoke: PASS.
- Raw dev key persistence hygiene: PASS.

## Current M3-B source gates

- Unity `ClientRuntimeConfig` API endpoint configuration: PASS static/source validation.
- Unity `LinhGioi.Account` asmdef and dependency graph: PASS static/source validation.
- Unity `AccountApiClient` real HTTP implementation with `UnityWebRequest`: PASS static/source validation.
- Unity login/list/create/load/save-position model surface: PASS static/source validation.
- Unity command-line smoke path `--lgo-m3b-account-character-smoke`: PASS static/source validation.
- Restart-aware smoke path `--lgo-m3b-expect-existing`: PASS static/source validation.

## M3-B runtime gates pending until current Unity player execution

- Current Unity Linux player built from M3-B source.
- Unity player first-pass API smoke.
- API restart.
- Unity player restart-pass API smoke with persisted character reuse.
- Raw `m3b-unity-dev-key` absence in `players-v1.json`.

## M3-B owner override note

`OWNER_OVERRIDE_FROM_M3_SERVER_API_CLOSED` is recorded because the owner explicitly opened Unity client integration after the M3 server/API persistence closure while M2 Unity local evidence remains separately pending. This does not claim `M2_ONLINE_SESSION_RUNTIME_CLOSED`.


Historical milestone reference: `M3 Account / Character Persistence` is closed at server/API persistence level before M3-B.
