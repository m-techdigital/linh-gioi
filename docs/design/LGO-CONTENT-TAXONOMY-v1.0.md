# Linh Gioi Content Taxonomy v1.0

Marker: `LGO_CONTENT_TAXONOMY_READY`

## Purpose

This taxonomy gives future content work a shared language before schemas or production content pipelines are opened.

## Top-Level Domains

| Domain | Purpose | Current status |
|---|---|---|
| `world` | zones, hubs, gates, landmarks, training objects | local playable shell only |
| `character` | player avatars, classes, NPCs, enemies | placeholder/candidate art only |
| `dialogue` | NPC lines, tutorial prompts, local guidance | lightweight local dialogue only |
| `combat` | skills, targets, cooldowns, hit feedback | M6 foundation only |
| `progression` | cultivation stages, training objectives, unlocks | not production implemented |
| `item` | future items/materials/equipment | not opened |
| `event` | future world events/boss events/live content | not opened |
| `ui` | screens, HUD, panels, icon roles | playable shell plus visual candidates |

## Naming Pattern

Use stable ids:

```text
<domain>.<subdomain>.<specific_name>
```

Examples:

- `world.hub.spirit_gate`
- `world.training.stone_basic`
- `dialogue.gate_keeper.intro`
- `combat.skill.wind_slash`
- `combat.target.training_dummy`

## Rules

- Taxonomy ids are planning identifiers, not protocol fields.
- Do not create parallel DTOs or schemas from this document.
- Do not mutate `gamedata/schemas/**` from taxonomy work.
- Player-facing text remains Vietnamese in runtime UI.
- Reference art and runtime candidate art must stay classified by provenance.

## Future Schema Entry Criteria

Before turning taxonomy into GameData schema work:

- create a scoped GameData contract-change request;
- identify owning runtime systems;
- define positive and negative validators;
- prove migration/backward-compatibility expectations;
- update docs without breaking existing M0-M6 gates.
