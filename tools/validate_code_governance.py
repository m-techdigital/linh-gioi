#!/usr/bin/env python3
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []

DOCS = [
    'docs/execution/CODE-GOVERNANCE-CONTRACT.md',
    'docs/execution/CODE-OWNERSHIP-MAP.md',
    'docs/execution/CODE-DUPLICATION-AUDIT-CHECKLIST.md',
    'docs/execution/CODE-QUALITY-GATES.md',
    'docs/execution/templates/CODE-QUALITY-HANDOFF-SECTION.md',
    'docs/execution/prompts/CODEX-DEFAULT-CODE-GOVERNANCE-RULES.md',
    'HANDOFF-LG-CODE-GOVERNANCE-CONTRACT-v1.0.md',
    'LGO-CODE-GOVERNANCE-CONTRACT-v1.0-CHANGED-FILES.txt',
    'LGO-CODE-GOVERNANCE-CONTRACT-v1.0-DELETIONS.txt',
]
FORBIDDEN_CHANGED_PREFIXES = ['protocol/', 'gamedata/schemas/', 'docs/adr/', 'client/Unity/Assets/Game/UI/design-tokens.json']
FORBIDDEN_OUTPUT_PREFIXES = [
    'build/',
    'client/Unity/Library/',
    'client/Unity/Temp/',
    'client/Unity/Logs/',
    'client/Unity/Assets/Game/Generated/',
    'client/Unity/Assets/Game/Protocol/Generated/',
    'server/api/target/',
    'server/realtime/target/',
    'server/shared/target/',
    'tools/__pycache__/',
]


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


def git_lines(*args: str) -> list[str]:
    result = subprocess.run(['git', '--no-pager', *args], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if result.returncode != 0:
        errors.append('git command failed: git --no-pager ' + ' '.join(args) + ' ' + result.stderr.strip())
        return []
    return result.stdout.splitlines()


def main() -> int:
    for path in DOCS:
        if not (ROOT / path).is_file():
            errors.append(f'missing governance artifact: {path}')

    require('README.md', 'CODE-GOVERNANCE-CONTRACT.md')
    require('START-HERE.md', 'CODE-GOVERNANCE-CONTRACT.md')
    require('docs/execution/06-PROJECT-GOVERNANCE-INDEX.md', 'CODE-GOVERNANCE-CONTRACT.md', 'CODE-OWNERSHIP-MAP.md', 'CODE-QUALITY-GATES.md')
    require('docs/execution/07-PHASE-GATES.md', 'Code governance', 'CODE-GOVERNANCE-CONTRACT.md')
    require('docs/execution/templates/TASK-HANDOFF-TEMPLATE.md', 'Code Quality / Duplication / Ownership Audit')
    require(
        'docs/execution/CODE-GOVERNANCE-CONTRACT.md',
        'Do not only make it work; make it maintainable.',
        'Anti-Duplication Rules',
        'Ownership Rules',
        'Refactor Rules',
        'Validator Rules',
        'Do not weaken validators just to pass.',
        'Player-facing copy should be Vietnamese',
        'Every handoff must include a code quality, duplication, and ownership audit.',
    )
    require(
        'docs/execution/CODE-OWNERSHIP-MAP.md',
        'UI runtime',
        'world/runtime layer',
        'server/API/realtime',
        'Future web/player/admin surfaces',
    )
    require(
        'docs/execution/CODE-QUALITY-GATES.md',
        'Duplicate/ownership audit',
        'Runtime evidence classification',
        'Visual evidence classification',
        'Technical debt/follow-up section',
    )
    require(
        'docs/execution/prompts/CODEX-DEFAULT-CODE-GOVERNANCE-RULES.md',
        'Before final handoff, read and obey docs/execution/CODE-GOVERNANCE-CONTRACT.md, docs/execution/CODE-OWNERSHIP-MAP.md, and docs/execution/CODE-QUALITY-GATES.md.',
    )
    require('LGO-CODE-GOVERNANCE-CONTRACT-v1.0-DELETIONS.txt', 'DELETED', 'none')

    for path in git_lines('diff', '--name-only'):
        for prefix in FORBIDDEN_CHANGED_PREFIXES:
            if path == prefix or path.startswith(prefix):
                errors.append(f'frozen surface modified: {path}')
    for line in git_lines('status', '--short', '--untracked-files=all'):
        path = line[3:] if len(line) >= 4 else line
        for prefix in FORBIDDEN_OUTPUT_PREFIXES:
            if path.startswith(prefix):
                errors.append(f'generated/cache/build output under source status: {path}')

    if errors:
        print('CODE GOVERNANCE VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('CODE GOVERNANCE VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
