#!/usr/bin/env python3
from __future__ import annotations

import re
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []

DOCS = [
    'docs/execution/LGO-MASTER-ROADMAP-v1.0.md',
    'docs/execution/LGO-MILESTONE-DEPENDENCY-MAP-v1.0.md',
    'docs/execution/LGO-PRODUCTION-READINESS-ROADMAP-v1.0.md',
    'docs/execution/LGO-AUTH-DB-COMBAT-ROADMAP-v1.0.md',
    'docs/execution/LGO-WEB-ADMIN-PLAYER-PORTAL-ROADMAP-v1.0.md',
    'docs/execution/LGO-ASSET-ANIMATION-PIPELINE-ROADMAP-v1.0.md',
    'docs/execution/LGO-NEXT-50-TASKS-BACKLOG-v1.0.md',
    'docs/execution/prompts/LGO-MASTER-ROADMAP-KEEPER.md',
    'HANDOFF-LG-MASTER-ROADMAP-v1.0.md',
    'LGO-MASTER-ROADMAP-v1.0-CHANGED-FILES.txt',
    'LGO-MASTER-ROADMAP-v1.0-DELETIONS.txt',
]
FORBIDDEN_CHANGED_PREFIXES = ['protocol/', 'gamedata/schemas/', 'docs/adr/', 'client/Unity/Assets/Game/UI/design-tokens.json', 'client/Unity/Assets/Game/', 'server/']
FORBIDDEN_OUTPUT_PREFIXES = ['build/', 'client/Unity/Library/', 'client/Unity/Temp/', 'client/Unity/Logs/', 'client/Unity/Assets/Game/Generated/', 'client/Unity/Assets/Game/Protocol/Generated/', 'tools/__pycache__/']
M6_ALLOWED_SOURCE_FILES = {
    'client/Unity/Assets/Game/Bootstrap/Runtime/GameBootstrap.cs',
    'client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs',
    'client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs',
    'client/Unity/Assets/Game/World/Runtime/M6MinimalLocalCombatSmokeRunner.cs',
    'client/Unity/Assets/Game/World/Runtime/M6MinimalLocalCombatSmokeRunner.cs.meta',
    'tools/run_m6_minimal_local_combat_once.sh',
    'tools/validate_m6_minimal_local_combat.py',
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


def git_lines(*args: str) -> list[str]:
    result = subprocess.run(['git', '--no-pager', *args], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if result.returncode != 0:
        errors.append('git command failed: git --no-pager ' + ' '.join(args) + ' ' + result.stderr.strip())
        return []
    return result.stdout.splitlines()


def main() -> int:
    for path in DOCS:
        if not (ROOT / path).is_file():
            errors.append(f'missing roadmap artifact: {path}')

    for path in DOCS[:8]:
        require(path, 'Code governance')

    require('docs/execution/LGO-MASTER-ROADMAP-v1.0.md', 'M6 pre-combat readiness', 'Combat M6', 'Auth', 'Database', 'Social/MMO', 'Asset/art/animation', 'No source inspection as runtime PASS')
    require('docs/execution/LGO-MILESTONE-DEPENDENCY-MAP-v1.0.md', 'Before Combat', 'Before Auth', 'Before DB', 'Before Economy', 'Before Social/Guild/Chat', 'Before Web/Admin/Player Portal', 'Before Production Art')
    require('docs/execution/LGO-WEB-ADMIN-PLAYER-PORTAL-ROADMAP-v1.0.md', 'Public Website', 'Player Portal', 'Admin-Dev Console', 'Admin-Prod Console', 'game-style', 'ops-first')
    require('docs/execution/LGO-AUTH-DB-COMBAT-ROADMAP-v1.0.md', 'Protocol needs', 'GameData needs', 'server-authoritative', 'No combat/auth/DB implementation')
    require('docs/execution/LGO-ASSET-ANIMATION-PIPELINE-ROADMAP-v1.0.md', 'provenance/license', 'UI atlas/import settings', 'No final production art')
    backlog = read('docs/execution/LGO-NEXT-50-TASKS-BACKLOG-v1.0.md')
    task_ids = set(re.findall(r'LGO-TASK-\d{3}', backlog))
    if len(task_ids) < 50:
        errors.append(f'next backlog has fewer than 50 task IDs: {len(task_ids)}')
    require('HANDOFF-LG-MASTER-ROADMAP-v1.0.md', 'LGO_MASTER_ROADMAP_ACCEPTED_v1.0', 'Code Quality / Duplication / Ownership Audit')
    require('LGO-MASTER-ROADMAP-v1.0-DELETIONS.txt', 'DELETED', 'none')

    forbidden_claims = ['production auth implemented', 'database implemented', 'full combat implemented', 'production admin implemented', 'production art complete']
    combined = '\n'.join(read(path) for path in DOCS[:8]).lower()
    for claim in forbidden_claims:
        if claim in combined:
            errors.append(f'roadmap makes forbidden implementation claim: {claim}')

    for path in git_lines('diff', '--name-only'):
        if path in {'tools/validate_master_roadmap.py', 'tools/lgo_playable_closure_check.sh'} or path in M6_ALLOWED_SOURCE_FILES:
            continue
        for prefix in FORBIDDEN_CHANGED_PREFIXES:
            if path == prefix or path.startswith(prefix):
                errors.append(f'forbidden source path modified: {path}')
    for line in git_lines('status', '--short', '--untracked-files=all'):
        path = line[3:] if len(line) >= 4 else line
        for prefix in FORBIDDEN_OUTPUT_PREFIXES:
            if path.startswith(prefix):
                errors.append(f'generated/cache/build output under source status: {path}')

    if errors:
        print('MASTER ROADMAP VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('MASTER ROADMAP VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
