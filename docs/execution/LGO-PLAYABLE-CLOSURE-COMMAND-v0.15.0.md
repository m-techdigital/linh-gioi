# LGO Playable Closure Command v0.15.0

Use `tools/lgo_playable_closure_check.sh` from repo root `LinhGioiOnline`.

Source gates:

```bash
./tools/lgo_playable_closure_check.sh --source-only
```

Package readiness:

```bash
./tools/lgo_playable_closure_check.sh --package-ready
```

Runtime gates:

```bash
./tools/lgo_playable_closure_check.sh --runtime
```

The wrapper keeps the existing M4 closure command intact and adds M5 first playable loop validation/runtime smoke.

Expected source marker:

```text
LGO_PLAYABLE_CLOSURE_SOURCE_GATES_PASS
```

Expected package marker:

```text
LGO_PLAYABLE_CLOSURE_PACKAGE_READY
```

Expected runtime markers:

```text
M3B_UNITY_ACCOUNT_CHARACTER_RUNTIME_SMOKE_PASS
M4_PLAYABLE_VERTICAL_SLICE_RUNTIME_SMOKE_PASS
M4_VISUAL_PLACEHOLDER_FOUNDATION_SMOKE_PASS
M5_FIRST_PLAYABLE_LOOP_RUNTIME_SMOKE_PASS
LGO_PLAYABLE_CLOSURE_RUNTIME_GATES_PASS
```

If Unity or a required runtime tool is unavailable, the wrapper must print:

```text
LGO_PLAYABLE_CLOSURE_RUNTIME_UNVERIFIED_ENVIRONMENT
```

Visible UI review remains manual/evidence-based:

```bash
./tools/run_m4_visible_ui_review.sh --rebuild
```

Visible acceptance must not be claimed when screenshot capture or human review is unavailable.
