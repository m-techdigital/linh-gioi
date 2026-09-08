"""Replace anatomical toe surfaces with a single closed leather boot last per foot."""
import bpy,bmesh,argparse,sys,math
from pathlib import Path
p=argparse.ArgumentParser();p.add_argument('--source',type=Path,required=True);p.add_argument('--output',type=Path,required=True)
a=p.parse_args(sys.argv[sys.argv.index('--')+1:]);bpy.ops.wm.open_mainfile(filepath=str(a.source.resolve()))
o=next(o for o in bpy.data.objects if o.type=='MESH' and o.name in ('Keeper Macro Reconstruction','Arrival Scene Reconstruction'))
assert not o.get('lgo_boot_last_finished'), 'Use an uncapped authoring source; boot last is already present.'
slot=o.data.attributes['appearance_slot'];parts=[];remove=[]
for side,sign in [('l',1),('r',-1)]:
 faces=[f for f in o.data.polygons if slot.data[f.index].value==14 and f.center.x*sign>0 and max(o.data.vertices[i].co.z for i in f.vertices)<.17]
 remove.extend(f.index for f in faces if max(o.data.vertices[i].co.z for i in f.vertices)<.12)
 # A boot last needs a concave instep; a convex hull turns it into a wedge.
 sections=[(-.15,.018,.018,.040),(-.13,.044,.010,.058),(-.08,.057,.008,.075),(-.03,.053,.008,.097),(.02,.047,.008,.142),(.065,.043,.008,.174),(.105,.034,.008,.145),(.13,.023,.010,.096),(.14,.008,.025,.060)]
 verts=[];polys=[];segments=24
 for y,width,bottom,top in sections:
  for j in range(segments):
   angle=j*2*math.pi/segments
   verts.append((sign*.0983+width*math.sin(angle),y,bottom+(top-bottom)*(.5+.5*math.cos(angle))))
 for k in range(len(sections)-1):
  for j in range(segments):
   q=(j+1)%segments;polys.append((k*segments+j,k*segments+q,(k+1)*segments+q,(k+1)*segments+j))
 polys.extend([tuple(reversed(range(segments))),tuple((len(sections)-1)*segments+j for j in range(segments))])
 mesh=bpy.data.meshes.new('Continuous boot last '+side);mesh.from_pydata(verts,[],polys);mesh.update()
 bm=bmesh.new();bm.from_mesh(mesh);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(mesh);bm.free()
 cap=bpy.data.objects.new(mesh.name,mesh);bpy.context.collection.objects.link(cap);mesh.materials.append(o.data.materials[0]);uv=mesh.uv_layers.new(name='UVMap')
 for f in mesh.polygons:
  f.use_smooth=f.normal.z>-.8
  for li in f.loop_indices:
   co=mesh.vertices[mesh.loops[li].vertex_index].co;u=max(.05,min(.95,.5+(co.x-sign*.0983)/.15));v=.035+.06*max(0,min(1,co.z/.17))
   uv.data[li].uv=((32.5+u*511)/4096,(32.5+v*511)/2048)
 for name,value in [('appearance_slot',14),('npc_part',3)]:
  layer=mesh.attributes.new(name,'INT','FACE')
  for item in layer.data:item.value=value
 foot=cap.vertex_groups.new(name='foot_'+side);calf=cap.vertex_groups.new(name='calf_'+side)
 for v in mesh.vertices:
  t=max(0,min(1,(v.co.z-.12)/.15));t=t*t*(3-2*t)
  foot.add([v.index],1-t,'REPLACE')
  if t:calf.add([v.index],t,'REPLACE')
 parts.append(cap)
bm=bmesh.new();bm.from_mesh(o.data);bm.faces.ensure_lookup_table();bmesh.ops.delete(bm,geom=[bm.faces[i] for i in remove],context='FACES');bm.to_mesh(o.data);bm.free()
bpy.ops.object.select_all(action='DESELECT');o.select_set(True)
for cap in parts:cap.select_set(True)
bpy.context.view_layer.objects.active=o;bpy.ops.object.join();o['lgo_boot_last_finished']=True
bpy.context.preferences.filepaths.save_version=0
bpy.ops.wm.save_as_mainfile(filepath=str(a.output.resolve()),compress=True)
print('LGO_BOOT_LAST_READY modules_unchanged=true shared_material=true')
