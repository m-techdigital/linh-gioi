#!/usr/bin/env python3
"""Trim and pack the six-character Map01A NPC generation as one reviewed batch."""
import argparse
import hashlib
import json
from collections import deque
from pathlib import Path
from PIL import Image, ImageFilter


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def remove_detached_noise(image, threshold=8, min_pixels=500):
    width, height = image.size
    alpha = list(image.getchannel('A').getdata())
    seen = set()
    keep = set()
    removed = 0
    for start, value in enumerate(alpha):
        if value <= threshold or start in seen:
            continue
        component = []
        queue = deque([start])
        seen.add(start)
        while queue:
            current = queue.popleft()
            component.append(current)
            x, y = current % width, current // width
            for nx, ny in ((x-1,y),(x+1,y),(x,y-1),(x,y+1)):
                if not (0 <= nx < width and 0 <= ny < height):
                    continue
                neighbor = ny * width + nx
                if neighbor not in seen and alpha[neighbor] > threshold:
                    seen.add(neighbor)
                    queue.append(neighbor)
        if len(component) >= min_pixels:
            keep.update(component)
        else:
            removed += len(component)
    pixels = []
    for index, pixel in enumerate(image.getdata()):
        pixels.append(pixel if index in keep else (0, 0, 0, 0))
    cleaned = Image.new('RGBA', image.size)
    cleaned.putdata(pixels)
    return cleaned, removed


def pack(source, output):
    with Image.open(source) as raw:
        sheet = raw.convert('RGBA')
    if sheet.size != (1536, 1024):
        raise ValueError('NPC cells require reviewed 1536x1024 source')
    output.mkdir(parents=True, exist_ok=False)
    atlas = Image.new('RGBA', (512, 512))
    names = ['ha-van', 'quan-thu-dong-lam', 'tong-phu', 'thanh-nhi', 'lao-tran', 'tieu-dong']
    parts = []
    changed = 0
    removed_noise = 0
    for index, name in enumerate(names):
        cell = sheet.crop(((index % 3) * 512, (index // 3) * 512, (index % 3 + 1) * 512, (index // 3 + 1) * 512))
        alpha = cell.getchannel('A')
        bbox = alpha.point(lambda value: 255 if value > 8 else 0).getbbox()
        if bbox is None:
            raise ValueError('Empty NPC cell: ' + name)
        cell = cell.crop(bbox)
        cell, removed = remove_detached_noise(cell)
        removed_noise += removed
        alpha = cell.getchannel('A')
        inside = alpha.filter(ImageFilter.MinFilter(5))
        pixels = []
        for (r, g, b, a), minimum in zip(cell.getdata(), inside.getdata()):
            if a and minimum < a and min(r, b) > g + 24:
                spill = min(r, b) - g - 24
                r -= spill
                b -= spill
                changed += 1
            pixels.append((r, g, b, a) if a else (0, 0, 0, 0))
        cell.putdata(pixels)
        cell.thumbnail((158, 210), Image.Resampling.LANCZOS)
        left = (index % 3) * 170 + (170 - cell.width) // 2
        top = (index // 3) * 256 + 246 - cell.height
        atlas.paste(cell, (left, top))
        parts.append({'id': name, 'x': left, 'y': 512 - top - cell.height,
                      'w': cell.width, 'h': cell.height, 'sourceCell': index,
                      'nativeAspectPreserved': True})
    atlas.save(output / 'npcs-atlas.png', optimize=True)
    review = Image.new('RGBA', atlas.size, '#263945')
    review.alpha_composite(atlas)
    review.convert('RGB').save(output / 'review-dark.jpg', quality=92)
    heights = {'ha-van':1.82, 'quan-thu-dong-lam':1.9, 'tong-phu':1.78,
               'thanh-nhi':1.72, 'lao-tran':1.86, 'tieu-dong':1.34}
    positions = {'ha-van':-3.0, 'quan-thu-dong-lam':6.0, 'tong-phu':23.0,
                 'thanh-nhi':27.0, 'lao-tran':35.0, 'tieu-dong':31.0}
    layers = []
    for part in parts:
        height = heights[part['id']]
        width = height * part['w'] / part['h']
        layers.append({'id':part['id'], 'part':part['id'], 'x':positions[part['id']],
                       'y':-1.62 + height/2, 'width':width, 'height':height,
                       'order':1, 'parallax':0})
    manifest = {'id':'map01a-six-npcs-v1', 'status':'DRAFT_REQUIRES_PLAYER_REVIEW',
                'source':str(source), 'sourceSha256':digest(source), 'parts':parts, 'layers':layers,
                'atlas':[512,512], 'pngBytes':(output/'npcs-atlas.png').stat().st_size,
                'estimatedBc3Bytes':512*512, 'upscaled':False, 'edgePixelsDespill':changed,
                'runtimePixelHeightBudget':{'observedAt720To768High':[149,159], 'packedMax':210, 'headroomMax':1.41}}
    manifest['detachedNoisePixelsRemoved'] = removed_noise
    (output / 'npcs-layout.json').write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + '\n')
    print(json.dumps({'count':len(parts), 'pngBytes':manifest['pngBytes'], 'edgePixelsDespill':changed}))


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--source', type=Path, required=True)
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    pack(args.source, args.output)
