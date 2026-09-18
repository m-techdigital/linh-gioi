# 12 — Content ID Registry v0.1

Reserved Founder Alpha namespaces and starter IDs. This registry also records the distinction between GameData content IDs and current product/runtime identity IDs; see `docs/LGO-PRODUCT-BIBLE-v2.md`.

## Maps
- `map.city.linh_thanh`
- `map.field.mist_forest`
- `map.field.spirit_river`
- `map.dungeon.shadow_gate`

## Product/account class identity IDs
Canonical new-write identity IDs:
- `vo`
- `kiem`
- `phap`
- `co`
- `linh`

Legacy durable compatibility IDs already used by GameData/persistence:
- `class.sword` → `kiem`
- `class.martial` → `vo`

Founder Alpha completes combat content first for `vo` and `kiem`; that staging does not remove the other canonical identities. Existing legacy values are not silently rewritten.


## Runtime playable map IDs
- `map-01a-cong-dong-lam` — concrete Map01A runtime-state ID shared by server persistence and Unity entry mapping.

`map.city.linh_thanh` and `map-01a-cong-dong-lam` are different ID kinds and are **not aliases**. Local authored key `dong-mon` is resource-local and must not be promoted to a cross-system persisted map ID without a contract change. Any future zone→map relationship must be explicit data.

## Spirits
- `spirit.fox.ember`
- `spirit.turtle.jade`
- `spirit.bird.storm`

## World Event
- `event.world.shadow_invasion`

## Initial sample content IDs
- `skill.sword.wind_slash`
- `item.weapon.sword.iron_01`
- `monster.shadow.slime`
- `boss.shadow_gate.guardian`

## Registry rule

Once data has reached a shared/integrated baseline, IDs are durable. Semantic rename requires a migration/alias decision rather than silent renaming.
