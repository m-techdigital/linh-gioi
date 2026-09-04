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
FORBIDDEN_MARKERS = [
    'HitPoints',
    'Damage',
    'Loot',
    'Inventory',
    'EnemyAttack',
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
        'docs/art/LGO-ART-DIRECTION-PACK-v0.20.0.md',
        'Item, skill, and VFX sheet feeds v0.23',
        'non-damaging local skill and VFX feedback placeholders',
    )
    require(
        'client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs',
        'PlaceholderVfxFeedbackState',
        'VfxFeedbackStateName',
        'PortalGatePulse',
        'WindSlashPreview',
        'SpiritPulse',
        'ShadowBindWarning',
        'LGO Portal Gate Pulse Placeholder',
        'LGO Wind Slash Preview Placeholder',
        'LGO Shadow Bind Warning Placeholder',
    )
    require(
        'client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs',
        'Hiệu ứng:',
        'chỉ là phản hồi hình ảnh cục bộ',
        'cổng, mạch linh khí, chém gió, cảnh báo bóng đều chỉ là hình ảnh',
    )
    require(
        'client/Unity/Assets/Game/World/Runtime/M5GuidedTrainingLoopSmokeRunner.cs',
        'initialVfxFeedbackState',
        'afterGateKeeperVfxFeedbackState',
        'finalVfxFeedbackState',
        'mạch sáng Linh Môn',
        'Chém Gió xem thử',
        'mạch linh khí',
    )
    require(
        'tools/lgo_playable_closure_check.sh',
        'validate_m5_vfx_feedback_placeholder.py',
        'm5_vfx_feedback_placeholder',
    )
    require(
        'docs/tasks/M5-VFX-FEEDBACK-PLACEHOLDER-v0.23.0.md',
        'M5_VFX_FEEDBACK_PLACEHOLDER_SOURCE_READY_v0.23.0',
        'visual-only',
    )
    require_file('tools/validate_art_direction_pack_v020.py', executable=True)

    world = read('client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs')
    for marker in FORBIDDEN_MARKERS:
        if marker in world:
            errors.append(f'world VFX placeholder contains forbidden gameplay marker: {marker}')
    if 'Cooldown' in world:
        m6_doc = read('docs/tasks/M6-MINIMAL-LOCAL-COMBAT-FOUNDATION-v0.34.0.md')
        if 'M6_MINIMAL_LOCAL_COMBAT_FOUNDATION_SOURCE_READY_v0.34.0' not in m6_doc or 'local/non-authoritative' not in m6_doc:
            errors.append('world VFX placeholder contains Cooldown without M6 local/non-authoritative approval')

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
        print('M5 VFX FEEDBACK PLACEHOLDER VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M5 VFX FEEDBACK PLACEHOLDER VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
