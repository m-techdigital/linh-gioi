# M6 Combat UX Readability Polish v0.53.0

Status: `M6_COMBAT_UX_READABILITY_POLISH_SOURCE_READY_v0.53.0`

## Scope

Polish the existing local/server combat placeholder presentation so the player can read target, range, cooldown, reject reason, hit feedback, and recovery state. No new combat mechanics were added.

## Implemented

- Added a Vietnamese target range line to the combat HUD.
- Clarified local reject copy for no target, out-of-range, and cooldown states.
- Clarified hit copy for the existing Wind Slash prototype result.
- Added UI accent updates for ready, cooldown, warning, and reject states.
- Kept existing runtime placeholder assets under `CombatPlaceholders`.

## Non-Claims

- No production combat.
- No production art.
- No protocol changes.
- No GameData schema changes.
- No enemy AI, loot, reward, inventory, economy, auth, DB, social, or live ops.
