"""Macro wardrobe pass from bed6df9: layered armor, waist relief and hip drapes.

Keeps the shared 65-bone rig/material atlas. New components have semantic slots
and are joined before export; no runtime renderer per ornament.
"""
import argparse, math, sys
from pathlib import Path
import bpy, bmesh
from mathutils import Matrix, Vector
from mathutils.bvhtree import BVHTree
p=argparse.ArgumentParser();p.add_argument('--output',type=Path,required=True)
a=p.parse_args(sys.argv[sys.argv.index('--')+1:]);a.output.mkdir(parents=True,exist_ok=True)
GOLD=((32.5+.5*511)/4096,(32.5+.985*511)/2048)
NAVY=((608.5+.065*511)/4096,(32.5+.55*511)/2048)
def armor_uv(u,v):return ((608.5+u*511)/4096,(32.5+v*511)/2048)
def cloth_uv(u,v):return ((608.5+u*511)/4096,(608.5+v*511)/2048)
for actor,source in [('keeper','gate-keeper/KeeperReconstruction.blend'),('player','arrival-scene/ArrivalScene.blend')]:
 bpy.ops.wm.open_mainfile(filepath='client/art-source/'+source)
 o=next(x for x in bpy.data.objects if x.type=='MESH' and x.name in ('Keeper Macro Reconstruction','Arrival Scene Reconstruction'));r=bpy.data.objects['Armature']
 assert not o.get('ensemble_macro'), 'One-time source migration already applied'
 for b in r.pose.bones:b.matrix_basis.identity()
 for side,angle in [('l',-60),('r',60)]:
  b=r.pose.bones['upperarm_'+side];b.rotation_mode='XYZ';b.rotation_euler.z=math.radians(angle)
 bpy.context.view_layer.update();tr={b.name:r.pose.bones[b.name].matrix@b.matrix_local.inverted() for b in r.data.bones};coords=[]
 for v in o.data.vertices:
  m=Matrix([[0.]*4 for _ in range(4)])
  for g in v.groups:m+=tr[o.vertex_groups[g.group].name]*g.weight
  coords.append(m@v.co)
 slot=o.data.attributes['appearance_slot'];body=[list(f.vertices) for f in o.data.polygons if slot.data[f.index].value==10]
 bvh=BVHTree.FromPolygons(coords,body);parts=[]
 def front(x,z,offset=.018):
  hit=bvh.ray_cast(Vector((x,-.65,z)),Vector((0,1,0)),1.)[0]
  assert hit is not None, ('surface fit missed',x,z)
  return hit.y-offset
 def add(name,vs,fs,bone,slot_id=16,uv=None):
  mesh=bpy.data.meshes.new(name);mesh.from_pydata([tr[bone].inverted()@Vector(v) for v in vs],[],fs);mesh.update()
  ob=bpy.data.objects.new(name,mesh);bpy.context.collection.objects.link(ob);mesh.materials.append(o.data.materials[0]);layer=mesh.uv_layers.new(name='UVMap')
  for f in mesh.polygons:
   f.use_smooth=True
   for li in f.loop_indices:layer.data[li].uv=uv[mesh.loops[li].vertex_index] if uv else GOLD
  for key,value in [('appearance_slot',slot_id),('npc_part',3)]:
   attr=mesh.attributes.new(key,'INT','FACE')
   for entry in attr.data:entry.value=value
  ob.vertex_groups.new(name=bone).add(list(range(len(vs))),1.,'REPLACE');parts.append(ob);return ob
 def tube(name,path,radius,bone,slot_id=16,color=GOLD):
  path=[Vector(v) for v in path];vs=[];fs=[];n=6
  for i,c in enumerate(path):
   d=(path[min(i+1,len(path)-1)]-path[max(0,i-1)]).normalized();axis=d.cross(Vector((0,1,0)))
   if axis.length<.01:axis=d.cross(Vector((1,0,0)))
   axis.normalize();other=d.cross(axis)
   for j in range(n):vs.append(c+radius*(axis*math.cos(j*math.tau/n)+other*math.sin(j*math.tau/n)))
  for i in range(len(path)-1):
   for j in range(n):k=i*n+j;q=i*n+(j+1)%n;fs.append((k,q,q+n,k+n))
  fs.extend([tuple(reversed(range(n))),tuple((len(path)-1)*n+j for j in range(n))]);return add(name,vs,fs,bone,slot_id,[color]*len(vs))
 def badge(name,c,rx,rz,bone,slot_id=16):
  c=Vector(c);vs=[c+Vector((0,-.014,0))];fs=[];tex=[armor_uv(.5,.5)];n=32
  for j in range(n):
   t=math.tau*j/n;vs.append(c+Vector((rx*math.cos(t),0,rz*math.sin(t))));tex.append(armor_uv(.5+.26*math.cos(t),.5+.26*math.sin(t)))
  for j in range(n):fs.append((0,j+1,(j+1)%n+1))
  add(name,vs,fs,bone,slot_id,tex);tube(name+' bound',vs[1:]+[vs[1]],.0025,bone,slot_id)
 # Replace bowl shoulders as a whole, preserving the garment body below them.
 bm=bmesh.new();bm.from_mesh(o.data);tag=bm.faces.layers.int['appearance_slot'];remove=[f for f in bm.faces if f[tag]==15];bmesh.ops.delete(bm,geom=remove,context='FACES');bm.to_mesh(o.data);bm.free()
 for side,sign in [('l',1),('r',-1)]:
  bone='clavicle_'+side;size=1 if actor=='keeper' else .86
  for tier in range(3 if actor=='keeper' else 2):
   vs=[];fs=[];tex=[];nx=12;ny=16
   for i in range(nx+1):
    u=i/nx;x=.167+size*(.165*u+.018*tier)
    for j in range(ny+1):
     v=j/ny;across=2*v-1;width=.095*(.75+.25*math.sin(math.pi*u))*(1-.12*tier)
     y=.035+across*width-.008*u
     z=1.505+.025*math.sin(math.pi*u)-.065*across*across+.034*u*u-.032*tier
     # The final third tapers to a swept tip instead of the old open bowl.
     y=.035+(y-.035)*(1-.65*max(0,(u-.7)/.3))
     vs.append((sign*x,y,z));tex.append(armor_uv(.08+.84*u,.08+.84*v))
   for i in range(nx):
    for j in range(ny):
     k=i*(ny+1)+j;f=(k,k+ny+1,k+ny+2,k+1);fs.append(f if sign>0 else tuple(reversed(f)))
   ids=list(range(ny+1))+[i*(ny+1)+ny for i in range(1,nx+1)]+[nx*(ny+1)+j for j in range(ny-1,-1,-1)]+[i*(ny+1) for i in range(nx-1,0,-1)]
   top_count=len(vs);top_faces=list(fs)
   vs.extend([(x,y,z-.006) for x,y,z in vs[:top_count]]);tex.extend(tex[:]);fs.extend([tuple(i+top_count for i in reversed(f)) for f in top_faces])
   for j,k in enumerate(ids):
    q=ids[(j+1)%len(ids)];fs.append((k,q,q+top_count,k+top_count))
   shell=add('Swept shoulder '+side+str(tier),vs,fs,bone,15,tex)
   normal_mesh=bmesh.new();normal_mesh.from_mesh(shell.data);bmesh.ops.recalc_face_normals(normal_mesh,faces=list(normal_mesh.faces));normal_mesh.to_mesh(shell.data);normal_mesh.free()
   tube('Shoulder binding '+side+str(tier),[vs[i] for i in ids]+[vs[ids[0]]],.003,bone,15)
  badge('Shoulder rosette '+side,(sign*.215,-.068,1.485),.028,.029,bone,15)
 # Broad curved belt shield and raised gold scrolls: a waist focal point.
 zc=1.155;vs=[];fs=[];tex=[];outline=[(-1,0),(-.86,.72),(-.35,.85),(0,1),(.35,.85),(.86,.72),(1,0),(.80,-.72),(.25,-.8),(0,-1),(-.25,-.8),(-.80,-.72)]
 for ring in (.08,.56,1):
  for u,v in outline:
   x=u*.108*ring;z=zc+v*.057*ring;vs.append((x,front(x,z,.024+.016*(1-ring)),z));tex.append(armor_uv(.5+.44*u*ring,.5+.44*v*ring))
 n=len(outline)
 for ring in range(2):
  for j in range(n):k=ring*n+j;q=ring*n+(j+1)%n;fs.append((k,q,q+n,k+n))
 add('Waist carved escutcheon',vs,fs,'pelvis',16,tex);tube('Waist shield rim',vs[-n:]+[vs[-n]],.0035,'pelvis')
 badge('Waist central seal',(0,front(0,zc,.047),zc),.035,.04,'pelvis')
 for sign in (-1,1):
  for zoff in (-.023,.023):
   path=[]
   for j in range(33):
    t=j/32;ang=t*math.pi*2.2;rad=.024*(1-.7*t);x=sign*(.061+rad*math.cos(ang));z=zc+zoff+rad*.58*math.sin(ang);path.append((x,front(x,z,.044),z))
   tube('Raised belt cloud',path,.0024,'pelvis')
 # Hip panels create readable layered hems; separate cloth pieces, not tile strips.
 for sign in (-1,1):
  vs=[];fs=[];tex=[];rows=18;cols=6;length=.42 if actor=='keeper' else .27
  for row in range(rows+1):
   t=row/rows
   for col in range(cols+1):
    u=col/cols;x=sign*(.080+(.100 if actor=="keeper" else .07)*t+(u-.5)*(.07+.025*t));z=1.08-length*t+.018*abs(2*u-1)*t
    y=front(x,z,.022)+.006*math.sin(u*math.tau)*math.sin(t*math.pi)
    vs.append((x,y,z));tex.append(cloth_uv(.06+.88*u,.04+.92*(1-t)))
  for row in range(rows):
   for col in range(cols):
    k=row*(cols+1)+col;f=(k,k+cols+1,k+cols+2,k+1);fs.append(f if sign>0 else tuple(reversed(f)))
  add('Ceremonial hip drape '+str(sign),vs,fs,'pelvis',10,tex)
  for col in (0,cols):tube('Hip bound edge',[vs[row*(cols+1)+col] for row in range(rows+1)],.002,'pelvis',10)
  # A pair of visible looped cords and weighted hanging seals match the reference.
  path=[]
  for j in range(33):
   t=j/32;x=sign*(.015+.09*t);z=1.12-.065*math.sin(math.pi*t);path.append((x,front(x,z,.035),z))
  tube('Hip ceremonial cord',path,.003,'pelvis')
  if actor=='keeper' or sign==1:
   x=sign*.105;z=.955;y=front(x,z,.046);tube('Pendant suspension',[(sign*.10,front(sign*.10,1.105,.04),1.105),(x,y,z+.042)],.003,'pelvis');badge('Hip hanging seal',(x,y,z),.025,.038,'pelvis')
   for strand in range(7):
    dx=(strand-3)*.0025;tube('Silk tassel',[(x+dx,y,z-.036),(x+dx*1.3,y-.002,z-.088),(x+dx*1.8,y+.006,z-.135)],.0016,'pelvis')
 # Chest chains connect the lapel clasp to the shoulder fastening points.
 for sign in (-1,1):
  path=[]
  for j in range(25):
   t=j/24;x=sign*(.11*(1-t))+.025*t;z=1.445-.11*t-.018*math.sin(math.pi*t);path.append((x,front(x,z,.022),z))
  tube('Chest ceremonial chain',path,.0024,'spine_03')
 bpy.ops.object.select_all(action='DESELECT');o.select_set(True)
 for part in parts:part.select_set(True)
 bpy.context.view_layer.objects.active=o;bpy.ops.object.join();o['ensemble_macro']='bed6df9'
 # Preserve the validated body normals; only new closed shoulder shells are recalculated.
 for b in r.pose.bones:b.matrix_basis.identity()
 bpy.context.view_layer.update();bpy.context.preferences.filepaths.save_version=0
 bpy.ops.wm.save_as_mainfile(filepath=str((a.output/f'{actor}.blend').resolve()),compress=True)
 print(f'LGO_ENSEMBLE_MACRO_READY actor={actor} source_components={len(parts)} joined_renderer=true atlas_unchanged=true',flush=True)
