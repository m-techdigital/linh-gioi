# P7 PROMPT — M1 OFFLINE COMBAT PROTOTYPE

You are implementing Linh Giới Online M1 after M0 runtime closure.

## Preconditions

- M0 final decision must be `M0_RUNTIME_CLOSED`.
- Baseline must be `linh-gioi-m0-runtime-closed-v0.4.1-full-source.zip` or a verified successor.
- Do not start from pre-v0.4.1 runtime-limited artifacts.

## Required reading

Read completely before changing source:

- `README.md`
- `START-HERE.md`
- `docs/01-PRODUCT-CONSTITUTION.md`
- `docs/02-GDD.md`
- `docs/03-TDD.md`
- `docs/05-GAMEDATA-CONTRACT.md`
- `docs/06-UI-DESIGN-SYSTEM.md`
- `docs/09-DEFINITION-OF-DONE.md`
- `docs/10-INTEGRATION-RULES.md`
- `docs/12-CONTENT-ID-REGISTRY.md`
- `docs/execution/PROJECT-STATE.md`
- `docs/execution/MILESTONE-ROADMAP.md`
- `docs/execution/03-HANDOFF-CONTRACT.md`
- `docs/tasks/M1-OFFLINE-COMBAT-PROTOTYPE.md`

## Goal

Build an offline only deterministic combat prototype using existing M0 Unity, GameData, and UI foundations.

## Allowed paths

- `client/Unity/Assets/Game/Combat/**`
- `client/Unity/Assets/Game/CombatUI/**`
- `client/Unity/Assets/Game/Bootstrap/**` only for minimal offline smoke entry wiring
- `client/Unity/Assets/Game/Foundation/Editor/**` only for generated prototype scene/tooling
- `client/Unity/Assets/Game/Tests/EditMode/**`
- `tools/validate_m1_*`
- `docs/tasks/M1-OFFLINE-COMBAT-PROTOTYPE.md`
- `docs/execution/**` M1 tracking docs/checklists/prompts

## Forbidden scope

- No protocol/schema changes.
- No server production behavior changes.
- No persistence, account, inventory transactions, economy, marketplace, guild, monetization, or online session systems.
- No broad content expansion.
- No generated Unity `Assets/Game/Generated/**`, `Library/**`, `Temp/**`, `Logs/**`, or build output in source delta.
- No `|| true`, skip-as-PASS, no-test PASS, or source inspection claimed as runtime PASS.

## Required implementation

1. Add a deterministic `OfflineCombatSimulator` with combatant state, basic attack, skill action, cooldown gate, range gate, damage application, defeat/victory result, and action history.
2. Add a `GameDataCombatCatalog` that consumes the compiled M0 GameData manifest and maps existing `skill.sword.wind_slash` and `monster.shadow.slime` into combat definitions.
3. Add an M1 scenario runner that performs a deterministic sword-vs-shadow-slime duel.
4. Add a minimal offline command-line smoke path: `--lgo-m1-offline-combat-smoke`.
5. Add prototype HUD view/controller under a combat-specific UI assembly; do not make base UI depend on combat.
6. Add EditMode tests for catalog parsing, duplicate/missing catalog rejection, invalid request rejection, deterministic damage, cooldown rejection, out-of-range rejection, victory, and HUD binding.
7. Add M1 static validator and regression wrapper.
8. Keep M1 runtime evidence closure separate unless Unity/runtime artifacts are actually executed.

## Required evidence

- `./tools/validate_m1_source.sh` PASS.
- M0 source regression PASS.
- Unity `6000.3.2f1` import/compile/EditMode test PASS when runtime is available.
- M1 offline combat smoke JSON `status=PASS` when player/editor runtime is available.
- M1 evidence verifier PASS before claiming runtime closure.
- Frozen contract audit confirms `protocol/**`, schema, ADR, design tokens, Product Constitution/GDD/TDD/Network/GameData contracts have no unauthorized drift.

## Handoff

Return:

1. `M1-OFFLINE-COMBAT-PROTOTYPE-REPORT.md`
2. `HANDOFF-LG-M1-OFFLINE-COMBAT-PROTOTYPE.md`
3. source delta ZIP without parent wrapper
4. full source successor if accepted source changed
5. SHA256 for all artifacts

Final decision must be one of:

- `M1_OFFLINE_COMBAT_PROTOTYPE_SOURCE_READY`
- `M1_OFFLINE_COMBAT_RUNTIME_CLOSED`
- `M1_RUNTIME_ENVIRONMENT_LIMITED`
- `FIX_REQUIRED`
- `BLOCKED_CONTRACT`
