#!/usr/bin/env python3
"""Pack one generated Map01A module batch; preserves source and alpha QA evidence.
Edge cleanup adapts the stopped class task's edge-only despill policy to magenta.
"""
import argparse
import hashlib
import json
from pathlib import Path
from PIL import Image, ImageFilter


def digest(p):
    return hashlib.sha256(p.read_bytes()).hexdigest()


def pack(source, output):
    original=source/'terrain-vegetation-alpha-v1.png'
    with Image.open(original) as raw:
        image=raw.convert('RGBA')
    if image.size!=(1254,1254):
        raise ValueError('Requires reviewed generated source dimensions; do not reuse rectangles blindly')
    alpha=image.getchannel('A');inside=alpha.filter(ImageFilter.MinFilter(5))
    pixels=[];changed=0
    for (r,g,b,a),minimum in zip(image.getdata(),inside.getdata()):
        if a and minimum<a and min(r,b)>g+24:
            spill=min(r,b)-g-24;r-=spill;b-=spill;changed+=1
        pixels.append((r,g,b,a) if a else (0,0,0,0))
    image.putdata(pixels)
    assert image.getchannel('A').tobytes()==alpha.tobytes()
    specs=[('stone-clean',(7,94,620,229)),('stone-moss',(637,81,1247,232)),
           ('grass-bank',(7,330,620,541)),('wood-bridge',(637,380,1247,548)),
           ('bamboo',(8,552,620,1023)),('peach',(631,550,1245,981)),
           ('ferns-flowers',(7,1018,619,1233)),('moss-rocks',(634,1001,1247,1241))]
    output.mkdir(parents=True,exist_ok=False)
    atlas=Image.new('RGBA',(1024,1024));parts=[]
    for i,(name,box) in enumerate(specs):
        crop=image.crop(box)
        crop.thumbnail((500,244),Image.Resampling.LANCZOS)
        x,y=(i%2)*512+6,(i//2)*256+6
        atlas.paste(crop,(x,y))
        crop.save(output/(name+'.png'),optimize=True)
        parts.append({'id':name,'x':x,'y':1024-y-crop.height,'w':crop.width,'h':crop.height,
                      'sourceRect':box,'sourcePixels':[box[2]-box[0],box[3]-box[1]],
                      'nativeAspectPreserved':True,'runtimeReady':False})
    atlas.save(output/'modules-atlas.png',optimize=True)
    backdrop=Image.new('RGBA',atlas.size,'#253945');backdrop.alpha_composite(atlas)
    backdrop.convert('RGB').save(output/'review-dark.jpg',quality=90)
    report={'id':'map01a-generated-modules-v1','status':'DRAFT_REQUIRES_PLAYER_REVIEW',
            'source':str(original),'sourceSha256':digest(original),'parts':parts,
            'atlas':[1024,1024],'pngBytes':(output/'modules-atlas.png').stat().st_size,
            'estimatedBc3Bytes':1024*1024,'estimatedAstc6x6Bytes':171*171*16,
            'alphaPreservedBeforePacking':True,'edgePixelsDespill':changed,
            'requestedSource':[1024,1024],'actualSource':[1254,1254],
            'note':'Tool ignored requested size/alpha; actual source and correction retained. Packing never upscales.'}
    (output/'manifest.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n')
    print(json.dumps({k:report[k] for k in ['status','pngBytes','edgePixelsDespill']}))


if __name__=='__main__':
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument('--source',type=Path,required=True);p.add_argument('--output',type=Path,required=True)
    a=p.parse_args();pack(a.source,a.output)
