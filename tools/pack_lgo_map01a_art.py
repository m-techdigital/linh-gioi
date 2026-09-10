#!/usr/bin/env python3
"""Pack source-grounded Map01A drafts for review; no automatic Unity ingest."""
import argparse
import hashlib
import json
from pathlib import Path
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def pack(source, output):
    manifest = json.loads((source / 'manifest.json').read_text())
    entries = {entry['file']: entry for entry in manifest['assets']}
    names = ['far-mountains-linh-thanh-draft-v1.png', 'gate-draft-v1.png',
             'ha-van-sprite-draft-v1.png', 'terrain-draft-v1.png', 'village-midground-draft-v1.png']
    for name in names:
        entry = entries[name]
        if digest(source / name) != entry['sha256']:
            raise ValueError('Source hash mismatch: ' + name)
        for reference in entry['references']:
            if digest(Path(reference['path'])) != reference['sha256']:
                raise ValueError('Reference hash mismatch: ' + reference['path'])
    output.mkdir(parents=True, exist_ok=True)
    atlas = Image.new('RGBA', (1536, 1536))
    parts = []
    # Top-left pixel bounds. Terrain intentionally uses only the opaque stone area:
    # its baked checkerboard export is NOT accepted as an alpha sprite.
    specs = [('gate', names[1], None, (8, 8), (1024, 683)),
             ('ha-van', names[2], None, (1050, 8), (384, 576)),
             ('terrain', names[3], (0, 330, 2172, 724), (8, 720), (2032, 369)),
             ('village', names[4], None, (8, 1110), (1600, 800))]
    for name, filename, crop, position, size in specs:
        position = tuple(round(v * .75) for v in position)
        size = tuple(round(v * .75) for v in size)
        with Image.open(source / filename) as raw:
            image = raw.convert('RGBA')
            if crop is None:
                histogram = image.getchannel('A').histogram()
                if histogram[0] < image.width * image.height * .15:
                    raise ValueError('Missing real alpha: ' + filename)
            else:
                if image.size != (2172, 724):
                    raise ValueError('Terrain crop requires reviewed source dimensions')
                image = image.crop(crop)
            image = image.resize(size, Image.Resampling.LANCZOS)
            atlas.paste(image, position)  # Preserve alpha without double multiplication.
            parts.append(dict(id=name, source=filename, sourceCrop=crop,
                              x=position[0], y=1536-position[1]-size[1], w=size[0], h=size[1]))
    atlas.save(output / 'props-atlas.png', optimize=True)
    with Image.open(source / names[0]) as raw:
        raw.convert('RGB').resize((1024, 576), Image.Resampling.LANCZOS).save(output / 'far-background.png', optimize=True)
    result = dict(id='cong-dong-lam-map01a-art-draft-v1', status='DRAFT_NOT_RUNTIME_APPROVED',
                  groundY=-1.62, parts=parts, layers=[
                      dict(id='far-landscape', part='skyline', x=0, y=0, width=12.6, height=7.0875, order=-30, parallax=.08),
                      dict(id='village-behind-gate', part='village', x=2.1, y=-.12, width=8.6, height=4.3, order=-24, parallax=0),
                      dict(id='grand-gate', part='gate', x=-.6, y=.77, width=7.8, height=5.2025, order=-20, parallax=0),
                      dict(id='flat-walkway', part='terrain', x=0, y=-2.56, width=12.6, height=2.288, order=-15, parallax=0),
                      dict(id='ha-van', part='ha-van', x=-2.65, y=-.62, width=1.18, height=1.77, order=1, parallax=0)],
                  sourceManifestSha256=digest(source / 'manifest.json'),
                  assets=[dict(file=n, sha256=digest(output/n)) for n in ['props-atlas.png', 'far-background.png']])
    source_bytes = sum((output/n).stat().st_size for n in ['props-atlas.png', 'far-background.png'])
    if source_bytes > 4 * 1024 * 1024:
        raise ValueError('Map01A source PNG budget exceeds 4 MiB')
    result['textureBudget'] = dict(sourcePngBytes=source_bytes, maxSourcePngBytes=4*1024*1024,
        uncompressedRgbaBytes=(1536*1536+1024*576)*4,
        estimatedDesktopGpuBytes=1536*1536+1024*576//2,
        estimatedAstc6x6GpuBytes=((1536+5)//6)**2*16+((1024+5)//6)*((576+5)//6)*16,
        mipmaps=False, readable=False, textureCount=2,
        note='GPU estimates exclude driver overhead; actual device profiling still required')
    (output / 'manifest.json').write_text(json.dumps(result, ensure_ascii=False, indent=2)+'\n')
    print('MAP01A_DRAFT_PACK_CREATED; runtime validation pending')


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--source-dir', type=Path, default=ROOT/'build/map01a-art/source')
    parser.add_argument('--out-dir', type=Path, default=ROOT/'build/map01a-art/pack')
    args = parser.parse_args()
    pack(args.source_dir.resolve(), args.out_dir.resolve())
