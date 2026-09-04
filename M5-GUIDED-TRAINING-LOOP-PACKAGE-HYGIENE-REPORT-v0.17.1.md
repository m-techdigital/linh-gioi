# M5 Guided Training Loop Package Hygiene Report v0.17.1

Final decision: `M5_GUIDED_TRAINING_LOOP_PACKAGE_HYGIENE_CLOSED_v0.17.1`

Baseline: `lgo-m5-guided-training-loop-closed-local-v0.17.0`

## Root Cause

The v0.17.0 packages were produced by ad hoc local ZIP creation instead of a shared package tool. That allowed `.DS_Store` files and disposable Unity generated output under `client/Unity/Assets/Game/Generated/**` into the full-source ZIP.

## Fix

- Added `tools/package_source.py` with standard source package exclusions.
- Added `tools/validate_package_hygiene.py`.
- Integrated package hygiene validation into `tools/lgo_playable_closure_check.sh --package-ready`.

## Non-Claims

No gameplay, UI redesign, art change, protocol change, GameData schema change, production auth, DB persistence, full combat, inventory, economy, guild, chat, market, party, live ops, final production UI, or final production art is claimed.
