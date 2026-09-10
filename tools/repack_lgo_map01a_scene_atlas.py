#!/usr/bin/env python3
"""Repack only the still-used Map01A gate and village layers at runtime pixel budget."""
import argparse
import hashlib
import json
from pathlib import Path
from PIL import Image


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def pack(source, output):
    source_manifest = json.loads((source / 'manifest.json').read_text())
    declared = {entry['file']: entry for entry in source_manifest['assets']}
    specs = [('gate', 'gate-draft-v1.png', (768, 512), (8, 8)),
             ('village', 'village-midground-draft-v1.png', (832, 416), (8, 584))]
    atlas = Image.new('RGBA', (1024, 1024))
    parts = []
    for part_id, filename, size, position in specs:
        path = source / filename
        if digest(path) != declared[filename]['sha256']:
            raise ValueError('Source hash mismatch: ' + filename)
        with Image.open(path) as raw:
            image = raw.convert('RGBA')
        if image.getchannel('A').histogram()[0] < image.width * image.height * .15:
            raise ValueError('Missing reviewed alpha: ' + filename)
        image.thumbnail(size, Image.Resampling.LANCZOS)
        atlas.paste(image, position)
        parts.append({'id':part_id, 'x':position[0], 'y':1024-position[1]-image.height,
                      'w':image.width, 'h':image.height, 'source':filename,
                      'upscaled':False})
    output.parent.mkdir(parents=True, exist_ok=True)
    atlas.save(output, optimize=True)
    report = {'id':'map01a-scene-atlas-v2', 'atlas':[1024,1024], 'parts':parts,
              'pngBytes':output.stat().st_size, 'estimatedBc3Bytes':1024*1024,
              'removedParts':['terrain','ha-van'], 'reason':'Replaced by authored modules and coherent six-NPC atlas'}
    output.with_suffix('.json').write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n')
    print(json.dumps({'pngBytes':report['pngBytes'], 'parts':len(parts)}))


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--source', type=Path, required=True)
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    pack(args.source, args.output)
