"""Shape shared shoulder armor into swept plates and raise the existing V lapel surface."""
import argparse,sys,math
from pathlib import Path
import bpy
from mathutils import Matrix
p=argparse.ArgumentParser();p.add_argument('--source',type=Path,required=True);p.add_argument('--output',type=Path,required=True)
a=p.parse_args(sys.argv[sys.argv.index('--')+1:]);bpy.ops.wm.open_mainfile(filepath=str(a.source.resolve()))
o=next(o for o in bpy.data.objects if o.type=='MESH' and o.name in ('Keeper Macro Reconstruction','Arrival Scene Reconstruction'));r=bpy.data.objects['Armature']
for b in r.pose.bones:b.matrix_basis.identity()
for side,angle in [('l',-60),('r',60)]:
 b=r.pose.bones['upperarm_'+side];b.rotation_mode='XYZ';b.rotation_euler.z=math.radians(angle)
bpy.context.view_layer.update();transforms={b.name:r.pose.bones[b.name].matrix@b.matrix_local.inverted() for b in r.data.bones}
slot=o.data.attributes['appearance_slot'];shoulder=set();upper=set()
for f in o.data.polygons:
 if slot.data[f.index].value==15:shoulder.update(f.vertices)
 elif slot.data[f.index].value==10:upper.update(f.vertices)
counts=[0,0]
for i in shoulder|upper:
 v=o.data.vertices[i];weights={o.vertex_groups[g.group].name:g.weight for g in v.groups};m=Matrix([[0.]*4 for _ in range(4)])
 for n,w in weights.items():m+=transforms[n]*w
 co=m@v.co
 if i in shoulder:
  side='l' if co.x>0 else 'r';sign=1 if co.x>0 else -1;q=max(0,min(1,(abs(co.x)-.205)/.108))
  co.x=sign*(.205+(abs(co.x)-.205)*1.14)
  co.y=.061+(co.y-.061)*.88-.012*q
  co.z+=.060*q*q*q
  name='clavicle_'+side;v.co=transforms[name].inverted()@co
  for group in o.vertex_groups:group.remove([i])
  o.vertex_groups[name].add([i],1.,'REPLACE');counts[0]+=1
 elif 1.37<co.z<1.575 and abs(co.x)<.12 and co.y<-.015:
  # Raised lapel follows the existing painted V, without a floating ribbon or new slot.
  t=(co.z-1.37)/.205;line=.012+.060*t;ridge=math.exp(-((abs(co.x)-line)/.018)**2)
  co.y-=.014*ridge*math.sin(math.pi*t)**.5
  v.co=m.inverted()@co;counts[1]+=1
for b in r.pose.bones:b.matrix_basis.identity()
bpy.context.view_layer.update();bpy.context.preferences.filepaths.save_version=0;a.output.parent.mkdir(parents=True,exist_ok=True)
bpy.ops.wm.save_as_mainfile(filepath=str(a.output.resolve()),compress=True)
print('LGO_UPPER_ARMOR_SHAPED shoulder_vertices=%d lapel_vertices=%d materials_unchanged=true'%tuple(counts))
