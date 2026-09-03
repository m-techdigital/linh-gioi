# Linh Giới Online Integration Ledger

This ledger records accepted source successors and active follow-up gates. It is intentionally conservative: a source-ready task is not runtime-closed until runtime evidence is accepted.

## Current authority

- Current baseline: `linh-gioi-m1-offline-combat-runtime-closed-v0.5.3-full-source.zip`
- Current milestone: `M1 Offline Combat Prototype`
- Current decision: `M1_OFFLINE_COMBAT_RUNTIME_CLOSED`
- Last fully closed milestone: `M0_RUNTIME_CLOSED`

## Accepted / active ledger

| Order | Task | Artifact / overlay | SHA256 | Verification | Decision | Next allowed step |
|---:|---|---|---|---|---|---|
| 0 | M0 Foundation Baseline | `linh-gioi-m0-sequential-ready-v0.1.zip` | `5fb62b2082022a7e234c2dd76fb53461009062142eecc06050202e3d1d26e060` | static foundation | superseded by unified source | M0 unified/runtime closure |
| 1 | M0 Unified Foundation | `linh-gioi-m0-unified-foundation-v0.2.zip` | `307cf5c42c9b76ee1a450f7b25544a6532caf8f1955662f31d189756552999d7` | source integration | superseded by runtime-closed source | M0 runtime closure |
| 2 | M0 Server Runtime Closure | `linh-gioi-m0-runtime-closure-delta-v0.3.3.zip` | `bd0d149b7e17e54b4f217efc6c26a637c8bb44e3ad23dc77367ce5465659ba57` | Java 25/Maven/server runtime smoke | `M0_SERVER_RUNTIME_CLOSED_UNITY_ENV_LIMITED` | Unity evidence route |
| 3 | M0 Hybrid Unity Evidence Tooling | `linh-gioi-m0-hybrid-runtime-support-v0.3.13-full-source.zip` | `305191eae4aa489fef091c96939033696125b701f5b620d7fb6812c0afc08d3f` | source validation + local Unity artifact support | accepted tooling successor | final runtime replay |
| 4 | M0 Runtime Closed | `linh-gioi-m0-runtime-closed-v0.4.1-full-source.zip` | `382fa31fd358812eddc64ddf21bd958d2f941e4ba027e9d13dbfcec785e19283` | Java runtime + Unity editor/player + Unity player to Java handshake | `M0_RUNTIME_CLOSED` | M1 Offline Combat Prototype |
| 5 | M1 Offline Combat Prototype | `linh-gioi-m1-offline-combat-prototype-v0.5.0-full-source.zip` | `94c0439b0a93cf01148ad5919eb858fc0aeacf1bfac77e68cd0001407d7c8820` | M0 source regression + M1 static validator | `M1_OFFLINE_COMBAT_RUNTIME_CLOSED` | runtime closed / plan M2 |
| 6 | M1 Independent Audit Hardening | `linh-gioi-m1-offline-combat-runtime-closed-v0.5.3-full-source.zip` | `<computed in handoff>` | stricter static validation + project-state consistency | `M1_OFFLINE_COMBAT_RUNTIME_CLOSED` | M2 Online Session Prototype planning |

## Historical superseded M0 debug deltas

| Task | Status | Notes |
|---|---|---|
| `LG-M0-RUNTIME-CLOSURE v0.3` | superseded | initial source/tooling failure discovery |
| `LG-M0-RUNTIME-CLOSURE v0.3.1` | superseded | server test bootstrap fix path |
| `LG-M0-RUNTIME-CLOSURE v0.3.2` | superseded | Netty embedded channel output fix path |
| `LG-M0-HYBRID-RUNTIME-SUPPORT v0.3.5-v0.3.12` | superseded | local Unity evidence route iterations before v0.3.13 |

## Gate rule

A future sandbox must start from the latest accepted full source successor, not from historical deltas, unless the owner explicitly asks for forensic comparison.
