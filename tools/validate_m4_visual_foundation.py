#!/usr/bin/env python3
from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []

REQUIRED_DIRS = [
    'client/Unity/Assets/Game/Art',
    'client/Unity/Assets/Game/Art/Characters',
    'client/Unity/Assets/Game/Art/NPCs',
    'client/Unity/Assets/Game/Art/Monsters',
    'client/Unity/Assets/Game/Art/Items',
    'client/Unity/Assets/Game/Art/Skills',
    'client/Unity/Assets/Game/Art/Maps',
    'client/Unity/Assets/Game/Art/UI',
    'client/Unity/Assets/Game/Art/VFX',
    'client/Unity/Assets/Game/Art/Shared',
]

REQUIRED_ASSETS = [
    'client/Unity/Assets/Game/Art/Characters/lgo_character_hero_sword_placeholder.svg',
    'client/Unity/Assets/Game/Art/NPCs/lgo_npc_keeper_placeholder.svg',
    'client/Unity/Assets/Game/Art/Monsters/lgo_monster_shadow_slime_placeholder.svg',
    'client/Unity/Assets/Game/Art/Items/lgo_item_iron_sword_icon.svg',
    'client/Unity/Assets/Game/Art/Items/lgo_item_spirit_stone_icon.svg',
    'client/Unity/Assets/Game/Art/Items/lgo_item_healing_gourd_icon.svg',
    'client/Unity/Assets/Game/Art/Skills/lgo_skill_wind_slash_icon.svg',
    'client/Unity/Assets/Game/Art/Skills/lgo_skill_shadow_bind_icon.svg',
    'client/Unity/Assets/Game/Art/Skills/lgo_skill_spirit_guard_icon.svg',
    'client/Unity/Assets/Game/Art/Maps/lgo_map_training_ground_tile.svg',
    'client/Unity/Assets/Game/Art/UI/lgo_ui_frame_panel.svg',
    'client/Unity/Assets/Game/Art/UI/lgo_ui_button_rune.svg',
    'client/Unity/Assets/Game/Art/UI/lgo_ui_status_api_icon.svg',
    'client/Unity/Assets/Game/Art/VFX/lgo_vfx_spirit_burst_marker.svg',
    'client/Unity/Assets/Game/Art/Shared/lgo_shared_palette_motifs.svg',
]


def read(path: str) -> str:
    target = ROOT / path
    if not target.exists():
        errors.append(f'missing: {path}')
        return ''
    return target.read_text(encoding='utf-8')


def require_contains(path: str, marker: str) -> None:
    if marker not in read(path):
        errors.append(f'{path} missing marker: {marker}')


def main() -> int:
    for rel in REQUIRED_DIRS:
        path = ROOT / rel
        if not path.is_dir():
            errors.append(f'missing directory: {rel}')
        if not (ROOT / (rel + '.meta')).exists():
            errors.append(f'missing Unity meta for directory: {rel}.meta')

    manifest = read('docs/art/LGO-RUNTIME-ART-MANIFEST-v0.10.0.md')
    guide = read('docs/art/LGO-VISUAL-IDENTITY-GUIDE-v0.10.0.md')
    if 'M4_VISUAL_PLACEHOLDER_FOUNDATION_SOURCE_READY' not in guide:
        errors.append('visual identity guide missing M4 status marker')

    for rel in REQUIRED_ASSETS:
        path = ROOT / rel
        if not path.exists():
            errors.append(f'missing asset: {rel}')
            continue
        if path.stat().st_size > 32_768:
            errors.append(f'placeholder asset too large: {rel}')
        if not (ROOT / (rel + '.meta')).exists():
            errors.append(f'missing Unity meta: {rel}.meta')
        if rel not in manifest:
            errors.append(f'manifest missing asset entry: {rel}')
        content = path.read_text(encoding='utf-8')
        if '<svg' not in content or '</svg>' not in content:
            errors.append(f'asset is not valid simple SVG text: {rel}')

    for rel in REQUIRED_ASSETS:
        if not re.match(r'^client/Unity/Assets/Game/Art/[A-Za-z]+/lgo_[a-z0-9_]+\.svg$', rel):
            errors.append(f'asset naming rule violation: {rel}')

    require_contains('client/Unity/Assets/Game/Art/Runtime/RuntimeArtCatalog.cs', 'Version = "0.10.0"')
    require_contains('client/Unity/Assets/Game/Art/Runtime/M4VisualFoundationSmokeRunner.cs', '--lgo-m4-visual-foundation-smoke')
    require_contains('client/Unity/Assets/Game/Bootstrap/Runtime/GameBootstrap.cs', 'M4VisualFoundationSmokeRunner.ShouldRun()')
    require_contains('client/Unity/Assets/Game/World/Runtime/PlayableWorldController.cs', 'LGO NPC Keeper Placeholder')
    require_contains('client/Unity/Assets/Game/UI/Runtime/M4PlayableClientController.cs', 'RuntimeArtCatalog')
    require_contains('tools/run_m4_visual_foundation_once.sh', 'M4_VISUAL_PLACEHOLDER_FOUNDATION_SMOKE_PASS')

    forbidden_roots = [
        ROOT / 'client/Unity/Assets/Game/Art/Library',
        ROOT / 'client/Unity/Assets/Game/Art/Temp',
        ROOT / 'client/Unity/Assets/Game/Art/Logs',
        ROOT / 'client/Unity/Assets/Game/Art/build',
    ]
    for path in forbidden_roots:
        if path.exists():
            errors.append(f'forbidden generated/cache directory under art: {path.relative_to(ROOT)}')

    if errors:
        print('M4 VISUAL PLACEHOLDER FOUNDATION VALIDATION FAILED', file=sys.stderr)
        for error in errors:
            print(' - ' + error, file=sys.stderr)
        return 1
    print('M4 VISUAL PLACEHOLDER FOUNDATION VALIDATION PASS')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
