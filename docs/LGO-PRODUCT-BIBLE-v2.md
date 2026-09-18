# Linh Giới Online — Product Bible v2

Status: **CURRENT PRODUCT VOCABULARY / STAGING AUTHORITY**
Scope: resolves product-level class staging and map machine-ID conflicts without reopening closed runtime/auth/persistence behavior.

## Authority boundary

`docs/01-PRODUCT-CONSTITUTION.md` remains above this document for P1–P10. This Product Bible v2 is authoritative when older Vision/GDD/content-registry prose conflicts on class identity cardinality, Founder Alpha staging, or the meaning of current world/map machine IDs. Runtime/server contracts remain authoritative for implementation details such as Map01A lane bounds and persistence format.

The companion `docs/LGO-PRODUCT-BIBLE-v2.json` is the machine-checkable form. `tools/validate_lgo_product_bible_v2.py` binds this policy back to current server, Unity and GameData sources.

## Class identity and Founder Alpha

**Founder Alpha staging is capability staging, not identity-schema staging.** The product has five canonical identities: Võ, Kiếm, Pháp, Cơ, Linh.

Canonical new-write IDs are:

`vo`, `kiem`, `phap`, `co`, `linh`.

Founder Alpha makes **Võ (`vo`) and Kiếm (`kiem`) the first two combat-complete Paths**. This does not forbid the other three canonical identities from appearing in compatible account/profile/UI/data contracts before their combat content is complete.

Historical persisted/content identifiers `class.martial` and `class.sword` stay compatible and resolve to `vo` and `kiem`. Existing rows are not silently rewritten merely to modernize vocabulary. New product/domain implementation should issue a canonical five-class ID when it owns the write. Current compatibility endpoints may continue accepting legacy values until a separately reviewed migration closes them.

Historical planning labels `class.arcane`, `class.tech`, and `class.spirit` are not canonical product identity IDs for new writes. Their modern product identities are `phap`, `co`, and `linh`; no migration alias is inferred for persisted data unless an explicit compatibility task defines one.

## Stable world/map machine-ID policy

Machine IDs are typed by purpose; similar narrative names do not make two IDs aliases.

- `map.city.linh_thanh` is a **content-zone** ID owned by GameData/world taxonomy. It can anchor events, content grouping and city semantics.
- `map-01a-cong-dong-lam` is the current **runtime-playable-map** ID persisted for Map01A lane/facing state and shared by server and Unity entry mapping.
- Local authored resource key `dong-mon` is an **authoring-local-map-key**. It is not a stable cross-system persistence/API ID.

`map.city.linh_thanh` and `map-01a-cong-dong-lam` are **not aliases**. The current narrative places the tutorial in the Linh Thành/Đông Môn opening flow, but a parent/containment relationship must be represented by explicit future data rather than inferred from names or rewritten IDs.

A durable ID that already reached shared/integrated state is never silently renamed. Any semantic rename, merge, alias or parent mapping requires a migration/compatibility decision with the owning domain.

## Supersession register

| Historical statement / stage | Current authority |
|---|---|
| “Founder Alpha has 2 Paths/classes” read as only two identity values | Five canonical identity values exist; Alpha completes combat content first for Võ + Kiếm. |
| `class.sword` / `class.martial` treated as the canonical product identity vocabulary | Canonical new product identity writes use `kiem` / `vo` (and `phap/co/linh`); legacy values remain compatible and durable where already stored/shared. |
| Future product identities described as `class.arcane/class.tech/class.spirit` | Canonical product identities are `phap/co/linh`; old planning labels do not become automatic persisted aliases. |
| `map.city.linh_thanh` and `map-01a-cong-dong-lam` treated as interchangeable | They are different machine-ID kinds and are not aliases. |
| Early M5 Shadow Slime “non-combat marker” stage | Active 2D GDD, direction lock and current tutorial micro-slice use Shadow Slime as tutorial combat. Historical non-combat wording remains stage history only. |

## Change rule

Changing the five canonical class IDs, the legacy alias map, or the meaning of either current stable map ID is a cross-system contract change. It must be posted to the shared Mission before source mutation and must preserve explicit migration/read compatibility. This document does not itself authorize database, API, protocol or gameplay expansion.
