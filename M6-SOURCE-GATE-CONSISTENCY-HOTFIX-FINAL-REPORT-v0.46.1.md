# M6 Source/Gate Consistency Hotfix Final Report v0.46.1

Final decision: `M6_SOURCE_GATE_CONSISTENCY_HOTFIX_CLOSED_v0.46.1`

## Scope

This hotfix makes the v0.46 source and package gates reproducible from a fresh full-source unzip. It does not add gameplay, combat mechanics, production art, DB/auth, economy, social, or live-ops scope.

## Root Cause

- Source/package validators still coupled normal source validation to historical release ZIP presence in the repo root.
- The playable closure orchestration did not globally prevent Python bytecode writes, so validator order could be contaminated by `__pycache__` output.
- `M2-RUNTIME-CANDIDATE-LOCAL-COMMANDS-v0.6.2.md` existed locally but was not tracked, so fresh full-source unzip lost the M2 local command markers.

## Fix

- `tools/lgo_playable_closure_check.sh` now exports `PYTHONDONTWRITEBYTECODE=1`.
- `tools/validate_package_hygiene.py` validates source hygiene without requiring historical ZIPs and supports explicit artifact ZIP validation through `--artifact-zip`.
- `tools/validate_m6_package_hygiene_hotfix.py` keeps source validation independent from historical ZIPs and requires v0.44.1 artifacts only in `--artifact-mode`.
- `tools/validate_m6_server_authoritative_combat_closure.py` keeps source validation independent from the historical v0.44.0 artifact checksum manifest and requires it only in `--artifact-mode`.
- `M2-RUNTIME-CANDIDATE-LOCAL-COMMANDS-v0.6.2.md` is included in the source package and carries `M2_LOCAL_RUNTIME_CANDIDATE_READY` / `M2_LOCAL_RUNTIME_CANDIDATE_PARTIAL` guidance.

## Validation

- `git --no-pager diff --check`: PASS.
- `python3.12 -m py_compile ...`: PASS.
- `python3.12 tools/validate_m2_online_session.py`: PASS.
- `python3.12 tools/validate_m6_runtime_usable_combat_asset_pack.py`: PASS.
- `python3.12 tools/validate_m6_unity_combat_placeholder_asset_import.py`: PASS.
- `python3.12 tools/validate_package_hygiene.py`: PASS.
- `./tools/lgo_playable_closure_check.sh --source-only`: PASS.
- `./tools/lgo_playable_closure_check.sh --package-ready`: PASS.

Fresh unzip validation status: PASS after regenerated package validation.

## Non-Claims

This hotfix does not claim production art, new combat mechanics, full MMO readiness, or broader M0 runtime closure.
