# Linh Gioi Map/Zone Model v1.0

Marker: `LGO_ZONE_MODEL_READY`

## Purpose

The map/zone model defines how future playable spaces should be described before schema, persistence, streaming, or MMO-scale systems are opened.

## Zone Types

| Type | Purpose | Current implementation status |
|---|---|---|
| `gate_hub` | entry area, character arrival, Gate Keeper guidance | local playable shell |
| `training_ground` | non-combat training interaction | local guided loop |
| `combat_trial` | controlled target dummy/combat prototype space | M6 foundation only |
| `social_hub` | future player gathering space | not opened |
| `dungeon_instance` | future private/party combat space | not opened |
| `world_event_area` | future shared event/boss area | not opened |

## Zone Descriptor Concept

Planning-only fields:

- stable zone id;
- display name in Vietnamese;
- zone type;
- allowed interactions;
- entry/exit rule;
- camera framing intent;
- visual reference group;
- runtime smoke coverage;
- forbidden systems.

## Current Safe Reuse

- M4/M5 world shell can host `gate_hub` and `training_ground` concepts.
- M6 target dummy area can host `combat_trial` concept.
- Existing smoke runners can validate local shell behavior.

## Not Yet Open

- streaming;
- server shard/zone routing;
- persistence-backed zone state;
- party/guild/social presence;
- economy rewards;
- production content tooling.

## Future Entry Criteria

Before implementing zones:

- decide whether zone ids become GameData or server-owned config;
- create contract-change request if schemas/protocol must change;
- add validator for zone references;
- add runtime smoke for entering/exiting each implemented zone type;
- keep Vietnamese player-facing labels in UI.
