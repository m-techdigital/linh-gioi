# M6 Combat GameData Balance Adversarial v0.54.0

Status: M6_COMBAT_GAMEDATA_BALANCE_SOURCE_READY_v0.54.0

## Scope

Harden the M6 combat foundation by validating existing GameData values and adversarial failure cases without changing schemas, protocol, or runtime combat mechanics.

## Implemented

- Added a v0.54 source validator for combat-related GameData balance checks.
- Verified current `skill.sword.wind_slash` cooldown, range, target requirement, placeholder amount, and coefficient stay inside documented M6 development bounds.
- Verified current monster/dummy data has valid HP/readiness values within the existing monster schema.
- Added adversarial fixture checks in temporary copies of `gamedata/`:
  - duplicate combat skill ID rejected;
  - missing class reference rejected;
  - negative cooldown rejected;
  - invalid range rejected;
  - invalid target rule rejected;
  - effect amount outside schema bound rejected;
  - invalid monster HP rejected;
  - deterministic manifest remains stable for unchanged data.

## Non-Claims

- No production balance claim.
- No new combat mechanic.
- No enemy AI.
- No loot, reward, economy, inventory, DB, auth, social, or live ops.
- No protocol/schema/ADR/design-token change.

