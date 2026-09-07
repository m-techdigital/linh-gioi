"""Rebuild player Head using a dedicated face atlas; keep shared body atlas and four outfit slots exact."""
import argparse,bpy,bmesh,hashlib,json,math,sys
sys.dont_write_bytecode=True
from pathlib import Path
from mathutils import Vector
import numpy as np

def rebuild_head(o,rig,root,face_path=None):
 R=Path(root);rest={b.name:b.matrix_local.copy() for b in rig.data.bones}
 for bone in rig.pose.bones:bone.matrix_basis.identity()
 bpy.context.view_layer.update()
 face_path=Path(face_path) if face_path else R/'client/art-source/arrival-scene/ArrivalFace.png'
 if not face_path.is_file():raise FileNotFoundError('Dedicated player face atlas required: '+str(face_path))
 if bpy.data.images.get('ArrivalFace'):bpy.data.images['ArrivalFace'].name='ArrivalFacePrevious'
 face_image=bpy.data.images.load(str(face_path),check_existing=False);face_image.name='ArrivalFace';face_image.pack()
 face_material=bpy.data.materials['Keeper Reconstruction Face']
 for node in face_material.node_tree.nodes:
  if node.type=='TEX_IMAGE':node.image=face_image

 slots={v['key']:int(k) for k,v in json.loads((R/'client/art-source/appearance-slots.json').read_text()).items()};head=slots['Head']
 def pixels(im):return hashlib.sha256(np.asarray(im.pixels[:],dtype=np.float32).tobytes()).hexdigest()
 image_hashes={im.name:pixels(im) for im in bpy.data.images if im.name in ['KeeperReconstructionAlbedo','KeeperReconstructionFace']}
 def other_hash(obj):
  rows=[];slot=obj.data.attributes['appearance_slot'];uv=obj.data.uv_layers.active
  for poly in obj.data.polygons:
   if slot.data[poly.index].value in [head,slots['UpperBody'],slots['Hair']]:continue
   rows.append((slot.data[poly.index].value,[(tuple(obj.data.vertices[obj.data.loops[li].vertex_index].co),tuple(uv.data[li].uv)) for li in poly.loop_indices]))
  return hashlib.sha256(repr(rows).encode()).hexdigest()
 import importlib.util
 neck_spec=importlib.util.spec_from_file_location('lgo_player_neck',R/'tools/art/finish_player_neck.py');neck_helper=importlib.util.module_from_spec(neck_spec);neck_spec.loader.exec_module(neck_helper);neck_helper.remove_lining(o)
 untouched=other_hash(o)
 def upperbody_hash(obj):
  rows=[];tag=obj.data.attributes['appearance_slot'];uv=obj.data.uv_layers.active
  for poly in obj.data.polygons:
   if tag.data[poly.index].value!=slots['UpperBody']:continue
   corners=[]
   for li in poly.loop_indices:
    v=obj.data.vertices[obj.data.loops[li].vertex_index]
    corners.append((tuple(v.co),tuple(uv.data[li].uv),sorted((obj.vertex_groups[g.group].name,g.weight) for g in v.groups)))
   rows.append((obj.data.materials[poly.material_index].name,corners))
  return hashlib.sha256(repr(rows).encode()).hexdigest()
 original_positions=[v.co.copy() for v in o.data.vertices]
 # Refit the neck-opening boundary and any interior rim vertices projecting above it.
 # This removes the original front tab/inner rim teeth instead of covering them.
 cm=bmesh.new();cm.from_mesh(o.data);ct=cm.faces.layers.int.get('appearance_slot')
 collar=[v for v in cm.verts if v.co.z>1.50 and abs(v.co.x)<.13 and abs(v.co.y)<.16 and v.link_faces and all(f[ct]==slots['UpperBody'] for f in v.link_faces) and (v.co.z>1.555 or any(e.is_boundary for e in v.link_edges))]
 collar_count=len(collar)
 collar_weights={}
 for v in collar:
  for g in o.data.vertices[v.index].groups:
   name=o.vertex_groups[g.group].name;collar_weights[name]=collar_weights.get(name,0)+g.weight/collar_count
 assert 3<=collar_count<250,collar_count
 collar_indices={v.index for v in collar}
 for v in collar:v.co.z=1.555
 cm.to_mesh(o.data);cm.free()
 assert all(v.index in collar_indices or v.co==original_positions[v.index] for v in o.data.vertices)
 upperbody_expected=upperbody_hash(o)

 # Dedicated generated atlas: normalized face center .75, skin on left half.
 # Front landmarks are mapped explicitly after inspecting the actual generated image.
 skin_uv=(.25,.50)
 bm=bmesh.new();bm.from_mesh(o.data);tag=bm.faces.layers.int.get('appearance_slot');bmesh.ops.delete(bm,geom=[f for f in bm.faces if f[tag]==head],context='FACES');bmesh.ops.delete(bm,geom=[v for v in bm.verts if not v.link_faces],context='VERTS');bm.to_mesh(o.data);bm.free()
 verts=[];faces=[];uvs=[]
 # One anatomically continuous ring surface; front and back connect through cheeks/jaw.
 levels=np.array([1.48,1.54,1.575,1.595,1.62,1.65,1.68,1.71,1.74,1.765,1.780])
 widths=np.array([.030,.031,.031,.030,.036,.055,.066,.068,.060,.037,.001])
 fronts=np.array([-.013,-.012,-.023,-.035,-.074,-.084,-.080,-.080,-.064,-.024,.026])
 backs=np.array([.067,.072,.076,.085,.093,.101,.111,.112,.100,.070,.027])
 # Cubic Hermite interpolation gives smooth jaw/forehead transitions without ring steps.
 def value(table,z):
  i=max(0,min(len(levels)-2,int(np.searchsorted(levels,z)-1)));h=levels[i+1]-levels[i];t=(z-levels[i])/h
  m0=(table[min(i+1,len(table)-1)]-table[max(i-1,0)])/(levels[min(i+1,len(table)-1)]-levels[max(i-1,0)])
  m1=(table[min(i+2,len(table)-1)]-table[i])/(levels[min(i+2,len(table)-1)]-levels[i])
  return (2*t**3-3*t*t+1)*table[i]+(t**3-2*t*t+t)*h*m0+(-2*t**3+3*t*t)*table[i+1]+(t**3-t*t)*h*m1
 nu,nv=96,100
 for j in range(nv+1):
  z=levels[0]+(levels[-1]-levels[0])*j/nv;w=value(widths,z);fy=value(fronts,z);by=value(backs,z);cy=(fy+by)/2;depth=(by-fy)/2
  for i in range(nu):
   angle=math.tau*i/nu;c=math.cos(angle);x=w*math.sin(angle);y=cy-depth*c
   if c>0:
    # Soft nose bridge/tip, eye sockets, lips and chin preserve measured front landmarks.
    nose=.019*math.exp(-((z-1.674)/.012)**2)+.008*math.exp(-((z-1.691)/.026)**2)
    y-=nose*c**22
    y+=.0035*math.exp(-((z-1.710)/.009)**2)*(math.exp(-((x-.028)/.013)**2)+math.exp(-((x+.028)/.013)**2))*c
    y-=.003*math.exp(-((z-1.655)/.008)**2)*c**8
   verts.append((x,y,z))
 for j in range(nv):
  for i in range(nu):
   ni=(i+1)%nu;indices=(j*nu+i,j*nu+ni,(j+1)*nu+ni,(j+1)*nu+i)
   # UV boundary follows lateral cheek, hidden under side locks; avoids stretching eyes onto ears.
   center=sum((Vector(verts[k]) for k in indices),Vector())/4
   frontface=math.cos(math.tau*(i+.5)/nu)>0
   def continuous_uv(k):
    x,y,z=verts[k]
    vv=float(np.interp(z,[1.48,1.54,1.60,1.62,1.655,1.674,1.710,1.780],[.03,.07,.20,.30,.435,.53,.670,.98]))
    uu=max(.590,min(.910,.750+x*2.65)) if frontface else max(.590,min(.910,.750+(1 if math.sin(math.tau*(i+.5)/nu)>0 else -1)*value(widths,z)*2.65))
    return (uu,vv)
   for tri in [(indices[0],indices[1],indices[2]),(indices[0],indices[2],indices[3])]:
    faces.append(tri);uvs.append([continuous_uv(k) for k in tri])
 # Neck base and crown are closed, including under the hair.
 for ring in [0,nv*nu]:
  center=len(verts);verts.append(tuple(sum((Vector(verts[ring+i]) for i in range(nu)),Vector())/nu))
  for i in range(nu):faces.append((center,ring+i,ring+(i+1)%nu));uvs.append([skin_uv]*3)
 # Small rounded ears sit between the brow and nose height, close to the skull.
 for side in [-1,1]:
  offset=len(verts);eu,ev=24,18
  for j in range(ev+1):
   theta=.008+(math.pi-.016)*j/ev
   for i in range(eu):
    angle=math.tau*i/eu;s=math.sin(theta);x=side*(.065+.007*s*math.cos(angle));y=.008+.011*s*math.sin(angle);z=1.684+.027*math.cos(theta)
    verts.append((x,y,z))
  for j in range(ev):
   for i in range(eu):
    q=(offset+j*eu+i,offset+j*eu+(i+1)%eu,offset+(j+1)*eu+(i+1)%eu,offset+(j+1)*eu+i)
    for tri in [(q[0],q[1],q[2]),(q[0],q[2],q[3])]:
     faces.append(tri);uvs.append([((.945 if side>0 else .548)+(verts[k][1]-.008)/.011*.024,.590+(verts[k][2]-1.684)/.027*.084) for k in tri])
  for ring in [offset,offset+ev*eu]:
   center=len(verts);verts.append(tuple(sum((Vector(verts[ring+i]) for i in range(eu)),Vector())/eu))
   for i in range(eu):faces.append((center,ring+i,ring+(i+1)%eu));uvs.append([skin_uv]*3)
 data=bpy.data.meshes.new('Continuous player head anatomy');data.from_pydata(verts,[],faces);data.update();new=bpy.data.objects.new('Continuous player head',data);bpy.context.collection.objects.link(new)
 data.materials.append(bpy.data.materials['Keeper Reconstruction Face']);sa=data.attributes.new('appearance_slot','INT','FACE');coarse=data.attributes.new('npc_part','INT','FACE');uv=data.uv_layers.new(name='UVMap')
 for poly,values in zip(data.polygons,uvs):
  poly.use_smooth=True;sa.data[poly.index].value=head;coarse.data[poly.index].value=2
  for li,val in zip(poly.loop_indices,values):uv.data[li].uv=val
 bm=bmesh.new();bm.from_mesh(data);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(data);bm.free()
 for b in rig.data.bones:new.vertex_groups.new(name=b.name)
 head_name=next(b.name for b in rig.data.bones if b.name.lower()=='head')
 for v in data.vertices:
  blend=max(0,min(1,(v.co.z-1.54)/.095));blend=blend*blend*(3-2*blend)
  ws={n:w*(1-blend) for n,w in collar_weights.items()};ws[head_name]=ws.get(head_name,0)+blend
  entries=sorted([(n,w) for n,w in ws.items() if w>1e-7],key=lambda item:-item[1])[:4];total=sum(w for _,w in entries)
  for n,w in entries:new.vertex_groups[n].add([v.index],w/total,'REPLACE')
 bpy.ops.object.select_all(action='DESELECT');o.select_set(True);new.select_set(True);bpy.context.view_layer.objects.active=o;bpy.ops.object.join()
 import importlib.util
 spec=importlib.util.spec_from_file_location('lgo_player_hair',R/'tools/art/rebuild_player_hair.py');hair_helper=importlib.util.module_from_spec(spec);spec.loader.exec_module(hair_helper);o=hair_helper.rebuild_hair(o,rig,lambda z:(value(widths,z),value(fronts,z),value(backs,z)))
 old=list(o.data.materials);canonical=[bpy.data.materials[n] for n in ['Keeper Reconstruction','Keeper Reconstruction Face']];indices=[canonical.index(old[p.material_index]) for p in o.data.polygons];o.data.materials.clear()
 for m in canonical:o.data.materials.append(m)
 for poly,index in zip(o.data.polygons,indices):poly.material_index=index
 assert other_hash(o)==untouched
 assert upperbody_hash(o)==upperbody_expected
 neck_info=neck_helper.finish_neck(o,rig)
 assert all(pixels(bpy.data.images[n])==h for n,h in image_hashes.items())
 assert len(rig.data.bones)==65 and all(rig.data.bones[n].matrix_local==m for n,m in rest.items())
 assert set(e.value for e in o.data.attributes['appearance_slot'].data)=={2,3,10,11,13,14,15}
 receipt={'triangles':len(o.data.polygons),'vertices':len(o.data.vertices),'rest_bones':65,'materials':2,'four_other_slots_geometry_uv_exact':True,'upperbody_neck_rim_vertices':collar_count,'body_atlas_pixels_exact':True,'dedicated_face_image':str(face_path),'runtime_verified':False}
 receipt.update(neck_info)
 return o,receipt

def main(argv=None):
 p=argparse.ArgumentParser(description=__doc__);p.add_argument('--project-root',type=Path,default=Path.cwd());p.add_argument('--source',type=Path);p.add_argument('--output-dir',type=Path);p.add_argument('--face-texture',type=Path);p.add_argument('--views',default='head-front,head-side,head-back,head-quarter,player-full')
 a=p.parse_args(argv if argv is not None else (sys.argv[sys.argv.index('--')+1:] if '--' in sys.argv else []))
 R=a.project_root.resolve();O=a.output_dir or R/'build/asset-staging/player-head-macro';O.mkdir(parents=True,exist_ok=True)
 bpy.ops.wm.open_mainfile(filepath=str(a.source or R/'client/art-source/arrival-scene/ArrivalScene.blend'))
 o=bpy.data.objects['Arrival Scene Reconstruction'];rig=bpy.data.objects['Armature']
 o,receipt=rebuild_head(o,rig,R,a.face_texture)
 bpy.ops.wm.save_as_mainfile(filepath=str(O/'PlayerHeadMacro.blend'),compress=True)
 scene=bpy.context.scene;scene.render.engine='CYCLES';scene.cycles.samples=32;scene.render.resolution_x=1000;scene.render.resolution_y=1000;scene.render.resolution_percentage=100;scene.render.film_transparent=False;cam=scene.camera;cam.data.type='ORTHO'
 for name,loc,full in [('head-front',(0,-4,1.66),False),('head-side',(4,0,1.66),False),('head-back',(0,4,1.66),False),('head-quarter',(2,-4,1.66),False),('player-full',(0,-5,1),True)]:
  if name not in a.views.split(','):continue
  for side,angle in [('l',-70),('r',70)]:rig.pose.bones['upperarm_'+side].rotation_euler.z=math.radians(angle)
  target=Vector((0,0,1 if full else 1.65));cam.location=loc;cam.rotation_euler=(target-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.ortho_scale=2.05 if full else .40;scene.render.filepath=str(O/(name+'.png'));bpy.ops.render.render(write_still=True)

 source_path=a.source or R/'client/art-source/arrival-scene/ArrivalScene.blend';face_path=a.face_texture or R/'client/art-source/arrival-scene/ArrivalFace.png'
 receipt.update({'source':str(source_path.relative_to(R)) if source_path.is_absolute() else str(source_path),'source_sha256':hashlib.sha256(source_path.read_bytes()).hexdigest(),'face':str(face_path.relative_to(R)) if face_path.is_absolute() else str(face_path),'face_sha256':hashlib.sha256(face_path.read_bytes()).hexdigest(),'blender':bpy.app.version_string,'helpers_sha256':{str(path.relative_to(R)):hashlib.sha256(path.read_bytes()).hexdigest() for path in [R/'tools/art/rebuild_player_head.py',R/'tools/art/rebuild_player_hair.py',R/'tools/art/finish_player_neck.py']}})
 (O/'head-receipt.json').write_text(json.dumps(receipt,indent=2));print('PLAYER_HEAD_MACRO',json.dumps(receipt),flush=True)

if __name__=='__main__':main()
