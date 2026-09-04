#!/usr/bin/env python3
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []
FROZEN_PREFIXES = ['protocol/', 'gamedata/schemas/', 'docs/adr/', 'client/Unity/Assets/Game/UI/design-tokens.json']


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


def require_any(path: str, *markers: str) -> None:
    content = read(path)
    if not any(marker in content for marker in markers):
        errors.append(f'{path} missing one of markers: {", ".join(markers)}')


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


def main() -> int:
    require('docs/tasks/M6-COMBAT-UX-FEEDBACK-POLISH-v0.35.0.md', 'M6_COMBAT_UX_FEEDBACK_POLISH_SOURCE_READY_v0.35.0', 'local cooldown indicator', 'Vietnamese help text', 'Code Quality / Duplication / Ownership Audit')
    require('client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs', 'Sân luyện an toàn / Mục tiêu luyện tập', 'TargetDummyHitFlash', 'Chỉ là mô phỏng cục bộ')
    require_any('client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs', 'Lại gần vòng sáng', 'Ngoài tầm: lại gần vòng chọn màu vàng')
    require('client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs', 'Tấn công thử')
    require_any('client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs', 'Luyện mục tiêu cục bộ', 'Bia luyện cục bộ')
    require_any('client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs', 'Kích hoạt phản hồi đánh thử cục bộ', 'Gửi ý định Chém Gió vào bia luyện tập')
    require('tools/lgo_playable_closure_check.sh', 'validate_m6_combat_ux_feedback.py', 'm6_combat_ux_feedback')
    require('M6-COMBAT-UX-FEEDBACK-POLISH-FINAL-REPORT-v0.35.0.md', 'M6_COMBAT_UX_FEEDBACK_POLISH_SOURCE_READY_v0.35.0')
    require('HANDOFF-LG-M6-COMBAT-UX-FEEDBACK-POLISH-v0.35.0.md', 'Code Quality / Duplication / Ownership Audit', 'Frozen Surface Audit')
    require('LGO-M6-COMBAT-UX-FEEDBACK-POLISH-v0.35.0-DELETIONS.txt', 'DELETED', 'none')
    require_file('tools/validate_m6_combat_ux_feedback.py', executable=True)

    for path in git_lines('diff', '--name-only'):
        for prefix in FROZEN_PREFIXES:
            if path == prefix or path.startswith(prefix):
                errors.append(f'frozen surface modified: {path}')

    if errors:
        print('M6 COMBAT UX FEEDBACK VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M6 COMBAT UX FEEDBACK VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
