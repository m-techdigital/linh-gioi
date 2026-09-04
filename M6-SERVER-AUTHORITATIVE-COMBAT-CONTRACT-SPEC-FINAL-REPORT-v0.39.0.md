# M6 Server Authoritative Combat Contract Spec Final Report v0.39.0

Final decision: `M6_SERVER_COMBAT_CONTRACT_SPEC_ACCEPTED_v0.39.0`

## Result

Created the server-authoritative combat contract spec, design notes, contract change request, v0.40 implementation prompt, and validator.

## Contract Summary

Future combat must follow client intent / server authority separation:

- client sends combat intent/input;
- server validates;
- server computes accepted outcome later;
- client displays predicted/local feedback separately.

Required future protocol messages: `CombatIntent`, `CombatAccepted`, `CombatRejected`, `CombatResult`, `CombatStateSnapshot`.

Required future GameData schema areas: skill activation, cooldown, targeting rule, effect rule, telegraph rule.

## Non-Claims

- No protocol change.
- No GameData schema change.
- No server combat implementation.
- No production combat.
- No auth, DB, inventory, economy, social, or live ops.
