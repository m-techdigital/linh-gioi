#!/usr/bin/env python3
"""Batch lossless source crops; never silently promote design crops to runtime."""
import argparse
import hashlib
import json
import re
from pathlib import Path
from PIL import Image, ImageDraw


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def run(plan_path, source_root, output):
    plan = json.loads(plan_path.read_text())
    prepared, names = [], set()
    # Validate the entire batch before creating outputs.
    for sheet in plan['sheets']:
        source = source_root / sheet['file']
        sha = digest(source)
        if sha != sheet['sha256']:
            raise ValueError(f'Source changed: {source}')
        with Image.open(source) as image:
            width, height = image.size
        rw, rh = sheet['reference_size']
        for item in sheet['items']:
            name = item['name']
            if not re.fullmatch(r'[a-z0-9-]+', name) or name in names:
                raise ValueError(f'Invalid or duplicate name: {name}')
            names.add(name)
            x0, y0, x1, y1 = item['rect']
            if not (0 <= x0 < x1 <= rw and 0 <= y0 < y1 <= rh):
                raise ValueError(f'Out of bounds: {name}')
            normalized = [x0/rw, y0/rh, x1/rw, y1/rh]
            box = tuple(round(v*s) for v, s in zip(normalized, [width,height,width,height]))
            if box[0] >= box[2] or box[1] >= box[3]:
                raise ValueError(f'Empty crop: {name}')
            prepared.append((source, sha, item, normalized, box))
    output.mkdir(parents=True, exist_ok=False)
    records = []
    for source, sha, item, normalized, box in prepared:
        path = output / (item['name'] + '.png')
        with Image.open(source) as image:
            crop = image.crop(box)
            crop.save(path, optimize=True)
        records.append({**item, 'source': str(source), 'source_sha256': sha,
                        'normalized_rect': normalized, 'native_rect': list(box),
                        'file': path.name, 'size': list(crop.size),
                        'sha256': digest(path), 'runtime_ready': False,
                        'status': 'SOURCE_CROP_NEEDS_CLEANUP_AND_REVIEW'})
    # Contact pages are review aids only; source crops are never resized.
    for page_start in range(0, len(records), 36):
        subset = records[page_start:page_start+36]
        canvas = Image.new('RGB', (1200, ((len(subset)+5)//6)*160), '#d8e0e8')
        draw = ImageDraw.Draw(canvas)
        for i, record in enumerate(subset):
            x, y = (i%6)*200, (i//6)*160
            with Image.open(output / record['file']) as image:
                image.thumbnail((190, 125))
                canvas.paste(image, (x+(200-image.width)//2, y))
            draw.text((x+4,y+128), record['name'], fill='black')
        canvas.save(output / f'contact-{page_start//36+1:02}.jpg', quality=88)
    result = {'selection': plan['selection'], 'exclusions': plan['exclusions'],
              'count': len(records), 'entries': records}
    (output / 'MANIFEST.json').write_text(json.dumps(result, ensure_ascii=False, indent=2)+'\n')
    print(json.dumps({'output': str(output), 'crops': len(records),
                      'png_bytes': sum((output/r['file']).stat().st_size for r in records),
                      'runtime_ready': False}, ensure_ascii=False))


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--plan', type=Path, required=True)
    parser.add_argument('--source-root', type=Path, required=True)
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    run(args.plan, args.source_root, args.output)
