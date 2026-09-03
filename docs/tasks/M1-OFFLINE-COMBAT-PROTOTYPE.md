# M1 — Offline Combat Prototype

## Status

`M1_OFFLINE_COMBAT_PROTOTYPE_SOURCE_READY`

## Entry

M1 starts only after M0 final decision is `M0_RUNTIME_CLOSED`.

Accepted baseline for this task:

- `linh-gioi-m0-runtime-closed-v0.4.1-full-source.zip`

## Goal

Create the first deterministic offline combat slice in Unity without opening online session, persistence, economy, guild, marketplace, or broad content production.

M1 proves:

1. Unity-side combat state can be simulated deterministically.
2. Existing GameData skill/monster values can drive combat math.
3. A minimal prototype HUD can display player/enemy health, skill action, and combat result.
4. M0 protocol/server runtime remains untouched and regresses cleanly.

## Allowed scope

- `client/Unity/Assets/Game/Combat/**`
- `client/Unity/Assets/Game/CombatUI/**`
- minimal bootstrap wiring for an offline smoke command
- Unity EditMode tests for deterministic offline combat
- M1 documentation/checklists/prompts under `docs/**`
- validation tooling under `tools/**`

## Forbidden scope

- No protocol/schema changes.
- No server production behavior changes.
- No account/character persistence.
- No database/Redis work.
- No guild, marketplace, monetization, or economy feature.
- No broad content expansion beyond consuming existing M0 starter fixtures.
- No hand-written replacement for generated protobuf DTOs.

## Implemented prototype shape

```text
GameData compiled manifest
        |
        v
GameDataCombatCatalog
        |
        v
M1OfflineCombatScenario
        |
        v
OfflineCombatSimulator
        |
        +-- deterministic damage/cooldown/range/victory tests
        +-- OfflineCombatSmokeRunner command-line evidence
        +-- OfflineCombatHudView prototype UI binding
```

## Acceptance gates

- [ ] M0 regression source validation PASS.
- [ ] Unity project imports/compiles on Unity `6000.3.2f1`.
- [ ] M1 EditMode combat tests execute with count > 0.
- [ ] `CatalogReadsSkillAndMonsterFromCompiledGameData` PASS.
- [ ] `WindSlashDealsDeterministicGameDataDrivenDamage` PASS.
- [ ] `SkillCooldownRejectsEarlyRepeatWithoutChangingHp` PASS.
- [ ] `OutOfRangeActionIsRejected` PASS.
- [ ] `DeterministicDuelDefeatsStarterMonster` PASS.
- [ ] Prototype HUD test PASS.
- [ ] Catalog rejects duplicate/missing canonical combat GameData IDs.
- [ ] Invalid combat requests reject without mutating HP.
- [ ] Optional player smoke command writes JSON result with `status=PASS`.
- [ ] M1 runtime evidence verifier PASS.
- [ ] Frozen contract audit PASS.

## Runtime command

When Unity player/editor evidence is available, run with:

```bash
--lgo-m1-offline-combat-smoke \
--lgo-gamedata-manifest /absolute/path/to/gamedata/compiled/gamedata-manifest.json \
--lgo-m1-result /absolute/path/to/lgo-m1-offline-combat-result.json
```

Expected result includes:

```json
{
  "status": "PASS",
  "result": {
    "targetDefeated": true,
    "skillId": "skill.sword.wind_slash",
    "enemyContentId": "monster.shadow.slime"
  }
}
```

## Handoff rule

M1 can be source-ready from sandbox static validation, but must not be called fully runtime-closed until Unity `6000.3.2f1` imports, compiles, runs EditMode tests, and produces runtime evidence for the offline combat smoke path.
