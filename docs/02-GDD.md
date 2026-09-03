# 02 — Game Design Document v0.1

## 1. Core loop

### Moment-to-moment
Move -> Attack -> Dodge -> Skill -> Reaction -> Loot.

### Session loop
Quest / field event -> combat -> loot -> city -> upgrade / social / trade -> next activity.

### Long loop
Build -> relationships -> guild -> collection -> housing identity -> reputation -> new content.

## 2. Founder Alpha world

### `map.city.linh_thanh`
Primary social hub and signature world-event battleground.

Must eventually contain:
- central plaza;
- market district;
- café/social landmark;
- crafting district;
- guild access;
- housing access;
- fishing/social edge;
- world-event portal anchors.

### `map.field.mist_forest`
Intro field emphasizing readable melee combat.

### `map.field.spirit_river`
Second field emphasizing movement, ranged hazards and gathering hooks.

### `map.dungeon.shadow_gate`
First 4-player dungeon, final boss teaching break/telegraph mechanics.

## 3. Paths/classes

Founder Alpha supports:

### `class.sword`
- fantasy: agile sword fighter;
- strengths: mobility, crit, combo flow;
- skill ceiling: animation cancel / perfect dodge follow-up.

### `class.martial`
- fantasy: close-range martial fighter;
- strengths: counter, stagger, pressure;
- skill ceiling: timing and resource rhythm.

Future paths such as `class.arcane`, `class.tech`, `class.spirit` are reserved but out of current scope.

## 4. Combat loadout

Per character:
- basic attack chain;
- dodge;
- four active skills;
- one ultimate;
- one spirit skill.

Boss content must not be reducible to standing still and damage-spamming.

## 5. Spirit companions

Founder Alpha target IDs:
- `spirit.fox.ember` — mobility/crit leaning;
- `spirit.turtle.jade` — shield/counter leaning;
- `spirit.bird.storm` — attack-speed/chain effect leaning.

Spirits change build behavior. Cosmetic variants may monetize appearance; canonical combat availability cannot depend on paid-only acquisition.

## 6. Progression

Founder Alpha vertical progression:
- character level 1–20;
- equipment tier;
- skill ranks with bounded modifiers.

Horizontal progression:
- spirit collection;
- achievement;
- profile titles;
- cosmetic collection;
- housing trophy display.

## 7. Social

Minimum Founder Alpha social graph:
- nearby presence;
- chat;
- friend;
- party;
- guild-lite;
- profile;
- home visit.

## 8. Economy

Initial flow:
Monster/gathering -> material -> craft -> equipment/consumable -> marketplace -> combat/social use.

Server records currency mutations in an auditable ledger. Marketplace trades use escrow semantics.

## 9. Signature event

### `event.world.shadow_invasion`
Vietnamese display name: **Âm Giới Xâm Lăng**.

State machine:
1. scheduled;
2. warning;
3. invasion_open;
4. wave_defense;
5. boss;
6. success / failure;
7. rewards;
8. restoration.

Contribution must support more than raw DPS in later iterations. Founder Alpha may start with combat + objective contribution.

## 10. Monetization constitution

Allowed direction:
- fashion;
- weapon appearance;
- mount/spirit appearance;
- housing decoration;
- emotes;
- profile cosmetics;
- cosmetic-oriented season pass;
- bounded QoL.

Forbidden direction:
- paid-only best-in-slot power;
- paid-only PvP stat advantage;
- paid currency directly becoming unlimited tradable gold.
