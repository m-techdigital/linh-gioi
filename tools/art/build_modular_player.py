"""Build a distinct tied-hair player from the proven shared Keeper wardrobe and atlases.

Run from project root using Blender --background --python this_file -- [options].
No texture creation: clean shared garment/head UVs and both images stay byte-identical.
"""
import argparse, bpy, bmesh, hashlib, json, math, sys
from pathlib import Path
from mathutils import Matrix, Vector, kdtree
parser=argparse.ArgumentParser(description=__doc__)
parser.add_argument('--project-root',type=Path,default=Path.cwd())
parser.add_argument('--source',type=Path)
parser.add_argument('--output-dir',type=Path)
args=parser.parse_args(sys.argv[sys.argv.index('--')+1:] if '--' in sys.argv else [])
R=args.project_root.resolve();O=args.output_dir or R/'build/asset-staging/player-modular-recovery';O.mkdir(parents=True,exist_ok=True)
source=args.source or R/'client/art-source/gate-keeper/KeeperReconstruction.blend'
SLOTS={v['key']:int(k) for k,v in json.loads((R/'client/art-source/appearance-slots.json').read_text()).items()}
bpy.ops.wm.open_mainfile(filepath=str(source));o=bpy.data.objects['Keeper Macro Reconstruction'];rig=bpy.data.objects['Armature']
rest={b.name:b.matrix_local.copy() for b in rig.data.bones}
def image_hash(im):
 import numpy as np
 return hashlib.sha256(np.asarray(im.pixels[:],dtype=np.float32).tobytes()).hexdigest()
images={im.name:image_hash(im) for im in bpy.data.images if im.name in ['KeeperReconstructionAlbedo','KeeperReconstructionFace']}
for b in rig.pose.bones:b.matrix_basis.identity()
for side,a in [('l',-60),('r',60)]:rig.pose.bones['upperarm_'+side].rotation_mode='XYZ';rig.pose.bones['upperarm_'+side].rotation_euler.z=math.radians(a)
bpy.context.view_layer.update();posed={b.name:rig.pose.bones[b.name].matrix@b.matrix_local.inverted() for b in rig.data.bones}
mesh=o.evaluated_get(bpy.context.evaluated_depsgraph_get()).data
for v,ev in zip(o.data.vertices,mesh.vertices):v.co=ev.co
for mod in list(o.modifiers):o.modifiers.remove(mod)
# Preserve authentic hair texture coordinates before replacing only its geometry.
hair_samples=[];uv=o.data.uv_layers.active;slot=o.data.attributes['appearance_slot']
for p in o.data.polygons:
 if slot.data[p.index].value==SLOTS['Hair']:
  for li in p.loop_indices:hair_samples.append((o.data.vertices[o.data.loops[li].vertex_index].co.copy(),uv.data[li].uv.copy()))
tree=kdtree.KDTree(len(hair_samples))
for i,(co,_) in enumerate(hair_samples):tree.insert(co,i)
tree.balance()
def hair_uv(co):return hair_samples[tree.find(co)[1]][1]
face_loops=[li for p in o.data.polygons if p.material_index==1 for li in p.loop_indices]
cheek=Vector((.047,-.08,1.63))
skin_li=min(face_loops,key=lambda li:(o.data.vertices[o.data.loops[li].vertex_index].co-cheek).length_squared)
skin_uv=uv.data[skin_li].uv.copy()
bm=bmesh.new();bm.from_mesh(o.data);layer=bm.faces.layers.int.get('appearance_slot')
bmesh.ops.delete(bm,geom=[f for f in bm.faces if f[layer] in [SLOTS['Headwear'],SLOTS['Cape'],SLOTS['Hair']]],context='FACES')
bmesh.ops.delete(bm,geom=[v for v in bm.verts if not v.link_faces],context='VERTS')
# Isolate the existing continuous wrapped coat by topology, not broad spatial pins.
seen=set();coat_vertices=0
for seed in bm.verts:
 if seed in seen:continue
 stack=[seed];component=[];seen.add(seed)
 while stack:
  v=stack.pop();component.append(v)
  for edge in v.link_edges:
   other=edge.other_vert(v)
   if other not in seen:seen.add(other);stack.append(other)
 face_slots={f[layer] for v in component for f in v.link_faces}
 if face_slots=={SLOTS['UpperBody']} and max(v.co.z for v in component)<1.13 and len(component)>500:
  for v in component:
   v.co.z=1.09+(v.co.z-1.09)*.74;v.co.x*=.90;v.co.y=.026+(v.co.y-.026)*.95
   t=max(0,min(1,(v.co.z-.94)/.15));t=t*t*(3-2*t)
   v.co.x*=1-.19*t;v.co.y=.026+(v.co.y-.026)*(1-.28*t);v.co.z+=.025*t
  coat_vertices+=len(component)
for v in bm.verts:
 slots={f[layer] for f in v.link_faces}
 if slots=={SLOTS['Shoulders']}:
  center=Vector((.205 if v.co.x>=0 else -.205,.061,1.468));v.co=center+(v.co-center)*.78
 elif SLOTS['UpperBody'] in slots and abs(v.co.x)<.18 and v.co.z>1.1:v.co.x*=.94
bm.to_mesh(o.data);bm.free();assert coat_vertices>500
# Few broad flowing hair forms, all in one semantic Hair slot, mapped to native hair texels.
hair_vertices=[];hair_faces=[]
def surface(nu,nv,fn,closed=False):
 offset=len(hair_vertices)
 for j in range(nv+1):
  for i in range(nu if closed else nu+1):hair_vertices.append(fn(i/nu,j/nv))
 stride=nu if closed else nu+1
 if closed:
  hair_faces.append(tuple(offset+i for i in reversed(range(stride))))
  hair_faces.append(tuple(offset+nv*stride+i for i in range(stride)))
 for j in range(nv):
  for i in range(nu):
   ni=(i+1)%stride;hair_faces.append((offset+j*stride+i,offset+j*stride+ni,offset+(j+1)*stride+ni,offset+(j+1)*stride+i))
# Swept crown follows skull; open edge stays behind eyebrows, covering scalp without cap-like band.
def scalp(u,t):
 a=math.tau*u;theta=.04+(2.05+.70*math.cos(a))*t
 return Vector((.093*math.sin(theta)*math.sin(a),.032+.099*math.sin(theta)*math.cos(a),1.683+.102*math.cos(theta)))
surface(48,16,scalp,True)
def lock(points,width,depth):
 pts=[Vector(p) for p in points]
 def fn(u,t):
  f=t*(len(pts)-1);i=min(len(pts)-2,int(f));center=pts[i].lerp(pts[i+1],f-i);a=u*math.tau
  taper=(1-t)**.45;return center+Vector((width*taper*math.cos(a),depth*taper*math.sin(a),0))
 surface(20,24,fn,True)
# High tie/knot and one ponytail; broad face-framing swept locks leave eyes visible.
def knot(u,t):
 a=math.tau*u;theta=.015+(math.pi-.03)*t
 return Vector((.035*math.sin(theta)*math.cos(a),.071+.040*math.sin(theta)*math.sin(a),1.786+.044*math.cos(theta)))
surface(32,20,knot,True)
lock([(.005,.099,1.785),(.022,.152,1.70),(.038,.179,1.55),(.06,.201,1.36)],.029,.021)
lock([(-.057,-.021,1.758),(-.076,-.061,1.716),(-.083,-.073,1.61)],.013,.010)
lock([(.054,-.024,1.761),(.074,-.065,1.719),(.085,-.059,1.605)],.012,.010)
data=bpy.data.meshes.new('Player tied hair');data.from_pydata(hair_vertices,[],hair_faces);data.update();bm=bmesh.new();bm.from_mesh(data);bmesh.ops.remove_doubles(bm,verts=list(bm.verts),dist=.000001);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(data);bm.free();hair=bpy.data.objects.new('Player high tied hair',data);bpy.context.collection.objects.link(hair)
hair.data.materials.append(bpy.data.materials['Keeper Reconstruction']);a=data.attributes.new('appearance_slot','INT','FACE');c=data.attributes.new('npc_part','INT','FACE')
for p in data.polygons:p.use_smooth=True;a.data[p.index].value=SLOTS['Hair'];c.data[p.index].value=2
uv=data.uv_layers.new(name='UVMap')
for p in data.polygons:
 for li in p.loop_indices:uv.data[li].uv=hair_uv(data.vertices[data.loops[li].vertex_index].co)
for b in rig.data.bones:hair.vertex_groups.new(name=b.name)
hair.vertex_groups[next(b.name for b in rig.data.bones if b.name.lower()=='head')].add(list(range(len(data.vertices))),1,'REPLACE')
# Closed inner skull and nape replace missing anatomy formerly hidden by Keeper long hair.
hair_vertices=[];hair_faces=[]
def skull(u,t):
 a=u*math.tau;theta=.01+(math.pi-.02)*t
 z=1.670+.087*math.cos(theta);jaw=max(0,min(1,(z-1.595)/.080));rx=.040+.027*jaw
 return Vector((rx*math.sin(theta)*math.sin(a),.034+.061*math.sin(theta)*math.cos(a),z))
def nape(u,t):
 a=u*math.tau;radius=.029+.005*math.sin(math.pi*t)
 return Vector((radius*math.sin(a),.038+radius*1.12*math.cos(a),1.51+.10*t))
surface(40,24,skull,True);surface(32,12,nape,True)
nd=bpy.data.meshes.new('Closed player skull and nape');nd.from_pydata(hair_vertices,[],hair_faces);nd.update()
bm=bmesh.new();bm.from_mesh(nd);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(nd);bm.free()
anatomy=bpy.data.objects.new('Player closed head anatomy',nd);bpy.context.collection.objects.link(anatomy);nd.materials.append(bpy.data.materials['Keeper Reconstruction Face'])
sa=nd.attributes.new('appearance_slot','INT','FACE');ca=nd.attributes.new('npc_part','INT','FACE');suv=nd.uv_layers.new(name='UVMap')
for p in nd.polygons:
 p.use_smooth=True;sa.data[p.index].value=SLOTS['Head'];ca.data[p.index].value=2
 for li in p.loop_indices:suv.data[li].uv=skin_uv
for b in rig.data.bones:anatomy.vertex_groups.new(name=b.name)
head_name=next(b.name for b in rig.data.bones if b.name.lower()=='head');anatomy.vertex_groups[head_name].add(list(range(len(nd.vertices))),1,'REPLACE')
bpy.ops.object.select_all(action='DESELECT');hair.select_set(True);anatomy.select_set(True);o.select_set(True);bpy.context.view_layer.objects.active=o;bpy.ops.object.join();o.name='Arrival Scene Reconstruction'
# Return A-pose fitted surfaces to the untouched shared rest matrices.
for v in o.data.vertices:
 weights=[(o.vertex_groups[g.group].name,g.weight) for g in v.groups];total=sum(w for _,w in weights);skin=Matrix([[0.0]*4 for _ in range(4)])
 for n,w in weights:skin+=posed[n]*(w/total)
 v.co=skin.inverted()@v.co
bm=bmesh.new();bm.from_mesh(o.data);bmesh.ops.triangulate(bm,faces=list(bm.faces));bm.to_mesh(o.data);bm.free()
old=list(o.data.materials);canonical=[bpy.data.materials[n] for n in ['Keeper Reconstruction','Keeper Reconstruction Face']];indices=[canonical.index(old[p.material_index]) for p in o.data.polygons];o.data.materials.clear()
for mat in canonical:o.data.materials.append(mat)
for p,i in zip(o.data.polygons,indices):p.material_index=i
o.modifiers.new('Shared65bone player rig','ARMATURE').object=rig
for b in rig.pose.bones:b.matrix_basis.identity()
assert len(rig.data.bones)==65 and all(rig.data.bones[n].matrix_local==m for n,m in rest.items())
assert all(image_hash(bpy.data.images[n])==digest for n,digest in images.items())
assert all(1<=len(v.groups)<=4 and abs(sum(g.weight for g in v.groups)-1)<1e-5 for v in o.data.vertices)
counts={}
for e in o.data.attributes['appearance_slot'].data:counts[e.value]=counts.get(e.value,0)+1
assert set(counts)<={2,3,10,11,13,14,15,16} and 0 not in counts
bpy.ops.wm.save_as_mainfile(filepath=str(O/'ModularPlayer.blend'),compress=True)
scene=bpy.context.scene;scene.render.engine='CYCLES';scene.cycles.samples=32;scene.render.resolution_x=1080;scene.render.resolution_y=1440;scene.render.resolution_percentage=100;scene.view_settings.view_transform='AgX';scene.render.film_transparent=False;cam=scene.camera;cam.data.type='ORTHO';cam.data.ortho_scale=2.03
for name,loc,moving in [('player-front',(0,-5,1),False),('player-back',(0,5,1),False),('player-side',(5,0,1),False),('player-movement',(3,-5,1.1),True)]:
 for b in rig.pose.bones:b.matrix_basis.identity()
 for side,a in [('l',-70),('r',70)]:rig.pose.bones['upperarm_'+side].rotation_euler.z=math.radians(a)
 if moving:
  for n,angle in [('thigh_l',-18),('thigh_r',15),('calf_r',-20)]:rig.pose.bones[n].rotation_mode='XYZ';rig.pose.bones[n].rotation_euler.x=math.radians(angle)
  rig.pose.bones['upperarm_l'].rotation_euler.z=math.radians(-35)
 cam.location=loc;cam.rotation_euler=(Vector((0,0,1))-cam.location).to_track_quat('-Z','Y').to_euler();scene.render.filepath=str(O/(name+'.png'));bpy.ops.render.render(write_still=True)
receipt={'triangles':len(o.data.polygons),'vertices':len(o.data.vertices),'slots':counts,'shared_texture_sha256':images,'shared_rest_bones':65,'materials':2,'coat_vertices_refit':coat_vertices,'runtime_verified':False}
(O/'player-receipt.json').write_text(json.dumps(receipt,indent=2));print('MODULAR_PLAYER',json.dumps(receipt),flush=True)
