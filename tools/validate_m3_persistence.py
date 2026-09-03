#!/usr/bin/env python3
from __future__ import annotations
import json
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []

REQUIRED_FILES = [
    'server/api/src/main/java/com/linhgioi/server/api/persistence/AccountProfile.java',
    'server/api/src/main/java/com/linhgioi/server/api/persistence/CharacterProfile.java',
    'server/api/src/main/java/com/linhgioi/server/api/persistence/JsonFilePlayerProfileStore.java',
    'server/api/src/main/java/com/linhgioi/server/api/persistence/PlayerProfileStore.java',
    'server/api/src/main/java/com/linhgioi/server/api/persistence/PersistenceConfiguration.java',
    'server/api/src/main/java/com/linhgioi/server/api/account/AccountCharacterController.java',
    'server/api/src/test/java/com/linhgioi/server/api/persistence/JsonFilePlayerProfileStoreTest.java',
    'server/api/src/test/java/com/linhgioi/server/api/account/AccountCharacterControllerTest.java',
    'server/scripts/m3-persistence-smoke.py',
    'tools/run_m3_api_persistence_once.sh',
    'tools/m3_api_persistence_runtime.py',
    'tools/validate_m3_source.sh',
    'docs/tasks/M3-ACCOUNT-CHARACTER-PERSISTENCE.md',
    'docs/execution/M3-PERSISTENCE-EVIDENCE.md',
    'docs/execution/checklists/M3-PERSISTENCE-CLOSURE-CHECKLIST.md',
    'docs/execution/prompts/P10-M3-ACCOUNT-CHARACTER-PERSISTENCE.md',
    'm3-manifest.json',
]


def read(path: str) -> str:
    target = ROOT / path
    if not target.exists():
        errors.append(f'missing: {path}')
        return ''
    return target.read_text(encoding='utf-8')


def require_contains(label: str, content: str, needle: str) -> None:
    if needle not in content:
        errors.append(f'{label} missing marker: {needle}')


def require_regex(label: str, content: str, pattern: str) -> None:
    if not re.search(pattern, content, re.MULTILINE):
        errors.append(f'{label} missing pattern: {pattern}')


def main() -> int:
    for rel in REQUIRED_FILES:
        if not (ROOT / rel).exists():
            errors.append(f'missing: {rel}')

    store = read('server/api/src/main/java/com/linhgioi/server/api/persistence/JsonFilePlayerProfileStore.java')
    for marker in [
        'SCHEMA_VERSION = 1',
        'players-v1.json',
        'StandardCopyOption.ATOMIC_MOVE',
        'loginDev',
        'createCharacter',
        'saveCharacterPosition',
        'unsupported player persistence schema version',
        'raw dev key',
        'MessageDigest.getInstance("SHA-256")',
    ]:
        require_contains('JsonFilePlayerProfileStore', store, marker)
    require_regex('JsonFilePlayerProfileStore', store, r'account\.dev\.')
    require_regex('JsonFilePlayerProfileStore', store, r'character\.')

    controller = read('server/api/src/main/java/com/linhgioi/server/api/account/AccountCharacterController.java')
    for marker in [
        '/dev/auth/login',
        '/accounts/{accountId}/characters',
        '/characters/{characterId}',
        '/characters/{characterId}/position',
        'HttpStatus.BAD_REQUEST',
        'HttpStatus.NOT_FOUND',
    ]:
        require_contains('AccountCharacterController', controller, marker)

    tests = read('server/api/src/test/java/com/linhgioi/server/api/persistence/JsonFilePlayerProfileStoreTest.java')
    for marker in [
        'devLoginCreatesStableAccountWithoutPersistingRawDevKey',
        'createsListsSavesAndReloadsCharacter',
        'rejectsInvalidCreateAndPositionRequestsWithoutCorruptingStore',
        'rejectsUnsupportedFutureSchemaVersion',
    ]:
        require_contains('JsonFilePlayerProfileStoreTest', tests, marker)

    integration = read('server/api/src/test/java/com/linhgioi/server/api/account/AccountCharacterControllerTest.java')
    for marker in [
        'devLoginCreateCharacterSavePositionAndRejectInvalidRequestsOverController',
        'ResponseStatusException',
    ]:
        require_contains('AccountCharacterControllerTest', integration, marker)

    smoke = read('server/scripts/m3-persistence-smoke.py')
    for marker in [
        'M3_PERSISTENCE_SMOKE_PASS',
        '--expect-existing',
        '/dev/auth/login',
        '/position',
    ]:
        require_contains('m3-persistence-smoke.py', smoke, marker)

    runner = read('tools/run_m3_api_persistence_once.sh')
    runtime_helper = read('tools/m3_api_persistence_runtime.py')
    runner_runtime = runner + '\n' + runtime_helper
    for marker in [
        'M3_API_PERSISTENCE_RUNTIME_SMOKE_PASS',
        'LG_API_PERSISTENCE_DIR',
        '--expect-existing',
        'raw dev key leaked',
        'UPLOAD-THESE-FILES-M3-PERSISTENCE',
    ]:
        require_contains('M3 runtime runner/helper', runner_runtime, marker)

    manifest_content = read('m3-manifest.json')
    try:
        manifest = json.loads(manifest_content)
    except json.JSONDecodeError as exc:
        errors.append(f'm3-manifest.json invalid JSON: {exc}')
    else:
        serialized = json.dumps(manifest, sort_keys=True)
        for marker in ['M3_ACCOUNT_CHARACTER_PERSISTENCE_SOURCE_READY', 'M2 owner override', 'players-v1.json']:
            if marker not in serialized:
                errors.append(f'm3-manifest.json missing marker: {marker}')

    project_state = read('docs/execution/PROJECT-STATE.md')
    require_contains('PROJECT-STATE', project_state, 'M3 Account / Character Persistence')
    require_contains('PROJECT-STATE', project_state, 'OWNER_OVERRIDE_FROM_M2_RUNTIME_CANDIDATE')

    if errors:
        print('M3 PERSISTENCE STATIC VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M3 PERSISTENCE STATIC VALIDATION PASS')
    return 0


if __name__ == '__main__':
    sys.exit(main())
