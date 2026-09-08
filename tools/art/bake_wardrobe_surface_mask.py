"""Bake a low-resolution URP metallic(R)/smoothness(A) data map for shared wardrobe gold."""
import argparse,sys,bpy,numpy as np
from pathlib import Path
p=argparse.ArgumentParser();p.add_argument('--albedo',type=Path,required=True);p.add_argument('--output',type=Path,required=True)
a=p.parse_args(sys.argv[sys.argv.index('--')+1:]);im=bpy.data.images.load(str(a.albedo.resolve()));w,h=im.size;assert (w,h)==(4096,2048)
c=np.array(im.pixels[:],dtype=np.float32).reshape(h,w,4);r,g,b=c[:,:,0],c[:,:,1],c[:,:,2]
gold=np.clip((r-b-.015)/.08,0,1)*np.clip((g-b-.01)/.05,0,1)
# Face/hair modeling area is excluded; face also has its own separate material.
owned=np.zeros((h,w),np.float32);owned[:,2048:]=1;owned[:576,:1152]=1;owned[192:768,1152:1728]=1;owned[576:1152,:1152]=1;gold*=np.clip(((r-b)/(r+.01)-.35)/.2,0,1)
gold*=owned
out=np.zeros_like(c);out[:,:,0]=gold*.58;out[:,:,3]=.17-.07*owned+gold*.48
mask=bpy.data.images.new('Wardrobe surface data',width=w,height=h,alpha=True);mask.colorspace_settings.name='Non-Color';mask.pixels=out.ravel();mask.scale(1024,512)
a.output.parent.mkdir(parents=True,exist_ok=True);mask.filepath_raw=str(a.output.resolve());mask.file_format='PNG';mask.save()
print('LGO_WARDROBE_SURFACE_MASK_READY size=1024x512 metallic_max=0.58 smoothness=0.10..0.58')
