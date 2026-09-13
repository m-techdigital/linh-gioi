"""Pack canonical pose review images without promoting them to runtime assets."""

import argparse
import hashlib
import json
from functools import lru_cache
from pathlib import Path

from PIL import Image
from pack_lgo_vo_lv1_map_avatar import CANVAS, project_canvas_rect
from validate_lgo_outfit_surface_contract import validate_contract


OUTFIT_REVIEW_DIRS = {
    'main-weapon-review',
    'head-hair-review',
    'inner-top-review',
    'outer-top-review',
    'lower-body-review',
    'waist-belt-review',
    'arm-guard-review',
    'footwear-review',
    'shoulder-chest-guard-review',
    'class-accessory-review',
}
OUTFIT_SOURCE_MARKERS = {'slotId', 'reviewSlot', 'itemId', 'layerOrderProfile', 'fitFamily'}
REJECTED_SELECTION_MARKERS = ('REJECTED', 'WITHDRAWN')


def is_outfit_review_pack(sources: list[dict], output_dir: Path) -> bool:
    if output_dir.name in OUTFIT_REVIEW_DIRS:
        return True
    return any(any(marker in source for marker in OUTFIT_SOURCE_MARKERS) for source in sources)


def validate_selectable_sources(sources: list[dict]) -> None:
    """Block rejected evidence at the pack boundary, regardless of caller flags."""
    for source in sources:
        path = Path(source['source']).resolve()
        if 'rejected-evidence' in {part.lower() for part in path.parts}:
            raise ValueError('Source is quarantined rejected-evidence: ' + str(path))
        for parent in path.parents:
            for marker_name in ('DO-NOT-SELECT.md', 'DO-NOT-PACK.md'):
                marker = parent / marker_name
                if marker.is_file():
                    raise ValueError(f'Source is blocked by {marker_name}: {marker}')
            selection_path = parent / 'authoring-selection.json'
            if not selection_path.is_file():
                continue
            selection = json.loads(selection_path.read_text(encoding='utf-8'))
            status = str(selection.get('status', '')).upper()
            if any(marker in status for marker in REJECTED_SELECTION_MARKERS):
                raise ValueError(f'Source authoring selection is {status}: {selection_path}')


def compact_rows(unique, width):
    """Exact shelf partition for small pose sets; no sprite resampling or rotation."""
    items = list(unique.items())
    count = len(items)
    if count > 10:
        return None
    rows = {}
    for mask in range(1, 1 << count):
        members = [i for i in range(count) if mask & (1 << i)]
        if 2 + sum(items[i][1].width + 2 for i in members) <= width:
            rows[mask] = max(items[i][1].height for i in members) + 2

    @lru_cache(None)
    def partition(mask):
        if not mask:
            return 0, ()
        anchor = mask & -mask
        best = (float('inf'), ())
        subset = mask
        while subset:
            if subset & anchor and subset in rows:
                height, tail = partition(mask ^ subset)
                candidate = (rows[subset] + height, (subset, *tail))
                if candidate < best:
                    best = candidate
            subset = (subset - 1) & mask
        return best

    height, groups = partition((1 << count) - 1)
    if height == float('inf'):
        return None
    positions, y = {}, 2
    for group in groups:
        x = 2
        for i, (key, sprite) in enumerate(items):
            if group & (1 << i):
                positions[key] = [x, y, sprite.width, sprite.height]
                x += sprite.width + 2
        y += rows[group]
    return 1 << (y - 1).bit_length(), positions


def pack_review(sources, divisor=4, max_side=1024, *, allow_empty_components=False):
    if not sources:
        raise ValueError('Source list is empty')
    validate_selectable_sources(sources)
    if not isinstance(divisor, int) or divisor <= 0 or any(n % divisor for n in CANVAS):
        raise ValueError('Divisor must divide the canonical canvas')
    ids, unique, sprites = set(), {}, []
    for source in sources:
        component = source.get('componentId') or 'main'
        pose_key = (component, source['id'])
        if pose_key in ids:
            raise ValueError('Duplicate pose id: ' + source['id'])
        ids.add(pose_key)
        path = Path(source['source'])
        digest = hashlib.sha256(path.read_bytes()).hexdigest()
        if digest != source['sourceSha256']:
            raise ValueError('Source fingerprint changed: ' + str(path))
        with Image.open(path) as opened:
            if opened.size != CANVAS:
                raise ValueError('Source must use canonical canvas: ' + str(path))
            if 'A' not in opened.getbands():
                raise ValueError('Source PNG must contain explicit alpha: ' + str(path))
            if opened.getchannel('A').getextrema()[0] == 255:
                raise ValueError('Source sprite must contain transparent background: ' + str(path))
            sampled = opened.convert('RGBA').resize(
                tuple(n // divisor for n in CANVAS), Image.Resampling.LANCZOS)
        box = sampled.getchannel('A').getbbox()
        empty = box is None
        if box is None:
            if not allow_empty_components or not source.get('componentId'):
                raise ValueError('Source sprite is empty: ' + str(path))
            # A fully occluded component still has a pose entry. Keep it genuinely
            # transparent; never invent a visible pixel to satisfy the frame count.
            box = (0, 0, 1, 1)
        trimmed = sampled.crop(box)
        key = (trimmed.size, hashlib.sha256(trimmed.tobytes()).hexdigest())
        unique.setdefault(key, trimmed)
        part = {'id': source['id'], 'source': str(path), 'sourceSha256': digest,
                **project_canvas_rect([n * divisor for n in box]), '_key': key}
        if source.get('componentId'):
            part['componentId'] = component
            if 'order' in source:
                part['order'] = source['order']
        if empty:
            part['emptyComponent'] = True
        sprites.append(part)

    # Uniform sampling precedes trimming. Atlas placement never changes world placement.
    layouts = []
    width = 1
    while width <= max_side:
        x = y = 2
        row_height = 0
        positions = {}
        for key, sprite in sorted(unique.items(), key=lambda item: -item[1].height):
            if sprite.width + 4 > width:
                break
            if x + sprite.width + 2 > width:
                x, y, row_height = 2, y + row_height + 2, 0
            positions[key] = [x, y, sprite.width, sprite.height]
            x += sprite.width + 2
            row_height = max(row_height, sprite.height)
        else:
            height = 1 << (y + row_height + 1).bit_length()
            if height <= max_side:
                layouts.append((width * height, width, height, positions))
        width *= 2
    # Keep existing layout bytes when equally compact; optimize only wasted atlas area.
    width = 1
    while width <= max_side:
        compact = compact_rows(unique, width)
        if compact is not None:
            height, positions = compact
            if height <= max_side:
                layouts.append((width * height, width, height, positions))
        width *= 2
    if not layouts:
        raise ValueError('Atlas budget exceeded; increase budget explicitly, never silently resample')
    _, width, height, positions = min(layouts, key=lambda layout: layout[:3])
    atlas = Image.new('RGBA', (width, height))
    for key, sprite in unique.items():
        atlas.paste(sprite, tuple(positions[key][:2]))
    for sprite in sprites:
        sprite['atlasRectTopLeft'] = positions[sprite.pop('_key')]
    return atlas, {'runtimeEligible': False, 'status': 'REVIEW_ONLY',
                   'sourceCanvas': list(CANVAS), 'samplingDivisor': divisor,
                   'atlasSize': [width, height], 'rgba8Bytes': width * height * 4,
                   'uniqueSprites': len(unique), 'sprites': sprites}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--sources', type=Path, required=True, help='JSON list of id/source/sourceSha256')
    parser.add_argument('--output-dir', type=Path, required=True)
    parser.add_argument('--divisor', type=int, default=4)
    parser.add_argument('--max-side', type=int, default=1024)
    parser.add_argument('--allow-empty-components', action='store_true',
                        help='Allow transparent poses only for explicitly named item components')
    parser.add_argument('--jump-pivot-source', type=int, nargs=2, metavar=('X', 'Y'),
                        help='Registered source-space pivot for jump_tuck; never inferred from its trim')
    parser.add_argument('--surface-contract', type=Path,
                        help='Required for outfit review packs; must validate PASS and source artifact accepted before pack')
    args = parser.parse_args()
    sources = json.loads(args.sources.read_text())
    contract_report = None
    if is_outfit_review_pack(sources, args.output_dir):
        if args.surface_contract is None:
            parser.error('surface contract required for outfit review pack')
        contract_report = validate_contract(args.surface_contract)
        if contract_report['status'] != 'PASS':
            parser.error('Surface contract not ready for pack: ' + contract_report['status'])
    elif args.surface_contract is not None:
        contract_report = validate_contract(args.surface_contract)
        if contract_report['status'] != 'PASS':
            parser.error('Surface contract not ready for pack: ' + contract_report['status'])
    has_jump = any(source['id'] == 'jump_tuck' for source in sources)
    if has_jump and args.jump_pivot_source is None:
        parser.error('jump pivot required for jump_tuck (--jump-pivot-source X Y)')
    if args.jump_pivot_source is not None and (
            not has_jump or any(value < 0 or value > limit
                                for value, limit in zip(args.jump_pivot_source, CANVAS))):
        parser.error('jump pivot must lie on canonical canvas and requires jump_tuck')
    atlas, report = pack_review(sources, args.divisor, args.max_side,
                                allow_empty_components=args.allow_empty_components)
    if has_jump:
        report['jumpPivotSource'] = args.jump_pivot_source
    if contract_report is not None:
        report['surfaceContractStatus'] = contract_report['status']
        report['surfaceContractDeclarationStatus'] = contract_report['declarationStatus']
        report['sourceArtifactStatus'] = contract_report['sourceArtifactStatus']
        report['sourceArtifactValid'] = contract_report['sourceArtifactValid']
    args.output_dir.mkdir(parents=True, exist_ok=False)
    atlas_path = args.output_dir / 'atlas-review.png'
    atlas.save(atlas_path, optimize=True)
    report['pngBytes'] = atlas_path.stat().st_size
    report['atlasSha256'] = hashlib.sha256(atlas_path.read_bytes()).hexdigest()
    (args.output_dir / 'atlas-review.json').write_text(json.dumps(report, indent=2) + '\n')
    print(json.dumps({key: report[key] for key in ('status', 'atlasSize', 'rgba8Bytes', 'pngBytes')}))


if __name__ == '__main__':
    main()
