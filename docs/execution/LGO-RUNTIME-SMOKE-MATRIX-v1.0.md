# Linh Gioi Runtime Smoke Matrix v1.0

Marker: `LGO_RUNTIME_SMOKE_MATRIX_READY`

## Purpose

This matrix keeps runtime evidence discoverable and prevents repeated stop/start behavior caused by ad hoc command selection.

## Source Gates

| Gate | Command | Claim |
|---|---|---|
| Package hygiene | `python3.12 tools/validate_package_hygiene.py` | source tree has no forbidden cache/package clutter |
| Continuous mode | `python3.12 tools/validate_lgo_continuous_development_mode.py` | continuous workflow docs/tools are present |
| Playable source closure | `./tools/lgo_playable_closure_check.sh --source-only` | source validators pass |
| Package ready closure | `./tools/lgo_playable_closure_check.sh --package-ready` | source can be packaged cleanly |

## Runtime Gates

| Gate | Command | Required marker |
|---|---|---|
| M4 inherited runtime | `./tools/lgo_m4_closure_check.sh --runtime` | `LGO_M4_CLOSURE_RUNTIME_GATES_PASS` |
| M5 first playable loop | `./tools/run_m5_first_playable_loop_once.sh` | `M5_FIRST_PLAYABLE_LOOP_RUNTIME_SMOKE_PASS` |
| M5 guided training loop | `./tools/run_m5_guided_training_loop_once.sh` | `M5_GUIDED_TRAINING_LOOP_RUNTIME_SMOKE_PASS` |
| M5 lightweight dialogue | `./tools/run_m5_lightweight_dialogue_once.sh` | `M5_LIGHTWEIGHT_NPC_DIALOGUE_RUNTIME_SMOKE_PASS` |
| M6 minimal local combat | `./tools/run_m6_minimal_local_combat_once.sh` | `M6_MINIMAL_LOCAL_COMBAT_RUNTIME_SMOKE_PASS` |
| M6 Unity combat intent client | `./tools/run_m6_unity_combat_intent_client_once.sh` | `M6_UNITY_COMBAT_INTENT_CLIENT_RUNTIME_SMOKE_PASS` |
| M6 Unity Java combat smoke | `./tools/run_m6_unity_java_combat_smoke.sh` | `M6_UNITY_JAVA_COMBAT_SMOKE_PASS` |
| M6 server-authoritative pilot | `./tools/run_m6_server_authoritative_combat_pilot.sh` | `M6_SERVER_AUTHORITATIVE_COMBAT_PILOT_RUNTIME_PASS` |
| M6 Unity Java combat E2E | `./tools/run_m6_unity_java_combat_e2e.sh` | `M6_UNITY_JAVA_COMBAT_E2E_PASS_v0.52.0` |

## One-Command Runtime Closure

Use:

```bash
./tools/lgo_playable_closure_check.sh --runtime
```

The final runtime closure marker is:

```text
LGO_PLAYABLE_CLOSURE_RUNTIME_GATES_PASS
```

## Failure Rules

- Source/static failure is `FIX_REQUIRED`.
- Missing Unity/player runtime is `UNVERIFIED_ENVIRONMENT`.
- `executed=0` is not a pass.
- Existing runtime smoke can be hardened, but gameplay semantics must not change inside this task.
- Do not mask failures with `|| true` in evidence commands.
