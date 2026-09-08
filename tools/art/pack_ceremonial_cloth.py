"""One-time cloth migration from the pre-cloth sources (checkpoint 5e86dad).

Run before promoting the output blends. Re-running on migrated sources is
intentionally rejected by the UV overlap guard. Garment additions are joined
into UpperBody, retaining the existing skeleton, slots and materials.
"""
import bpy,math,argparse,sys,numpy as np
from pathlib import Path
from mathutils import Matrix,Vector
from mathutils.bvhtree import BVHTree
p=argparse.ArgumentParser();p.add_argument('--output',type=Path,required=True)
a=p.parse_args(sys.argv[sys.argv.index('--')+1:]);a.output.mkdir(parents=True,exist_ok=True)
sources=[('keeper','client/art-source/gate-keeper/KeeperReconstruction.blend'),('player','client/art-source/arrival-scene/ArrivalScene.blend')]
patches=[(1152,192,'ceremonial-torso-albedo.png'),(0,576,'ivory-brocade-albedo.png')]
for actor,path in sources:
 bpy.ops.wm.open_mainfile(filepath=path);o=next(o for o in bpy.data.objects if o.type=='MESH' and o.name in ('Keeper Macro Reconstruction','Arrival Scene Reconstruction'))
 for f in o.data.polygons:
  if f.material_index!=0:continue
  uv=[o.data.uv_layers.active.data[i].uv for i in f.loop_indices]
  for x,y,_ in patches:
   assert max(v.x for v in uv)<x/4096 or min(v.x for v in uv)>(x+576)/4096 or max(v.y for v in uv)<y/2048 or min(v.y for v in uv)>(y+576)/2048,'Fabric patch intersects existing UVs'
original=None
for actor,path in sources:
 bpy.ops.wm.open_mainfile(filepath=path);o=next(o for o in bpy.data.objects if o.type=='MESH' and o.name in ('Keeper Macro Reconstruction','Arrival Scene Reconstruction'))
 atlas=bpy.data.images['KeeperReconstructionAlbedo'];pixels=np.array(atlas.pixels[:],dtype=np.float32).reshape(2048,4096,4)
 if original is None:original=pixels.copy()
 else:assert np.array_equal(original,pixels),'Shared actor atlases differ'
 owned=np.zeros((2048,4096),bool)
 for x,y,name in patches:
  tile=bpy.data.images.load(str(Path('client/art-source/shared-wardrobe',name).resolve()));tile.scale(512,512)
  pixels[y:y+576,x:x+576]=np.pad(np.array(tile.pixels[:],dtype=np.float32).reshape(512,512,4),((32,32),(32,32),(0,0)),mode='edge');owned[y:y+576,x:x+576]=True
 assert np.array_equal(pixels[~owned],original[~owned]);atlas.pixels=pixels.ravel();atlas.pack()
 if actor=='keeper':atlas.filepath_raw=str((a.output/'KeeperReconstructionAlbedo.png').resolve());atlas.file_format='PNG';atlas.save()
 r=bpy.data.objects['Armature']
 for b in r.pose.bones:b.matrix_basis.identity()
 for side,angle in [('l',-60),('r',60)]:
  b=r.pose.bones['upperarm_'+side];b.rotation_mode='XYZ';b.rotation_euler.z=math.radians(angle)
 bpy.context.view_layer.update();transforms={b.name:r.pose.bones[b.name].matrix@b.matrix_local.inverted() for b in r.data.bones};coords=[];armweights=[]
 for vertex in o.data.vertices:
  m=Matrix([[0.]*4 for _ in range(4)]);arm=0
  for g in vertex.groups:
   name=o.vertex_groups[g.group].name;m+=transforms[name]*g.weight
   if name.startswith(('upperarm_','lowerarm_','clavicle_')):arm+=g.weight
  coords.append(m@vertex.co);armweights.append(arm)
 slots=o.data.attributes['appearance_slot']
 torso_faces=[];arm_faces={'l':[],'r':[]}
 for f in o.data.polygons:
  if slots.data[f.index].value not in (10,11,13):continue
  center=sum((coords[i] for i in f.vertices),Vector())/len(f.vertices)
  arm=sum(armweights[i] for i in f.vertices)/len(f.vertices)>.35
  if not arm and 1.0<center.z<1.35:torso_faces.append(list(f.vertices))
  elif arm and 1.16<center.z<1.39:arm_faces['l' if center.x>0 else 'r'].append(list(f.vertices))
 torso_bvh=BVHTree.FromPolygons(coords,torso_faces)
 arm_bvh={side:BVHTree.FromPolygons(coords,faces) for side,faces in arm_faces.items()}
 uv=o.data.uv_layers.active.data;counts=[0,0]
 for f in o.data.polygons:
  if f.material_index!=0 or slots.data[f.index].value!=10:continue
  center=sum((coords[i] for i in f.vertices),Vector())/len(f.vertices);arm=sum(armweights[i] for i in f.vertices)/len(f.vertices)>.35
  if arm and 1.265<center.z<1.525:
   patch=1;bone=r.pose.bones['upperarm_'+('l' if center.x>0 else 'r')];axis=bone.tail-bone.head;direction=axis.normalized();front=Vector((0,-1,0));front=(front-direction*front.dot(direction)).normalized();side=direction.cross(front)
   def project(co):
    d=co-bone.head;return math.atan2(d.dot(side),d.dot(front)),max(.02,min(.98,d.dot(axis)/axis.length_squared))
   mid=project(center)[0]
  elif not arm and 1.115<center.z<1.575:patch=0
  else:continue
  x,y,_=patches[patch]
  for li in f.loop_indices:
   co=coords[o.data.loops[li].vertex_index]
   if patch==0:u=.5+co.x/.37*(1 if center.y<.035 else -1);v=(co.z-1.10)/.475
   else:
    angle,v=project(co)
    while angle-mid>math.pi:angle-=math.tau
    while angle-mid<-math.pi:angle+=math.tau
    u=.5+angle/math.tau
   u=max(.02,min(.98,u));v=max(.02,min(.98,v));uv[li].uv=((x+32.5+u*511)/4096,(y+32.5+v*511)/2048);counts[patch]+=1
 # One coherent chest clasp restores the reference jewelry as actual volume.
 bpy.ops.mesh.primitive_uv_sphere_add(segments=24,ring_count=12,location=(.025,-.128,1.335))
 clasp=bpy.context.object;clasp.name='Ceremonial chest clasp';clasp.scale=(.035,.011,.039)
 bpy.ops.object.transform_apply(location=True,rotation=True,scale=True);clasp.data.materials.append(o.data.materials[0])
 for f in clasp.data.polygons:
  f.use_smooth=True
  for li in f.loop_indices:
   co=clasp.data.vertices[clasp.data.loops[li].vertex_index].co
   u=.5+.14*(co.x-.025)/.035;v=.5+.14*(co.z-1.335)/.039
   clasp.data.uv_layers.active.data[li].uv=((608.5+u*511)/4096,(32.5+v*511)/2048)
 for name,value in [('appearance_slot',10),('npc_part',3)]:
  layer=clasp.data.attributes.new(name,'INT','FACE')
  for item in layer.data:item.value=value
 clasp.vertex_groups.new(name='spine_03').add(list(range(len(clasp.data.vertices))),1.,'REPLACE')
 parts=[clasp]
 buckle=clasp.copy();buckle.data=clasp.data.copy();buckle.name='Ceremonial waist buckle';bpy.context.collection.objects.link(buckle)
 hit=torso_bvh.ray_cast(Vector((0,-.5,1.155)),Vector((0,1,0)),.5)[0]
 assert hit is not None,'Waist buckle fitting missed torso'
 for v in buckle.data.vertices:v.co+=Vector((-.025,hit.y-.011+.128,-.18))
 for group in buckle.vertex_groups:group.remove(list(range(len(buckle.data.vertices))))
 buckle.vertex_groups['spine_03'].add(list(range(len(buckle.data.vertices))),.3,'REPLACE');buckle.vertex_groups.new(name='pelvis').add(list(range(len(buckle.data.vertices))),.7,'REPLACE');parts.append(buckle)
 def accessory(name,vertices,faces,bone,texcoords):
  mesh=bpy.data.meshes.new(name);mesh.from_pydata([transforms[bone].inverted()@v for v in vertices],[],faces);mesh.update()
  ob=bpy.data.objects.new(name,mesh);bpy.context.collection.objects.link(ob);mesh.materials.append(o.data.materials[0]);uvlayer=mesh.uv_layers.new(name='UVMap')
  for f in mesh.polygons:
   f.use_smooth=True
   faceuv=[texcoords[mesh.loops[li].vertex_index] for li in f.loop_indices]
   wrap=f.index>=64 and f.index<len(mesh.polygons)-64 and max(u[0] for u in faceuv)-min(u[0] for u in faceuv)>.09
   for li in f.loop_indices:
    u,v=texcoords[mesh.loops[li].vertex_index]
    if f.index<64 or f.index>=len(mesh.polygons)-64:u,v=(32.5+.5*511)/4096,(32.5+.985*511)/2048
    elif wrap and u<(608.5+.5*511)/4096:u+=511/4096
    uvlayer.data[li].uv=(u,v)
  for key,value in [('appearance_slot',10),('npc_part',3)]:
   layer=mesh.attributes.new(key,'INT','FACE')
   for item in layer.data:item.value=value
  if name=='Ceremonial belt border':
   spine=ob.vertex_groups.new(name='spine_03');pelvis=ob.vertex_groups.new(name='pelvis')
   for i,co in enumerate(vertices):
    t=max(0,min(1,(co.z-1.)/.43));t=t*t*(3-2*t);spine.add([i],t,'REPLACE');pelvis.add([i],1-t,'REPLACE')
  else:ob.vertex_groups.new(name=bone).add(list(range(len(vertices))),1.,'REPLACE')
  parts.append(ob)
 # Broad waist girdle and sleeve borders are real joined wardrobe surfaces.
 for kind in ['belt','l','r']:
  n=64;vs=[];fs=[];tex=[]
  if kind!='belt':
   bone=r.pose.bones['upperarm_'+kind];axis=(bone.tail-bone.head).normalized();center=bone.head+(bone.tail-bone.head)*((1.265-bone.head.z)/(bone.tail.z-bone.head.z));front=Vector((0,-1,0));front=(front-axis*front.dot(axis)).normalized();side=axis.cross(front)
  for row,t in enumerate([0,.04,.2,.35,.5,.65,.8,.96,1]):
   for j in range(n+1):
    angle=math.tau*j/n
    if kind=='belt':
     origin=Vector((0,.018,1.08+.15*t));radial=Vector((math.sin(angle),-math.cos(angle),0));bvh=torso_bvh
    else:
     origin=center+axis*((t-.5)*.072);radial=front*math.cos(angle)+side*math.sin(angle);bvh=arm_bvh[kind]
    hit=bvh.ray_cast(origin+radial*.35,-radial,.35)[0]
    assert hit is not None,'Clothing fitting ray missed '+kind
    co=hit+radial*.009
    vs.append(co)
    if row in (0,8):tex.append(((32.5+.5*511)/4096,(32.5+.985*511)/2048))
    else:
     u=.065;v=.5+.10*t
     tex.append(((608.5+u*511)/4096,(32.5+v*511)/2048))
  for row in range(8):
   for j in range(n):
    k=row*(n+1)+j;fs.append((k,k+1,k+n+2,k+n+1))
  accessory('Ceremonial '+kind+' border',vs,fs,'spine_03' if kind=='belt' else 'upperarm_'+kind,tex)
 bpy.ops.object.select_all(action='DESELECT');o.select_set(True)
 for part in parts:part.select_set(True)
 bpy.context.view_layer.objects.active=o;bpy.ops.object.join()
 for b in r.pose.bones:b.matrix_basis.identity()
 bpy.context.view_layer.update();bpy.context.preferences.filepaths.save_version=0
 bpy.ops.wm.save_as_mainfile(filepath=str((a.output/f'{actor}.blend').resolve()),compress=True)
 print(f'LGO_CLOTH_ATLAS_READY actor={actor} torso_loops={counts[0]} sleeve_loops={counts[1]} outside_patch_unchanged=true')
