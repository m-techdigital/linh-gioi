#!/usr/bin/env python3
"""Trim and pack the six-landmark Map01A generation as one runtime-sized batch."""
import argparse
import hashlib
import json
from collections import deque
from pathlib import Path
from PIL import Image, ImageFilter


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def remove_noise(image, threshold=8, min_pixels=500):
    width, height = image.size
    alpha = list(image.getchannel('A').getdata())
    seen, keep = set(), set()
    removed = 0
    for start, value in enumerate(alpha):
        if value <= threshold or start in seen:
            continue
        component, queue = [], deque([start])
        seen.add(start)
        while queue:
            current = queue.popleft()
            component.append(current)
            x, y = current % width, current // width
            for nx, ny in ((x-1,y),(x+1,y),(x,y-1),(x,y+1)):
                if 0 <= nx < width and 0 <= ny < height:
                    neighbor = ny * width + nx
                    if neighbor not in seen and alpha[neighbor] > threshold:
                        seen.add(neighbor)
                        queue.append(neighbor)
        if len(component) >= min_pixels:
            keep.update(component)
        else:
            removed += len(component)
    cleaned = Image.new('RGBA', image.size)
    cleaned.putdata([pixel if index in keep else (0,0,0,0) for index, pixel in enumerate(image.getdata())])
    return cleaned, removed


def pack(source, output):
    with Image.open(source) as raw:
        sheet = raw.convert('RGBA')
    if sheet.size != (1536, 1024):
        raise ValueError('Landmark cells require reviewed 1536x1024 source')
    output.mkdir(parents=True, exist_ok=False)
    atlas = Image.new('RGBA', (1024, 1024))
    names = ['social-hall', 'merchant-stall', 'herbal-clinic', 'village-well', 'hunter-post', 'suoi-thanh-minh-portal']
    parts, removed_noise, changed = [], 0, 0
    for index, name in enumerate(names):
        cell = sheet.crop(((index % 3) * 512, (index // 3) * 512, (index % 3 + 1) * 512, (index // 3 + 1) * 512))
        cell, removed = remove_noise(cell)
        removed_noise += removed
        bbox = cell.getchannel('A').point(lambda value:255 if value > 8 else 0).getbbox()
        if bbox is None:
            raise ValueError('Empty landmark cell: ' + name)
        cell = cell.crop(bbox)
        alpha = cell.getchannel('A')
        inside = alpha.filter(ImageFilter.MinFilter(5))
        pixels = []
        for (r,g,b,a), minimum in zip(cell.getdata(), inside.getdata()):
            if a and minimum < a and min(r,b) > g + 24:
                spill = min(r,b) - g - 24
                r -= spill
                b -= spill
                changed += 1
            pixels.append((r,g,b,a) if a else (0,0,0,0))
        cell.putdata(pixels)
        cell.thumbnail((330, 370), Image.Resampling.LANCZOS)
        left = (index % 3) * 341 + (341-cell.width)//2
        top = (index // 3) * 512 + 490-cell.height
        atlas.paste(cell, (left, top))
        parts.append({'id':name, 'x':left, 'y':1024-top-cell.height, 'w':cell.width, 'h':cell.height,
                      'sourceCell':index, 'nativeAspectPreserved':True})
    atlas.save(output/'landmarks-atlas.png', optimize=True)
    review = Image.new('RGBA', atlas.size, '#263945')
    review.alpha_composite(atlas)
    review.convert('RGB').save(output/'review-dark.jpg', quality=92)
    heights = {'social-hall':2.8, 'merchant-stall':2.55, 'herbal-clinic':2.75,
               'village-well':2.15, 'hunter-post':2.25, 'suoi-thanh-minh-portal':2.8}
    positions = {'social-hall':18.5, 'merchant-stall':23.0, 'herbal-clinic':27.0,
                 'village-well':31.0, 'hunter-post':35.0, 'suoi-thanh-minh-portal':43.0}
    layers=[]
    for part in parts:
        height=heights[part['id']]
        layers.append({'id':part['id'], 'part':part['id'], 'x':positions[part['id']],
                       'y':-1.62+height/2, 'width':height*part['w']/part['h'], 'height':height,
                       'order':-4, 'parallax':0})
    manifest={'id':'map01a-six-landmarks-v1', 'status':'DRAFT_REQUIRES_PLAYER_REVIEW',
              'source':str(source), 'sourceSha256':digest(source), 'parts':parts, 'layers':layers,
              'atlas':[1024,1024], 'pngBytes':(output/'landmarks-atlas.png').stat().st_size,
              'estimatedBc3Bytes':1024*1024, 'upscaled':False, 'edgePixelsDespill':changed,
              'detachedNoisePixelsRemoved':removed_noise}
    (output/'landmarks-layout.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2)+'\n')
    print(json.dumps({'count':len(parts),'pngBytes':manifest['pngBytes'],'removedNoise':removed_noise}))


if __name__ == '__main__':
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--source',type=Path,required=True)
    parser.add_argument('--output',type=Path,required=True)
    args=parser.parse_args()
    pack(args.source,args.output)
