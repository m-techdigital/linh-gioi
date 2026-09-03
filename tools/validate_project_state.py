#!/usr/bin/env python3
from __future__ import annotations
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
ERRORS: list[str] = []

REQUIRED_MARKERS = {
    'README.md': [
        'M4 Playable UI And Art Quality Pass',
        'M4_PLAYABLE_UI_ART_QUALITY_SOURCE_READY',
        'M4_PLAYABLE_VERTICAL_SLICE_RUNTIME_SMOKE_PASS',
        'M3_ACCOUNT_CHARACTER_PERSISTENCE_RUNTIME_SMOKE_CLOSED',
        'M2_RUNTIME_CANDIDATE_HARDENED_READY_FOR_LOCAL_EVIDENCE',
        'M1_OFFLINE_COMBAT_RUNTIME_CLOSED',
        'M0_RUNTIME_CLOSED',
    ],
    'START-HERE.md': [
        'M4-2/M4-3 Playable UI And Art Quality Pass',
        'M4_PLAYABLE_UI_ART_QUALITY_SOURCE_READY',
        './tools/validate_m4_source.sh',
        './tools/run_m4_playable_vertical_slice_once.sh',
        'M4_PLAYABLE_VERTICAL_SLICE_RUNTIME_SMOKE_PASS',
    ],
    'VERSIONING.md': [
        'source_package_version = 0.12.0',
        'client_version = 0.4.0-m4',
        'm1_status = M1_OFFLINE_COMBAT_RUNTIME_CLOSED',
        'm2_status = M2_RUNTIME_CANDIDATE_HARDENED_READY_FOR_LOCAL_EVIDENCE',
        'm3_status = M3_ACCOUNT_CHARACTER_PERSISTENCE_RUNTIME_SMOKE_CLOSED',
        'm3b_status = M3B_UNITY_ACCOUNT_CHARACTER_SOURCE_READY_FOR_VERIFY',
        'M4_PLAYABLE_UI_ART_QUALITY_SOURCE_READY',
    ],
    'm1-manifest.json': [
        'M1_OFFLINE_COMBAT_RUNTIME_CLOSED',
        'M0_RUNTIME_CLOSED',
        'validate_m1_source.sh',
    ],
    'm2-manifest.json': [
        'M2_RUNTIME_CANDIDATE_HARDENED_READY_FOR_LOCAL_EVIDENCE',
        'M1_OFFLINE_COMBAT_RUNTIME_CLOSED',
        '--lgo-m2-online-session-smoke',
    ],
    'm3-manifest.json': [
        'M3_ACCOUNT_CHARACTER_PERSISTENCE_SOURCE_READY',
        'M2 owner override',
        'players-v1.json',
    ],
    'docs/execution/PROJECT-STATE.md': [
        'M4-2/M4-3 Playable UI And Art Quality Pass',
        'M4_PLAYABLE_UI_ART_QUALITY_SOURCE_READY',
        'M4_PLAYABLE_VERTICAL_SLICE_FOUNDATION_SOURCE_READY',
        'M4_VISUAL_PLACEHOLDER_FOUNDATION_SOURCE_READY',
        'M3_ACCOUNT_CHARACTER_PERSISTENCE_RUNTIME_SMOKE_CLOSED',
        'OWNER_OVERRIDE_FROM_M2_RUNTIME_CANDIDATE',
        'OWNER_OVERRIDE_FROM_M3_SERVER_API_CLOSED',
        'M2_RUNTIME_CANDIDATE_HARDENED_READY_FOR_LOCAL_EVIDENCE',
        'M1_OFFLINE_COMBAT_RUNTIME_CLOSED',
        'M0_RUNTIME_CLOSED',
    ],
    'docs/execution/06-PROJECT-GOVERNANCE-INDEX.md': [
        'Current milestone: `M4-2/M4-3 Playable UI And Art Quality Pass`',
        'Current safe implementation state: `M4_PLAYABLE_UI_ART_QUALITY_SOURCE_READY`',
    ],
    'docs/execution/07-PHASE-GATES.md': [
        'Current state: `M4_PLAYABLE_UI_ART_QUALITY_SOURCE_READY`',
        'M1 final state: `M1_OFFLINE_COMBAT_RUNTIME_CLOSED`',
    ],
    'docs/execution/M2-RUNTIME-EVIDENCE.md': [
        'M2_ONLINE_SESSION_RUNTIME_CLOSED',
        '--lgo-m2-online-session-smoke',
        'Unity-to-Java session smoke',
    ],
    'docs/execution/M3-PERSISTENCE-EVIDENCE.md': [
        'M3_ACCOUNT_CHARACTER_PERSISTENCE_RUNTIME_CLOSED',
        'M3_PERSISTENCE_SMOKE_PASS',
        'restart',
    ],
    'docs/execution/checklists/M3-PERSISTENCE-CLOSURE-CHECKLIST.md': [
        'Raw dev key not persisted',
        'API restart reload smoke PASS',
        'Unsupported future schema rejected',
    ],
    'docs/execution/prompts/P10-M3-ACCOUNT-CHARACTER-PERSISTENCE.md': [
        'M3 Account / Character Persistence Prototype',
        'validate_m3_source.sh',
        'run_m3_api_persistence_once.sh',
    ],
    'docs/tasks/M3B-UNITY-ACCOUNT-CHARACTER-INTEGRATION.md': [
        'LG-M3B-UNITY-ACCOUNT-CHARACTER-INTEGRATION-v0.8.0',
        'M3B_UNITY_ACCOUNT_CHARACTER_SOURCE_READY_FOR_VERIFY',
    ],
    'docs/execution/prompts/P11-M3B-UNITY-ACCOUNT-CHARACTER-INTEGRATION.md': [
        'M3-B Unity Account / Character Integration',
        'validate_m3b_source.sh',
        'run_m3b_unity_account_character_once.sh',
    ],
    'docs/execution/M3B-UNITY-ACCOUNT-CHARACTER-EVIDENCE.md': [
        '--lgo-m3b-account-character-smoke',
        'M3B_UNITY_ACCOUNT_CHARACTER_RUNTIME_SMOKE_PASS',
    ],
    'm3b-manifest.json': [
        'LG-M3B-UNITY-ACCOUNT-CHARACTER-INTEGRATION',
        'M3B_UNITY_ACCOUNT_CHARACTER_SOURCE_READY_FOR_VERIFY',
    ],
}

OUTDATED_CURRENT_TRUTH = [
    ('docs/execution/06-PROJECT-GOVERNANCE-INDEX.md', r'Current milestone: `M0 Foundation Runtime Closure`'),
    ('docs/execution/06-PROJECT-GOVERNANCE-INDEX.md', r'Current milestone: `M1 Offline Combat Prototype`'),
    ('docs/execution/06-PROJECT-GOVERNANCE-INDEX.md', r'Current milestone: `M2 Online Session Prototype`'),
    ('docs/execution/06-PROJECT-GOVERNANCE-INDEX.md', r'Current milestone: `M3 Account / Character Persistence`'),
    ('docs/execution/06-PROJECT-GOVERNANCE-INDEX.md', r'Current milestone: `M3-B Unity Account / Character Integration`'),
    ('docs/execution/06-PROJECT-GOVERNANCE-INDEX.md', r'Current safe implementation state: `M0_SERVER_RUNTIME_CLOSED_UNITY_ENV_LIMITED`'),
    ('docs/execution/06-PROJECT-GOVERNANCE-INDEX.md', r'Current safe implementation state: `M2_RUNTIME_CANDIDATE_HARDENED_READY_FOR_LOCAL_EVIDENCE`'),
    ('docs/execution/06-PROJECT-GOVERNANCE-INDEX.md', r'Current safe implementation state: `M3_ACCOUNT_CHARACTER_PERSISTENCE_SOURCE_READY`'),
    ('docs/execution/06-PROJECT-GOVERNANCE-INDEX.md', r'Current safe implementation state: `M3B_UNITY_ACCOUNT_CHARACTER_SOURCE_READY_FOR_VERIFY`'),
    ('docs/execution/07-PHASE-GATES.md', r'Current state: `M0_SERVER_RUNTIME_CLOSED_UNITY_ENV_LIMITED`'),
    ('docs/execution/07-PHASE-GATES.md', r'Current state: `M1_OFFLINE_COMBAT_RUNTIME_CLOSED`'),
    ('docs/execution/07-PHASE-GATES.md', r'Current state: `M2_RUNTIME_CANDIDATE_HARDENED_READY_FOR_LOCAL_EVIDENCE`'),
    ('docs/execution/07-PHASE-GATES.md', r'Current state: `M3_ACCOUNT_CHARACTER_PERSISTENCE_SOURCE_READY`'),
]


def read(path: str) -> str:
    p = ROOT / path
    if not p.exists():
        ERRORS.append(f'missing project-state file: {path}')
        return ''
    return p.read_text(encoding='utf-8')


def main() -> int:
    for path, markers in REQUIRED_MARKERS.items():
        content = read(path)
        for marker in markers:
            if marker not in content:
                ERRORS.append(f'{path} missing marker: {marker}')

    for path, pattern in OUTDATED_CURRENT_TRUTH:
        content = read(path)
        if re.search(pattern, content):
            ERRORS.append(f'{path} still contains outdated current-truth marker: {pattern}')

    if ERRORS:
        print('PROJECT STATE VALIDATION FAILED', file=sys.stderr)
        for error in ERRORS:
            print(' - ' + error, file=sys.stderr)
        return 1

    print('PROJECT STATE VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
