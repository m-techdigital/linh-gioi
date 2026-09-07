"""Connected collar facing and smooth tied-hair deformation on the canonical player rig."""
import argparse,bpy,bmesh,hashlib,json,math,sys
sys.dont_write_bytecode=True
from pathlib import Path
from mathutils import Vector

def remove_lining(o):
 bm=bmesh.new();bm.from_mesh(o.data);tag=bm.faces.layers.int.get('arrival_collar_facing')
 if tag:
  bmesh.ops.delete(bm,geom=[f for f in bm.faces if f[tag]==1],context='FACES')
  bmesh.ops.delete(bm,geom=[v for v in bm.verts if not v.link_faces],context='VERTS');bm.to_mesh(o.data)
 bm.free()

def finish_neck(o,rig):
 remove_lining(o)
 bm=bmesh.new();bm.from_mesh(o.data);slot=bm.faces.layers.int.get('appearance_slot');coarse=bm.faces.layers.int.get('npc_part');marker=bm.faces.layers.int.get('arrival_collar_facing') or bm.faces.layers.int.new('arrival_collar_facing');uv=bm.loops.layers.uv.active;deform=bm.verts.layers.deform.active
 edges=[e for e in bm.edges if e.is_boundary and all(f[slot]==10 for f in e.link_faces) and all(v.co.z>1.53 and abs(v.co.x)<.13 and abs(v.co.y)<.16 for v in e.verts)]
 rim={v for e in edges for v in e.verts};assert len(edges)==44 and len(rim)==44,(len(edges),len(rim))
 inner={};rimuv={}
 for v in rim:
  angle=math.atan2(v.co.x,v.co.y-.029);n=bm.verts.new((.025*math.sin(angle),.029+.032*math.cos(angle),1.549))
  for group,weight in v[deform].items():n[deform][group]=weight
  inner[v]=n;rimuv[v]=next(loop[uv].uv.copy() for f in v.link_faces if f[slot]==10 for loop in f.loops if loop.vert==v)
 added=[]
 for edge in edges:
  a,b=edge.verts
  for points in [(a,b,inner[b]),(a,inner[b],inner[a])]:
   f=bm.faces.new(points);f[slot]=10;f[coarse]=3;f[marker]=1;f.material_index=0;f.smooth=True
   for loop in f.loops:
    original=loop.vert if loop.vert in rim else next(v for v,n in inner.items() if n==loop.vert)
    loop[uv].uv=rimuv[original]
   f.normal_update()
   if f.normal.z<0:f.normal_flip()
   added.append(f)
 assert all(not e.is_boundary for e in edges),'Facing must connect to every original rim edge'
 bm.to_mesh(o.data);bm.free()
 # Crown/fringe remain rigid to Head. Only the three long connected tail sections blend down the spine.
 bm=bmesh.new();bm.from_mesh(o.data);slot=bm.faces.layers.int.get('appearance_slot');seen=set();tail_ids=[]
 for seed in bm.verts:
  if seed in seen or not seed.link_faces or any(f[slot]!=3 for f in seed.link_faces):continue
  todo=[seed];seen.add(seed);part=[]
  while todo:
   v=todo.pop();part.append(v)
   for edge in v.link_edges:
    nxt=edge.other_vert(v)
    if nxt not in seen:seen.add(nxt);todo.append(nxt)
  if min(v.co.z for v in part)<1.45:tail_ids.extend(v.index for v in part)
 bm.free();assert 3000<len(tail_ids)<6000,len(tail_ids)
 head=next(b.name for b in rig.data.bones if b.name.lower()=='head')
 def smooth(a,b,x):t=max(0,min(1,(x-a)/(b-a)));return t*t*(3-2*t)
 for index in tail_ids:
  v=o.data.vertices[index];h=smooth(1.60,1.75,v.co.z);neck=smooth(1.37,1.60,v.co.z)*(1-h);spine=max(0,1-h-neck)
  for group in [g.group for g in v.groups]:o.vertex_groups[group].remove([index])
  for name,w in [(head,h),('neck_01',neck),('spine_03',spine)]:
   if w>1e-7:o.vertex_groups[name].add([index],w,'REPLACE')
 assert all(1<=len(v.groups)<=4 and abs(sum(g.weight for g in v.groups)-1)<1e-5 for v in o.data.vertices)
 return {'collar_facing_triangles':len(added),'collar_facing_vertices':len(inner),'weighted_tail_vertices':len(tail_ids)}

def main():
 p=argparse.ArgumentParser(description=__doc__);p.add_argument('--source',type=Path,default=Path('client/art-source/arrival-scene/ArrivalScene.blend'));p.add_argument('--output-dir',type=Path,default=Path('build/asset-staging/player-neck-runtime'))
 a=p.parse_args(sys.argv[sys.argv.index('--')+1:] if '--' in sys.argv else []);a.output_dir.mkdir(parents=True,exist_ok=True)
 bpy.ops.wm.open_mainfile(filepath=str(a.source));o=bpy.data.objects['Arrival Scene Reconstruction'];rig=bpy.data.objects['Armature']
 for b in rig.pose.bones:b.matrix_basis.identity()
 info=finish_neck(o,rig);first=(len(o.data.vertices),len(o.data.polygons));second=finish_neck(o,rig);assert first==(len(o.data.vertices),len(o.data.polygons)) and info==second,'Idempotent facing/deformation repair'
 bpy.ops.wm.save_as_mainfile(filepath=str(a.output_dir/'PlayerNeckFinal.blend'),compress=True)
 scene=bpy.context.scene;scene.render.engine='CYCLES';scene.cycles.samples=32;scene.render.resolution_x=1100;scene.render.resolution_y=1100;scene.render.resolution_percentage=100;scene.render.film_transparent=False;cam=scene.camera;cam.data.type='ORTHO';cam.data.ortho_scale=.57
 head=next(b.name for b in rig.data.bones if b.name.lower()=='head')
 for name,yaw,location in [('neck-idle',0,(0,-3,2.05)),('neck-yaw-left',40,(1.2,-3,2.0)),('neck-yaw-right',-40,(-1.2,-3,2.0)),('tail-yaw',40,(0,3,1.70))]:
  for b in rig.pose.bones:b.matrix_basis.identity()
  for side,angle in [('l',-70),('r',70)]:rig.pose.bones['upperarm_'+side].rotation_euler.z=math.radians(angle)
  rig.pose.bones[head].rotation_mode='XYZ';rig.pose.bones[head].rotation_euler.y=math.radians(yaw)
  target=Vector((0,0,1.58));cam.location=location;cam.rotation_euler=(target-cam.location).to_track_quat('-Z','Y').to_euler();scene.render.filepath=str(a.output_dir/(name+'.png'));bpy.ops.render.render(write_still=True)
 sha=lambda path:hashlib.sha256(Path(path).read_bytes()).hexdigest()
 face_path=Path('client/art-source/arrival-scene/ArrivalFace.png')
 info.update({'face':str(face_path),'face_sha256':sha(face_path),'helpers_sha256':{str(path):sha(path) for path in [Path('tools/art/build_modular_player.py'),Path('tools/art/rebuild_player_head.py'),Path('tools/art/rebuild_player_hair.py'),Path(__file__)]}})
 info.update({'source':str(a.source),'source_sha256':sha(a.source),'helper_sha256':sha(__file__),'blender':bpy.app.version_string,'runtime_verified':False,'idempotent_second_run':True})
 (a.output_dir/'neck-receipt.json').write_text(json.dumps(info,indent=2));print('PLAYER_NECK_REPAIR',json.dumps(info),flush=True)
if __name__=='__main__':main()
