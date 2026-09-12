#!/usr/bin/env python3.12
"""Build a review pack whose jump art matches the locked body scale.

The transform is baked into source-review PNGs. Runtime root scale, camera,
canvas, pivot, idle and four run phases remain unchanged.
"""
import argparse
import hashlib
import json
from pathlib import Path
import shutil

import numpy as np
from PIL import Image, ImageDraw

from pack_lgo_pose_review_atlas import pack_review


POSES = ('idle', 'run_contact_a', 'run_a', 'run_contact_b', 'run_b', 'jump_tuck')
SLOT_DIRS = (
    'main-weapon-review', 'head-hair-review', 'inner-top-review', 'outer-top-review',
    'lower-body-review', 'waist-belt-review', 'arm-guard-review', 'footwear-review',
    'shoulder-chest-guard-review', 'class-accessory-review',
)


def scale_rgba_about(image: np.ndarray, scale: float, pivot: tuple[int, int]) -> np.ndarray:
    if not 0 < scale <= 1:
        raise ValueError('Scale correction must be in (0, 1]')
    if scale == 1:
        return image.copy()
    height, width = image.shape[:2]
    resized = Image.fromarray(image, 'RGBA').resize(
        (max(1, round(width * scale)), max(1, round(height * scale))),
        Image.Resampling.LANCZOS,
    )
    left = round(pivot[0] - pivot[0] * scale)
    top = round(pivot[1] - pivot[1] * scale)
    canvas = Image.new('RGBA', (width, height))
    canvas.alpha_composite(resized, (left, top))
    return np.asarray(canvas).copy()


def _sha(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def _write_sources(manifest: dict, directory: Path, scale: float, pivot: tuple[int, int]) -> list[dict]:
    directory.mkdir(parents=True, exist_ok=False)
    sources = []
    by_id = {row['id']: row for row in manifest['sprites']}
    for pose in POSES:
        source = Path(by_id[pose]['source'])
        image = np.asarray(Image.open(source).convert('RGBA'))
        if pose == 'jump_tuck':
            image = scale_rgba_about(image, scale, pivot)
        output = directory / f'{pose}.png'
        Image.fromarray(image, 'RGBA').save(output, compress_level=4)
        sources.append({'id': pose, 'source': str(output.resolve()), 'sourceSha256': _sha(output)})
    return sources


def _write_atlas(sources: list[dict], old: dict, directory: Path) -> dict:
    atlas, manifest = pack_review(sources, old['samplingDivisor'], max_side=2048)
    directory.mkdir(parents=True, exist_ok=False)
    atlas_path = directory / 'atlas-review.png'
    atlas.save(atlas_path, optimize=True)
    for key, value in old.items():
        if key not in {'sprites', 'atlasSize', 'rgba8Bytes', 'pngBytes', 'atlasSha256'}:
            manifest[key] = value
    old_by_id = {row['id']: row for row in old['sprites']}
    for row in manifest['sprites']:
        for key in ('componentId', 'order'):
            if key in old_by_id[row['id']]:
                row[key] = old_by_id[row['id']][key]
    manifest['pngBytes'] = atlas_path.stat().st_size
    manifest['atlasSha256'] = _sha(atlas_path)
    (directory / 'atlas-review.json').write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + '\n')
    return manifest


def _compose_board(source_root: Path, output_pack: Path) -> None:
    body = {pose: Image.open(source_root / 'body' / f'{pose}.png').convert('RGBA') for pose in POSES}
    slots = []
    for slot_dir in SLOT_DIRS:
        manifest = json.loads((output_pack / slot_dir / 'atlas-review.json').read_text())
        order = manifest['sprites'][0].get('order', 25)
        images = {pose: Image.open(source_root / slot_dir / f'{pose}.png').convert('RGBA') for pose in POSES}
        slots.append((order, slot_dir, images))

    def compose(pose: str, hidden: str | None = None) -> Image.Image:
        layers = [(24, 'body', body[pose]), *[(order, name, images[pose]) for order, name, images in slots]]
        result = Image.new('RGBA', body[pose].size)
        for _, name, layer in sorted(layers):
            if name != hidden:
                result = Image.alpha_composite(result, layer)
        return result

    motion = Image.new('RGB', (1440, 1440), (225, 225, 225)); draw = ImageDraw.Draw(motion)
    for index, pose in enumerate(POSES):
        image = compose(pose); image.thumbnail((440, 650), Image.Resampling.LANCZOS)
        col, row = index % 3, index // 3
        motion.paste(image, (col * 480 + (480 - image.width) // 2, row * 720 + 40), image)
        draw.text((col * 480 + 10, row * 720 + 10), pose, fill=(20, 20, 20))
    motion.save(source_root / 'six-pose-full-compose.jpg', quality=96, subsampling=0)

    toggle = Image.new('RGB', (1600, 1920), (225, 225, 225)); draw = ImageDraw.Draw(toggle)
    for index, hidden in enumerate((None, *SLOT_DIRS)):
        image = compose('idle', hidden); image.thumbnail((370, 540), Image.Resampling.LANCZOS)
        col, row = index % 4, index // 4
        toggle.paste(image, (col * 400 + (400 - image.width) // 2, row * 640 + 50), image)
        label = 'all_on' if hidden is None else 'off_' + hidden.removesuffix('-review').replace('-', '_')
        draw.text((col * 400 + 10, row * 640 + 14), label, fill=(20, 20, 20))
    toggle.save(source_root / 'idle-ten-slot-toggle-review.jpg', quality=96, subsampling=0)


def normalize_pack(input_pack: Path, output_pack: Path, output_source: Path,
                   scale: float, base_pack: Path | None = None) -> None:
    if output_pack.exists() or output_source.exists():
        raise FileExistsError('Output pack/source already exists')
    output_pack.parent.mkdir(parents=True, exist_ok=True)
    output_source.mkdir(parents=True)
    pivot = tuple(json.loads((input_pack / 'atlas-review.json').read_text())['jumpPivotSource'])
    if base_pack is None:
        old_body = json.loads((input_pack / 'atlas-review.json').read_text())
        body_sources = _write_sources(old_body, output_source / 'body', scale, pivot)
        body_manifest = _write_atlas(body_sources, old_body, output_pack)
        body_manifest['poseScaleCorrections'] = {'jump_tuck': scale}
        (output_pack / 'atlas-review.json').write_text(json.dumps(body_manifest, ensure_ascii=False, indent=2) + '\n')
    else:
        output_pack.mkdir(parents=True)
        shutil.copy2(base_pack / 'atlas-review.png', output_pack / 'atlas-review.png')
        shutil.copy2(base_pack / 'atlas-review.json', output_pack / 'atlas-review.json')
        base_manifest = json.loads((base_pack / 'atlas-review.json').read_text())
        _write_sources(base_manifest, output_source / 'body', 1, pivot)
    body_atlas_hash = _sha(output_pack / 'atlas-review.png')
    body_manifest_hash = _sha(output_pack / 'atlas-review.json')
    for slot_dir in SLOT_DIRS:
        old = json.loads((input_pack / slot_dir / 'atlas-review.json').read_text())
        sources = _write_sources(old, output_source / slot_dir, scale, pivot)
        manifest = _write_atlas(sources, old, output_pack / slot_dir)
        manifest['basePoseAtlasSha256'] = body_atlas_hash
        manifest['basePoseManifestSha256'] = body_manifest_hash
        manifest['poseScaleCorrections'] = {'jump_tuck': scale}
        (output_pack / slot_dir / 'atlas-review.json').write_text(
            json.dumps(manifest, ensure_ascii=False, indent=2) + '\n')
    _compose_board(output_source, output_pack)
    (output_pack / 'TEN-SLOT-REVIEW.md').write_text(
        '# Source-pose normalized review pack\n\n'
        f'`jump_tuck` source art is baked at {scale:.6f} around source pivot {pivot}; '
        'idle, four run phases, runtime root scale and camera remain unchanged.\n')


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--input-pack', type=Path, required=True)
    parser.add_argument('--output-pack', type=Path, required=True)
    parser.add_argument('--output-source', type=Path, required=True)
    parser.add_argument('--jump-scale', type=float, default=2 / 3)
    parser.add_argument('--base-pack', type=Path)
    args = parser.parse_args()
    normalize_pack(args.input_pack.resolve(), args.output_pack.resolve(), args.output_source.resolve(),
                   args.jump_scale, args.base_pack.resolve() if args.base_pack else None)
    print(f'LGO_SOURCE_POSE_JUMP_NORMALIZED pack={args.output_pack} scale={args.jump_scale:.6f}')


if __name__ == '__main__':
    main()
