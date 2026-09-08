"""Pack a dedicated boot surface into unused shared atlas space and unwrap both actors cylindrically."""
import bpy,math,json,argparse,sys,numpy as np
from pathlib import Path
p=argparse.ArgumentParser();p.add_argument('--output',type=Path,required=True)
a=p.parse_args(sys.argv[sys.argv.index('--')+1:]);a.output.mkdir(parents=True,exist_ok=True)
sources=[('keeper','client/art-source/gate-keeper/KeeperReconstruction.blend'),('player','client/art-source/arrival-scene/ArrivalScene.blend')]
# Protect the entire union of non-boot body UVs, including face area of every polygon.
for actor,path in sources:
 bpy.ops.wm.open_mainfile(filepath=path)
 o=next(o for o in bpy.data.objects if o.type=='MESH' and o.name in ('Keeper Macro Reconstruction','Arrival Scene Reconstruction'))
 for f in o.data.polygons:
  if f.material_index!=0 or o.data.attributes['appearance_slot'].data[f.index].value==14:continue
  uv=[o.data.uv_layers.active.data[i].uv for i in f.loop_indices]
  assert min(v.x for v in uv)>608/4096 or min(v.y for v in uv)>608/2048,'Reserved atlas region intersects existing UVs'
original=None
for actor,path in sources:
 bpy.ops.wm.open_mainfile(filepath=path)
 o=next(o for o in bpy.data.objects if o.type=='MESH' and o.name in ('Keeper Macro Reconstruction','Arrival Scene Reconstruction'))
 atlas=bpy.data.images['KeeperReconstructionAlbedo'];pixels=np.array(atlas.pixels[:],dtype=np.float32).reshape(2048,4096,4)
 if original is None:original=pixels.copy()
 else:assert np.array_equal(original,pixels),'Actors must begin with the same shared atlas'
 tile=bpy.data.images.load(str(Path('client/art-source/shared-wardrobe/ceremonial-boot-albedo.png').resolve()));tile.scale(512,512)
 patch=np.array(tile.pixels[:],dtype=np.float32).reshape(512,512,4)
 #32px gutter prevents neighboring islands bleeding into the boot at mip boundaries.
 pixels[:576,:576]=np.pad(patch,((32,32),(32,32),(0,0)),mode='edge')
 assert np.array_equal(pixels[576:],original[576:]) and np.array_equal(pixels[:576,576:],original[:576,576:])
 atlas.pixels=pixels.ravel();atlas.pack()
 if actor=='keeper':
  atlas.filepath_raw=str((a.output/'KeeperReconstructionAlbedo.png').resolve());atlas.file_format='PNG';atlas.save()
 slot=o.data.attributes['appearance_slot'];uv=o.data.uv_layers.active.data;r=bpy.data.objects['Armature'];changed=0
 for face in o.data.polygons:
  if slot.data[face.index].value!=14:continue
  side='l' if face.center.x>=0 else 'r';bone=r.data.bones['calf_'+side]
  def cylinder(co):
   t=max(0,min(1,(co.z-bone.tail_local.z)/(bone.head_local.z-bone.tail_local.z)))
   center=bone.tail_local.lerp(bone.head_local,t)
   return math.atan2(co.x-center.x,-(co.y-center.y))
  mid=cylinder(face.center)
  for li in face.loop_indices:
   co=o.data.vertices[o.data.loops[li].vertex_index].co;angle=cylinder(co)
   while angle-mid>math.pi:angle-=math.tau
   while angle-mid<-math.pi:angle+=math.tau
   u=max(0,min(1,.5+angle/math.tau));v=max(.02,min(.995,(co.z-.085)/.47))
   if co.z<.14:v=.035+.06*max(0,min(1,co.z/.14))
   uv[li].uv=((32.5+u*511)/4096,(32.5+v*511)/2048);changed+=1
 bpy.context.preferences.filepaths.save_version=0
 bpy.ops.wm.save_as_mainfile(filepath=str((a.output/(actor+'.blend')).resolve()),compress=True)
 print('LGO_CEREMONIAL_BOOTS_PACKED actor='+actor+' loops='+str(changed)+' atlas=4096x2048 unchanged_outside_patch=true')
