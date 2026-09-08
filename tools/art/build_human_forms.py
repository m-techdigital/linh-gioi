"""Build reusable adult shape-key bases and fit two independent hair families.

Blender --python tools/art/build_human_forms.py -- --output STAGING [--render]
Produces authoring data, not rigged/runtime-ready NPCs. No identity is assigned
to a class, wardrobe or gameplay role by the base-form catalogue.
"""
import argparse,sys,json,math
from pathlib import Path
import bpy
from mathutils import Vector
from mathutils.bvhtree import BVHTree
sys.dont_write_bytecode=True
sys.path.insert(0,str(Path(__file__).resolve().parent))
from human_forms import HumanForm,presets,verify_seed,topology,coordinates,raw_coordinates,landmarks
p=argparse.ArgumentParser();p.add_argument('--output',type=Path,required=True);p.add_argument('--render',action='store_true')
a=p.parse_args(sys.argv[sys.argv.index('--')+1:]);a.output.mkdir(parents=True,exist_ok=True)
root=Path(__file__).resolve().parents[2];verify_seed()
# Read only the authored hair. It is a separate input; no outfit geometry is required.
with bpy.data.libraries.load(str(root/'client/art-source/arrival-scene/ArrivalWardrobe.blend'),link=False) as (src,dst):dst.objects=['Hair']
hair=dst.objects[0]
hair_vertices=[v.co.copy() for v in hair.data.vertices];hair_faces=[tuple(p.vertices) for p in hair.data.polygons]
bpy.data.objects.remove(hair,do_unlink=True)
bpy.ops.object.select_all(action='SELECT');bpy.ops.object.delete(use_global=False)
forms=presets();reference=HumanForm();base=coordinates(reference);raw=raw_coordinates(reference)
verts,groups,uvs,face_uvs=topology();faces=groups['body']+groups['helper-l-eye']+groups['helper-r-eye']
ids=sorted(set(i for f in faces for i in f));lookup={i:n for n,i in enumerate(ids)};localfaces=[tuple(lookup[i] for i in f) for f in faces]
allcoords={f.key:coordinates(f) for f in forms}

def material(name,color,rough=.75):
 m=bpy.data.materials.new(name);m.diffuse_color=(*color,1);m.use_nodes=True;b=m.node_tree.nodes.get('Principled BSDF');b.inputs['Base Color'].default_value=(*color,1);b.inputs['Roughness'].default_value=rough;return m
clay=material('Neutral anatomy study',(.38,.43,.43));dark=material('Shared hair study',(.032,.045,.053))
def mesh_object(name,points,polygons,mat):
 data=bpy.data.meshes.new(name);data.from_pydata(points,[],polygons);data.materials.append(mat);data.update()
 for face in data.polygons:face.use_smooth=True
 obj=bpy.data.objects.new(name,data);bpy.context.collection.objects.link(obj);return obj
body=mesh_object('Human base — shared topology',[base[i] for i in ids],localfaces,clay)
uv=body.data.uv_layers.new(name='MakeHuman UV')
uvfaces=face_uvs['body']+face_uvs['helper-l-eye']+face_uvs['helper-r-eye']
for poly,indices in zip(body.data.polygons,uvfaces):
 for loop,index in zip(poly.loop_indices,indices):uv.data[loop].uv=uvs[index]
idattr=body.data.attributes.new('source_vertex_id','INT','POINT')
for value,index in zip(idattr.data,ids):value.value=index
body.shape_key_add(name='Basis')
for form in forms:
 key=body.shape_key_add(name=form.key)
 for value,index in zip(key.data,ids):value.co=allcoords[form.key][index]
body['purpose']='Authoring base; no runtime rig or final identity/material';body['form_height_m']=reference.height
# The base and every form use identical vertex/face indices. Hair bindings store
# a triangle and barycentric weights once, then follow its displacement.
body_ids=set(i for f in groups['body'] for i in f);scale=reference.height/(max(raw[i][1] for i in body_ids)-min(raw[i][1] for i in body_ids))
hairpoints=[Vector((v.x*scale/.089,(v.y-.047)*scale/.083,reference.height+(v.z-1.790)*scale/.083)) for v in hair_vertices]
triangles=[]
for face in groups['body']:
 if min(base[i][2] for i in face)>1.49:
  for k in range(1,len(face)-1):triangles.append((face[0],face[k],face[k+1]))
bvh=BVHTree.FromPolygons([Vector(v) for v in base],triangles,all_triangles=True)
def barycentric(point,triangle):
 x,y,z=[Vector(base[i]) for i in triangle];v0=y-x;v1=z-x;v2=point-x
 d00=v0.dot(v0);d01=v0.dot(v1);d11=v1.dot(v1);d20=v2.dot(v0);d21=v2.dot(v1);den=d00*d11-d01*d01
 if abs(den)<1e-15:raise ValueError('Degenerate attachment triangle')
 b=(d11*d20-d01*d21)/den;c=(d00*d21-d01*d20)/den;return (1-b-c,b,c)
bindings=[]
for point in hairpoints:
 hit,normal,index,distance=bvh.find_nearest(point)
 triangle=triangles[index];weights=barycentric(hit,triangle);bindings.append((triangle,weights))
def fitted(form):
 target=allcoords[form.key];out=[]
 for point,(triangle,weights) in zip(hairpoints,bindings):
  delta=sum(((Vector(target[i])-Vector(base[i]))*w for i,w in zip(triangle,weights)),Vector());out.append(point+delta)
 return out
hair_forms={f.key:fitted(f) for f in forms}
# Short hair is the crown plus the four authored swept locks. The long family
# adds the independent tie/ponytail components. Detect connected components,
# never assume a vertex offset from the previous migration.
adj=[set() for _ in hairpoints]
for face in hair_faces:
 for x,y in zip(face,face[1:]+face[:1]):adj[x].add(y);adj[y].add(x)
seen=set();short_ids=set()
for i in range(len(adj)):
 if i in seen:continue
 todo=[i];seen.add(i);component=[]
 while todo:
  j=todo.pop();component.append(j)
  for k in adj[j]:
   if k not in seen:seen.add(k);todo.append(k)
 if len(component)>1000 and min(hair_vertices[j].z for j in component)>1.60:short_ids.update(component)
styles={}
for name,chosen in [('tied_long',set(range(len(hairpoints)))),('swept_short',short_ids)]:
 selected=sorted(chosen);mapping={v:i for i,v in enumerate(selected)};polys=[tuple(mapping[i] for i in f) for f in hair_faces if all(i in chosen for i in f)]
 obj=mesh_object('Hair '+name,[hairpoints[i] for i in selected],polys,dark);obj.shape_key_add(name='Basis')
 for form in forms:
  key=obj.shape_key_add(name=form.key)
  for value,index in zip(key.data,selected):value.co=hair_forms[form.key][index]
 for axis in range(3):
  ia=obj.data.attributes.new('bind_vertex_'+str(axis),'INT','POINT');wa=obj.data.attributes.new('bind_weight_'+str(axis),'FLOAT','POINT')
  for j,index in enumerate(selected):
   obj.data.attributes['bind_vertex_'+str(axis)].data[j].value=bindings[index][0][axis]
   obj.data.attributes['bind_weight_'+str(axis)].data[j].value=bindings[index][1][axis]
 obj['bind_base']='Human base — shared topology';obj['fit_stage']='Authoring displacement, not cloth collision';styles[name]=(obj,selected,polys)
# Keep two distinct source styles available without drawing both on top of each other.
styles['swept_short'][0].hide_render=True;styles['swept_short'][0].hide_set(True)
receipt={'stage':'AUTHORING_BASES_NOT_RUNTIME_NPCS','forms':[dict(key=f.key,sex=f.sex,age=f.age,build=f.build,height=f.height,landmarks=landmarks(f)) for f in forms],'vertices':len(ids),'polygons':len(localfaces),'hair_styles':{n:len(v[1]) for n,v in styles.items()},'shared_topology':True,'new_runtime_assets':0,'rigged':False,'art_final':False,'reference':'docs/reference-art/v0.20.0/lgo-npc-direction-sheet-v0200.png','binding':'source triangle IDs and barycentric displacement; preserved authored hair shape'}
# Validate the whole matrix once, including distinct geometry and finite bindings.
assert len({tuple(tuple(v) for v in allcoords[f.key]) for f in forms})==18
assert all(abs(sum(w)-1)<1e-6 for _,w in bindings)
assert all(math.isfinite(c) for points in hair_forms.values() for v in points for c in v)
assert all(len(points)==len(base) for points in allcoords.values())
bpy.data.orphans_purge(do_recursive=True)
bpy.context.preferences.filepaths.save_version=0
bpy.ops.wm.save_as_mainfile(filepath=str((a.output/'HumanForms.blend').resolve()),compress=True)
(a.output/'forms.json').write_text(json.dumps(receipt,ensure_ascii=False,indent=2)+'\n')
print('HUMAN_FORMS_AUTHORED forms=18 hair_styles=2 common_topology=true rigged=false',flush=True)
if not a.render:raise SystemExit(0)
letter=material('Study labels',(.72,.78,.77))
# Technical comparison boards only. Neutral materials reveal actual form changes;
# labels explicitly distinguish them from finished character or wardrobe art.
for obj in [body]+[v[0] for v in styles.values()]:obj.hide_render=True
scene=bpy.context.scene;scene.render.engine='CYCLES';scene.cycles.samples=20;scene.world.color=(.13,.13,.13)
scene.render.resolution_x=2400;scene.render.resolution_y=1400;scene.render.resolution_percentage=100
for name,position,power,size in [('Key',(-3,-6,7),900,5),('Fill',(5,-3,4),500,4),('Rim',(0,3,6),1000,3)]:
 data=bpy.data.lights.new(name,'AREA');data.energy=power;data.shape='DISK';data.size=size;obj=bpy.data.objects.new(name,data);scene.collection.objects.link(obj);obj.location=position;obj.rotation_euler=(Vector((0,0,2))-obj.location).to_track_quat('-Z','Y').to_euler()
camdata=bpy.data.cameras.new('Forms comparison');cam=bpy.data.objects.new('Forms comparison',camdata);scene.collection.objects.link(cam);scene.camera=cam;camdata.type='ORTHO'
def label(text,x,z,size=.08):
 data=bpy.data.curves.new(text,'FONT');data.body=text;data.align_x='CENTER';data.size=size;data.materials.append(letter);obj=bpy.data.objects.new(text,data);scene.collection.objects.link(obj);obj.location=(x,-.35,z);obj.rotation_euler=(math.pi/2,0,0);return obj
preview=[]
def clear():
 for obj in preview:bpy.data.objects.remove(obj,do_unlink=True)
 preview.clear()
def render(name,center,scale):
 cam.location=(0,-12,center);cam.rotation_euler=(math.pi/2,0,0);camdata.ortho_scale=scale;scene.render.filepath=str((a.output/(name+'.png')).resolve());bpy.ops.render.render(write_still=True)
for idx,form in enumerate(forms):
 col=(idx//9)*3+idx%3;row=(idx%9)//3;x=(col-2.5)*1.25;z=(2-row)*2.12
 obj=mesh_object(form.key,[allcoords[form.key][i] for i in ids],localfaces,clay);obj.location=(x,0,z);preview.append(obj);preview.append(label(form.key.replace('_',' '),x,z-.12))
render('body-forms',3.05,11.2);clear()
head_faces=[f for f in groups['body']+groups['helper-l-eye']+groups['helper-r-eye'] if all(base[i][2]>1.51 for i in f)]
head_ids=sorted(set(i for f in head_faces for i in f));head_map={v:i for i,v in enumerate(head_ids)};head_polys=[tuple(head_map[i] for i in f) for f in head_faces]
for idx,form in enumerate(forms):
 col=(idx//9)*3+idx%3;row=(idx%9)//3;x=(col-2.5)*.44;z=(2-row)*.49
 obj=mesh_object(form.key+' head',[allcoords[form.key][i] for i in head_ids],head_polys,clay);obj.location=(x,0,z-1.51);preview.append(obj);preview.append(label(form.key.replace('_',' '),x,z-.06,.028))
render('face-forms',.64,2.7);clear()
for idx,form in enumerate(f for f in forms if f.build=='balanced'):
 for row,(name,(_,selected,polys)) in enumerate(styles.items()):
  x=(idx-2.5)*.44;z=(1-row)*.7
  obj=mesh_object(form.key+' head',[allcoords[form.key][i] for i in head_ids],head_polys,clay);obj.location=(x,0,z-1.51);preview.append(obj)
  obj=mesh_object(name+' fitted',[hair_forms[form.key][i] for i in selected],polys,dark);obj.location=(x,0,z-1.51);preview.append(obj);preview.append(label(form.key.replace('_balanced','')+' '+name,x,z-.35,.023))
render('hair-forms',.35,2.75)
print('HUMAN_FORMS_BOARDS_RENDERED count=3',flush=True)
