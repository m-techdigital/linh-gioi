# M6 Combat Protocol + GameData Contract Final Report v0.40.0

Final decision: M6_COMBAT_PROTOCOL_GAMEDATA_CONTRACT_ACCEPTED_v0.40.0.

Root cause: v0.39.0 intentionally stopped at a contract proposal, so canonical protobuf/GameData support for server-authoritative combat did not yet exist.

Implemented:

- Added canonical combat intent/accepted/rejected/result/snapshot protobuf messages.
- Added skill activation, cooldown, targeting, effect placeholder, and telegraph/readability placeholder GameData rules.
- Added invalid GameData regression tests for combat nested rules.
- Added a v0.40 validator and narrow v0.40 awareness to legacy M6 governance validators.
- Removed tracked GameData test bytecode cache from source status.

Non-claims:

- No Java server combat validation implementation.
- No Unity combat intent client integration.
- No production combat, auth, DB, inventory, economy, social, live ops, or production art.
- No runtime PASS is claimed by v0.40.0.
