# M6 Combat Hardening Continuation v0.56.0

Use only after `M6_COMBAT_FOUNDATION_CLOSED_LOCAL_v0.55.0`.

Branch: A.

## Goal

Harden the existing M6 combat foundation without opening new gameplay systems.

## Allowed

- Add tests, diagnostics, validation, and runtime evidence around existing combat intent/result behavior.
- Improve failure observability for accepted/rejected combat intents.
- Preserve Vietnamese player-facing copy.

## Forbidden

- No new combat mechanics.
- No protocol changes without a contract-change request.
- No GameData schema changes without a contract-change request.
- No production art claim.
- No DB, auth, economy, inventory, loot, social, guild, party, market, or live-ops work.

## Required First Step

Audit v0.49-v0.55 reports and decide the smallest hardening target before editing source.

