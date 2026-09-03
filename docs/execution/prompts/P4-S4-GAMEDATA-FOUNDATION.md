# P4 PROMPT — S4 GAMEDATA FOUNDATION

You are S4. Build the deterministic GameData foundation on the current accepted integration baseline.

## Required reading

- `README.md`
- `docs/01-PRODUCT-CONSTITUTION.md`
- `docs/03-TDD.md`
- `docs/05-GAMEDATA-CONTRACT.md`
- `docs/09-DEFINITION-OF-DONE.md`
- `docs/10-INTEGRATION-RULES.md`
- `docs/12-CONTENT-ID-REGISTRY.md`
- `docs/tasks/S4-GAMEDATA-FOUNDATION.md`
- all current `gamedata/**`
- `docs/execution/03-HANDOFF-CONTRACT.md`

## Goal

Turn the starter GameData examples into a deterministic, schema-validated content pipeline suitable for M1 content authoring.

## Allowed paths

- `gamedata/**` except schema semantic changes require S0 approval
- `tools/gamedata/**` if not conflicting with accepted S5 ownership
- `tests/gamedata/**`

## Forbidden paths

- production `client/**`
- production `server/**`
- `protocol/**`
- `docs/adr/**`
- silent schema semantic redesign

## Required implementation

1. Validate YAML content against existing JSON Schemas.
2. Validate global unique content IDs across applicable source categories.
3. Implement starter reference validation using the existing registry/contract rather than inventing a competing ID system.
4. Produce deterministic compiled manifest/output containing `gamedata_version` and content hash.
5. Guarantee identical canonical source input produces identical output/hash.
6. Keep useful positive fixtures.
7. Add deliberate invalid fixtures for at least duplicate ID, invalid bounds/schema, and invalid reference once reference wiring exists.
8. Diagnostics must identify source file and actionable ID/field.
9. Do not expand into full balance/economy design.

## Acceptance

- current valid samples pass;
- duplicate ID fails;
- malformed/bounds-invalid skill fails;
- invalid reference fails;
- deterministic repeat produces same compiled output/hash;
- failures return non-zero;
- no contract/schema semantic drift.
