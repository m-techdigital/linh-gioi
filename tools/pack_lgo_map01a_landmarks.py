#!/usr/bin/env python3
"""Trim and pack the six-landmark Map01A generation as one runtime-sized batch."""
import argparse
import hashlib
import json
from collections import deque
from pathlib import Path
from PIL import Image, ImageFilter, ImageDraw, ImageChops


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


def prepare_regions(sheet, regions):
    if len(regions) != 6:
        raise ValueError('Exactly six landmark regions are required')
    coverage = Image.new('L', sheet.size)
    draw = ImageDraw.Draw(coverage)
    for index, rect in enumerate(regions):
        if len(rect) != 4 or any(type(value) is not int for value in rect):
            raise ValueError('Region must contain four integer coordinates')
        x0, y0, x1, y1 = rect
        if not (0 <= x0 < x1 <= sheet.width and 0 <= y0 < y1 <= sheet.height):
            raise ValueError('Region outside source boundary')
        for other in regions[:index]:
            if max(x0, other[0]) < min(x1, other[2]) and max(y0, other[1]) < min(y1, other[3]):
                raise ValueError('Landmark regions overlap')
        draw.rectangle((x0, y0, x1-1, y1-1), fill=255)
        alpha = sheet.crop(rect).getchannel('A').point(lambda value: 255 if value > 8 else 0)
        bbox = alpha.getbbox()
        if bbox is None:
            raise ValueError('Empty landmark region')
        if bbox[0] == 0 or bbox[1] == 0 or bbox[2] == alpha.width or bbox[3] == alpha.height:
            raise ValueError('Landmark cut at region boundary; author the whole object, not a fixed grid')
    significant = sheet.getchannel('A').point(lambda value: 255 if value > 8 else 0)
    if ImageChops.subtract(significant, coverage).getbbox() is not None:
        raise ValueError('Landmark region coverage drops source content')


def key_magenta(sheet):
    # Same soft chroma radius as the existing equipment-sheet ingestion tool.
    # Only ingestion changes alpha; opaque artwork RGB remains untouched here.
    result = sheet.copy()
    pixels = []
    for red, green, blue, alpha in sheet.getdata():
        distance = ((255-red)**2 + green**2 + (255-blue)**2)**.5
        keyed = min(alpha, max(0, min(255, round((distance-70)*6))))
        pixels.append((red, green, blue, keyed) if keyed else (0, 0, 0, 0))
    result.putdata(pixels)
    return result


def pack(source, output, regions=None, chroma_key=False):
    with Image.open(source) as raw:
        sheet = raw.convert('RGBA')
    if sheet.size != (1536, 1024):
        raise ValueError('Landmark cells require reviewed 1536x1024 source')
    if chroma_key:
        sheet = key_magenta(sheet)
    sheet, removed_noise = remove_noise(sheet)
    if regions is None:
        regions = [[i % 3 * 512, i // 3 * 512, (i % 3+1)*512, (i // 3+1)*512] for i in range(6)]
    prepare_regions(sheet, regions)
    output.mkdir(parents=True, exist_ok=False)
    atlas = Image.new('RGBA', (1024, 1024))
    names = ['social-hall', 'merchant-stall', 'herbal-clinic', 'village-well', 'hunter-post', 'suoi-thanh-minh-portal']
    parts, changed = [], 0
    for index, name in enumerate(names):
        region = regions[index]
        cell = sheet.crop(region)
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
        cell.thumbnail((500, 329), Image.Resampling.LANCZOS)
        left = (index % 2) * 512 + (512-cell.width)//2
        top = (index // 2) * 341 + 335-cell.height
        atlas.paste(cell, (left, top))
        parts.append({'id':name, 'x':left, 'y':1024-top-cell.height, 'w':cell.width, 'h':cell.height,
                      'sourceRect':region, 'sourceContentRect':[bbox[0]+region[0], bbox[1]+region[1], bbox[2]+region[0], bbox[3]+region[1]], 'nativeAspectPreserved':True})
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
              'detachedNoisePixelsRemoved':removed_noise, 'sourcePixelCoverage':1,
              'alphaPolicy':'soft-magenta-radius-70-edge-6' if chroma_key else 'source-alpha'}
    (output/'landmarks-layout.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2)+'\n')
    print(json.dumps({'count':len(parts),'pngBytes':manifest['pngBytes'],'removedNoise':removed_noise}))


if __name__ == '__main__':
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--source',type=Path,required=True)
    parser.add_argument('--output',type=Path,required=True)
    parser.add_argument('--regions', type=Path, help='JSON array of six source rectangles, reviewed against the source')
    parser.add_argument('--chroma-key', action='store_true', help='Remove the authored magenta background before region packing')
    args=parser.parse_args()
    pack(args.source,args.output, json.loads(args.regions.read_text()) if args.regions else None, args.chroma_key)
