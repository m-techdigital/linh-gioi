# Linh Giới Online — M2 Runtime Candidate Local Commands v0.6.2

This runbook is the owner-side command set for the hardened M2 runtime candidate.
It keeps the workflow to one real local run after source stabilizes.

## 1. Update local project

Download `linh-gioi-m2-runtime-candidate-v0.6.2-full-source.zip` to `~/Downloads`,
then unzip it into your local repo root or replace your current M2 candidate root.
Close Unity before running the evidence command.

## 2. One-command local candidate run

```bash
cd "$HOME/Projects/LinhGioiOnline"
./tools/run_m2_local_runtime_once.sh
```

Optional output directory:

```bash
./tools/run_m2_local_runtime_once.sh --output-dir "$HOME/Projects/LinhGioiOnline-M2-Evidence/dist"
```

## 3. Required final marker

```text
M2_LOCAL_RUNTIME_CANDIDATE_READY
```

Do not upload runtime evidence for closure from a run that ends with:

```text
M2_LOCAL_RUNTIME_CANDIDATE_PARTIAL
```

`PARTIAL` means a diagnostic skip flag was used. It is useful for debugging but
cannot close M2 runtime.

## 4. What v0.6.2 proves

- source validation passes before local Unity generated outputs are produced;
- Java 25/Maven server tests pass;
- Python online-session smoke proves first movement, duplicate-sequence
  idempotence, and second authoritative movement;
- Unity EditMode includes M2 serialization and client movement validation tests;
- Unity evidence bundle includes a Linux player for sandbox replay;
- upload manifest is written only for the produced evidence files.

## 5. Upload manifest

After `M2_LOCAL_RUNTIME_CANDIDATE_READY`, upload exactly the files listed in:

```text
build/m2-local-runtime-candidate/UPLOAD-THESE-FILES-M2-RUNTIME-CANDIDATE.txt
```

## 6. If it fails

Upload or paste only:

```text
build/m2-local-runtime-candidate/m2-local-runtime-once.log
```

Do not rerun repeatedly unless the failing dependency was fixed.
