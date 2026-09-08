"""Fit Keeper's continuous cape root across the back of the shoulders (from 31c3bd7)."""
import bpy,math,sys,argparse
from pathlib import Path
from mathutils import Matrix,Vector
from mathutils.bvhtree import BVHTree
p=argparse.ArgumentParser();p.add_argument('--source',type=Path,required=True);p.add_argument('--output',type=Path,required=True);a=p.parse_args(sys.argv[sys.argv.index('--')+1:]);bpy.ops.wm.open_mainfile(filepath=str(a.source.resolve()))
o=bpy.data.objects['Keeper Macro Reconstruction'];r=bpy.data.objects['Armature']
for b in r.pose.bones:b.matrix_basis.identity()
for side,angle in [('l',-60),('r',60)]:
 b=r.pose.bones['upperarm_'+side];b.rotation_mode='XYZ';b.rotation_euler.z=math.radians(angle)
bpy.context.view_layer.update();tr={b.name:r.pose.bones[b.name].matrix@b.matrix_local.inverted() for b in r.data.bones};coords=[];weights=[]
for v in o.data.vertices:
 w={o.vertex_groups[g.group].name:g.weight for g in v.groups};m=Matrix([[0.]*4 for _ in range(4)])
 for n,value in w.items():m+=tr[n]*value
 coords.append(m@v.co);weights.append(w)
slot=o.data.attributes['appearance_slot'];cape=set();body=[]
for f in o.data.polygons:
 if slot.data[f.index].value==12:cape.update(f.vertices)
 elif slot.data[f.index].value in (10,15):body.append(list(f.vertices))
bvh=BVHTree.FromPolygons(coords,body)
def ease(t):
 t=max(0,min(1,t));return t*t*(3-2*t)
changed=0;fitted=0
for i in cape:
 co=coords[i].copy();t=ease((co.z-1.28)/.21)
 if t<=0:continue
 width=abs(co.x)/max(.165,.165+.25*(1.49-co.z));co.x*=1+.48*t;co.z+=.027*t*min(1,width)**.6
 hit=bvh.ray_cast(Vector((co.x,.6,co.z)),Vector((0,-1,0)),.6)[0]
 co.y+=.025*t
 if hit is not None:co.y=max(co.y,hit.y+.022*t);fitted+=1
 w=weights[i].copy();clavicle=.55*t*ease((abs(co.x)-.12)/.10)
 if clavicle>0:
  w={n:value*(1-clavicle) for n,value in w.items()};name='clavicle_'+('l' if co.x>0 else 'r');w[name]=w.get(name,0)+clavicle
 m=Matrix([[0.]*4 for _ in range(4)])
 for group in o.vertex_groups:group.remove([i])
 for n,value in w.items():o.vertex_groups[n].add([i],value,'REPLACE');m+=tr[n]*value
 o.data.vertices[i].co=m.inverted()@co;changed+=1
for b in r.pose.bones:b.matrix_basis.identity()
bpy.context.view_layer.update();bpy.context.preferences.filepaths.save_version=0;a.output.parent.mkdir(parents=True,exist_ok=True);bpy.ops.wm.save_as_mainfile(filepath=str(a.output.resolve()),compress=True)
print(f'LGO_CAPE_MANTLE_READY changed={changed} fitted={fitted} topology_materials_uv_unchanged=true')
