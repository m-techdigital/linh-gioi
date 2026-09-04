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


def main() -> int:
    require('client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs', 'Mục tiêu 1/2', 'Mục tiêu 2/2', 'Bước 1:', 'Bước 2:', 'mạch sáng lam', 'Sân luyện an toàn / cảnh báo bóng phía đông')
    require('client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs', 'Hoàn tất luyện tập. Hãy lưu vị trí hoặc về Điện Nhân Vật.', 'Bước 1 Người Giữ Cổng / Bước 2 Đá Luyện', 'Lưu vị trí', 'Về điện nhân vật', 'Thoát')
    require('client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs', 'DialogueActive', 'DialogueCompleted')
    require('tools/validate_m5_lightweight_dialogue.py', 'M5 LIGHTWEIGHT NPC DIALOGUE VALIDATION PASS')
    require('docs/tasks/M5-TRAINING-OBJECTIVE-UX-v0.25.0.md', 'M5_TRAINING_OBJECTIVE_UX_SOURCE_READY_v0.25.0', 'no rewards')
    result = subprocess.run(['git', '--no-pager', 'diff', '--name-only'], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False)
    if result.returncode != 0:
        errors.append(result.stderr.strip())
    for path in result.stdout.splitlines():
        if path == 'client/Unity/Assets/Game/UI/design-tokens.json' or path.startswith(('protocol/', 'gamedata/schemas/', 'docs/adr/')):
            errors.append(f'frozen surface modified: {path}')
    joined = read('client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs') + read('client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs')
    for marker in ['XP', 'RewardSystem', 'Inventory', 'HitPoints', 'Damage']:
        if marker in joined:
            errors.append(f'forbidden marker present: {marker}')
    if errors:
        print('M5 TRAINING OBJECTIVE UX VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M5 TRAINING OBJECTIVE UX VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
