"""Editable garment authoring from c4fac56; preview before Unity integration.

Semantic objects remain editable in authoring.blend. A joined export copy retains
shared materials and the existing rig. Lower coat is replaced as a whole with
continuous quad panels and a continuous shared pelvis/thigh support field.
"""
import bpy,bmesh,math,argparse,sys,json
from pathlib import Path
from mathutils import Vector
p=argparse.ArgumentParser();p.add_argument('--output',type=Path,required=True);a=p.parse_args(sys.argv[sys.argv.index('--')+1:]);a.output.mkdir(parents=True,exist_ok=True)
bpy.ops.wm.open_mainfile(filepath='client/art-source/gate-keeper/KeeperReconstruction.blend')
o=bpy.data.objects['Keeper Macro Reconstruction'];rig=bpy.data.objects['Armature']
for b in rig.pose.bones:b.matrix_basis.identity()
slot=o.data.attributes['appearance_slot'];removed=[]
for f in o.data.polygons:
 if slot.data[f.index].value!=10:continue
 z=sum(o.data.vertices[i].co.z for i in f.vertices)/len(f.vertices)
 arm=sum(sum(g.weight for g in o.data.vertices[i].groups if o.vertex_groups[g.group].name.startswith(('upperarm_','lowerarm_','clavicle_'))) for i in f.vertices)/len(f.vertices)
 if z<1.075 and arm<.35:removed.append(f.index)
assert len(removed)>1000
# Preserve the original coat as the weight donor, isolated from cape/skin.
donor=o.copy();donor.data=o.data.copy();bpy.context.collection.objects.link(donor);donor.name='Weight donor - previous coat'
bm=bmesh.new();bm.from_mesh(donor.data);bm.faces.ensure_lookup_table();keep=set(removed);bmesh.ops.delete(bm,geom=[f for f in bm.faces if f.index not in keep],context='FACES');bm.to_mesh(donor.data);bm.free()
for mod in list(donor.modifiers):donor.modifiers.remove(mod)
bm=bmesh.new();bm.from_mesh(o.data);bm.faces.ensure_lookup_table();bmesh.ops.delete(bm,geom=[bm.faces[i] for i in removed],context='FACES');bm.to_mesh(o.data);bm.free()
parts=[]
for name,amin,amax,inner in [('Outer robe left',.30,math.pi,False),('Outer robe right',-math.pi,-.30,False),('Ivory under robe',-.61,.61,True)]:
 vs=[];fs=[];uv=[];rows=24;cols=24
 for row in range(rows+1):
  t=row/rows
  for col in range(cols+1):
   u=col/cols;angle=amin+(amax-amin)*u
   if not inner:angle+=math.copysign(.30*t*t,angle)*math.sin(math.pi*u)
   z=1.09-(.55 if inner else .70)*t
   if inner:z+=.075*t**3*math.exp(-(angle/.16)**2)
   else:z+=.10*t**3*math.cos(angle)**2
   height_t=(1.09-z)/.70
   rx=.125+.115*height_t;ry=.125+.065*height_t
   if inner:rx-=.014;ry-=.014
   fold=.010*math.sin(math.pi*height_t)*math.cos(angle*8)
   vs.append(((rx+fold)*math.sin(angle),.027-(ry+fold)*math.cos(angle),z))
   if inner:uv.append(((1184.5+(.025+.20*u)*511)/4096,(224.5+(.04+.56*(1-t))*511)/2048))
   else:uv.append(((608.5+(.06+.88*u)*511)/4096,(608.5+(.035+.93*(1-t))*511)/2048))
 for row in range(rows):
  for col in range(cols):k=row*(cols+1)+col;fs.append((k,k+cols+1,k+cols+2,k+1))
 mesh=bpy.data.meshes.new(name);mesh.from_pydata(vs,[],fs);mesh.update();ob=bpy.data.objects.new(name,mesh);bpy.context.collection.objects.link(ob);mesh.materials.append(o.data.materials[0]);layer=mesh.uv_layers.new(name='UVMap')
 for f in mesh.polygons:
  f.use_smooth=True
  for li in f.loop_indices:layer.data[li].uv=uv[mesh.loops[li].vertex_index]
 for key,value in [('appearance_slot',10),('npc_part',3)]:
  attr=mesh.attributes.new(key,'INT','FACE')
  for d in attr.data:d.value=value
 for g in donor.vertex_groups:ob.vertex_groups.new(name=g.name)
 bpy.context.view_layer.objects.active=ob
 # Continuous garment support: the old donor contains pelvis-only patches
 # below thigh-weighted strips, so nearest-surface transfer creates fold spikes.
 # Share one height-based support field across all layers, with a smooth centre
 # blend between legs. The waist stays anchored while the hem follows the stride.
 for v in ob.data.vertices:
  t=max(0.,min(1.,(.99-v.co.z)/.40));leg=.65*t*t*(3-2*t)
  left=.5+.5*math.tanh(v.co.x/.065)
  weights={'pelvis':1-leg,'thigh_l':leg*left,'thigh_r':leg*(1-left)}
  for n,value in weights.items():
   if value>1e-6:ob.vertex_groups[n].add([v.index],value,'REPLACE')
 solid=ob.modifiers.new('Cloth thickness','SOLIDIFY');solid.thickness=.003;solid.offset=0
 armature=ob.modifiers.new('Shared rig','ARMATURE');armature.object=rig;ob.parent=rig;parts.append(ob)
# Store a master file with named semantic pieces rather than a flattened-only model.
donor.hide_render=True;donor.hide_viewport=True;donor.hide_set(True)
# Split remaining body by existing wardrobe ownership without guessing geometry.
catalog=json.loads(Path('client/art-source/appearance-slots.json').read_text());ids=sorted(set(x.value for x in o.data.attributes['appearance_slot'].data))
for sid in ids:
 part=o.copy();part.data=o.data.copy();part.name=catalog[str(sid)]['key'];bpy.context.collection.objects.link(part)
 bm=bmesh.new();bm.from_mesh(part.data);tag=bm.faces.layers.int['appearance_slot'];bmesh.ops.delete(bm,geom=[f for f in bm.faces if f[tag]!=sid],context='FACES');bm.to_mesh(part.data);bm.free();parts.append(part)
bpy.data.objects.remove(o,do_unlink=True)
for part in parts:part.asset_mark()
for side,angle in [('l',-60),('r',60)]:
 b=rig.pose.bones['upperarm_'+side];b.rotation_mode='XYZ';b.rotation_euler.z=math.radians(angle)
bpy.context.view_layer.update();bpy.context.preferences.filepaths.save_version=0
bpy.ops.wm.save_as_mainfile(filepath=str((a.output/'authoring.blend').resolve()),compress=True)
# Deterministic shaded preview of the whole outfit, without launching Unity.
s=bpy.context.scene;s.render.engine='BLENDER_WORKBENCH';s.display.shading.light='STUDIO';s.display.shading.color_type='TEXTURE';s.display.shading.show_shadows=True;s.display.shading.show_cavity=True
s.render.resolution_x=800;s.render.resolution_y=1000;s.render.resolution_percentage=100
bpy.ops.object.camera_add();cam=bpy.context.object;cam.data.type='ORTHO';cam.data.ortho_scale=2.15;s.camera=cam
for name,azimuth in [('front',0),('side',90),('back',180)]:
 rad=math.radians(azimuth);cam.location=(3*math.sin(rad),-3*math.cos(rad),1.15);cam.rotation_euler=(Vector((0,0,1.05))-cam.location).to_track_quat('-Z','Y').to_euler();s.render.filepath=str((a.output/(name+'.png')).resolve());bpy.ops.render.render(write_still=True)
# A runtime export copy; authoring pieces and their editable thickness remain in master.
for b in rig.pose.bones:b.matrix_basis.identity()
bpy.context.view_layer.update()
for part in parts:
 bpy.context.view_layer.objects.active=part
 for mod in list(part.modifiers):
  if mod.type=='SOLIDIFY':bpy.ops.object.modifier_apply(modifier=mod.name)
 bpy.ops.object.select_all(action='DESELECT')
bpy.data.objects.remove(donor,do_unlink=True)
for part in parts:part.select_set(True)
bpy.context.view_layer.objects.active=parts[0];bpy.ops.object.join();joined=bpy.context.object;joined.name='Keeper Macro Reconstruction';joined.asset_clear()
# Slot material order required by the existing exporter.
materials=list(joined.data.materials);body=next(m for m in materials if m.name=='Keeper Reconstruction');face=next(m for m in materials if m.name=='Keeper Reconstruction Face');indices=[0 if materials[f.material_index]==body else 1 for f in joined.data.polygons];joined.data.materials.clear();joined.data.materials.append(body);joined.data.materials.append(face)
for f,index in zip(joined.data.polygons,indices):f.material_index=index
bpy.ops.wm.save_as_mainfile(filepath=str((a.output/'keeper.blend').resolve()),compress=True)
print('LGO_AUTHORED_OUTFIT_READY removed_faces='+str(len(removed))+' semantic_parts='+str(len(parts)),flush=True)
