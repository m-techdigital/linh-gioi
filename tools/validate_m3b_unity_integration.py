#!/usr/bin/env python3
from __future__ import annotations

import json
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []

REQUIRED_FILES = [
    'client/Unity/Assets/Game/Account/LinhGioi.Account.asmdef',
    'client/Unity/Assets/Game/Account/Runtime/AccountModels.cs',
    'client/Unity/Assets/Game/Account/Runtime/AccountApiClient.cs',
    'client/Unity/Assets/Game/Account/Runtime/M3BAccountCharacterSmokeRunner.cs',
    'client/Unity/Assets/Game/Tests/EditMode/M3BAccountCharacterTests.cs',
    'tools/m3b_unity_account_character_runtime.py',
    'tools/run_m3b_unity_account_character_once.sh',
    'tools/validate_m3b_unity_integration.py',
    'tools/validate_m3b_source.sh',
    'docs/tasks/M3B-UNITY-ACCOUNT-CHARACTER-INTEGRATION.md',
    'docs/execution/M3B-UNITY-ACCOUNT-CHARACTER-EVIDENCE.md',
    'docs/execution/checklists/M3B-UNITY-ACCOUNT-CHARACTER-CLOSURE-CHECKLIST.md',
    'docs/execution/prompts/P11-M3B-UNITY-ACCOUNT-CHARACTER-INTEGRATION.md',
    'm3b-manifest.json',
]


def read(path: str) -> str:
    target = ROOT / path
    if not target.exists():
        errors.append(f'missing: {path}')
        return ''
    return target.read_text(encoding='utf-8')


def require_contains(label: str, content: str, marker: str) -> None:
    if marker not in content:
        errors.append(f'{label} missing marker: {marker}')


def require_regex(label: str, content: str, pattern: str) -> None:
    if not re.search(pattern, content, re.MULTILINE):
        errors.append(f'{label} missing pattern: {pattern}')


def validate_asmdefs() -> None:
    asmdefs = []
    names: dict[str, Path] = {}
    for path in sorted((ROOT / 'client/Unity/Assets/Game').rglob('*.asmdef')):
        if 'Generated' in path.parts:
            continue
        try:
            data = json.loads(path.read_text(encoding='utf-8'))
        except json.JSONDecodeError as exc:
            errors.append(f'{path.relative_to(ROOT)} invalid JSON: {exc}')
            continue
        name = data.get('name')
        if not name:
            errors.append(f'asmdef without name: {path.relative_to(ROOT)}')
            continue
        if name in names:
            errors.append(f'duplicate asmdef name: {name}')
        names[name] = path
        asmdefs.append((path, data))

    if 'LinhGioi.Account' not in names:
        errors.append('LinhGioi.Account asmdef missing from asmdef graph')
    bootstrap = json.loads((ROOT / 'client/Unity/Assets/Game/Bootstrap/LinhGioi.Bootstrap.asmdef').read_text(encoding='utf-8'))
    if 'LinhGioi.Account' not in bootstrap.get('references', []):
        errors.append('Bootstrap asmdef must reference LinhGioi.Account for M3B smoke entrypoint')
    tests = json.loads((ROOT / 'client/Unity/Assets/Game/Tests/EditMode/LinhGioi.Tests.EditMode.asmdef').read_text(encoding='utf-8'))
    if 'LinhGioi.Account' not in tests.get('references', []):
        errors.append('EditMode tests asmdef must reference LinhGioi.Account')

    graph = {data['name']: [r for r in data.get('references', []) if r in names] for _, data in asmdefs if data.get('name')}
    visiting: set[str] = set()
    visited: set[str] = set()

    def visit(node: str, stack: list[str]) -> None:
        if node in visiting:
            errors.append('asmdef cycle: ' + ' -> '.join(stack + [node]))
            return
        if node in visited:
            return
        visiting.add(node)
        for child in graph.get(node, []):
            visit(child, stack + [node])
        visiting.remove(node)
        visited.add(node)

    for node in graph:
        visit(node, [])


def main() -> int:
    for rel in REQUIRED_FILES:
        if not (ROOT / rel).exists():
            errors.append(f'missing: {rel}')

    config = read('client/Unity/Assets/Game/Foundation/Runtime/ClientRuntimeConfig.cs')
    for marker in ['apiBaseUrl', 'apiTimeoutSeconds', 'absolute HTTP(S) URL', 'apiTimeoutSeconds must be between 1 and 120']:
        require_contains('ClientRuntimeConfig', config, marker)

    streaming = read('client/Unity/Assets/StreamingAssets/linhgioi-client.json')
    try:
        streaming_json = json.loads(streaming)
    except json.JSONDecodeError as exc:
        errors.append(f'linhgioi-client.json invalid JSON: {exc}')
    else:
        if streaming_json.get('apiBaseUrl') != 'http://127.0.0.1:18083':
            errors.append('linhgioi-client.json must include default apiBaseUrl http://127.0.0.1:18083')
        if streaming_json.get('apiTimeoutSeconds') != 10:
            errors.append('linhgioi-client.json must include default apiTimeoutSeconds 10')
        client_version = streaming_json.get('clientVersion', '')
        if 'm3b' not in client_version and 'm4' not in client_version:
            errors.append('linhgioi-client.json clientVersion must identify M3B source or M4 successor source')

    models = read('client/Unity/Assets/Game/Account/Runtime/AccountModels.cs')
    for cls in ['DevLoginRequest', 'CreateCharacterRequest', 'SaveCharacterPositionRequest', 'AccountResponse', 'CharacterResponse', 'DevLoginResponse', 'CharacterListResponse']:
        require_regex('AccountModels', models, rf'\bclass\s+{cls}\b')
    for field in ['accountId', 'characterId', 'classId', 'entityId', 'yawDegrees']:
        require_contains('AccountModels', models, field)

    client = read('client/Unity/Assets/Game/Account/Runtime/AccountApiClient.cs')
    for marker in [
        'UnityWebRequest',
        '/dev/auth/login',
        '/accounts/',
        '/characters/',
        '/position',
        'ParseCharacterListJson',
        'WrapTopLevelArray',
        'expected HTTP',
    ]:
        require_contains('AccountApiClient', client, marker)

    runner = read('client/Unity/Assets/Game/Account/Runtime/M3BAccountCharacterSmokeRunner.cs')
    for marker in [
        'LGO_M3B_ACCOUNT_CHARACTER_SMOKE',
        '--lgo-m3b-account-character-smoke',
        'LoginDevAsync',
        'ListCharactersAsync',
        'CreateCharacterAsync',
        'SaveCharacterPositionAsync',
        'LoadCharacterAsync',
        '--lgo-m3b-expect-existing',
        'expected persisted character',
        'M3B account/character smoke status=',
    ]:
        require_contains('M3BAccountCharacterSmokeRunner', runner, marker)

    bootstrap = read('client/Unity/Assets/Game/Bootstrap/Runtime/GameBootstrap.cs')
    require_contains('GameBootstrap', bootstrap, 'M3BAccountCharacterSmokeRunner.ShouldRun()')
    require_contains('GameBootstrap', bootstrap, 'M3BAccountCharacterSmokeRunner.RunFromCommandLineAsync')

    tests = read('client/Unity/Assets/Game/Tests/EditMode/M3BAccountCharacterTests.cs')
    for marker in ['ClientRuntimeConfigCarriesApiEndpointForM3B', 'CharacterListParserHandlesM3TopLevelArrayContract', 'CharacterListParserRejectsNonArrayBodies']:
        require_contains('M3BAccountCharacterTests', tests, marker)

    shell = read('tools/run_m3b_unity_account_character_once.sh')
    runtime = read('tools/m3b_unity_account_character_runtime.py')
    for marker in ['UNVERIFIED_ENVIRONMENT', 'LGO_M3B_UNITY_PLAYER', 'M3B_UNITY_ACCOUNT_CHARACTER_RUNTIME_SMOKE_PASS', 'raw M3B Unity dev key leaked']:
        require_contains('M3B runtime tools', shell + '\n' + runtime, marker)

    manifest_content = read('m3b-manifest.json')
    try:
        manifest = json.loads(manifest_content)
    except json.JSONDecodeError as exc:
        errors.append(f'm3b-manifest.json invalid JSON: {exc}')
    else:
        serialized = json.dumps(manifest, sort_keys=True)
        for marker in ['LG-M3B-UNITY-ACCOUNT-CHARACTER-INTEGRATION', 'M3B_UNITY_ACCOUNT_CHARACTER_SOURCE_READY_FOR_VERIFY', 'M3_ACCOUNT_CHARACTER_PERSISTENCE_RUNTIME_SMOKE_CLOSED']:
            if marker not in serialized:
                errors.append(f'm3b-manifest.json missing marker: {marker}')

    unity_manifest = read('client/Unity/Packages/manifest.json')
    try:
        unity_manifest_json = json.loads(unity_manifest)
    except json.JSONDecodeError as exc:
        errors.append(f'Unity manifest invalid JSON: {exc}')
    else:
        unity_deps = unity_manifest_json.get('dependencies', {})
        if unity_deps.get('com.unity.modules.unitywebrequest') != '1.0.0':
            errors.append('Unity manifest must include com.unity.modules.unitywebrequest 1.0.0 for AccountApiClient')

    protocol_prepare = read('tools/prepare_unity_protocol.py')
    for marker in ['precompiledReferences', 'Google.Protobuf.dll', 'overrideReferences', 'Assets/Game/Protocol/Generated']:
        require_contains('prepare_unity_protocol.py', protocol_prepare, marker)
    if not (ROOT / 'client/Unity/Assets/Game/Protocol/LinhGioi.Protocol.asmdef').exists():
        errors.append('stable LinhGioi.Protocol asmdef missing')

    local_assets = read('tools/prepare_unity_local_assets.sh')
    for marker in ['UNITY_WEBREQUEST_MODULE_READY', 'Google.Protobuf.dll']:
        require_contains('prepare_unity_local_assets.sh', local_assets, marker)

    validate_asmdefs()

    if errors:
        print('M3B UNITY ACCOUNT CHARACTER STATIC VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M3B UNITY ACCOUNT CHARACTER STATIC VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
