# Handoff LG M5 Guided Training Loop Package Hygiene v0.17.1

Baseline: `lgo-m5-guided-training-loop-closed-local-v0.17.0`

Final decision: `M5_GUIDED_TRAINING_LOOP_PACKAGE_HYGIENE_CLOSED_v0.17.1`

## Verify

```bash
git --no-pager diff --check
python3.12 -m py_compile tools/package_source.py tools/validate_package_hygiene.py
python3.12 tools/validate_package_hygiene.py
./tools/lgo_playable_closure_check.sh --source-only
./tools/lgo_playable_closure_check.sh --package-ready
```

## Package Exclusions

Full-source packages exclude `.git/**`, `build/**`, `.DS_Store`, `__MACOSX/**`, Unity `Library/Temp/Logs`, Unity generated assets, and generated protocol output.
