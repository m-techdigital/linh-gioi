# M4 Closure Command v0.13.0

Use `tools/lgo_m4_closure_check.sh` from repo root `LinhGioiOnline`.

Source gates:

```bash
./tools/lgo_m4_closure_check.sh --source-only
```

This mode cleans disposable Unity/generated/build outputs, runs whitespace diff validation, project-state validation, M4 playable validation, M4 visual validation, M4-2 UI validation, M4 stabilization validation, inherited M4 source validation, and Python compile checks. It prints `LGO_M4_CLOSURE_SOURCE_GATES_PASS` only when all source gates pass.

Runtime gates:

```bash
./tools/lgo_m4_closure_check.sh --runtime
```

This mode runs source gates first, sources `.lgo-local-env` when present, verifies pinned protobuf tooling, requires `UNITY_EDITOR`, prepares Unity local assets, generates disposable Unity protocol sources, builds the server, builds a macOS Unity player, then runs M3-B, M4 playable, and M4 visual runtime smokes.

Required runtime markers:

```text
M3B_UNITY_ACCOUNT_CHARACTER_RUNTIME_SMOKE_PASS
M4_PLAYABLE_VERTICAL_SLICE_RUNTIME_SMOKE_PASS
M4_VISUAL_PLACEHOLDER_FOUNDATION_SMOKE_PASS
```

If runtime environment is missing, the command prints `LGO_M4_CLOSURE_RUNTIME_UNVERIFIED_ENVIRONMENT` with the exact reason. It must not be treated as runtime PASS.

Package readiness:

```bash
./tools/lgo_m4_closure_check.sh --package-ready
```

This mode runs source gates, cleans disposable outputs, verifies package hygiene, writes `build/lgo-m4-closure/latest-summary.txt`, writes `build/lgo-m4-closure/latest-summary.json`, and copies the final text summary to `LGO-M4-PLAYABLE-STABILIZATION-CLOSURE-SUMMARY-v0.13.0.txt`.

Non-claims:

- full M0 runtime is not newly claimed by M4 stabilization
- production auth is not claimed
- DB persistence is not claimed
- full MMO gameplay is not claimed
- combat is not claimed
- final production art is not claimed
