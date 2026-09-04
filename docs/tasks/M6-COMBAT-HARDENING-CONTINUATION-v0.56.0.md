# M6 Combat Hardening Continuation v0.56.0

Marker: `M6_COMBAT_HARDENING_CONTINUATION_SOURCE_READY_v0.56.0`

## Scope

This task hardens the existing M6 combat foundation by improving diagnostic evidence emitted by Unity smoke runners. It does not add gameplay.

## Current Hardening Target

Runtime evidence now records accepted intent identity, sequence, cooldown, result outcome, snapshot validity, rejection codes, retryable flags, and cooldown remaining values for existing accepted and rejected combat paths.

## Non-Claims

- No new combat mechanics.
- No production combat.
- No production art claim.
- No protocol changes.
- No GameData schema changes.
- No auth, DB, economy, inventory, loot, social, guild, party, market, or live-ops work.

## Runtime Evidence

The local combat smoke and Unity-Java combat E2E smoke remain the authoritative runtime checks for existing M6 combat behavior. v0.56 only makes their JSON outputs easier to inspect when a gate fails.

## Next Gate

Run source/package gates, then runtime gates when Unity is available.

Package cache hygiene remains owned by `tools/validate_package_hygiene.py`; this task validator only checks the v0.56 diagnostic evidence contract and frozen source boundaries.
