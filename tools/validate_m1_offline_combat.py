#!/usr/bin/env python3
from __future__ import annotations
import json
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []

REQUIRED_FILES = [
    'client/Unity/Assets/Game/Combat/Runtime/OfflineCombatSimulator.cs',
    'client/Unity/Assets/Game/Combat/Runtime/GameDataCombatCatalog.cs',
    'client/Unity/Assets/Game/Combat/Runtime/M1OfflineCombatScenario.cs',
    'client/Unity/Assets/Game/Combat/Runtime/OfflineCombatSmokeRunner.cs',
    'client/Unity/Assets/Game/Combat/Runtime/M1OfflineCombatPrototypeController.cs',
    'client/Unity/Assets/Game/CombatUI/LinhGioi.CombatUI.asmdef',
    'client/Unity/Assets/Game/CombatUI/Runtime/OfflineCombatHudView.cs',
    'client/Unity/Assets/Game/CombatUI/Runtime/OfflineCombatHudController.cs',
    'client/Unity/Assets/Game/Tests/EditMode/M1OfflineCombatTests.cs',
    'client/Unity/Assets/Game/Tests/EditMode/M1OfflineCombatHudTests.cs',
    'docs/tasks/M1-OFFLINE-COMBAT-PROTOTYPE.md',
    'docs/execution/prompts/P7-M1-OFFLINE-COMBAT-PROTOTYPE.md',
]

FORBIDDEN_M1_PATTERNS = [
    ('protocol/', 'M1 source validator must not require protocol edits'),
    ('server/', 'M1 offline combat must not require server production edits'),
]


def read(path: str) -> str:
    p = ROOT / path
    if not p.exists():
        errors.append(f'missing: {path}')
        return ''
    return p.read_text(encoding='utf-8')


def require_contains(label: str, content: str, needle: str) -> None:
    if needle not in content:
        errors.append(f'{label} missing required marker: {needle}')


def require_regex(label: str, content: str, pattern: str) -> None:
    if not re.search(pattern, content, re.MULTILINE):
        errors.append(f'{label} missing required pattern: {pattern}')


def validate_asmdef(path: str, required_refs: list[str]) -> None:
    p = ROOT / path
    if not p.exists():
        errors.append(f'missing asmdef: {path}')
        return
    data = json.loads(p.read_text(encoding='utf-8'))
    refs = data.get('references', [])
    for ref in required_refs:
        if ref not in refs:
            errors.append(f'{path} missing asmdef reference: {ref}')


def main() -> int:
    for rel in REQUIRED_FILES:
        if not (ROOT / rel).exists():
            errors.append(f'missing: {rel}')

    simulator = read('client/Unity/Assets/Game/Combat/Runtime/OfflineCombatSimulator.cs')
    require_contains('OfflineCombatSimulator', simulator, 'BasicAttackActionId')
    require_contains('OfflineCombatSimulator', simulator, 'RejectedInvalidRequest')
    require_contains('OfflineCombatSimulator', simulator, 'RejectedCooldown')
    require_contains('OfflineCombatSimulator', simulator, 'IsSupportedKind')
    require_contains('OfflineCombatSimulator', simulator, 'RejectedOutOfRange')
    require_contains('OfflineCombatSimulator', simulator, 'CombatActionStatus.Victory')
    require_regex('OfflineCombatSimulator', simulator, r'Mathf\.RoundToInt\(source\.attackPower \* coefficient\)')

    catalog = read('client/Unity/Assets/Game/Combat/Runtime/GameDataCombatCatalog.cs')
    require_contains('GameDataCombatCatalog', catalog, 'FromCompiledManifestJson')
    require_contains('GameDataCombatCatalog', catalog, 'skill.sword.wind_slash')
    require_contains('GameDataCombatCatalog', catalog, 'monster.shadow.slime')
    require_contains('GameDataCombatCatalog', catalog, 'JsonUtility.FromJson')
    require_contains('GameDataCombatCatalog', catalog, 'RequireUnique')
    require_contains('GameDataCombatCatalog', catalog, 'Unsupported compiled GameData version')

    smoke = read('client/Unity/Assets/Game/Combat/Runtime/OfflineCombatSmokeRunner.cs')
    require_contains('OfflineCombatSmokeRunner', smoke, '--lgo-m1-offline-combat-smoke')
    require_contains('OfflineCombatSmokeRunner', smoke, '"gamedata"')
    require_contains('OfflineCombatSmokeRunner', smoke, '"compiled"')
    require_contains('OfflineCombatSmokeRunner', smoke, '"gamedata-manifest.json"')
    require_contains('OfflineCombatSmokeRunner', smoke, 'M1OfflineCombatScenario.RunDeterministicDuel')

    bootstrap = read('client/Unity/Assets/Game/Bootstrap/Runtime/GameBootstrap.cs')
    require_contains('GameBootstrap', bootstrap, 'OfflineCombatSmokeRunner.ShouldRun()')

    tests = read('client/Unity/Assets/Game/Tests/EditMode/M1OfflineCombatTests.cs')
    for name in [
        'CatalogReadsSkillAndMonsterFromCompiledGameData',
        'WindSlashDealsDeterministicGameDataDrivenDamage',
        'SkillCooldownRejectsEarlyRepeatWithoutChangingHp',
        'OutOfRangeActionIsRejected',
        'CatalogRejectsDuplicateSkillIds',
        'CatalogRejectsMissingDefaultSkill',
        'InvalidSkillRequestIsRejectedWithoutChangingHp',
        'DeterministicDuelDefeatsStarterMonster',
    ]:
        require_contains('M1OfflineCombatTests', tests, name)

    hud = read('client/Unity/Assets/Game/CombatUI/Runtime/OfflineCombatHudView.cs')
    require_contains('OfflineCombatHudView', hud, 'HealthBar')
    require_contains('OfflineCombatHudView', hud, 'SkillButton')
    require_contains('OfflineCombatHudView', hud, 'BindResult')

    generator = read('client/Unity/Assets/Game/Foundation/Editor/M1OfflineCombatProjectGenerator.cs')
    require_contains('M1OfflineCombatProjectGenerator', generator, 'CreateDefaultCamera')
    require_contains('M1OfflineCombatProjectGenerator', generator, 'CreateDefaultLight')
    require_contains('M1OfflineCombatProjectGenerator', generator, 'Main Camera')
    require_contains('M1OfflineCombatProjectGenerator', generator, 'MainCamera')
    require_contains('M1OfflineCombatProjectGenerator', generator, 'Directional Light')
    require_contains('M1OfflineCombatProjectGenerator', generator, 'AudioListener')

    validate_asmdef('client/Unity/Assets/Game/Combat/LinhGioi.Combat.asmdef', ['LinhGioi.Foundation', 'LinhGioi.GameData'])
    validate_asmdef('client/Unity/Assets/Game/CombatUI/LinhGioi.CombatUI.asmdef', ['LinhGioi.Combat', 'LinhGioi.UI', 'UnityEngine.UIElementsModule'])
    validate_asmdef('client/Unity/Assets/Game/Bootstrap/LinhGioi.Bootstrap.asmdef', ['LinhGioi.Combat'])
    validate_asmdef('client/Unity/Assets/Game/Tests/EditMode/LinhGioi.Tests.EditMode.asmdef', ['LinhGioi.Combat', 'LinhGioi.CombatUI'])

    prompt = read('docs/execution/prompts/P7-M1-OFFLINE-COMBAT-PROTOTYPE.md')
    require_contains('M1 prompt', prompt, 'M0_RUNTIME_CLOSED')
    require_contains('M1 prompt', prompt, 'No protocol/schema changes')
    require_contains('M1 prompt', prompt, 'offline only')

    runtime_doc = read('docs/execution/M1-RUNTIME-EVIDENCE.md')
    require_contains('M1 runtime evidence', runtime_doc, '--lgo-m1-offline-combat-smoke')
    require_contains('M1 runtime evidence', runtime_doc, 'M1_OFFLINE_COMBAT_RUNTIME_CLOSED')

    closure_checklist = read('docs/execution/checklists/M1-RUNTIME-CLOSURE-CHECKLIST.md')
    require_contains('M1 runtime closure checklist', closure_checklist, 'Unity EditMode')
    require_contains('M1 runtime closure checklist', closure_checklist, 'Offline combat smoke')

    project_state = read('docs/execution/PROJECT-STATE.md')
    require_contains('PROJECT-STATE', project_state, 'M1 Offline Combat Prototype')
    require_contains('PROJECT-STATE', project_state, 'M1_OFFLINE_COMBAT_RUNTIME_CLOSED')

    # Hygiene: no checked-in Unity generated output is allowed.
    generated = ROOT / 'client/Unity/Assets/Game/Generated'
    if generated.exists() and any(p.is_file() for p in generated.rglob('*')):
        errors.append('client/Unity/Assets/Game/Generated contains disposable generated files; clean before packaging')

    if errors:
        print('M1 OFFLINE COMBAT STATIC VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M1 OFFLINE COMBAT STATIC VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
