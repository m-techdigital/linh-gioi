"""Dedicated polar shoulder UVs in a guarded free patch of the existing shared atlas."""
import bpy,math,argparse,sys,numpy as np
from pathlib import Path
from mathutils import Matrix
p=argparse.ArgumentParser();p.add_argument('--output',type=Path,required=True)
a=p.parse_args(sys.argv[sys.argv.index('--')+1:]);a.output.mkdir(parents=True,exist_ok=True)
sources=[('keeper','client/art-source/gate-keeper/KeeperReconstruction.blend'),('player','client/art-source/arrival-scene/ArrivalScene.blend')]
for actor,path in sources:
 bpy.ops.wm.open_mainfile(filepath=path);o=next(o for o in bpy.data.objects if o.type=='MESH' and o.name in ('Keeper Macro Reconstruction','Arrival Scene Reconstruction'))
 for f in o.data.polygons:
  if f.material_index!=0 or o.data.attributes['appearance_slot'].data[f.index].value==15:continue
  uv=[o.data.uv_layers.active.data[i].uv for i in f.loop_indices]
  assert max(v.x for v in uv)<576/4096 or min(v.x for v in uv)>1152/4096 or min(v.y for v in uv)>576/2048,'Atlas patch intersects another wardrobe part'
original=None
for actor,path in sources:
 bpy.ops.wm.open_mainfile(filepath=path);o=next(o for o in bpy.data.objects if o.type=='MESH' and o.name in ('Keeper Macro Reconstruction','Arrival Scene Reconstruction'))
 atlas=bpy.data.images['KeeperReconstructionAlbedo'];pixels=np.array(atlas.pixels[:],dtype=np.float32).reshape(2048,4096,4)
 if original is None:original=pixels.copy()
 else:assert np.array_equal(original,pixels),'Shared actor atlases differ'
 tile=bpy.data.images.load(str(Path('client/art-source/shared-wardrobe/ceremonial-armor-albedo.png').resolve()));tile.scale(512,512)
 pixels[:576,576:1152]=np.pad(np.array(tile.pixels[:],dtype=np.float32).reshape(512,512,4),((32,32),(32,32),(0,0)),mode='edge')
 assert np.array_equal(pixels[576:],original[576:]) and np.array_equal(pixels[:576,:576],original[:576,:576]) and np.array_equal(pixels[:576,1152:],original[:576,1152:])
 atlas.pixels=pixels.ravel();atlas.pack()
 if actor=='keeper':atlas.filepath_raw=str((a.output/'KeeperReconstructionAlbedo.png').resolve());atlas.file_format='PNG';atlas.save()
 r=bpy.data.objects['Armature']
 for b in r.pose.bones:b.matrix_basis.identity()
 for side,angle in [('l',-60),('r',60)]:
  b=r.pose.bones['upperarm_'+side];b.rotation_mode='XYZ';b.rotation_euler.z=math.radians(angle)
 bpy.context.view_layer.update();transforms={b.name:r.pose.bones[b.name].matrix@b.matrix_local.inverted() for b in r.data.bones}
 uv=o.data.uv_layers.active.data;slots=o.data.attributes['appearance_slot'];count=0
 for f in o.data.polygons:
  if slots.data[f.index].value!=15:continue
  for li in f.loop_indices:
   v=o.data.vertices[o.data.loops[li].vertex_index];m=Matrix([[0.]*4 for _ in range(4)])
   for g in v.groups:m+=transforms[o.vertex_groups[g.group].name]*g.weight
   co=m@v.co
   # Recover the original dome coordinates before the swept plate deformation.
   x=(abs(co.x)-.205)/1.14;q=max(0,min(1,x/.108));y=(co.y-.061+.012*q)/.88;z=co.z-.060*q*q*q
   sx=x/.108;sy=y/.115;angle=math.atan2(sy,sx)
   phi=math.atan2(math.hypot(sx,sy),(z-1.468-.014*max(0,sx))/.075)
   radius=.40*max(0,min(1,phi/(.07+math.pi*.53)))
   u=.5+radius*math.cos(angle);vcoord=.5+radius*math.sin(angle)
   uv[li].uv=((608.5+u*511)/4096,(32.5+vcoord*511)/2048);count+=1
 for b in r.pose.bones:b.matrix_basis.identity()
 bpy.context.view_layer.update();bpy.context.preferences.filepaths.save_version=0
 bpy.ops.wm.save_as_mainfile(filepath=str((a.output/f'{actor}.blend').resolve()),compress=True)
 print(f'LGO_ARMOR_ATLAS_READY actor={actor} loops={count} outside_patch_unchanged=true')
