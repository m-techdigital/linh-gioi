#!/usr/bin/env python3
from __future__ import annotations

import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []


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
    require(
        'client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs',
        'SetApiError',
        'Phiên hiện tại bị chặn khi ',
        'Phiên hiện tại chưa sẵn sàng hoặc từ chối yêu cầu. Kiểm tra kết nối rồi thử lại.',
        'Phiên bị gián đoạn: kiểm tra kết nối rồi thử lại.',
        'RunSafelyAsync',
    )
    require('tools/lgo_playable_closure_check.sh', 'validate_m5_api_error_resilience.py', 'm5_api_error_resilience')
    require('docs/tasks/M5-API-ERROR-RESILIENCE-v0.29.0.md', 'M5_API_ERROR_RESILIENCE_RUNTIME_CLOSED_LOCAL_v0.29.0', 'No production auth')

    for path in git_lines('diff', '--name-only'):
        if path == 'client/Unity/Assets/Game/UI/design-tokens.json':
            errors.append(f'frozen surface modified: {path}')
        for prefix in ['protocol/', 'gamedata/schemas/', 'docs/adr/']:
            if path.startswith(prefix):
                errors.append(f'frozen surface modified: {path}')

    if errors:
        print('M5 API ERROR RESILIENCE VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M5 API ERROR RESILIENCE VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
