# M0 Unified Execution — Initial Foundation

## Decision

For the initial M0 foundation, S0/S1/S2/S3/S4/S5 are **module and ownership boundaries**, not separate execution sandboxes.

The authoritative M0 workflow is now one continuous source tree:

```text
M0 spec
  + accepted S5-A protocol tooling
  + accepted S2-A Java scaffold
        ↓
unified source
        ↓
S2-B protocol integration
        ↓
S1 Unity foundation
        ↓
S4 GameData pipeline
        ↓
S3 UI foundation
        ↓
S5 QA/CI integration
        ↓
M0 source closure
        ↓
M0 runtime exit gate
```

This supersedes the earlier recommendation to execute the first M0 implementation batch across multiple independent sandboxes. The old task files remain useful as scope/ownership contracts.

## Rules

1. One authoritative source tree.
2. `protocol/*.proto` remains frozen unless S0 explicitly approves a contract change.
3. Generated C#/Java protocol source remains disposable.
4. Unity generated scenes/theme/URP assets under `Assets/Game/Generated/` are reproducible and disposable; do not hand-edit their YAML.
5. Source validation and runtime validation remain separate so environment limitations cannot become false PASS.
6. M1 Combat does not begin until required M0 runtime gates are executed on the final unified source.

## Canonical validation

Source-only, executable in environments without Java 25/Unity:

```bash
./tools/validate_m0_source.sh
```

Full runtime closure:

```bash
./tools/validate_m0_runtime.sh
```

Everything:

```bash
./tools/validate_m0.sh
```

`validate_m0_runtime.sh` is expected to fail non-zero if Java 25, Maven, or Unity 6000.3.2f1 are unavailable. That result is `UNVERIFIED_ENVIRONMENT`, never PASS.
