#!/usr/bin/env python3.12
"""Repartition a full source-pose outfit against canonical per-pose slot anchors."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import shutil
import subprocess

import cv2
import numpy as np
from PIL import Image, ImageDraw

POSES = ('idle', 'run_contact_a', 'run_a', 'run_contact_b', 'run_b', 'jump_tuck')
SLOTS = ('main_weapon', 'head_hair', 'inner_top', 'outer_top', 'lower_body', 'waist_belt',
         'arm_guard', 'footwear', 'shoulder_chest_guard', 'class_accessory')


def alpha(image: np.ndarray) -> np.ndarray:
    return image[:, :, 3] > 8


def choose_accessory(mask: np.ndarray, anchor: np.ndarray) -> np.ndarray:
    near = cv2.dilate(anchor.astype(np.uint8), cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (161, 161))) > 0
    count, labels, stats, _ = cv2.connectedComponentsWithStats(mask.astype(np.uint8), 8)
    choices = [index for index in range(1, count)
               if 20 <= stats[index, cv2.CC_STAT_AREA] <= 8000]
    if choices:
        best = max(choices, key=lambda index: int(np.count_nonzero((labels == index) & near)))
        if np.any((labels == best) & near):
            return labels == best
    return mask & anchor


def choose_nearest_component(mask: np.ndarray, anchor: np.ndarray) -> np.ndarray:
    count, labels, stats, _ = cv2.connectedComponentsWithStats(mask.astype(np.uint8), 8)
    if count <= 1:
        return mask
    distance = cv2.distanceTransform((~anchor).astype(np.uint8), cv2.DIST_L2, 5)
    candidates = range(1, count)
    best = min(candidates, key=lambda index: (float(distance[labels == index].min()),
                                               -int(stats[index, cv2.CC_STAT_AREA])))
    return labels == best


def repartition_pose(images: dict[str, np.ndarray], anchors: dict[str, np.ndarray]) -> dict[str, np.ndarray]:
    union = np.logical_or.reduce([alpha(images[slot]) for slot in SLOTS])
    weapon = alpha(images['main_weapon'])
    accessory_mask = choose_accessory(alpha(images['class_accessory']), anchors['waist_belt'])
    if not accessory_mask.any():
        accessory_mask = choose_accessory(union & ~weapon, anchors['waist_belt'])
    if not accessory_mask.any():
        raise ValueError('No detachable waist ornament could be selected')
    donor = np.zeros_like(images['outer_top'])
    ownership_count = np.zeros(union.shape, dtype=np.uint8)
    for slot in SLOTS:
        mask = alpha(images[slot])
        ownership_count += mask
        donor[mask] = images[slot][mask]
    if np.any(ownership_count > 1):
        raise AssertionError('Input layers overlap; lossless repartition requires disjoint sources')
    assignable = tuple(slot for slot in SLOTS if slot not in ('main_weapon', 'class_accessory'))
    distance = np.stack([cv2.distanceTransform((~anchors[slot]).astype(np.uint8), cv2.DIST_L2, 5)
                         for slot in assignable])
    owner = np.argmin(distance, axis=0)
    reserved = {slot: choose_nearest_component(alpha(images[slot]) & ~weapon & ~accessory_mask,
                                                anchors[slot]) for slot in assignable}
    reserved_union = np.logical_or.reduce(list(reserved.values()))
    result = {'main_weapon': images['main_weapon'].copy()}
    for index, slot in enumerate(assignable):
        mask = reserved[slot] | (union & ~weapon & ~accessory_mask & ~reserved_union & (owner == index))
        image = donor.copy()
        image[:, :, 3] = np.where(mask, donor[:, :, 3], 0)
        result[slot] = image
    accessory_image = np.zeros_like(donor)
    accessory_image[accessory_mask] = donor[accessory_mask]
    result['class_accessory'] = accessory_image
    after = np.logical_or.reduce([alpha(result[slot]) for slot in SLOTS])
    if not np.array_equal(union, after):
        raise AssertionError('Layer reassignment changed full-compose alpha coverage')
    after_count = np.zeros(union.shape, dtype=np.uint8)
    for slot in SLOTS:
        mask = alpha(result[slot])
        after_count += mask
        if not np.array_equal(result[slot][mask], donor[mask]):
            raise AssertionError('Layer reassignment changed source RGBA pixels')
    if np.any(after_count > 1):
        raise AssertionError('Layer reassignment introduced overlapping ownership')
    return result


def source_root(pack: Path) -> Path:
    manifest = json.loads((pack / 'outer-top-review/atlas-review.json').read_text())
    return Path(manifest['sprites'][0]['source']).resolve().parents[1]


def body_sources(pack: Path) -> dict[str, Path]:
    manifest = json.loads((pack / 'atlas-review.json').read_text())
    return {row['id']: Path(row['source']).resolve() for row in manifest['sprites'] if row['id'] in POSES}


def write_surface(pack: Path, anchor_surface: Path, output: Path) -> None:
    source = source_root(pack)
    bodies = body_sources(pack)
    if output.exists():
        raise FileExistsError(output)
    output.mkdir(parents=True)
    records = []
    composed = {}
    for pose in POSES:
        images = {slot: np.asarray(Image.open(source / slot / f'{pose}.png').convert('RGBA')).copy()
                  for slot in SLOTS}
        anchors = {slot: alpha(np.asarray(Image.open(anchor_surface / slot / f'{pose}.png').convert('RGBA')))
                   for slot in SLOTS if slot not in ('main_weapon', 'class_accessory')}
        repaired = repartition_pose(images, anchors)
        for slot, image in repaired.items():
            directory = output / slot
            directory.mkdir(exist_ok=True)
            path = directory / f'{pose}.png'
            Image.fromarray(image, 'RGBA').save(path, compress_level=4)
            records.append({'pose': pose, 'slot': slot, 'pixels': int(alpha(image).sum()),
                            'sha256': hashlib.sha256(path.read_bytes()).hexdigest()})
        comp = Image.open(bodies[pose]).convert('RGBA')
        for slot in SLOTS:
            comp = Image.alpha_composite(comp, Image.fromarray(repaired[slot], 'RGBA'))
        comp.save(output / f'{pose}-full-compose.png', compress_level=4)
        composed[pose] = comp
    _write_boards(output, composed, bodies)
    (output / 'semantic-reassignment.json').write_text(json.dumps({
        'status': 'SOURCE_REVIEW_REQUIRED', 'inputSurface': str(source),
        'anchorSurface': str(anchor_surface.resolve()), 'method': 'nearest canonical slot anchor plus bounded waist accessory',
        'fullComposeAlphaInvariant': True, 'records': records,
    }, ensure_ascii=False, indent=2) + '\n')


def _write_boards(output: Path, composed: dict[str, Image.Image], bodies: dict[str, Path]) -> None:
    motion = Image.new('RGB', (1440, 1440), (225, 225, 225)); draw = ImageDraw.Draw(motion)
    for index, pose in enumerate(POSES):
        image = composed[pose].copy(); image.thumbnail((440, 650), Image.Resampling.LANCZOS)
        col, row = index % 3, index // 3
        motion.paste(image, (col * 480 + (480 - image.width) // 2, row * 720 + 40), image)
        draw.text((col * 480 + 10, row * 720 + 10), pose, fill=(20, 20, 20))
    motion.save(output / 'six-pose-full-compose.jpg', quality=96, subsampling=0)
    toggle = Image.new('RGB', (1600, 1920), (225, 225, 225)); draw = ImageDraw.Draw(toggle)
    images = {slot: Image.open(output / slot / 'idle.png').convert('RGBA') for slot in SLOTS}
    for index, hidden in enumerate((None, *SLOTS)):
        comp = Image.open(bodies['idle']).convert('RGBA')
        for slot in SLOTS:
            if slot != hidden: comp = Image.alpha_composite(comp, images[slot])
        comp.thumbnail((370, 540), Image.Resampling.LANCZOS)
        col, row = index % 4, index // 4
        toggle.paste(comp, (col * 400 + (400 - comp.width) // 2, row * 640 + 50), comp)
        draw.text((col * 400 + 10, row * 640 + 14), 'all_on' if hidden is None else 'off_' + hidden,
                  fill=(20, 20, 20))
    toggle.save(output / 'idle-ten-slot-toggle-review.jpg', quality=96, subsampling=0)


def repack(pack: Path, surface: Path, output: Path, repo: Path) -> None:
    if output.exists():
        raise FileExistsError(output)
    shutil.copytree(pack, output)
    for slot in SLOTS:
        directory = output / (slot.replace('_', '-') + '-review')
        shutil.rmtree(directory)
        sources = [{'id': pose, 'source': str((surface / slot / f'{pose}.png').resolve())}
                   for pose in POSES]
        source_file = surface / f'{slot}-pack-sources.json'
        for row in sources:
            row['sourceSha256'] = hashlib.sha256(Path(row['source']).read_bytes()).hexdigest()
        source_file.write_text(json.dumps(sources, indent=2) + '\n')
        old = json.loads((pack / directory.name / 'atlas-review.json').read_text())
        env = os.environ.copy(); env['PYTHONPATH'] = str(repo / 'tools')
        subprocess.run([str(repo / 'build/rig-authoring-venv/bin/python'),
                        str(repo / 'tools/pack_lgo_pose_review_atlas.py'), '--sources', str(source_file),
                        '--output-dir', str(directory), '--divisor', '2', '--max-side', '2048',
                        '--jump-pivot-source', '512', '820'], cwd=repo, env=env, check=True)
        path = directory / 'atlas-review.json'; new = json.loads(path.read_text())
        for key in ('reviewSlot', 'basePoseAtlasSha256', 'basePoseManifestSha256', 'fitFamily',
                    'unlockLevel', 'itemId', 'gender', 'fitStatus', 'reviewPurpose'):
            if key in old: new[key] = old[key]
        order = old['sprites'][0].get('order')
        for row in new['sprites']:
            row.update(componentId='main', order=order)
        path.write_text(json.dumps(new, ensure_ascii=False, indent=2) + '\n')


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--pack', type=Path, required=True)
    parser.add_argument('--anchor-surface', type=Path, required=True)
    parser.add_argument('--output-surface', type=Path, required=True)
    parser.add_argument('--output-pack', type=Path, required=True)
    args = parser.parse_args()
    repo = Path(__file__).resolve().parents[1]
    write_surface(args.pack.resolve(), args.anchor_surface.resolve(), args.output_surface.resolve())
    repack(args.pack.resolve(), args.output_surface.resolve(), args.output_pack.resolve(), repo)
    print(f'LGO_SOURCE_POSE_SLOTS_REPARTITIONED pack={args.output_pack}')


if __name__ == '__main__':
    main()
