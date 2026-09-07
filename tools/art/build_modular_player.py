"""Build a distinct tied-hair player from the proven shared Keeper wardrobe and atlases.

Run from project root using Blender --background --python this_file -- [options].
No texture creation in this script: body atlas is shared unchanged; a dedicated
player face image is supplied to the coherent head helper.
"""
import argparse, bpy, bmesh, hashlib, json, math, sys
sys.dont_write_bytecode=True
from pathlib import Path
from mathutils import Matrix, Vector, kdtree
parser=argparse.ArgumentParser(description=__doc__)
parser.add_argument('--project-root',type=Path,default=Path.cwd())
parser.add_argument('--source',type=Path)
parser.add_argument('--face-texture',type=Path)
parser.add_argument('--output-dir',type=Path)
parser.add_argument('--no-preview',action='store_true')
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
bm=bmesh.new();bm.from_mesh(o.data);layer=bm.faces.layers.int.get('appearance_slot')
bmesh.ops.delete(bm,geom=[f for f in bm.faces if f[layer] in [SLOTS['Headwear'],SLOTS['Cape']]],context='FACES')
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
o.name='Arrival Scene Reconstruction'
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
# Full rebuilds use the same coherent Head implementation as standalone head-only recovery.
import importlib.util
spec=importlib.util.spec_from_file_location('lgo_player_head',R/'tools/art/rebuild_player_head.py');head_helper=importlib.util.module_from_spec(spec);spec.loader.exec_module(head_helper)
o,_head_receipt=head_helper.rebuild_head(o,rig,R,args.face_texture)
assert len(rig.data.bones)==65 and all(rig.data.bones[n].matrix_local==m for n,m in rest.items())
assert all(image_hash(bpy.data.images[n])==digest for n,digest in images.items())
assert all(1<=len(v.groups)<=4 and abs(sum(g.weight for g in v.groups)-1)<1e-5 for v in o.data.vertices)
counts={}
for e in o.data.attributes['appearance_slot'].data:counts[e.value]=counts.get(e.value,0)+1
assert set(counts)<={2,3,10,11,13,14,15,16} and 0 not in counts
bpy.ops.wm.save_as_mainfile(filepath=str(O/'ModularPlayer.blend'),compress=True)
scene=bpy.context.scene;scene.render.engine='CYCLES';scene.cycles.samples=32;scene.render.resolution_x=1080;scene.render.resolution_y=1440;scene.render.resolution_percentage=100;scene.view_settings.view_transform='AgX';scene.render.film_transparent=False;cam=scene.camera;cam.data.type='ORTHO';cam.data.ortho_scale=2.03
for name,loc,moving in [('player-front',(0,-5,1),False),('player-back',(0,5,1),False),('player-side',(5,0,1),False),('player-movement',(3,-5,1.1),True)]:
 if args.no_preview:continue
 for b in rig.pose.bones:b.matrix_basis.identity()
 for side,a in [('l',-70),('r',70)]:rig.pose.bones['upperarm_'+side].rotation_euler.z=math.radians(a)
 if moving:
  for n,angle in [('thigh_l',-18),('thigh_r',15),('calf_r',-20)]:rig.pose.bones[n].rotation_mode='XYZ';rig.pose.bones[n].rotation_euler.x=math.radians(angle)
  rig.pose.bones['upperarm_l'].rotation_euler.z=math.radians(-35)
 cam.location=loc;cam.rotation_euler=(Vector((0,0,1))-cam.location).to_track_quat('-Z','Y').to_euler();scene.render.filepath=str(O/(name+'.png'));bpy.ops.render.render(write_still=True)
receipt={'triangles':len(o.data.polygons),'vertices':len(o.data.vertices),'slots':counts,'texture_sha256':{'KeeperReconstructionAlbedo':images['KeeperReconstructionAlbedo'],'ArrivalFace':image_hash(bpy.data.images['ArrivalFace'])},'shared_rest_bones':65,'materials':2,'coat_vertices_refit':coat_vertices,'runtime_verified':False}
face_path=args.face_texture or R/'client/art-source/arrival-scene/ArrivalFace.png'
receipt.update({'source':str(source.relative_to(R)) if source.is_absolute() else str(source),'source_sha256':hashlib.sha256(source.read_bytes()).hexdigest(),'face':str(face_path.relative_to(R)) if face_path.is_absolute() else str(face_path),'face_sha256':hashlib.sha256(face_path.read_bytes()).hexdigest(),'blender':bpy.app.version_string,'helpers_sha256':{str(path.relative_to(R)):hashlib.sha256(path.read_bytes()).hexdigest() for path in [R/'tools/art/build_modular_player.py',R/'tools/art/rebuild_player_head.py',R/'tools/art/rebuild_player_hair.py',R/'tools/art/finish_player_neck.py']}})
(O/'player-receipt.json').write_text(json.dumps(receipt,indent=2));print('MODULAR_PLAYER',json.dumps(receipt),flush=True)
