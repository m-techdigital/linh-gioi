#!/usr/bin/env python3
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []
FORBIDDEN_CHANGED_PREFIXES = ['protocol/', 'gamedata/schemas/', 'docs/adr/']
FORBIDDEN_OUTPUT_PREFIXES = [
    'build/',
    'client/Unity/Library/',
    'client/Unity/Temp/',
    'client/Unity/Logs/',
    'client/Unity/Assets/Game/Generated/',
    'client/Unity/Assets/Game/Protocol/Generated/',
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


def require_any(path: str, label: str, *markers: str) -> None:
    content = read(path)
    if not any(marker in content for marker in markers):
        errors.append(f'{path} missing {label}: expected one of {", ".join(markers)}')


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
        errors.append('git command failed: git --no-pager ' + ' '.join(args) + ' ' + result.stderr.strip())
        return []
    return result.stdout.splitlines()


def main() -> int:
    require(
        'client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs',
        'LGO Spirit Gate Landmark South',
        'LGO Safe Training Circle Center',
        'LGO Training Stone Cyan Beacon',
        'LGO Shadow Slime Warning Plinth',
        'ObjectiveDirectionHint',
        'WorldLandmarkSummary',
        'Linh Môn phía nam / Người Giữ Cổng tây bắc / Đá Luyện phía bắc / Bia đọc mục tiêu phía đông / Bóng Tối xa phía đông',
    )
    require_any(
        'client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs',
        'Gate Keeper readability marker',
        'LGO Gate Keeper Gold Readability Pillar',
        'LGO Gate Keeper Ground Halo',
    )
    require_any(
        'client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs',
        'world-space landmark labels',
        'CreateWorldLabel',
        'LGO Gate Keeper World Label',
    )
    require(
        'client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs',
        'Chỉ dẫn: ',
        'Mốc sân luyện: Linh Môn phía nam / Người Giữ Cổng tây bắc / Đá Luyện phía bắc / Bia đọc mục tiêu phía đông / Bóng Tối xa phía đông.',
        'Sân Luyện An Toàn',
    )
    require(
        'tools/lgo_playable_closure_check.sh',
        'validate_m5_world_hub_readability.py',
        'm5_world_hub_readability',
    )
    require('docs/tasks/M5-WORLD-HUB-READABILITY-v0.19.0.md', 'M5_WORLD_HUB_READABILITY_SOURCE_READY_v0.19.0', 'No minimap')
    require_file('tools/package_source.py', executable=True)
    require_file('tools/validate_package_hygiene.py', executable=True)

    for path in git_lines('diff', '--name-only'):
        if path == 'client/Unity/Assets/Game/UI/design-tokens.json':
            errors.append(f'frozen surface modified: {path}')
        for prefix in FORBIDDEN_CHANGED_PREFIXES:
            if path.startswith(prefix):
                errors.append(f'frozen surface modified: {path}')
    for line in git_lines('status', '--short', '--untracked-files=all'):
        path = line[3:] if len(line) >= 4 else line
        for prefix in FORBIDDEN_OUTPUT_PREFIXES:
            if path.startswith(prefix):
                errors.append(f'generated/cache/build output under source status: {path}')

    if errors:
        print('M5 WORLD HUB READABILITY VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M5 WORLD HUB READABILITY VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
