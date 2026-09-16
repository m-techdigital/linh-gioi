#!/usr/bin/env python3
"""Append registered item content to the existing UI atlas, never class-specific UI."""
from __future__ import annotations
import argparse
import copy
import hashlib
import io
import json
import re
from pathlib import Path
from PIL import Image

PNG = 'map01a-character-equipment-icons.png'
IDENTITY = ('classId', 'itemId', 'slot', 'gender', 'level')
VIEWPORT = (64, 64, 320, 320)
PROFILE = 'lgo-item-content384-center256-to120-cell128-v1'
MAX_BYTES = 250_000


def sha(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def identity(item: dict) -> tuple:
    for field in (*IDENTITY[:-1], 'iconId'):
        if not isinstance(item.get(field), str) or not re.fullmatch(r'[a-z][a-z0-9_-]*', item[field]):
            raise ValueError('Invalid exact identity field: ' + field)
    if item['classId'] not in {'vo', 'kiem', 'phap', 'co', 'linh'} or item['gender'] not in {'male', 'female'}:
        raise ValueError('Unknown class or gender')
    if type(item.get('level')) is not int or not 1 <= item['level'] <= 100:
        raise ValueError('Invalid level')
    return tuple(item[k] for k in IDENTITY)


def read_art(item: dict, root: Path) -> Image.Image:
    source = (root / item['source']).resolve()
    design = (root / item['designSource']).resolve()
    if sha(source) != item['sourceSha256'] or sha(design) != item['designSha256']:
        raise ValueError('Artwork or design hash mismatch')
    rect = item.get('designRect')
    with Image.open(design) as reference:
        if (not isinstance(rect, list) or len(rect) != 4 or any(type(v) is not int for v in rect)
                or min(rect) < 0 or rect[2] <= 0 or rect[3] <= 0
                or rect[0] + rect[2] > reference.width or rect[1] + rect[3] > reference.height):
            raise ValueError('Invalid design rect [x,y,width,height]')
    with Image.open(source) as image:
        if image.mode != 'RGBA' or image.size != (384, 384):
            raise ValueError('Source must be native RGBA 384x384')
        art = image.copy()
    alpha = art.getchannel('A')
    bounds = alpha.point(lambda a: 255 if a > 8 else 0).getbbox()
    if not bounds or bounds[0] < 64 or bounds[1] < 64 or bounds[2] > 320 or bounds[3] > 320:
        raise ValueError('Artwork exceeds the shared fixed viewport or is empty')
    return art.crop(VIEWPORT).resize((120, 120), Image.Resampling.LANCZOS)


def encode_rgb(image: Image.Image, config: dict) -> tuple[Image.Image, dict]:
    """Opt-in bounded RGB reduction; alpha and coordinates are never quantized."""
    if (not isinstance(config, dict) or set(config) != {'rgbStep'}
            or type(config['rgbStep']) is not int or config['rgbStep'] not in (1, 4)):
        raise ValueError('Encoding must explicitly use rgbStep 1 or 4; alpha reduction is forbidden')
    if config['rgbStep'] == 1:
        return image, {}
    lut = [min(255, ((value + 2) // 4) * 4) for value in range(256)]
    r, g, b, a = image.split()
    encoded = Image.merge('RGBA', (r.point(lut), g.point(lut), b.point(lut), a))
    return encoded, {'profile': 'rgba8-rgb-round4-alpha-exact-v1', 'rgbStep': 4,
                     'maxChannelError': 2, 'alphaExact': True}


def run(base: Path, registry: Path, output: Path) -> dict:
    base, registry, output = Path(base).resolve(), Path(registry).resolve(), Path(output).resolve()
    if output == base or output.exists():
        raise ValueError('Use a new candidate output directory; never overwrite a valid atlas')
    manifest = json.loads((base / 'manifest.json').read_text())
    if sha(base / PNG) != manifest['sha256']:
        raise ValueError('Base PNG provenance mismatch')
    with Image.open(base / PNG) as original:
        old = original.convert('RGBA')
    if old.width != 640 or old.height % 128 or old.height > 1024:
        raise ValueError('Unsupported base grid')
    parts = manifest['parts']
    ids = {p['id'] for p in parts}
    if len(ids) != len(parts):
        raise ValueError('Duplicate base sprite IDs')
    for part in parts:
        if (any(type(part.get(k)) is not int for k in ('x', 'y', 'w', 'h'))
                or min(part['x'], part['y']) < 0 or part['w'] <= 0 or part['h'] <= 0
                or part['x'] + part['w'] > old.width or part['y'] + part['h'] > old.height):
            raise ValueError('Invalid existing sprite rect')
    bindings = manifest.get('itemBindings', [])
    keys = set()
    for binding in bindings:
        key = identity(binding)
        if key in keys or binding['iconId'] not in ids:
            raise ValueError('Invalid existing item registration')
        keys.add(key)
    document = json.loads(registry.read_text())
    review = document.get('review', {})
    if (review.get('status') != 'SELF_REVIEWED' or not review.get('evidence')
            or sha(registry.parent / review['evidence']) != review.get('sha256')):
        raise ValueError('A verified source review is required; this is not owner acceptance')
    items = document.get('items', [])
    if not items or not isinstance(items, list):
        raise ValueError('Registry must contain new reviewed items')
    prepared = []
    for item in items:
        key = identity(item)
        if key in keys or item['iconId'] in ids:
            raise ValueError('Duplicate item identity or sprite ID')
        keys.add(key); ids.add(item['iconId'])
        prepared.append((item, read_art(item, registry.parent)))
    extra_height = ((len(prepared) + 4) // 5) * 128
    if old.height + extra_height > 1024:
        raise ValueError('Atlas dimension budget exceeded')
    atlas = Image.new('RGBA', (640, old.height + extra_height))
    atlas.paste(old, (0, 0))
    result = copy.deepcopy(manifest)
    for part in result['parts']:
        part['y'] += extra_height
    for index, (item, art) in enumerate(prepared):
        x, top = index % 5 * 128, old.height + index // 5 * 128
        atlas.paste(art, (x + 4, top + 4))
        result['parts'].append({'id': item['iconId'], 'x': x, 'y': atlas.height - top - 128,
                                'w': 128, 'h': 128, 'role': 'item-content',
                                'sourceSha256': item['sourceSha256'], 'sourceSpaceProfile': PROFILE})
        result.setdefault('itemBindings', []).append({k: item[k] for k in (*IDENTITY, 'iconId')})
        result.setdefault('itemDesignBindings', []).append({k: item[k] for k in
            (*IDENTITY, 'iconId', 'source', 'sourceSha256', 'designSource', 'designSha256', 'designRect')})
    atlas, encoding = encode_rgb(atlas, document.get('encoding', {'rgbStep': 1}))
    buffer = io.BytesIO(); atlas.save(buffer, format='PNG', optimize=True)
    data = buffer.getvalue()
    if len(data) > MAX_BYTES:
        raise ValueError('PNG byte budget exceeded')
    digest = hashlib.sha256(data).hexdigest()
    result.update(status='DRAFT_RUNTIME_REVIEW', runtimeApproved=False,
                  textureSize=list(atlas.size), pngBytes=len(data), sha256=digest,
                  itemBindingStatus='PARTIAL_REVIEWED_ITEM_IDENTITIES')
    for asset in result.get('assets', []):
        if Path(asset['path']).name == PNG:
            asset.update(sha256=digest, generator='pack_lgo_equipment_item_icons')
    result.setdefault('itemArtworkIntakes', []).append({
        'registry': str(registry), 'registrySha256': sha(registry), 'review': review,
        'baseSha256': manifest['sha256'], 'baseTextureSize': list(old.size), 'basePartCount': len(manifest['parts']),
        'addedIconIds': [item['iconId'] for item, _ in prepared],
        'profile': PROFILE, 'sourceCanvas': [384, 384], 'sourceViewport': list(VIEWPORT),
        'outputInset': 4, 'contentSize': [120, 120],
    })
    if encoding:
        result['itemArtworkIntakes'][-1]['encoding'] = encoding
    # Validation and PNG encoding finish before any output is created.
    encoded = json.dumps(result, ensure_ascii=False, indent=2) + '\n'
    output.mkdir(parents=True)
    (output / PNG).write_bytes(data)
    (output / 'manifest.json').write_text(encoded)
    return result


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--base', type=Path, required=True)
    parser.add_argument('--registry', type=Path, required=True)
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    result = run(args.base, args.registry, args.output)
    print(json.dumps({'parts': len(result['parts']), 'items': len(result['itemBindings']),
                      'bytes': result['pngBytes'], 'status': result['status']}))
