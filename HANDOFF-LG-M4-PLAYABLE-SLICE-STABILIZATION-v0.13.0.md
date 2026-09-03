# Handoff LG M4 Playable Slice Stabilization v0.13.0

Final decision: `M4_PLAYABLE_SLICE_STABILIZATION_SOURCE_CLOSED_RUNTIME_UNVERIFIED_ENVIRONMENT_v0.13.0`.

Baseline used: `lgo-m4-playable-ui-art-quality-closed-local-v0.12.0`.

Use from repo root:

```bash
./tools/lgo_m4_closure_check.sh --source-only
./tools/lgo_m4_closure_check.sh --package-ready
```

Runtime verification, when Unity `6000.3.2f1` is configured through `UNITY_EDITOR`:

```bash
./tools/lgo_m4_closure_check.sh --runtime
```

Expected source marker:

```text
LGO_M4_CLOSURE_SOURCE_GATES_PASS
```

Expected package marker:

```text
LGO_M4_CLOSURE_PACKAGE_READY
```

Expected runtime markers:

```text
M3B_UNITY_ACCOUNT_CHARACTER_RUNTIME_SMOKE_PASS
M4_PLAYABLE_VERTICAL_SLICE_RUNTIME_SMOKE_PASS
M4_VISUAL_PLACEHOLDER_FOUNDATION_SMOKE_PASS
```

Runtime status for this sandbox:

```text
LGO_M4_CLOSURE_RUNTIME_UNVERIFIED_ENVIRONMENT
REASON=UNITY_EDITOR is not set
```

Changed inventory: `LGO-M4-PLAYABLE-SLICE-STABILIZATION-v0.13.0-CHANGED-FILES.txt`.

Deleted inventory: `LGO-M4-PLAYABLE-SLICE-STABILIZATION-v0.13.0-DELETIONS.txt`.

Artifact hash inventory: `LGO-M4-PLAYABLE-SLICE-STABILIZATION-v0.13.0-ARTIFACTS.sha256`.

Non-claims:

- full M0 runtime not claimed
- production auth not claimed
- DB persistence not claimed
- full MMO gameplay not claimed
- combat not claimed
- final production art not claimed
