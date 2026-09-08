"""Smooth anatomical cloth bulges, separate back UVs and transfer cuff skinning.

Migration from 51fa165. Keeps topology, slots, materials and atlas unchanged.
"""
import argparse, math, sys
from pathlib import Path
import bpy
from mathutils import Matrix, Vector
from mathutils.bvhtree import BVHTree
p=argparse.ArgumentParser();p.add_argument('--source',type=Path,required=True);p.add_argument('--output',type=Path,required=True)
a=p.parse_args(sys.argv[sys.argv.index('--')+1:]);bpy.ops.wm.open_mainfile(filepath=str(a.source.resolve()))
o=next(x for x in bpy.data.objects if x.type=='MESH' and x.name in ('Keeper Macro Reconstruction','Arrival Scene Reconstruction'));r=bpy.data.objects['Armature']
for b in r.pose.bones:b.matrix_basis.identity()
for side,angle in [('l',-60),('r',60)]:
 b=r.pose.bones['upperarm_'+side];b.rotation_mode='XYZ';b.rotation_euler.z=math.radians(angle)
bpy.context.view_layer.update();tr={b.name:r.pose.bones[b.name].matrix@b.matrix_local.inverted() for b in r.data.bones}
coords=[];weights=[];matrices=[]
for v in o.data.vertices:
 w={o.vertex_groups[g.group].name:g.weight for g in v.groups};m=Matrix([[0.]*4 for _ in range(4)])
 for n,value in w.items():m+=tr[n]*value
 weights.append(w);matrices.append(m);coords.append(m@v.co)
uv=o.data.uv_layers.active.data;slot=o.data.attributes['appearance_slot'];torso=set();sleeves=set();cuffs=set();clothfaces=[]
for f in o.data.polygons:
 if slot.data[f.index].value!=10:continue
 q=uv[f.loop_indices[0]].uv;center=sum((coords[i] for i in f.vertices),Vector())/len(f.vertices)
 if 1152/4096<q.x<1728/4096 and 192/2048<q.y<768/2048:
  torso.update(f.vertices);clothfaces.append(f.index)
  # Back is continuous ivory cloth; the decorative crossing belongs at the front.
  if center.y>.035:
   for li in f.loop_indices:
    u,v=uv[li].uv;localu=(u*4096-1184.5)/511;localv=(v*2048-224.5)/511
    uv[li].uv=((32.5+localu*511)/4096,(608.5+localv*511)/2048)
 elif 0<q.x<576/4096 and 576/2048<q.y<1152/2048:sleeves.update(f.vertices);clothfaces.append(f.index)
 elif len(f.vertices)==4 and center.z>1.23 and abs(center.x)>.18:cuffs.update(f.vertices)
# Recover the complete connected cuff shells, including their innermost seam.
adj={i:set() for i in range(len(coords))}
for edge in o.data.edges:
 i,j=edge.vertices;adj[i].add(j);adj[j].add(i)
pending=list(cuffs)
while pending:
 i=pending.pop()
 for j in adj[i]:
  if j not in cuffs:cuffs.add(j);pending.append(j)
assert len(cuffs)==1170, 'Expected two isolated 585-vertex cuff shells'
selected=torso|sleeves;neighbors={i:set() for i in selected}
for edge in o.data.edges:
 i,j=edge.vertices
 if i in selected and j in selected:neighbors[i].add(j);neighbors[j].add(i)
smooth=[c.copy() for c in coords]
for _ in range(20):
 previous=[v.copy() for v in smooth]
 for i in selected:
  ns=neighbors[i]
  if ns:smooth[i]=previous[i].lerp(sum((previous[j] for j in ns),Vector())/len(ns),.45)
def ease(t):
 t=max(0.,min(1.,t));return t*t*(3-2*t)
changed=0
for i in selected:
 co=coords[i];fade=ease((co.z-1.235)/.09)*ease((1.575-co.z)/.075)
 if i in torso:
  # Preserve silhouette width/height and neckline; smooth the pectoral/scapular bulges.
  co.y=co.y*(1-.88*fade)+smooth[i].y*.88*fade
  co.y+=(-1 if co.y<.025 else 1)*.006*fade
  if co.y>.055:
   # A continuous back envelope removes the source's diagonal scapular ridges.
   backfade=ease((co.z-1.235)/.08)*ease((1.575-co.z)/.035)*ease((.205-abs(co.x))/.06)
   radius=.135+.015*math.sin(math.pi*max(0,min(1,(co.z-1.24)/.31)))
   target=.022+radius*math.sqrt(max(.05,1-(co.x/.195)**2))
   co.y=co.y*(1-backfade)+target*backfade
 else:
  # Sleeves keep their hem and shoulder attachment, with softer volume between them.
  fade=ease((co.z-1.29)/.075)*ease((1.555-co.z)/.055)
  co=co.lerp(smooth[i],.65*fade);co.z-=.006*fade
 o.data.vertices[i].co=matrices[i].inverted()@co;coords[i]=co;changed+=fade>.01
# Transfer cuff weights from actual sleeve triangles, retaining a stable 9mm shell.
triangles=[]
for fi in clothfaces:
 f=o.data.polygons[fi]
 if not any(i in sleeves for i in f.vertices):continue
 ids=list(f.vertices)
 for j in range(1,len(ids)-1):triangles.append((ids[0],ids[j],ids[j+1]))
bvh=BVHTree.FromPolygons(coords,triangles,all_triangles=True)
for i in cuffs:
 point,normal,fi,distance=bvh.find_nearest(coords[i]);assert point is not None and distance<.12
 ids=triangles[fi];a0,b0,c0=[coords[j] for j in ids];v0=b0-a0;v1=c0-a0;v2=point-a0
 d00=v0.dot(v0);d01=v0.dot(v1);d11=v1.dot(v1);den=d00*d11-d01*d01;assert abs(den)>1e-14
 beta=(d11*v2.dot(v0)-d01*v2.dot(v1))/den;gamma=(d00*v2.dot(v1)-d01*v2.dot(v0))/den;mix=[max(0,1-beta-gamma),max(0,beta),max(0,gamma)];w={}
 for j,factor in zip(ids,mix):
  for n,value in weights[j].items():w[n]=w.get(n,0)+factor*value
 w=dict(sorted(w.items(),key=lambda item:item[1],reverse=True)[:4]);total=sum(w.values());w={n:value/total for n,value in w.items() if value>1e-6};total=sum(w.values());w={n:value/total for n,value in w.items()};m=Matrix([[0.]*4 for _ in range(4)])
 for group in o.vertex_groups:group.remove([i])
 for n,value in w.items():o.vertex_groups[n].add([i],value,'REPLACE');m+=tr[n]*value
 o.data.vertices[i].co=m.inverted()@coords[i]
for b in r.pose.bones:b.matrix_basis.identity()
bpy.context.view_layer.update();bpy.context.preferences.filepaths.save_version=0;a.output.parent.mkdir(parents=True,exist_ok=True)
bpy.ops.wm.save_as_mainfile(filepath=str(a.output.resolve()),compress=True)
print(f'LGO_CLOTH_DRAPE_READY shaped_vertices={changed} cuff_weights={len(cuffs)} topology_unchanged=true')
