"""One-time anatomical head/hair migration of the a01b39c arrival wardrobe.

Uses pinned CC0 MakeHuman asset data, the existing facial albedo and rest rig.
Output remains an editable 14-object wardrobe. This is not a character creator.
Run through Blender --python ... -- --source ORIGINAL.blend --output STAGING.
"""
import bpy,bmesh,json,math,hashlib
from pathlib import Path
from mathutils import Vector,kdtree
R=Path(__file__).resolve().parents[2]
import argparse,sys
parser=argparse.ArgumentParser(description=__doc__)
parser.add_argument('--source',type=Path,required=True)
parser.add_argument('--output',type=Path,required=True)
args=parser.parse_args(sys.argv[sys.argv.index('--')+1:])
assert hashlib.sha256(args.source.read_bytes()).hexdigest() == '0047c582560852f335deddaedad95c9884e117338ebe6706576c163c30ab11df', 'Migration requires the pre-anatomy master from a01b39c; do not run on an already migrated master.'
sys.dont_write_bytecode=True
sys.path.insert(0,str(Path(__file__).resolve().parent))
from human_forms import verify_seed,topology,target
verify_seed()
O=args.output;O.mkdir(parents=True,exist_ok=True)
bpy.ops.wm.open_mainfile(filepath=str(args.source.resolve()))
rig=bpy.data.objects['Armature'];head=bpy.data.objects['Head'];hair=bpy.data.objects['Hair']
for b in rig.pose.bones:b.matrix_basis.identity()
old=[v.co.copy() for v in head.data.vertices];weights=[[(head.vertex_groups[g.group].name,g.weight) for g in v.groups] for v in head.data.vertices]
tree=kdtree.KDTree(len(old))
for i,v in enumerate(old):tree.insert(v,i)
tree.balance()
vertices=[Vector(v) for v in topology()[0]];groups=topology()[1]
for index,delta in target('asian-male-young.target').items():vertices[index]+=Vector(delta)
def center(group):
 ids=set(i for f in groups[group] for i in f);return sum((vertices[i] for i in ids),Vector())/len(ids)
eye=center('helper-l-eye');top=max(vertices[i].y for f in groups['body'] for i in f)
print('HEAD_LANDMARKS',eye[:],top,flush=True)
# Fit the anatomical head to the existing neck/crown envelope, preserving its jaw and facial topology.
def transform(v):return Vector((v.x*.089,-v.z*.083+.047,1.790+(v.y-top)*.083))
selected=[f for f in groups['body'] if all(vertices[i].y>top-2.80 for i in f) and all(abs(vertices[i].x)<1.2 for i in f)]
for g in ['helper-l-eye','helper-r-eye']:selected+=groups[g]
ids=sorted(set(i for f in selected for i in f));lookup={v:i for i,v in enumerate(ids)}
vs=[transform(vertices[i]) for i in ids];fs=[[lookup[i] for i in f] for f in selected]
data=bpy.data.meshes.new('Arrival anatomical head from CC0 MakeHuman');data.from_pydata(vs,[],fs);data.update()
head.data=data
# Cut a clean neck ring and extend it under the collar before subdivision.
bm=bmesh.new();bm.from_mesh(data)
bmesh.ops.bisect_plane(bm,geom=list(bm.verts)+list(bm.edges)+list(bm.faces),dist=.000001,
 plane_co=(0,0,1.575),plane_no=(0,0,1),clear_inner=True)
ring=[e for e in bm.edges if e.is_boundary and all(abs(v.co.z-1.575)<.00002 for v in e.verts)]
assert len(ring)>12,len(ring)
extruded=bmesh.ops.extrude_edge_only(bm,edges=ring)['geom']
newverts=[v for v in extruded if isinstance(v,bmesh.types.BMVert)]
for v in newverts:
 angle=math.atan2(v.co.x,v.co.y-.028)
 v.co=Vector((math.sin(angle)*.032,.028+math.cos(angle)*.043,1.535))
bottom=[e for e in bm.edges if e.is_boundary and all(abs(v.co.z-1.535)<.00002 for v in e.verts)]
bmesh.ops.holes_fill(bm,edges=bottom,sides=0)
bm.to_mesh(data);bm.free()
# Keep lower neck inside the outfit opening; remove shoulder flare from the neutral body seed.
for v in data.vertices:
 t=max(0,min(1,(1.625-v.co.z)/.035));t=t*t*(3-2*t)
 angle=math.atan2(v.co.x,v.co.y-.028)
 v.co=v.co.lerp(Vector((math.sin(angle)*.032,.028+math.cos(angle)*.043,v.co.z)),t)
# Smooth once using the supplied facial edge loops, rather than adding analytic rings.
bpy.context.view_layer.objects.active=head
sub=head.modifiers.new('Anatomical surface refinement','SUBSURF');sub.levels=1
bpy.ops.object.modifier_move_up(modifier=sub.name)
bpy.ops.object.modifier_apply(modifier=sub.name)
data=head.data
# Reuse the dedicated face albedo, with landmarks mapped to the anatomical eye/nose/mouth heights.
eye_world=transform(eye);print('WORLD_EYE',eye_world[:],flush=True)
control=[(1.53,.04),(1.59,.18),(1.615,.30),(1.643,.435),(1.663,.53),(eye_world.z,.67),(1.790,.98)]
def interp(z):
 for (a,u),(b,v) in zip(control,control[1:]):
  if z<=b:return u+(v-u)*max(0,min(1,(z-a)/(b-a)))
 return .98
uv=data.uv_layers.new(name='UVMap');data.materials.append(bpy.data.materials['Keeper Reconstruction Face'])
for p in data.polygons:
 p.use_smooth=True
 for li in p.loop_indices:
  v=data.vertices[data.loops[li].vertex_index].co
  u=max(.585,min(.915,.75+v.x*(.070/max(.01,eye_world.x))))
  # Gradually leave the portrait at the back of the head; no hard UV switch across the jaw.
  back=max(0,min(1,(v.y-.015)/.04));back=back*back*(3-2*back)
  above_neck=max(0,min(1,(v.z-1.60)/.065));above_neck=above_neck*above_neck*(3-2*above_neck)
  u+=( (.915 if v.x>0 else .585)-u)*back*above_neck
  uv.data[li].uv=(u,interp(v.z))
for name,val in [('appearance_slot',2),('npc_part',2)]:
 tag=data.attributes.new(name,'INT','FACE')
 for a in tag.data:a.value=val
for name in ('Head','neck_01','spine_03'):
 assert name in rig.data.bones
 if head.vertex_groups.get(name) is None:head.vertex_groups.new(name=name)
def smooth(a,b,value):
 t=max(0,min(1,(value-a)/(b-a)));return t*t*(3-2*t)
for v in data.vertices:
 for group in head.vertex_groups:group.remove([v.index])
 # Anchor the jaw to Head and distribute the continuous neck through its own joint.
 h=smooth(1.565,1.642,v.co.z)
 jaw=smooth(1.595,1.62,v.co.z)*(1-smooth(-.025,.015,v.co.y))
 h=max(h,jaw);base=(1-h)*(1-smooth(1.54,1.575,v.co.z))
 for name,w in [('Head',h),('neck_01',1-h-base),('spine_03',base)]:
  if w>0:head.vertex_groups[name].add([v.index],w,'REPLACE')
# Fit the scalp by rays through the real cranium; carry its four swept front locks with it.
from mathutils.bvhtree import BVHTree
bm=bmesh.new();bm.from_mesh(data);surface=BVHTree.FromBMesh(bm);bm.free()
cap_count=2730
assert len(hair.data.vertices)==13022
oldcap=[v.co.copy() for v in hair.data.vertices[:cap_count]]
cap_tree=kdtree.KDTree(cap_count);delta=[]
for i,point in enumerate(oldcap):
 direction=(point-Vector((0,.025,1.69))).normalized()
 hit,normal,_,_=surface.ray_cast(Vector((0,.025,1.69)),direction,.4)
 target=hit+direction*.004 if hit is not None and i<cap_count-2 else point
 delta.append(target-point);hair.data.vertices[i].co=target;cap_tree.insert(point,i)
cap_tree.balance()
for v in hair.data.vertices[cap_count:cap_count+4*1374]:
 _,i,d=cap_tree.find(v.co)
 v.co+=delta[i]*max(0,min(1,1-d/.07))
# Ear openings belong to the scalp boundary, not a black shell conformed over the ear folds.
hbm=bmesh.new();hbm.from_mesh(hair.data);hbm.faces.ensure_lookup_table()
cut=[]
for face in hbm.faces:
 if any(v.index>=cap_count for v in face.verts):continue
 c=face.calc_center_median()
 if abs(c.x)>.052 and ((c.y-.012)/.023)**2+((c.z-1.691)/.029)**2<1:
  cut.append(face)
bmesh.ops.delete(hbm,geom=cut,context='FACES')
edgeverts=[v for v in hbm.verts if any(e.is_boundary for e in v.link_edges)]
for _ in range(4):bmesh.ops.smooth_vert(hbm,verts=edgeverts,factor=.5,use_axis_x=True,use_axis_y=True,use_axis_z=True)
hbm.to_mesh(hair.data);hbm.free()
# The separate tie was egg shaped; flatten that connected component to a compact gathered knot.
adj=[set() for _ in hair.data.vertices]
for e in hair.data.edges:a,b=e.vertices;adj[a].add(b);adj[b].add(a)
seen=set()
for start in range(len(adj)):
 if start in seen:continue
 todo=[start];seen.add(start);component=[]
 while todo:
  i=todo.pop();component.append(i)
  for n in adj[i]:
   if n not in seen:seen.add(n);todo.append(n)
 pts=[hair.data.vertices[i].co for i in component];c=sum(pts,Vector())/len(pts)
 if len(component)<800 and c.z>1.77 and .04<c.y<.12:
  for i in component:
   v=hair.data.vertices[i];v.co.z=c.z+(v.co.z-c.z)*.62;v.co.x*=.9
# Ensure normals agree around lips, nostrils and neck.
bm=bmesh.new();bm.from_mesh(data);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(data);bm.free()
bpy.context.preferences.filepaths.save_version=0
bpy.ops.wm.save_as_mainfile(filepath=str(O/'ArrivalWardrobe.blend'),compress=True)
data.calc_loop_triangles();print('ANATOMY_CANDIDATE',len(data.vertices),len(data.loop_triangles),flush=True)
