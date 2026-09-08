"""Bake shared ornament relief as tangent normal data, preserving the color atlas.

This derives a shallow height field from gold coverage; it is not a sculpted
high-to-low bake and does not replace the garment's silhouette geometry.
Run with Blender's bundled numpy. Face/hair and unowned atlas regions stay flat.
"""
import argparse,sys
from pathlib import Path
import bpy
import numpy as np
p=argparse.ArgumentParser();p.add_argument('--albedo',type=Path,required=True);p.add_argument('--output',type=Path,required=True)
a=p.parse_args(sys.argv[sys.argv.index('--')+1:]);im=bpy.data.images.load(str(a.albedo.resolve()));assert tuple(im.size)==(4096,2048)
im.scale(2048,1024);c=np.array(im.pixels[:],dtype=np.float32).reshape(1024,2048,4);r,g,b=c[:,:,0],c[:,:,1],c[:,:,2]
coverage=np.clip((r-b-.015)/.08,0,1)*np.clip((g-b-.01)/.05,0,1)*np.clip(((r-b)/(r+.01)-.35)/.2,0,1)
owned=np.zeros((1024,2048),np.float32)
# Reusable authored boots, shoulder armor, torso and robe tiles only.
for x,y,w,h in [(0,0,576,576),(576,0,576,576),(1152,192,576,576),(576,576,576,576)]:owned[y//2:(y+h)//2,x//2:(x+w)//2]=1
height=coverage*owned
# A shallow bevel on the ornament, avoiding noisy color-to-normal conversion.
for _ in range(3):
 q=np.pad(height,1,mode='edge');height=(q[1:-1,1:-1]*4+q[:-2,1:-1]+q[2:,1:-1]+q[1:-1,:-2]+q[1:-1,2:])/8
height*=owned
ny,nx=np.gradient(height);nx*=owned;ny*=owned;normal=np.stack((-nx*2.2,-ny*2.2,np.ones_like(height)),axis=-1);normal/=np.linalg.norm(normal,axis=-1,keepdims=True)
out=np.ones((1024,2048,4),np.float32);out[:,:,:3]=normal*.5+.5
assert np.allclose(out[owned==0,:3],(.5,.5,1)), 'Unowned skin/hair must remain neutral'
image=bpy.data.images.new('Wardrobe shallow relief',width=2048,height=1024,alpha=False);image.colorspace_settings.name='Non-Color';image.pixels=out.ravel();a.output.parent.mkdir(parents=True,exist_ok=True);image.filepath_raw=str(a.output.resolve());image.file_format='PNG';image.save()
print('LGO_WARDROBE_RELIEF_READY size=2048x1024 unowned_flat=true sculpt_bake=false',flush=True)
