#!/usr/bin/env python3
"""Extract pale board backgrounds from the reviewed opaque props as one batch.
Not suitable for characters, white clothes, vegetation, glow or arbitrary boards.
"""
from collections import deque
import argparse
import hashlib
import json
from pathlib import Path
from PIL import Image, ImageDraw


def run(source, output):
    manifest = json.loads((source/'MANIFEST.json').read_text())
    entries = [e for e in manifest['entries'] if e['group']=='props']
    if len(entries)!=16:
        raise ValueError('Expected complete reviewed sheet08 group of 16 props')
    for e in entries:
        if hashlib.sha256((source/e['file']).read_bytes()).hexdigest()!=e['sha256']:
            raise ValueError('Crop hash changed: '+e['file'])
    for e in entries:
        with Image.open(source/e['file']) as raw:
            if max(raw.size)>124:
                raise ValueError('Native crop exceeds atlas cell: '+e['name'])
    output.mkdir(parents=True, exist_ok=False)
    atlas = Image.new('RGBA',(512,512))
    review = Image.new('RGB',(1200,640),'#253845')
    draw = ImageDraw.Draw(review)
    parts=[]
    for index,e in enumerate(entries):
        with Image.open(source/e['file']) as raw:
            image=raw.convert('RGBA')
        original=list(image.getdata())
        width,height=image.size
        def matte(i):
            r,g,b,_=original[i]
            return min(r,g,b)>=155 and b>=r-6 and g>=r-6 and max(r,g,b)-min(r,g,b)<65
        border=set(range(width)) | set(range((height-1)*width,height*width))
        border.update(y*width for y in range(height))
        border.update(y*width+width-1 for y in range(height))
        background={i for i in border if matte(i)}
        queue=deque(background)
        while queue:
            i=queue.popleft();x,y=i%width,i//width
            for nx,ny in ((x-1,y),(x+1,y),(x,y-1),(x,y+1)):
                if not (0<=nx<width and 0<=ny<height): continue
                n=ny*width+nx
                if n not in background and matte(n):
                    background.add(n);queue.append(n)
        pixels=[(0,0,0,0) if i in background else pixel for i,pixel in enumerate(original)]
        image.putdata(pixels)
        if image.width>124 or image.height>124:
            raise ValueError('Native crop too large for group atlas: '+e['name'])
        x,y=(index%4)*128+2,(index//4)*128+2
        atlas.paste(image,(x,y))
        image.save(output/e['file'],optimize=True)
        parts.append({'id':e['name'],'x':x,'y':512-y-image.height,'w':image.width,'h':image.height,
                      'sourceCrop':e,'status':'ALPHA_CANDIDATE_REQUIRES_VISUAL_REVIEW',
                      'sha256':hashlib.sha256((output/e['file']).read_bytes()).hexdigest()})
        rx,ry=(index%8)*150,(index//8)*320
        for band,color in enumerate(['#253845','#b7c6b4']):
            tile=Image.new('RGBA',(150,135),color)
            tile.alpha_composite(image,((150-image.width)//2,5))
            review.paste(tile.convert('RGB'),(rx,ry+band*140))
        draw.text((rx+3,ry+281),e['name'].replace('prop-',''),fill='white')
    atlas.save(output/'sheet-props-atlas.png',optimize=True)
    review.save(output/'alpha-review.jpg',quality=92)
    # Four reviewed opaque silhouettes for the opt-in Player comparison.
    # Tea table, rack and planters retain baked interior matte; do not ingest them.
    selection=['prop-crate','prop-bench','prop-lamp-post','prop-stone-lamp']
    runtime=Image.new('RGBA',(256,256)); runtime_parts=[]
    for i,name in enumerate(selection):
        entry=next(p for p in parts if p['id']==name)
        with Image.open(output/(name+'.png')) as piece:
            x,y=(i%2)*128+2,(i//2)*128+2
            runtime.paste(piece,(x,y))
            runtime_parts.append({'id':name,'x':x,'y':256-y-piece.height,'w':piece.width,'h':piece.height})
    runtime.save(output/'sheet-props.png',optimize=True)
    layers=[]
    for name,part,x,w in [('market-crates','prop-crate',3.6,.82),('village-bench','prop-bench',2.2,1.08),('gate-lamp-left','prop-lamp-post',-4.2,.55),('gate-lamp-right','prop-stone-lamp',.95,.55)]:
        item=next(p for p in runtime_parts if p['id']==part)
        h=w*item['h']/item['w']
        layers.append({'id':name,'part':part,'x':x,'y':-1.62+h/2,'width':w,'height':h,'order':-2,'parallax':0})
    (output/'sheet-props-layout.json').write_text(json.dumps({'parts':runtime_parts,'layers':layers,'status':'DRAFT_PLAYER_COMPARISON','sourceManifest':str(output/'manifest.json')},indent=2)+'\n')
    report={'id':'map01a-sheet08-props-v1','status':'DRAFT_ALPHA_REVIEW',
            'sourceManifestSha256':hashlib.sha256((source/'MANIFEST.json').read_bytes()).hexdigest(),
            'parts':parts,'textureSize':[512,512],'pngBytes':(output/'sheet-props-atlas.png').stat().st_size,
            'estimatedBc3Bytes':512*512,'upscaled':False,'runtimeApproved':False}
    (output/'manifest.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n')
    print(json.dumps({'count':len(parts),'atlasPngBytes':report['pngBytes'],'output':str(output)}))


if __name__=='__main__':
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument('--source',type=Path,required=True)
    p.add_argument('--output',type=Path,required=True)
    a=p.parse_args();run(a.source,a.output)
