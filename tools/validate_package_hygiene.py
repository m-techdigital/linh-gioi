#!/usr/bin/env python3
from __future__ import annotations

import subprocess
import sys
import zipfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []

FORBIDDEN_ENTRIES = (
    '.DS_Store',
    '__pycache__/',
    '.pyc',
    '__MACOSX/',
    'build/',
    'client/Unity/Library/',
    'client/Unity/Temp/',
    'client/Unity/Logs/',
    'client/Unity/Assets/Game/Generated/',
    'client/Unity/Assets/Game/Generated.meta',
    'client/Unity/Assets/Game/Protocol/Generated/',
    'client/Unity/Assets/Game/Protocol/Generated.meta',
)
FORBIDDEN_CHANGED_PREFIXES = ('protocol/', 'gamedata/schemas/', 'docs/adr/')


def git_lines(*args: str) -> list[str]:
    result = subprocess.run(['git', '--no-pager', *args], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if result.returncode != 0:
        errors.append('git command failed: git --no-pager ' + ' '.join(args) + ' ' + result.stderr.strip())
        return []
    return result.stdout.splitlines()


def check_zip(path: Path) -> None:
    with zipfile.ZipFile(path) as archive:
        for name in archive.namelist():
            if name.startswith('.git/') or name == '.git':
                errors.append(f'{path.name} contains forbidden entry: {name}')
            if '/__pycache__/' in name or name.startswith('__pycache__/') or name.endswith('.pyc'):
                errors.append(f'{path.name} contains forbidden Python cache entry: {name}')
            for forbidden in FORBIDDEN_ENTRIES:
                if forbidden == '.DS_Store':
                    if name.endswith('/.DS_Store') or name == '.DS_Store':
                        errors.append(f'{path.name} contains forbidden entry: {name}')
                elif forbidden == '.pyc':
                    if name.endswith('.pyc'):
                        errors.append(f'{path.name} contains forbidden entry: {name}')
                elif name == forbidden.rstrip('/') or name.startswith(forbidden):
                    errors.append(f'{path.name} contains forbidden entry: {name}')


def main() -> int:
    package_tool = ROOT / 'tools/package_source.py'
    if not package_tool.is_file():
        errors.append('missing package tool: tools/package_source.py')
    else:
        content = package_tool.read_text(encoding='utf-8', errors='replace')
        for marker in ['.DS_Store', '__pycache__', '.pyc', '__MACOSX', 'client/Unity/Assets/Game/Generated/', 'client/Unity/Assets/Game/Protocol/Generated/', '.git', 'build/']:
            if marker not in content:
                errors.append(f'tools/package_source.py missing exclusion marker: {marker}')

    for path in git_lines('diff', '--name-only'):
        if path == 'client/Unity/Assets/Game/UI/design-tokens.json':
            errors.append(f'frozen surface modified: {path}')
        for prefix in FORBIDDEN_CHANGED_PREFIXES:
            if path.startswith(prefix):
                errors.append(f'frozen surface modified: {path}')

    for line in git_lines('status', '--short', '--untracked-files=all'):
        status = line[:2]
        if 'D' in status:
            continue
        path = line[3:] if len(line) >= 4 else line
        for forbidden in FORBIDDEN_ENTRIES:
            if forbidden == '.DS_Store':
                if path.endswith('/.DS_Store') or path == '.DS_Store':
                    errors.append(f'forbidden source artifact present: {path}')
            elif forbidden == '.pyc':
                if path.endswith('.pyc'):
                    errors.append(f'forbidden source artifact present: {path}')
            elif path == forbidden.rstrip('/') or path.startswith(forbidden):
                errors.append(f'forbidden source artifact present: {path}')

    for path in sorted(ROOT.glob('*.zip')):
        check_zip(path)

    if errors:
        print('PACKAGE HYGIENE VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('PACKAGE HYGIENE VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
