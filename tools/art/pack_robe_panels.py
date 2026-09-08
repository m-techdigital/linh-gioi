"""Replace lower-robe projection with shared cloth panels; reshape/unwrap bracers.

One-time migration from f5f5a2e. Re-running on migrated UVs is rejected.
"""
import bpy,math,sys,argparse,numpy as np
from pathlib import Path
from mathutils import Matrix,Vector
from mathutils.kdtree import KDTree
p=argparse.ArgumentParser();p.add_argument('--output',type=Path,required=True);a=p.parse_args(sys.argv[sys.argv.index('--')+1:]);a.output.mkdir(parents=True,exist_ok=True)
sources=[('keeper','gate-keeper/KeeperReconstruction.blend'),('player','arrival-scene/ArrivalScene.blend')]
for actor,path in sources:
 bpy.ops.wm.open_mainfile(filepath='client/art-source/'+path);o=next(x for x in bpy.data.objects if x.type=='MESH' and x.name in ('Keeper Macro Reconstruction','Arrival Scene Reconstruction'))
 for f in o.data.polygons:
  if f.material_index:continue
  us=[o.data.uv_layers.active.data[i].uv for i in f.loop_indices]
  assert max(u.x for u in us)<576/4096 or min(u.x for u in us)>1152/4096 or max(u.y for u in us)<576/2048 or min(u.y for u in us)>1152/2048,'New robe tile overlaps existing UV'
original=None
for actor,path in sources:
 bpy.ops.wm.open_mainfile(filepath='client/art-source/'+path);o=next(x for x in bpy.data.objects if x.type=='MESH' and x.name in ('Keeper Macro Reconstruction','Arrival Scene Reconstruction'));r=bpy.data.objects['Armature'];atlas=bpy.data.images['KeeperReconstructionAlbedo'];pixels=np.array(atlas.pixels[:],dtype=np.float32).reshape(2048,4096,4)
 if original is None:original=pixels.copy()
 else:assert np.array_equal(original,pixels)
 tile=bpy.data.images.load(str(Path('client/art-source/shared-wardrobe/ceremonial-robe-panel-albedo.png').resolve()));tile.scale(512,512);pixels[576:1152,576:1152]=np.pad(np.array(tile.pixels[:],dtype=np.float32).reshape(512,512,4),((32,32),(32,32),(0,0)),mode='edge');owned=np.zeros((2048,4096),bool);owned[576:1152,576:1152]=1;assert np.array_equal(pixels[~owned],original[~owned]);atlas.pixels=pixels.ravel();atlas.pack()
 if actor=='keeper':atlas.filepath_raw=str((a.output/'KeeperReconstructionAlbedo.png').resolve());atlas.file_format='PNG';atlas.save()
 for b in r.pose.bones:b.matrix_basis.identity()
 for side,angle in [('l',-60),('r',60)]:
  b=r.pose.bones['upperarm_'+side];b.rotation_mode='XYZ';b.rotation_euler.z=math.radians(angle)
 bpy.context.view_layer.update();tr={b.name:r.pose.bones[b.name].matrix@b.matrix_local.inverted() for b in r.data.bones};coords=[];matrices=[]
 for vertex in o.data.vertices:
  m=Matrix([[0.]*4 for _ in range(4)])
  for g in vertex.groups:m+=tr[o.vertex_groups[g.group].name]*g.weight
  matrices.append(m);coords.append(m@vertex.co)
 slot=o.data.attributes['appearance_slot'];uv=o.data.uv_layers.active.data;coat=set();bracer=set();coatfaces=[];bracerfaces=[]
 for f in o.data.polygons:
  center=sum((coords[i] for i in f.vertices),Vector())/len(f.vertices)
  if f.material_index:continue
  arm=sum(sum(g.weight for g in o.data.vertices[i].groups if o.vertex_groups[g.group].name.startswith(('upperarm_','lowerarm_','clavicle_'))) for i in f.vertices)/len(f.vertices)
  if uv[f.loop_indices[0]].uv.x<.5:continue
  if slot.data[f.index].value==10 and center.z<1.105 and arm<.35:coatfaces.append(f.index);coat.update(f.vertices)
  elif slot.data[f.index].value in (10,13) and arm>.35:
   side='l' if center.x>0 else 'r';bone=r.pose.bones['lowerarm_'+side];axis=bone.tail-bone.head;t=(center-bone.head).dot(axis)/axis.length_squared
   if .10<t<.90:bracerfaces.append((f.index,side));bracer.update(f.vertices)
 assert len(coatfaces)>1000 and len(bracerfaces)>20, 'Expected full robe and both bracers'
 zmin=min(coords[i].z for i in coat);zmax=1.115
 for fi in coatfaces:
  f=o.data.polygons[fi];center=sum((coords[i] for i in f.vertices),Vector())/len(f.vertices);theta=math.atan2(center.x,-(center.y-.03));panel=math.floor((theta+math.pi)/(math.pi/2));start=-math.pi+panel*math.pi/2
  for li in f.loop_indices:
   co=coords[o.data.loops[li].vertex_index];angle=math.atan2(co.x,-(co.y-.03))
   while angle-theta>math.pi:angle-=math.tau
   while angle-theta<-math.pi:angle+=math.tau
   v=max(.03,min(.97,(co.z-zmin)/(zmax-zmin)))
   u=max(.055,min(.945,(angle-start)/(math.pi/2)));uv[li].uv=((608.5+u*511)/4096,(608.5+v*511)/2048)
 for i in coat:
  co=coords[i];theta=math.atan2(co.x,-(co.y-.03));t=max(0,min(1,(1.115-co.z)/(1.115-zmin)));radial=Vector((co.x,co.y-.03,0)).normalized();co+=radial*(.009*math.sin(theta*8)**2*math.sin(math.pi*t));o.data.vertices[i].co=matrices[i].inverted()@co
 for fi,side in bracerfaces:
  f=o.data.polygons[fi];bone=r.pose.bones['lowerarm_'+side];axis=bone.tail-bone.head;direction=axis.normalized();front=Vector((0,-1,0));front=(front-direction*front.dot(direction)).normalized();across=direction.cross(front)
  center=sum((coords[i] for i in f.vertices),Vector())/len(f.vertices);mid=math.atan2((center-bone.head).dot(across),(center-bone.head).dot(front))
  for li in f.loop_indices:
   co=coords[o.data.loops[li].vertex_index];d=co-bone.head;angle=math.atan2(d.dot(across),d.dot(front))
   while angle-mid>math.pi:angle-=math.tau
   while angle-mid<-math.pi:angle+=math.tau
   u=max(.02,min(.98,.5+angle/math.tau));v=max(.02,min(.98,d.dot(axis)/axis.length_squared/.84));uv[li].uv=((608.5+u*511)/4096,(32.5+v*511)/2048)
 for i in bracer:
  co=coords[i];bone=r.pose.bones['lowerarm_'+('l' if co.x>0 else 'r')];axis=bone.tail-bone.head;t=(co-bone.head).dot(axis)/axis.length_squared;radial=co-bone.head-axis*t
  if radial.length:co+=radial.normalized()*.005*max(0,math.sin(math.pi*max(0,min(1,t/.84))))
  o.data.vertices[i].co=matrices[i].inverted()@co
 # Quiet inner trousers and one continuous ivory front tabard restore the layered reference.
 for f in o.data.polygons:
  if slot.data[f.index].value==11:
   for li in f.loop_indices:uv[li].uv=((608.5+.5*511)/4096,(608.5+.8*511)/2048)
 tree=KDTree(len(coat))
 for j,i in enumerate(sorted(coat)):tree.insert(coords[i],i)
 tree.balance();vs=[];ws=[];tex=[];fs=[];rows=24;cols=8
 for row in range(rows+1):
  t=row/rows;z=1.065+(zmin+.065-1.065)*t;near=[coords[i].y for i in coat if abs(coords[i].z-z)<.035];assert near
  front=min(near)-.012
  for col in range(cols+1):
   u=[0,.045,.1,.3,.5,.7,.9,.955,1][col];x=(u-.5)*(.145+.055*t);co=Vector((x,front+.024*(2*u-1)**2-.008*math.sin(t*math.pi),z+.015*abs(2*u-1)*t));near=tree.find_n(co,3);w={};total=sum(1/max(.005,d) for _,_,d in near)
   for _,i,d in near:
    factor=1/max(.005,d)/total
    for g in o.data.vertices[i].groups:
     n=o.vertex_groups[g.group].name;w[n]=w.get(n,0)+g.weight*factor
   w=dict(sorted(w.items(),key=lambda it:it[1],reverse=True)[:4]);total=sum(w.values());w={n:v/total for n,v in w.items()};m=Matrix([[0.]*4 for _ in range(4)])
   for n,v in w.items():m+=tr[n]*v
   vs.append(m.inverted()@co);ws.append(w);tex.append(((1184.5+(.025+.20*u)*511)/4096,(224.5+(.04+.56*(1-t))*511)/2048))
 for row in range(rows):
  for col in range(cols):
   k=row*(cols+1)+col;fs.append((k,k+cols+1,k+cols+2,k+1))
 mesh=bpy.data.meshes.new('Ivory front tabard');mesh.from_pydata(vs,[],fs);mesh.update();ob=bpy.data.objects.new('Ivory front tabard',mesh);bpy.context.collection.objects.link(ob);mesh.materials.append(o.data.materials[0]);layer=mesh.uv_layers.new(name='UVMap')
 for f in mesh.polygons:
  f.use_smooth=True
  row,col=divmod(f.index,cols)
  for li in f.loop_indices:
   vi=mesh.loops[li].vertex_index;vr,vc=divmod(vi,cols+1)
   if col in (0,7):
    # Gold bound edge, mapped from the existing embroidered panel tile.
    u=.065+.045*(vc-col);v=.1+.8*(1-vr/rows)
    layer.data[li].uv=((608.5+u*511)/4096,(608.5+v*511)/2048)
   elif row>=rows-2:
    u=.15+.7*vc/cols;v=.04+.09*(rows-vr)/2
    layer.data[li].uv=((608.5+u*511)/4096,(608.5+v*511)/2048)
   else:layer.data[li].uv=tex[vi]
 for n,value in [('appearance_slot',10),('npc_part',3)]:
  layer=mesh.attributes.new(n,'INT','FACE')
  for item in layer.data:item.value=value
 for i,w in enumerate(ws):
  for n,value in w.items():
   group=ob.vertex_groups.get(n) or ob.vertex_groups.new(name=n);group.add([i],value,'REPLACE')
 bpy.ops.object.select_all(action='DESELECT');o.select_set(True);ob.select_set(True);bpy.context.view_layer.objects.active=o;bpy.ops.object.join()
 for b in r.pose.bones:b.matrix_basis.identity()
 bpy.context.view_layer.update();bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str((a.output/f'{actor}.blend').resolve()),compress=True);print(f'LGO_ROBE_PANELS_READY actor={actor} robe_faces={len(coatfaces)} bracer_faces={len(bracerfaces)} outside_patch_unchanged=true')
