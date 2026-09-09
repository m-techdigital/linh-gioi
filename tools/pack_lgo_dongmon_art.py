#!/usr/bin/env python3
"""Deterministically resize/pack newly generated Dong Mon assets (requires Pillow).
Raw source defaults to build/dongmon-art/source; no reference-board cropping or alpha keying.
"""
import argparse
import hashlib
import json
from pathlib import Path
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]
PACK = 'client/Unity/Assets/Game/World/Runtime/Resources/LGOMaps/DongMonIllustrated'
# Source top-left atlas placement, with at least 8px clear gutter around each region.
PARTS = [('gate', 8, 8, 1024, 768), ('guide', 1056, 8, 512, 768), ('terrain', 8, 800, 2032, 677)]


LAYERS = [
    dict(id='far-city', part='skyline', x=0, y=.25, width=12.6, height=7.09, order=-30, parallax=.09),
    dict(id='city-gate', part='gate', x=-1.7, y=.54, width=5, height=3.75, order=-20, parallax=0),
    *[dict(id='walkway-'+str(i), part='terrain', x=i*5.4, y=-2.19, width=5.4, height=1.9, order=-15, parallax=0) for i in [-1,0,1]],
    dict(id='gate-terrace', part='terrain', x=-1.4, y=-1.35, width=4.6, height=1.53, order=-14, parallax=0),
    dict(id='gate-guide', part='guide', x=-1.35, y=-.375, width=1.1, height=1.65, order=1, parallax=0),
]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--source-dir', type=Path, default=ROOT / 'build/dongmon-art/source')
    args = parser.parse_args()
    source, out = args.source_dir.resolve(), ROOT / PACK
    out.mkdir(parents=True, exist_ok=True)
    prepared = []
    for name, x, y, width, height in PARTS:
        with Image.open(source / (name + '.png')) as raw:
            rgba = raw.convert('RGBA')
            histogram = rgba.getchannel('A').histogram()
            if histogram[0] < raw.width * raw.height * .1:
                raise ValueError(name + ': missing genuine transparent background; do not ingest checkerboard')
            prepared.append((rgba.resize((width, height), Image.Resampling.LANCZOS), x, y))
    with Image.open(source / 'skyline.png') as raw:
        raw.convert('RGB').resize((1536, 864), Image.Resampling.LANCZOS).save(out / 'skyline.png', optimize=True)
    atlas = Image.new('RGBA', (2048, 2048))
    for image, x, y in prepared:
        atlas.paste(image, (x, y))  # Preserve original alpha; no second alpha multiplication.
    atlas.save(out / 'props-atlas.png', optimize=True)
    assets = []
    for filename, width, height, role in [('skyline.png', 1536, 864, 'far-background'),
                                         ('props-atlas.png', 2048, 2048, 'runtime-atlas')]:
        raw = (out / filename).read_bytes()
        assets.append(dict(path=PACK + '/' + filename, sha256=hashlib.sha256(raw).hexdigest(),
            role=role, generator='image_gen', created='2026-09-10', referenceOnly=False, width=width, height=height))
    manifest = dict(id='dongmon-illustrated-draft-v1', status='DRAFT_OWNER_REVIEW', assets=assets,
        layers=LAYERS,
        parts=[dict(id=name, x=x, y=2048-y-height, w=width, h=height) for name,x,y,width,height in PARTS],
        sourceHashes={name + '.png': hashlib.sha256((source / (name + '.png')).read_bytes()).hexdigest()
                      for name in ['skyline', 'gate', 'terrain', 'guide']})
    (out / 'manifest.json').write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + '\n')
    print('LGO_DONGMON_ART_PACK_READY', ', '.join(a['sha256'][:12] for a in assets))


if __name__ == '__main__':
    main()
