# Handoff LG M6 Combat Protocol + GameData Contract v0.40.0

Decision: M6_COMBAT_PROTOCOL_GAMEDATA_CONTRACT_ACCEPTED_v0.40.0.

## Summary

v0.40.0 converts the accepted v0.39.0 contract request into minimal canonical protobuf and GameData source. It does not implement server validation or Unity runtime integration.

## Frozen Surface Audit

- `docs/adr/**`: unchanged.
- `client/Unity/Assets/Game/UI/design-tokens.json`: unchanged.
- Protocol/GameData changes are limited to the approved v0.40.0 contract task.
- Removed a tracked Python bytecode cache from `tests/gamedata/__pycache__` as package hygiene.

## Next Stage

Proceed to v0.41.0 Java server combat validation skeleton using generated protobuf classes and canonical GameData.
