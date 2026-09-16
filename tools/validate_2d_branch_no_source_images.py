#!/usr/bin/env python3
from __future__ import annotations

from pathlib import Path
import sys
import hashlib
import json
import re
import struct

ROOT = Path(__file__).resolve().parents[1]
IMAGE_SUFFIXES = {
    '.png', '.jpg', '.jpeg', '.webp', '.bmp', '.gif', '.psd', '.tga', '.tif', '.tiff'
}
SKIP_PARTS = {
    '.git', 'build', 'Library', 'Temp', 'Obj', 'Logs', 'UserSettings', 'Packages'
}


RUNTIME_ART_PACKS = [
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/VoRegisteredEquipmentLv1',
        'id': 'vo-male-lv001-registered-equipment-draft',
        'status': 'DRAFT_REGISTERED_EQUIPMENT_REVIEW',
        'assets': {'equipment-atlas.png': (512, 512, 'registered-equipment-atlas')},
        'generators': {'registered_material_equipment_pack'},
        'max_bytes': 150_000,
        'status_error': 'Equipment remains opt-in draft pending Player and full class review',
    },
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/VoRegisteredFemaleEquipmentLv1',
        'id': 'vo-female-lv001-registered-equipment-draft',
        'status': 'DRAFT_REGISTERED_EQUIPMENT_REVIEW',
        'assets': {'equipment-atlas.png': (512, 512, 'registered-equipment-atlas')},
        'generators': {'registered_material_equipment_pack'},
        'max_bytes': 150_000,
        'status_error': 'Equipment remains opt-in draft pending Player and full class review',
    },
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/VoClosedBodyLv1',
        'id': 'vo-closed-body-lv1-integration-draft',
        'status': 'DRAFT_INTEGRATION_ONLY',
        'assets': {'closed-body-atlas.png': (512, 512, 'closed-base-body-atlas')},
        'generators': {'base_preserving_closed_body_pack'},
        'max_bytes': 250_000,
        'status_error': 'Closed body remains draft pending Player and equipment review',
    },
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/VoClosedFarArmsLv1',
        'id': 'vo-closed-far-arms-lv1-v1',
        'status': 'DRAFT_CLOSED_FAR_ARM_REVIEW',
        'assets': {'closed-far-arm-atlas.png': (256, 256, 'closed-base-arm-attachments')},
        'generators': {'base_preserving_hidden_extension_pack'},
        'max_bytes': 50_000,
        'status_error': 'Closed arms remain opt-in draft until Player and full body review',
    },
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/VoRegisteredLv1',
        'id': 'vo-registered-lv1-v1',
        'status': 'DRAFT_REGISTERED_BIND_REVIEW',
        'assets': {'registered-outfit-atlas.png': (1024, 1024, 'registered-paper-doll-atlas')},
        'generators': {'registered_aligned_source_repack'},
        'max_bytes': 150_000,
        'status_error': 'Registered Võ remains opt-in draft until motion and slot semantic review',
    },
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/VoLv1MapAvatarArt',
        'id': 'vo-lv1-30-map-avatar-v9',
        'status': 'DRAFT_RUNTIME_REVIEW',
        'assets': {
            'vo-lv1-map-avatar-atlas.png': (1024, 1024, 'two-gender-tier-equipment-atlas'),
            'vo-lv10-equipment-atlas.png': (1024, 1024, 'two-gender-tier-equipment-atlas'),
            'vo-lv20-equipment-atlas.png': (1024, 1024, 'two-gender-tier-equipment-atlas'),
            'vo-lv30-equipment-atlas.png': (1024, 1024, 'two-gender-tier-equipment-atlas'),
            'vo-lv1-motion-atlas.png': (1024, 1024, 'two-gender-motion-atlas'),
            'vo-lv1-female-motion-atlas.png': (1024, 1024, 'two-gender-motion-atlas'),
            'vo-lv1-rig-atlas.png': (1024, 1024, 'two-gender-skeletal-rig-atlas'),
        },
        'generators': {'reference_guided_imagegen_attachment_batch', 'reference_guided_imagegen_motion_batch',
                       'reference_guided_imagegen_rig_batch'},
        'max_bytes': 300_000,
        'status_error': 'Võ Lv1-30 avatar must remain draft until animated paper-doll attachment review',
    },
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/CongDongLamMap01AArt',
        'id': 'cong-dong-lam-map01a-art-draft-v1',
        'status': 'DRAFT_NOT_RUNTIME_APPROVED',
        'assets': {'far-background.png': (1024, 576, 'far-background'),
                   'props-atlas.png': (1024, 1024, 'runtime-atlas'),
                   'sheet-props.png': (256, 256, 'runtime-atlas'),
                   'modules-atlas.png': (1024, 1024, 'runtime-atlas'),
                   'npcs-atlas.png': (512, 512, 'runtime-atlas'),
                   'landmarks-atlas.png': (1024, 1024, 'runtime-atlas'),
                   'combat-atlas.png': (512, 512, 'runtime-atlas')},
        'generators': {'image_gen', 'extracted_owner_source'},
        'max_bytes': 6_000_000,
        'status_error': 'Map01A art must remain an explicit draft until Player review',
    },
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/CongDongLamMap01AItems',
        'id': 'map01a-item-icons-v1',
        'status': 'DRAFT_RUNTIME_REVIEW',
        'assets': {'map01a-item-icons.png': (512, 128, 'ui-item-atlas')},
        'generators': {'pack_lgo_map01a_item_icons'},
        'max_bytes': 80_000,
        'status_error': 'Map01A item icons must remain draft until owner visual review',
    },
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/CongDongLamMap01ACharacterEquipmentIcons',
        'id': 'map01a-character-equipment-icons-v1',
        'status': 'DRAFT_RUNTIME_REVIEW',
        'assets': {'map01a-character-equipment-icons.png': (640, 256, 'ui-equipment-icon-atlas')},
        'generators': {'image_gen_alpha_extraction'},
        'max_bytes': 250_000,
        'status_error': 'Map01A character equipment icons must remain draft until owner visual review',
    },
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/CongDongLamMap01ABagCategoryIcons',
        'id': 'map01a-bag-category-icons-v1',
        'status': 'DRAFT_RUNTIME_REVIEW',
        'assets': {'map01a-bag-category-icons.png': (640, 128, 'ui-bag-category-icon-atlas')},
        'generators': {'image_gen_alpha_fit'},
        'max_bytes': 100_000,
        'status_error': 'Map01A bag category icons must remain draft until owner visual review',
    },
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/CongDongLamMap01ASkillIcons',
        'id': 'map01a-skill-icons-v1',
        'status': 'DRAFT_RUNTIME_REVIEW',
        'assets': {'map01a-skill-icons.png': (512, 512, 'ui-skill-icon-atlas')},
        'generators': {'pack_lgo_skill_icons'},
        'ui_import_limits': {'map01a-skill-icons.png': 512},
        'max_bytes': 400_000,
        'status_error': 'Map01A skill icons must remain draft until owner visual review',
    },
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/CongDongLamMap01APotentialIcons',
        'id': 'map01a-potential-icons-v1',
        'status': 'DRAFT_RUNTIME_REVIEW',
        'assets': {'map01a-potential-icons.png': (512, 256, 'ui-potential-icon-atlas')},
        'generators': {'build_lgo_character_hub_skin'},
        'ui_import_limits': {'map01a-potential-icons.png': 512},
        'max_bytes': 200_000,
        'status_error': 'Map01A potential icons must remain draft until owner visual review',
    },
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/CongDongLamMap01AHudIcons',
        'id': 'map01a-hud-icons-v1',
        'status': 'DRAFT_RUNTIME_REVIEW',
        'assets': {'map01a-hud-icons.png': (512, 512, 'ui-hud-atlas')},
        'generators': {'pack_lgo_map01a_hud_icons'},
        'max_bytes': 140_000,
        'status_error': 'Map01A HUD icons must remain draft until owner visual review',
    },
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/CongDongLamMap01ACharacterHub',
        'id': 'map01a-character-hub-art-v1',
        'status': 'DRAFT_RUNTIME_REVIEW',
        'assets': {
            'spirit-fox-preview.png': (1024, 704, 'character-hub-preview-atlas'),
            'spirit-fox-portrait.png': (192, 192, 'character-hub-spirit-pet-portrait'),
        },
        'generators': {'image_gen', 'deterministic crop from existing provenance-backed runtime hero'},
        'max_bytes': 1_000_000,
        'status_error': 'Map01A character-hub preview art must remain draft until Player visual review',
    },
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/CongDongLamMap01AUiSkin',
        'id': 'map01a-character-hub-chrome-v2',
        'status': 'DRAFT_RUNTIME_REVIEW',
        'assets': {
            'character-hub-surface.png': (1024, 676, 'ui-modal-surface'),
            'character-hub-panel-surface.png': (512, 512, 'ui-panel-surface'),
            'character-hub-tab-idle.png': (320, 72, 'ui-tab-idle'),
            'character-hub-tab-selected.png': (320, 72, 'ui-tab-selected'),
            'character-hub-action-blue.png': (384, 72, 'ui-action-blue'),
            'character-hub-action-gold.png': (384, 72, 'ui-action-gold'),
            'character-hub-close.png': (96, 96, 'ui-close-frame'),
            'character-hub-potential-topology.png': (600, 520, 'ui-potential-topology-template'),
            'character-hub-potential-core.png': (224, 224, 'ui-potential-meditation-core'),
        },
        'generators': {'build_lgo_character_hub_skin'},
        'ui_import_limits': {
            'character-hub-surface.png': 1024,
            'character-hub-panel-surface.png': 512,
            'character-hub-tab-idle.png': 512,
            'character-hub-tab-selected.png': 512,
            'character-hub-action-blue.png': 512,
            'character-hub-action-gold.png': 512,
            'character-hub-close.png': 128,
            'character-hub-potential-topology.png': 1024,
            'character-hub-potential-core.png': 512,
        },
        'max_bytes': 750_000,
        'status_error': 'Map01A character-hub chrome must remain draft until owner Player visual review',
    },
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/DongMonIllustrated',
        'id': 'dongmon-illustrated-draft-v1',
        'status': 'DRAFT_OWNER_REVIEW',
        'assets': {
            'skyline.png': (1536, 864, 'far-background'),
            'props-atlas.png': (2048, 2048, 'runtime-atlas'),
        },
        'generators': {'image_gen'},
        'max_bytes': 4_000_000,
        'status_error': 'Pack must remain an explicit draft',
    },
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/VoLv1ApprovedRuntimeArt',
        'id': 'vo-lv1-approved-runtime-art-v1',
        'status': 'APPROVED_RUNTIME_ART',
        'assets': {
            'vo-lv1-starter-atlas.png': (2048, 2048, 'paper-doll-atlas'),
            'vo-lv1-skill-atlas.png': (1024, 1024, 'skill-vfx-atlas'),
        },
        'generators': {'approved_original_2d_art', 'image_gen'},
        'max_bytes': 4_000_000,
        'status_error': 'Vo Lv1 runtime art must remain approved runtime art',
    },
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/KiemMixedLoadoutFitPreview',
        'id': 'kiem-lv1-30-equipment-runtime-v1',
        'status': 'DRAFT_RUNTIME_FIT',
        'assets': {
            'kiem-equipment-male-atlas.png': (1024, 1024, 'paper-doll-atlas'),
            'kiem-equipment-female-atlas.png': (1024, 1024, 'paper-doll-atlas'),
        },
        'generators': {'batch_pack_reviewed_source_items'},
        'max_bytes': 1_000_000,
        'status_error': 'Kiếm ten-slot fit pack must remain draft until Player motion review',
    },
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/PhapMixedLoadoutFitPreview',
        'id': 'phap-lv1-30-equipment-runtime-v1',
        'status': 'DRAFT_RUNTIME_FIT',
        'assets': {
            'phap-equipment-male-atlas.png': (1024, 1024, 'paper-doll-atlas'),
            'phap-equipment-female-atlas.png': (1024, 1024, 'paper-doll-atlas'),
        },
        'generators': {'pack_lgo_class_equipment_sheet'},
        'max_bytes': 1_000_000,
        'status_error': 'Pháp ten-slot fit pack must remain draft until Player motion review',
    },
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/CoMixedLoadoutFitPreview',
        'id': 'co-lv1-30-equipment-runtime-v1',
        'status': 'DRAFT_RUNTIME_FIT',
        'assets': {
            'co-equipment-male-atlas.png': (1024, 1024, 'paper-doll-atlas'),
            'co-equipment-female-atlas.png': (1024, 1024, 'paper-doll-atlas'),
        },
        'generators': {'pack_lgo_class_equipment_sheet'},
        'max_bytes': 1_000_000,
        'status_error': 'Cơ ten-slot fit pack must remain draft until Player motion review',
    },
    {
        'pack': 'client/Unity/Assets/Game/World/Runtime/Resources/LGOClasses/LinhMixedLoadoutFitPreview',
        'id': 'linh-lv1-30-equipment-runtime-v1',
        'status': 'DRAFT_RUNTIME_FIT',
        'assets': {
            'linh-equipment-male-atlas.png': (1024, 1024, 'paper-doll-atlas'),
            'linh-equipment-female-atlas.png': (1024, 1024, 'paper-doll-atlas'),
        },
        'generators': {'pack_lgo_class_equipment_sheet'},
        'max_bytes': 1_000_000,
        'status_error': 'Linh ten-slot fit pack must remain draft until Player motion review',
    },
]


def _expected_assets(spec: dict[str, object]) -> dict[str, tuple[int, int, str]]:
    pack = str(spec['pack'])
    assets = spec['assets']
    if not isinstance(assets, dict):
        raise ValueError('Invalid runtime art spec')
    return {pack + '/' + name: value for name, value in assets.items()}


def _skill_intake_spec(root: Path, spec: dict, data: dict) -> dict:
    """A larger Skill atlas requires declared, reviewed, known inner modules."""
    intake, parts = data['artworkIntake'], data.get('parts', [])
    if not isinstance(intake, dict) or not isinstance(parts, list):
        raise ValueError('Invalid Skill artwork intake')
    count = intake.get('contentCount')
    if type(count) is not int or not 1 <= count <= 36 or len(parts) != 13 + count:
        raise ValueError('Skill artwork count mismatch')
    review = intake.get('review', {})
    hashes = [intake.get('registrySha256'), review.get('sha256')]
    if review.get('status') != 'SELF_REVIEWED' or not review.get('evidence'):
        raise ValueError('Skill artwork review evidence required')
    if any(not isinstance(h, str) or not re.fullmatch('[0-9a-f]{64}', h) for h in hashes):
        raise ValueError('Skill intake must contain valid provenance hashes')
    if data.get('runtimeApproved') is not False or data.get('cellSize') != [128, 128]:
        raise ValueError('Skill intake is not an owner-approved release or a new cell profile')
    rows = json.loads((root / str(spec['pack']) / 'skill-library.json').read_text())['skills']
    known = {row['iconId'] for row in rows} | {'frame', 'category_active', 'category_passive', 'category_method'}
    keys = [part['id'] for part in parts]
    if len(keys) != len(set(keys)) or not set(keys) <= known or keys.count('frame') != 1:
        raise ValueError('Duplicate or unknown Skill module IDs')
    if any(part['role'] != ('shared-frame' if part['id'] == 'frame' else 'inner-symbol') for part in parts):
        raise ValueError('Only one frame master is allowed')
    sources = intake.get('sources', [])
    source_map = {source['id']: source for source in sources}
    if not sources or len(source_map) != len(sources):
        raise ValueError('Skill source IDs must be unique')
    for source in sources:
        size = source.get('size', [])
        if (not isinstance(source.get('sha256'), str) or not re.fullmatch('[0-9a-f]{64}', source['sha256'])
                or not source.get('path') or len(size) != 2
                or any(type(v) is not int or not 1 <= v <= 4096 for v in size)):
            raise ValueError('Invalid Skill source metadata')
    extras = [part for part in parts if 'sourceId' in part]
    if len(extras) != count or any(part['id'] == 'frame' for part in extras):
        raise ValueError('Skill intake may only add inner artwork')
    for part in extras:
        source, rect = source_map.get(part['sourceId']), part.get('sourceRect', [])
        if source is None or len(rect) != 4 or any(type(v) is not int for v in rect):
            raise ValueError('Missing source or invalid Skill rectangle')
        x, y, w, h = rect
        if w != h or w < 128 or min(x, y) < 0 or x+w > source['size'][0] or y+h > source['size'][1]:
            raise ValueError('Skill source rectangle is out of bounds or requires upscale')
    side = 512 if len(parts) <= 16 else 1024
    for index, part in enumerate(parts):
        expected = (index % (side // 128) * 128, side - (index // (side // 128) + 1) * 128, 128, 128)
        if tuple(part.get(key) for key in ('x', 'y', 'w', 'h')) != expected:
            raise ValueError('Skill atlas cells do not follow the common layout')
    budget = 400000 + 32768 * count
    if data.get('textureSize') != [side, side] or data.get('byteBudget') != budget:
        raise ValueError('Skill atlas size or derived byte budget mismatch')
    name = 'map01a-skill-icons.png'
    return dict(spec, assets={name: (side, side, 'ui-skill-icon-atlas')},
                max_bytes=budget, ui_import_limits={name: side})


def _equipment_intake_spec(spec: dict, data: dict) -> dict:
    """Allow additive item rows only with consistent identity/review/provenance metadata."""
    intakes, parts = data.get('itemArtworkIntakes', []), data.get('parts', [])
    if not intakes or data.get('runtimeApproved') is not False or data.get('cellSize', [128, 128]) != [128, 128]:
        raise ValueError('Invalid equipment intake state')
    height, added = 256, []
    for intake in intakes:
        keys = intake.get('addedIconIds', [])
        review = intake.get('review', {})
        hashes = [intake.get('registrySha256'), intake.get('baseSha256'), review.get('sha256')]
        if (intake.get('baseTextureSize') != [640, height] or not keys or not isinstance(keys, list)
                or review.get('status') != 'SELF_REVIEWED' or not review.get('evidence')
                or any(not isinstance(h, str) or not re.fullmatch('[0-9a-f]{64}', h) for h in hashes)):
            raise ValueError('Equipment source/review registration required')
        added.extend(keys)
        height += ((len(keys) + 4) // 5) * 128
    by_id = {p['id']: p for p in parts}
    if (height > 1024 or data.get('textureSize') != [640, height] or len(added) != len(set(added))
            or len(parts) != len(by_id) or len(parts) != intakes[0]['basePartCount'] + len(added)):
        raise ValueError('Invalid equipment grid or duplicate content')
    for intake in intakes:
        for index, key in enumerate(intake['addedIconIds']):
            p = by_id.get(key, {})
            rect = (index % 5 * 128, height - intake['baseTextureSize'][1] - (index // 5 + 1) * 128, 128, 128)
            if p.get('role') != 'item-content' or tuple(p.get(k) for k in ('x','y','w','h')) != rect:
                raise ValueError('Equipment content does not use the common grid')
    bindings, designs = data.get('itemBindings', []), data.get('itemDesignBindings', [])
    fields = ('classId','itemId','slot','gender','level','iconId')
    if (len(bindings) != len(added) or len(designs) != len(added)
            or {b['iconId'] for b in bindings} != set(added)
            or len({tuple(b.get(k) for k in fields[:-1]) for b in bindings}) != len(bindings)):
        raise ValueError('Equipment artwork must have exact unique item ownership')
    for binding in bindings:
        matches = [d for d in designs if all(d.get(k) == binding.get(k) for k in fields)]
        if (len(matches) != 1 or binding.get('classId') not in {'vo','kiem','phap','co','linh'}
                or binding.get('gender') not in {'male','female'} or type(binding.get('level')) is not int
                or not 1 <= binding['level'] <= 100):
            raise ValueError('Item/design identity mismatch')
        if any(not re.fullmatch('[0-9a-f]{64}', str(matches[0].get(k, '')))
               for k in ('sourceSha256','designSha256')):
            raise ValueError('Missing item/design hash')
    name = 'map01a-character-equipment-icons.png'
    return dict(spec, assets={name:(640,height,'ui-equipment-icon-atlas')},
                generators={'pack_lgo_equipment_item_icons'}, ui_import_limits={name:1024})


def _validate_runtime_pack(root: Path, spec: dict[str, object]) -> set[str]:
    pack = str(spec['pack'])
    manifest = root / pack / 'manifest.json'
    if not manifest.exists():
        return set()
    data = json.loads(manifest.read_text())
    if spec["id"] == "map01a-skill-icons-v1" and "artworkIntake" in data:
        spec = _skill_intake_spec(root, spec, data)
    if spec["id"] == "map01a-character-equipment-icons-v1" and "itemArtworkIntakes" in data:
        spec = _equipment_intake_spec(spec, data)
    expected = _expected_assets(spec)
    if data['id'] != spec['id'] or data['status'] != spec['status']:
        raise ValueError(str(spec['status_error']))
    entries = data['assets']
    if len(entries) != len(expected) or {a['path'] for a in entries} != set(expected):
        raise ValueError('Only declared runtime textures are allowed for ' + pack)
    generators = spec['generators']
    if not isinstance(generators, set):
        raise ValueError('Invalid runtime art generators')
    for entry in entries:
        path = root / entry['path']
        if path.is_symlink() or not path.is_file():
            raise ValueError('Missing or symlinked runtime texture: ' + entry['path'])
        raw = path.read_bytes()
        w, h, role = expected[entry['path']]
        if (entry['generator'] not in generators or entry['referenceOnly'] is not False
                or entry['role'] != role or len(raw) > int(spec['max_bytes'])
                or hashlib.sha256(raw).hexdigest() != entry['sha256']
                or raw[:8] != b'\x89PNG\r\n\x1a\n'
                or struct.unpack('>II', raw[16:24]) != (w, h)):
            raise ValueError('Invalid provenance/hash/PNG budget: ' + entry['path'])
    import_limits = spec.get('ui_import_limits', {})
    for name, max_size in import_limits.items():
        meta = root / pack / (name + '.meta')
        content = meta.read_text()
        limits = [int(value) for value in re.findall(r'^\s*maxTextureSize:\s*(\d+)\s*$', content, re.MULTILINE)]
        if ('enableMipMap: 0' not in content or 'nPOTScale: 0' not in content
                or not limits or any(value != max_size for value in limits)):
            raise ValueError('Invalid UI texture import policy: ' + pack + '/' + name)
    return set(expected)


def runtime_allowlist(root: Path) -> set[str]:
    allowed: set[str] = set()
    for spec in RUNTIME_ART_PACKS:
        try:
            allowed.update(_validate_runtime_pack(root, spec))
        except (KeyError, TypeError, OSError, json.JSONDecodeError, struct.error) as exc:
            raise ValueError('Invalid runtime art manifest: ' + str(exc)) from exc
    return allowed

def main() -> int:
    try:
        allowed = runtime_allowlist(ROOT)
    except ValueError as exc:
        print('LGO_2D_BRANCH_NO_SOURCE_IMAGES_FAIL', str(exc))
        return 1
    found: list[str] = []
    for path in ROOT.rglob('*'):
        if not path.is_file():
            continue
        rel = path.relative_to(ROOT)
        if any(part in SKIP_PARTS for part in rel.parts):
            continue
        if path.suffix.lower() in IMAGE_SUFFIXES and rel.as_posix() not in allowed:
            found.append(rel.as_posix())

    if found:
        print('LGO_2D_BRANCH_NO_SOURCE_IMAGES_FAIL')
        for rel in sorted(found)[:240]:
            print(rel)
        if len(found) > 240:
            print(f'... and {len(found) - 240} more')
        return 1

    print('LGO_2D_BRANCH_NO_SOURCE_IMAGES_PASS')
    return 0


if __name__ == '__main__':
    sys.exit(main())
