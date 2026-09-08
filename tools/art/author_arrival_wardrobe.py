"""Migrate e2f0282 arrival outfit to editable garments using the shared Keeper kit."""
import argparse,sys,math,json
from pathlib import Path
import bpy,bmesh
from mathutils import Matrix,Vector
p=argparse.ArgumentParser();p.add_argument('--output',type=Path,required=True);a=p.parse_args(sys.argv[sys.argv.index('--')+1:]);a.output.mkdir(parents=True,exist_ok=True)
bpy.ops.wm.open_mainfile(filepath='client/art-source/arrival-scene/ArrivalScene.blend')
o=bpy.data.objects['Arrival Scene Reconstruction'];rig=bpy.data.objects['Armature'];body=bpy.data.materials['Keeper Reconstruction'];face=bpy.data.materials['Keeper Reconstruction Face']
for b in rig.pose.bones:b.matrix_basis.identity()
slot=o.data.attributes['appearance_slot'];remove=[]
for f in o.data.polygons:
 sid=slot.data[f.index].value;z=sum(o.data.vertices[i].co.z for i in f.vertices)/len(f.vertices)
 arm=sum(sum(g.weight for g in o.data.vertices[i].groups if o.vertex_groups[g.group].name.startswith(('upperarm_','lowerarm_','clavicle_'))) for i in f.vertices)/len(f.vertices)
 if sid==15 or (sid==10 and z<1.108 and arm<.35):remove.append(f.index)
bm=bmesh.new();bm.from_mesh(o.data);bm.faces.ensure_lookup_table();bmesh.ops.delete(bm,geom=[bm.faces[i] for i in remove],context='FACES');bm.to_mesh(o.data);bm.free()
catalog=json.loads(Path('client/art-source/appearance-slots.json').read_text());ids=sorted(set(x.value for x in o.data.attributes['appearance_slot'].data))
for sid in ids:
 part=o.copy();part.data=o.data.copy();part.name=catalog[str(sid)]['key'];bpy.context.collection.objects.link(part)
 bm=bmesh.new();bm.from_mesh(part.data);tag=bm.faces.layers.int['appearance_slot'];bmesh.ops.delete(bm,geom=[f for f in bm.faces if f[tag]!=sid],context='FACES');bm.to_mesh(part.data);bm.free();part.asset_mark()
bpy.data.objects.remove(o,do_unlink=True)
names=['Outer robe left','Outer robe right','Ivory under robe','Shoulder mantle l','Shoulder mantle r']
with bpy.data.libraries.load('client/art-source/gate-keeper/KeeperWardrobe.blend',link=False) as (src,dst):dst.objects=names
for part in dst.objects:
 bpy.context.collection.objects.link(part);part.parent=rig
 for mod in part.modifiers:
  if mod.type=='ARMATURE':mod.object=rig
 part.data.materials.clear();part.data.materials.append(body)
 if part.name.startswith('Shoulder'):
  side=part.name[-1];anchor=rig.data.bones['clavicle_'+side].tail_local
  for v in part.data.vertices:v.co=anchor+(v.co-anchor)*.83
 else:
  for v in part.data.vertices:v.co.z=1.135-(1.135-v.co.z)*.76
 part.asset_mark()
# Split the front tabard at the actual garment seam. Each half can follow its
# corresponding thigh during high-knee locomotion without bridging both legs.
inner=bpy.data.objects['Ivory under robe']
for side,positive in [('l',True),('r',False)]:
 half=inner.copy();half.data=inner.data.copy();half.name='Ivory tabard '+side;bpy.context.collection.objects.link(half)
 bm=bmesh.new();bm.from_mesh(half.data);bmesh.ops.bisect_plane(bm,geom=list(bm.verts)+list(bm.edges)+list(bm.faces),dist=.000001,plane_co=(0,0,0),plane_no=(1,0,0),clear_inner=positive,clear_outer=not positive);bm.to_mesh(half.data);bm.free();half.asset_mark()
bpy.data.objects.remove(inner,do_unlink=True)
for name,side in [('Outer robe left','l'),('Outer robe right','r'),('Ivory tabard l','l'),('Ivory tabard r','r')]:
 part=bpy.data.objects[name]
 for v in part.data.vertices:
  t=max(0.,min(1.,(1.08-v.co.z)/.25));leg=t*t*(3-2*t)
  for group in part.vertex_groups:group.remove([v.index])
  for bone,weight in [('pelvis',1-leg),('thigh_'+side,leg)]:
   if weight>1e-6:part.vertex_groups[bone].add([v.index],weight,'REPLACE')
# Short traveller mantle from the storyboard; narrower than the guardian's cape.
vs=[];fs=[];uv=[];rows=28;cols=28
for row in range(rows+1):
 t=row/rows
 for col in range(cols+1):
  u=col/cols;s=2*u-1
  vs.append((s*(.18+.10*t),.22+.105*t+.012*math.sin(math.pi*t)*math.cos(s*math.pi*4),1.51-.71*t+.065*t**4*(1-s*s)))
  uv.append(((608.5+(.04+.92*u)*511)/4096,(608.5+(.34+.61*(1-t))*511)/2048))
for row in range(rows):
 for col in range(cols):k=row*(cols+1)+col;fs.append((k,k+1,k+cols+2,k+cols+1))
mesh=bpy.data.meshes.new('Traveller mantle');mesh.from_pydata(vs,[],fs);mesh.update();part=bpy.data.objects.new('Cape',mesh);bpy.context.collection.objects.link(part);mesh.materials.append(body);layer=mesh.uv_layers.new(name='UVMap')
for f in mesh.polygons:
 f.use_smooth=True
 for li in f.loop_indices:layer.data[li].uv=uv[mesh.loops[li].vertex_index]
for key,value in [('appearance_slot',12),('npc_part',3)]:
 attr=mesh.attributes.new(key,'INT','FACE')
 for d in attr.data:d.value=value
part.vertex_groups.new(name='spine_03').add(list(range(len(vs))),1.,'REPLACE');mod=part.modifiers.new('Cloth thickness','SOLIDIFY');mod.thickness=.003;mod.offset=0;mod=part.modifiers.new('Shared rig','ARMATURE');mod.object=rig;part.parent=rig;part.asset_mark()
# Appended library data may retain an unused donor rig; never export it.
for other in list(bpy.data.objects):
 if other.type=='ARMATURE' and other!=rig:bpy.data.objects.remove(other,do_unlink=True)
for side,angle in [('l',-60),('r',60)]:
 b=rig.pose.bones['upperarm_'+side];b.rotation_mode='XYZ';b.rotation_euler.z=math.radians(angle)
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str((a.output/'ArrivalWardrobe.blend').resolve()),compress=True)
print('LGO_ARRIVAL_WARDROBE_READY removed_faces='+str(len(remove)),flush=True)
