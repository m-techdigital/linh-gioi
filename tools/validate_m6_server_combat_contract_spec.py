#!/usr/bin/env python3
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []
FROZEN_PREFIXES = ['protocol/', 'gamedata/schemas/', 'docs/adr/', 'client/Unity/Assets/Game/UI/design-tokens.json', 'server/']
FORBIDDEN_OUTPUT_PREFIXES = ['build/', 'client/Unity/Library/', 'client/Unity/Temp/', 'client/Unity/Logs/', 'client/Unity/Assets/Game/Generated/', 'client/Unity/Assets/Game/Protocol/Generated/', 'server/scripts/__pycache__/', 'tools/__pycache__/']
DOCS = [
    'docs/tasks/M6-SERVER-AUTHORITATIVE-COMBAT-CONTRACT-SPEC-v0.39.0.md',
    'docs/design/LGO-SERVER-AUTHORITATIVE-COMBAT-DESIGN-v0.39.0.md',
    'CONTRACT_CHANGE_REQUEST-M6-SERVER-COMBAT-v0.39.0.md',
    'docs/execution/prompts/M6-SERVER-COMBAT-CONTRACT-IMPLEMENTATION-PROMPT-v0.40.0.md',
    'M6-SERVER-AUTHORITATIVE-COMBAT-CONTRACT-SPEC-FINAL-REPORT-v0.39.0.md',
    'HANDOFF-LG-M6-SERVER-AUTHORITATIVE-COMBAT-CONTRACT-SPEC-v0.39.0.md',
]
V040_CONTRACT_FILES = {
    'protocol/combat.proto',
    'gamedata/schemas/skill.schema.json',
    'gamedata/skills/wind_slash.yaml',
    'gamedata/compiled/gamedata-manifest.json',
    'tests/gamedata/test_gamedata_pipeline.py',
    'tests/gamedata/__pycache__/test_gamedata_pipeline.cpython-312.pyc',
    'tools/validate_m6_combat_protocol_gamedata_contract.py',
    'docs/tasks/M6-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-v0.40.0.md',
    'docs/design/LGO-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-v0.40.0.md',
    'M6-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-FINAL-REPORT-v0.40.0.md',
    'HANDOFF-LG-M6-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-v0.40.0.md',
    'LGO-M6-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-v0.40.0-CHANGED-FILES.txt',
    'LGO-M6-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-v0.40.0-DELETIONS.txt',
}


def read(path: str) -> str:
    target = ROOT / path
    if not target.exists():
        errors.append(f'missing: {path}')
        return ''
    return target.read_text(encoding='utf-8', errors='replace')


def require(path: str, *markers: str) -> None:
    content = read(path)
    for marker in markers:
        if marker not in content:
            errors.append(f'{path} missing marker: {marker}')


def require_file(path: str, executable: bool = False) -> None:
    target = ROOT / path
    if not target.is_file():
        errors.append(f'missing file: {path}')
        return
    if executable and not os.access(target, os.X_OK):
        errors.append(f'file is not executable: {path}')


def git_lines(*args: str) -> list[str]:
    result = subprocess.run(['git', '--no-pager', *args], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if result.returncode != 0:
        errors.append('git command failed: git --no-pager ' + ' '.join(args))
        return []
    return result.stdout.splitlines()


def v040_contract_is_active() -> bool:
    return (
        'M6_COMBAT_PROTOCOL_GAMEDATA_CONTRACT_ACCEPTED_v0.40.0'
        in read('docs/tasks/M6-COMBAT-PROTOCOL-GAMEDATA-CONTRACT-v0.40.0.md')
        and 'CONTRACT_CHANGE_REQUEST-M6-SERVER-COMBAT-v0.39.0.md'
        and (ROOT / 'CONTRACT_CHANGE_REQUEST-M6-SERVER-COMBAT-v0.39.0.md').is_file()
    )


def main() -> int:
    for path in DOCS:
        require_file(path)
    require_file('tools/validate_m6_server_combat_contract_spec.py', executable=True)
    combined = '\n'.join(read(path) for path in DOCS)
    for marker in [
        'M6_SERVER_COMBAT_CONTRACT_SPEC_ACCEPTED_v0.39.0',
        'CombatIntent',
        'CombatAccepted',
        'CombatRejected',
        'CombatResult',
        'CombatStateSnapshot',
        'client sends combat intent/input',
        'server validates',
        'client displays predicted/local feedback separately',
        'skill activation',
        'cooldown',
        'targeting rule',
        'effect rule',
        'telegraph rule',
        'No protocol/GameData changes are made by this v0.39.0 task',
        'v0.40 protocol proposal',
        'v0.41 GameData combat schema proposal',
        'v0.42 Java combat validation skeleton',
        'v0.43 Unity client integration skeleton',
        'v0.44 client-server combat smoke',
        'No duplicate DTO',
        'No parallel combat config',
        'No bypassing Protobuf/GameData',
    ]:
        if marker not in combined:
            errors.append(f'contract docs missing marker: {marker}')
    require('CONTRACT_CHANGE_REQUEST-M6-SERVER-COMBAT-v0.39.0.md', 'why current contracts are insufficient', 'Unity', 'Java realtime', 'tools/codegen', 'validators', 'smoke tests', 'backward compatibility')
    require('HANDOFF-LG-M6-SERVER-AUTHORITATIVE-COMBAT-CONTRACT-SPEC-v0.39.0.md', 'Frozen Surface Audit', 'Code Governance Audit', 'Contract Change Request Summary')
    require('LGO-M6-SERVER-AUTHORITATIVE-COMBAT-CONTRACT-SPEC-v0.39.0-DELETIONS.txt', 'DELETED', 'none')

    v040_active = v040_contract_is_active()
    for path in git_lines('diff', '--name-only'):
        if v040_active and path in V040_CONTRACT_FILES:
            continue
        if path in {
            'tools/validate_m6_server_combat_contract_spec.py',
            'tools/lgo_playable_closure_check.sh',
            'tools/validate_master_roadmap.py',
            'tools/validate_m6_combat_readiness_spec.py',
            'tools/validate_m6_combat_protocol_gamedata_contract.py',
            'server/realtime/src/main/java/com/linhgioi/server/realtime/combat/CombatValidationService.java',
            'server/realtime/src/test/java/com/linhgioi/server/realtime/combat/CombatValidationServiceTest.java',
            'tools/run_m6_server_authoritative_combat_pilot.sh',
            'tools/validate_m6_server_authoritative_combat_pilot.py',
        }:
            continue
        if path.startswith('docs/tasks/') or path.startswith('docs/design/') or path.startswith('docs/execution/prompts/') or path.startswith('HANDOFF') or path.startswith('M6') or path.startswith('LGO') or path.startswith('CONTRACT_CHANGE_REQUEST-M6-SERVER-COMBAT-v0.39.0.md'):
            continue
        for prefix in FROZEN_PREFIXES:
            if path == prefix or path.startswith(prefix):
                errors.append(f'forbidden implementation/frozen path modified: {path}')
    for line in git_lines('status', '--short', '--untracked-files=all'):
        status = line[:2]
        if 'D' in status:
            continue
        path = line[3:] if len(line) >= 4 else line
        for prefix in FORBIDDEN_OUTPUT_PREFIXES:
            if path.startswith(prefix):
                errors.append(f'generated/cache/build output under source status: {path}')

    if errors:
        print('M6 SERVER COMBAT CONTRACT SPEC VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M6 SERVER COMBAT CONTRACT SPEC VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
