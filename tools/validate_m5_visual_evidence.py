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


def require_any(path: str, *marker_groups: tuple[str, ...]) -> None:
    content = read(path)
    for marker_group in marker_groups:
        if all(marker in content for marker in marker_group):
            return
    expected = ' OR '.join(' + '.join(group) for group in marker_groups)
    errors.append(f'{path} missing marker group: {expected}')


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
        'client/Unity/Assets/Game/UI/Runtime/VisualRuntimeEvidenceRunner.cs',
        '--lgo-visual-runtime-review',
        'M4PlayableClientController.Attach',
        'CaptureEvidenceLoginAsync',
        'CaptureEvidenceCreateCharacterIfNeededAsync',
        'CaptureEvidenceEnterWorldAsync',
        'CaptureEvidenceTargetDummyState',
        'CaptureEvidenceOpenDialogue',
        'CaptureEvidenceOpenSessionMenu',
        'ReadPixels',
        'RuntimePngWriter.WriteRgbTexture',
        'login.png',
        'character-lobby.png',
        'character-select.png',
        'enter-world.png',
        'world-hub.png',
        'target-dummy-state.png',
        'npc-dialogue.png',
        'session-menu.png',
        'visual-runtime-evidence-manifest.json',
        'visual-runtime-evidence-review.md',
        'VISUAL_RUNTIME_PASS',
        'reviewChecklist',
        'pass_claim=false',
        '1920',
        '1080',
    )
    require(
        'client/Unity/Assets/Game/UI/Runtime/M5VisualEvidenceRunner.cs',
        '--lgo-m5-visual-evidence-review',
        'CaptureFrameToPng',
        'ReadPixels',
        'WritePng',
        'BuildZlibRgba',
        'gate-entry.png',
        'character-hall.png',
        'world-hud.png',
        'first-playable-loop-feedback.png',
        'visual-evidence-summary.json',
        'visual-evidence-summary.txt',
        'humanVisualAcceptancePending',
        'API status',
        'Character Hall',
        'Create Character',
        'World HUD',
        'Save Position',
        'Back to Lobby',
        'Objective',
        'Interact prompt',
        'Quit',
        'Escape',
    )
    require_any(
        'client/Unity/Assets/Game/UI/Runtime/M5VisualEvidenceRunner.cs',
        ('Open Gate', 'Enter World'),
        ('Vào Thế Giới', 'Vào sân luyện'),
    )
    require('client/Unity/Assets/Game/Bootstrap/Runtime/GameBootstrap.cs', 'M5VisualEvidenceRunner.ShouldRun()')
    require('client/Unity/Assets/Game/Bootstrap/Runtime/GameBootstrap.cs', 'VisualRuntimeEvidenceRunner.ShouldRun()')
    require_file('tools/lgo_visual_runtime_review.sh', executable=True)
    require(
        'tools/lgo_visual_runtime_review.sh',
        'LGO_VISUAL_RUNTIME_REVIEW_PHASE',
        '--lgo-visual-runtime-review',
        '--lgo-visual-runtime-evidence-dir',
        'build/visual-evidence/latest',
        '"-screen-width", screen_width',
        '"-screen-height", screen_height',
        'SCREEN_WIDTH="${LGO_VISUAL_RUNTIME_WIDTH:-1920}"',
        'SCREEN_HEIGHT="${LGO_VISUAL_RUNTIME_HEIGHT:-1080}"',
        'LGO_VISUAL_RUNTIME_EVIDENCE_READY',
        'LGO_VISUAL_RUNTIME_PASS_NOT_CLAIMED',
    )
    require_file('tools/run_m5_visual_evidence_review.sh', executable=True)
    require(
        'tools/run_m5_visual_evidence_review.sh',
        '--rebuild',
        '--open-existing',
        'LGO_PLAYABLE_VISUAL_EVIDENCE_READY',
        'LGO_PLAYABLE_VISUAL_EVIDENCE_SCREENSHOT_UNAVAILABLE',
        'visual-evidence-summary.json',
        "'-screen-width', '1280'",
        "'-screen-height', '720'",
        'build/visual-evidence/m5-latest',
    )
    require('docs/tasks/M5-VISUAL-EVIDENCE-AND-UX-ACCEPTANCE-v0.16.0.md', 'M5_VISUAL_EVIDENCE_UX_REVIEW_READY_RUNTIME_CLOSED_LOCAL', 'visual evidence', 'human visual acceptance')
    require('docs/execution/LGO-VISUAL-EVIDENCE-REVIEW-COMMAND-v0.16.0.md', './tools/run_m5_visual_evidence_review.sh --rebuild', 'build/visual-evidence')
    require('tools/lgo_playable_closure_check.sh', '--visual-evidence', 'validate_m5_visual_evidence.py', 'lgo_visual_runtime_review.sh', 'LGO_PLAYABLE_VISUAL_RUNTIME_EVIDENCE_READY')
    require('README.md', 'M5_VISUAL_EVIDENCE_UX_REVIEW_READY')
    require('START-HERE.md', 'M5 Visual Evidence UX Acceptance')
    require('VERSIONING.md', 'source_package_version = m6-combat-foundation-v0.55.0', 'client_version = 0.6.0-m6-combat-foundation', 'm6_governance_baseline = M6_COMBAT_READINESS_SPEC_CLOSED_v0.32.0', 'M5_VISUAL_EVIDENCE_UX_REVIEW_READY')
    require('docs/execution/PROJECT-STATE.md', 'M5_VISUAL_EVIDENCE_UX_REVIEW_READY')

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
        print('M5 VISUAL EVIDENCE VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M5 VISUAL EVIDENCE VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
