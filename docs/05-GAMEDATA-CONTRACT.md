# 05 — GameData Contract v1

## Goals

Game designers/content lanes must tune ordinary gameplay without modifying C# or Java source.

## ID format

Lowercase dot-separated semantic IDs:

- `class.sword`
- `skill.sword.wind_slash`
- `item.weapon.sword.iron_01`
- `monster.shadow.slime`
- `boss.shadow_gate.guardian`
- `event.world.shadow_invasion`

IDs are durable contracts. Rename requires migration, not search/replace.

## Schema policy

Every GameData document contains:
- `schema_version`;
- `id`;
- type-specific required fields.

Validation rejects:
- unknown/duplicate IDs;
- missing required fields;
- illegal values;
- invalid references;
- non-deterministic duplicate definitions.

## Ownership

S4 owns content values. S0 owns schema changes. S1/S2 consume compiled data and may not silently add private copies.

## Initial schemas

M0 includes:
- skill schema;
- item schema;
- monster schema;
- world-event schema.

Quest/recipe/loot schemas enter in the milestone that first needs them.
