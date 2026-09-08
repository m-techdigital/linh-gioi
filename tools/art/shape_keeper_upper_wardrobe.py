"""Whole upper wardrobe migration from 045a7de editable master; write staging only."""
import argparse,math,sys
from pathlib import Path
import bpy,bmesh
from mathutils import Vector,Matrix
from mathutils.bvhtree import BVHTree
p=argparse.ArgumentParser();p.add_argument('--output',type=Path,required=True);a=p.parse_args(sys.argv[sys.argv.index('--')+1:]);a.output.mkdir(parents=True,exist_ok=True)
bpy.ops.wm.open_mainfile(filepath='client/art-source/gate-keeper/KeeperWardrobe.blend')
r=bpy.data.objects['Armature'];bodymat=bpy.data.materials['Keeper Reconstruction']
for b in r.pose.bones:b.matrix_basis.identity()
for side,angle in [('l',-60),('r',60)]:
 b=r.pose.bones['upperarm_'+side];b.rotation_mode='XYZ';b.rotation_euler.z=math.radians(angle)
bpy.context.view_layer.update();tr={b.name:r.pose.bones[b.name].matrix@b.matrix_local.inverted() for b in r.data.bones}
def bindmatrix(o,v):
 m=Matrix([[0.]*4 for _ in range(4)])
 for g in v.groups:m+=tr[o.vertex_groups[g.group].name]*g.weight
 return m
# Remove the retained old waist flare as a whole, keep torso and sleeves.
o=bpy.data.objects['UpperBody'];coords=[bindmatrix(o,v)@v.co for v in o.data.vertices]
remove=[]
for f in o.data.polygons:
 center=sum((coords[i] for i in f.vertices),Vector())/len(f.vertices)
 arm=sum(sum(g.weight for g in o.data.vertices[i].groups if o.vertex_groups[g.group].name.startswith(('upperarm_','lowerarm_','clavicle_'))) for i in f.vertices)/len(f.vertices)
 if center.z<1.108 and arm<.35:remove.append(f.index)
# Billowed upper sleeves, preserving all existing weights and UVs.
for v in o.data.vertices:
 co=coords[v.index];arm=sum(g.weight for g in v.groups if o.vertex_groups[g.group].name.startswith('upperarm_'))
 if arm>.65 and 1.28<co.z<1.49:
  bone=r.pose.bones['upperarm_'+('l' if co.x>0 else 'r')];axis=(bone.tail-bone.head).normalized();along=(co-bone.head).dot(axis);radial=co-bone.head-axis*along
  co+=radial*(.16*math.sin(math.pi*(co.z-1.28)/.21));v.co=bindmatrix(o,v).inverted()@co
bm=bmesh.new();bm.from_mesh(o.data);bm.faces.ensure_lookup_table();bmesh.ops.delete(bm,geom=[bm.faces[i] for i in remove],context='FACES');bm.to_mesh(o.data);bm.free()
# Join the new skirt root under the retained belt; remove hidden trouser faces.
for name in ['Outer robe left','Outer robe right','Ivory under robe']:
 part=bpy.data.objects[name]
 for v in part.data.vertices:
  t=max(0.,min(1.,(v.co.z-.99)/.10));v.co.z+=.045*t*t*(3-2*t)
trousers=bpy.data.objects['LowerBody'];bm=bmesh.new();bm.from_mesh(trousers.data)
hidden=[f for f in bm.faces if min(v.co.z for v in f.verts)>.68]
print('HIDDEN_TROUSER_FACES',len(hidden));bmesh.ops.delete(bm,geom=hidden,context='FACES');bm.to_mesh(trousers.data);bm.free()
# Evaluate the shirt surface after sleeve shaping to fit the cape's root.
bpy.context.view_layer.update();shirt_tree=BVHTree.FromObject(o,bpy.context.evaluated_depsgraph_get())
# New self-contained garments replace the old flattened shoulders and cape.
old_cape_evaluated=bpy.data.objects['Cape'].evaluated_get(bpy.context.evaluated_depsgraph_get())
old_cape_coords=[v.co.copy() for v in old_cape_evaluated.data.vertices]
old_cape_tree=BVHTree.FromObject(bpy.data.objects['Cape'],bpy.context.evaluated_depsgraph_get())
for name in ['Shoulders','Cape']:bpy.data.objects.remove(bpy.data.objects[name],do_unlink=True)
GOLD=((32.5+.5*511)/4096,(32.5+.985*511)/2048)
def uvpatch(u,v,cloth=False):return ((608.5+u*511)/4096,((608.5 if cloth else 32.5)+v*511)/2048)
def meshpart(name,vs,fs,uv,weights,slot,thickness):
 mesh=bpy.data.meshes.new(name);mesh.from_pydata(vs,[],fs);mesh.update();o=bpy.data.objects.new(name,mesh);bpy.context.collection.objects.link(o);mesh.materials.append(bodymat);layer=mesh.uv_layers.new(name='UVMap')
 for f in mesh.polygons:
  f.use_smooth=True
  for li in f.loop_indices:layer.data[li].uv=uv[mesh.loops[li].vertex_index]
 for key,value in [('appearance_slot',slot),('npc_part',3)]:
  attr=mesh.attributes.new(key,'INT','FACE')
  for d in attr.data:d.value=value
 for i,w in enumerate(weights):
  m=Matrix([[0.]*4 for _ in range(4)])
  for n,value in w.items():
   group=o.vertex_groups.get(n) or o.vertex_groups.new(name=n);group.add([i],value,'REPLACE');m+=tr[n]*value
  mesh.vertices[i].co=m.inverted()@mesh.vertices[i].co
 if thickness:
  mod=o.modifiers.new('Editable thickness','SOLIDIFY');mod.thickness=thickness;mod.offset=0
 mod=o.modifiers.new('Shared rig','ARMATURE');mod.object=r;o.parent=r;o.asset_mark();return o
for side,sign in [('l',1),('r',-1)]:
 vs=[];fs=[];uv=[];weights=[];bone='clavicle_'+side;rows=20;cols=24
 for tier in range(3):
  offset=len(vs)
  for i in range(rows+1):
   u=i/rows;half=.097*(.55+.45*math.sin(math.pi*u))*(1-.80*u**3)
   for j in range(cols+1):
    v=j/cols;theta=(v-.5)*math.pi;across=math.sin(theta)
    x=sign*(.152+.195*u+.013*tier)
    y=.030+half*across
    z=1.523+.025*math.sin(math.pi*u)-.10*u*u+.052*u**6-.065*(1-math.cos(theta))-.030*tier
    vs.append((x,y,z));weights.append({bone:1.});uv.append(uvpatch(.05+.90*u,.05+.90*v))
  for i in range(rows):
   for j in range(cols):
    k=offset+i*(cols+1)+j;face=(k,k+cols+1,k+cols+2,k+1);fs.append(face if sign>0 else tuple(reversed(face)))
  # The perimeter is a narrow integrated band, not a separately managed object.
  ids=[offset+j for j in range(cols+1)]+[offset+i*(cols+1)+cols for i in range(1,rows+1)]+[offset+rows*(cols+1)+j for j in range(cols-1,-1,-1)]+[offset+i*(cols+1) for i in range(rows-1,0,-1)]
  start=len(vs)
  for k in ids:
   co=Vector(vs[k]);vs.append(tuple(co+Vector((0,0,.001))));uv.append(GOLD);weights.append({bone:1.})
   centre=Vector((sign*.245,.030,1.47));toward=centre-co;toward.z=0;toward.normalize();vs.append(tuple(co+toward*.003+Vector((0,0,.002))));uv.append(GOLD);weights.append({bone:1.})
  for j in range(len(ids)):
   k=start+2*j;q=start+2*((j+1)%len(ids));fs.append((k,q,q+1,k+1))
 meshpart('Shoulder mantle '+side,vs,fs,uv,weights,15,.005)
# A single quad cape with clean hems and deliberate vertical folds.
vs=[];fs=[];uv=[];weights=[];rows=40;cols=40
for i in range(rows+1):
 t=i/rows
 for j in range(cols+1):
  u=j/cols;s=2*u-1;width=.205+.115*t+.035*math.sin(math.pi*t)
  if t<.30:
   row_z=1.512-1.19*t
   near=[abs(co.x) for co in old_cape_coords if abs(co.z-row_z)<.018]
   if near:width=max(near)
  x=s*width;z=1.512-1.19*t+.20*t**5*math.exp(-(s/.25)**2)+.018*abs(s)*(1-t)
  y=.130+.095*t+.055*t*t+.014*math.sin(math.pi*t)*math.cos(s*math.pi*4)-.025*s*s*(1-t)
  if t<.30:
   hit=shirt_tree.ray_cast(Vector((x,.60,z)),Vector((0,-1,0)),.60)[0]
   if hit is not None:y=max(y,hit.y+.022)
   retained=old_cape_tree.ray_cast(Vector((x,.60,z)),Vector((0,-1,0)),.60)[0]
   if retained is not None:y=max(y,retained.y+.005)
  vs.append((x,y,z));uv.append(uvpatch(.035+.93*u,.025+.95*(1-t),True))
  root=(1-t)**5*.45*max(0,(abs(s)-.5)*2);weights.append({'spine_03':1-root,'clavicle_'+('l' if s>=0 else 'r'):root} if root>1e-6 else {'spine_03':1.})
for i in range(rows):
 for j in range(cols):
  k=i*(cols+1)+j;fs.append((k,k+1,k+cols+2,k+cols+1))
meshpart('Cape',vs,fs,uv,weights,12,.003)
bpy.context.preferences.filepaths.save_version=0
bpy.ops.wm.save_as_mainfile(filepath=str((a.output/'KeeperWardrobe.blend').resolve()),compress=True)
print('LGO_UPPER_WARDROBE_READY old_waist_faces_removed='+str(len(remove)),flush=True)
