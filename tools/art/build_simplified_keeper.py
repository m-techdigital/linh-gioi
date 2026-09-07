"""One-pass reference wardrobe: clean anatomy, four large cloth/armor forms, standard semantic slots."""
import bpy,bmesh,math,json,argparse,sys,numpy as np
from pathlib import Path
from mathutils import Vector,Matrix,kdtree
parser=argparse.ArgumentParser(description=__doc__)
parser.add_argument('--project-root',type=Path,default=Path.cwd())
parser.add_argument('--source',type=Path)
parser.add_argument('--base-anatomy',type=Path)
parser.add_argument('--front',type=Path)
parser.add_argument('--back',type=Path)
parser.add_argument('--output-dir',type=Path)
args=parser.parse_args(sys.argv[sys.argv.index('--')+1:] if '--' in sys.argv else [])
R=args.project_root.resolve();O=args.output_dir or R/'build/asset-staging/keeper-quality-recovery';P=R/'build/asset-staging/gate-keeper-v4/reconstruction-inputs'
O.mkdir(parents=True,exist_ok=True)
source_path=args.source or R/'client/art-source/gate-keeper/KeeperReconstruction.blend'
base_path=args.base_anatomy or R/'build/asset-staging/quaternius-base/arrival-outfit-study.blend'
front_path=args.front or P/'trellis-downloads/cc627f78e44067c5ecca2dffd3db68e64a5d438e0711a6333aacf5f3dde4d1f7/image.png'
back_path=args.back or P/'back-color.png'
SLOTS={entry['key']:int(slot_id) for slot_id,entry in json.loads((R/'client/art-source/appearance-slots.json').read_text()).items()}
# Current retained head/face/headwear and fixed shared skeleton.
bpy.ops.wm.open_mainfile(filepath=str(source_path))
retained=bpy.data.objects['Keeper Macro Reconstruction'];rig=bpy.data.objects['Armature'];rest={b.name:b.matrix_local.copy() for b in rig.data.bones}
for b in rig.pose.bones:b.matrix_basis.identity()
for side,a in [('l',-60),('r',60)]:rig.pose.bones['upperarm_'+side].rotation_mode='XYZ';rig.pose.bones['upperarm_'+side].rotation_euler.z=math.radians(a)
bpy.context.view_layer.update();ev=retained.evaluated_get(bpy.context.evaluated_depsgraph_get());coords=[v.co.copy() for v in ev.data.vertices]
for v,c in zip(retained.data.vertices,coords):v.co=c
for mod in list(retained.modifiers):retained.modifiers.remove(mod)
bm=bmesh.new();bm.from_mesh(retained.data);coarse=bm.faces.layers.int.get('npc_part');slot=bm.faces.layers.int.get('appearance_slot') or bm.faces.layers.int.new('appearance_slot')
bmesh.ops.delete(bm,geom=[f for f in bm.faces if f[coarse]==3],context='FACES')
for f in bm.faces:f[slot]=1 if f[coarse]==1 else (2 if f.material_index==1 else 3)
bmesh.ops.delete(bm,geom=[v for v in bm.verts if not v.link_faces],context='VERTS');bm.to_mesh(retained.data);bm.free()
sourceimage=bpy.data.images['KeeperReconstructionAlbedo'];iw,ih=sourceimage.size;retainedpixels=np.array(sourceimage.pixels[:],dtype=np.float32).reshape(ih,iw,4)[:2048,:2048].copy()
assert retainedpixels.shape==(2048,2048,4)
# Shared authoring helpers: explicit slot, smooth continuous weights, closed cloth thickness.
def smooth(a,b,x):
 t=max(0,min(1,(x-a)/(b-a)));return t*t*(3-2*t)
def normalize(ws):
 values=sorted([(n,w) for n,w in ws.items() if w>1e-7],key=lambda x:-x[1])[:4];total=sum(w for _,w in values);return {n:w/total for n,w in values}
def segment_distance(p,a,b):
 d=b-a;t=max(0,min(1,(p-a).dot(d)/d.length_squared));return (p-a-d*t).length
def capsules(p,names):
 return normalize({n:1/(.012+segment_distance(p,rig.pose.bones[n].head,rig.pose.bones[n].tail))**4 for n in names})
def torso_weights(p):
 upper=smooth(1.0,1.43,p.z);return normalize({'pelvis':1-upper,'spine_03':upper})
def boot_weights(p,side):
 calf=smooth(.12,.27,p.z);return normalize({'foot_'+side:1-calf,'calf_'+side:calf})
def apply_weights(obj,weights):
 for b in rig.data.bones:obj.vertex_groups.new(name=b.name)
 for i,ws in enumerate(weights):
  for name,w in normalize(ws).items():obj.vertex_groups[name].add([i],w,'REPLACE')
def part_mesh(name,vertices,faces,slots,weights,thickness=0):
 data=bpy.data.meshes.new(name);data.from_pydata(vertices,[],faces);data.update();ob=bpy.data.objects.new(name,data);bpy.context.collection.objects.link(ob)
 coarse=data.attributes.new('npc_part','INT','FACE');slot=data.attributes.new('appearance_slot','INT','FACE')
 for p,s in zip(data.polygons,slots):p.use_smooth=True;coarse.data[p.index].value=3;slot.data[p.index].value=s
 apply_weights(ob,weights)
 if thickness:
  bpy.ops.object.select_all(action='DESELECT');ob.select_set(True);bpy.context.view_layer.objects.active=ob;mod=ob.modifiers.new('Continuous cloth thickness','SOLIDIFY');mod.thickness=thickness;mod.offset=-1;bpy.ops.object.modifier_apply(modifier=mod.name)
 return ob
# Clean closed anatomical mesh from the licensed base, mapped to the exact current65 rest bones.
with bpy.data.libraries.load(str(base_path),link=False) as (src,dst):dst.objects=['SuperHero_Male','Armature']
base=next(o for o in dst.objects if o.type=='MESH');base_rig=next(o for o in dst.objects if o.type=='ARMATURE')
base_rest={b.name:b.matrix_local.copy() for b in base_rig.data.bones};posed={b.name:rig.pose.bones[b.name].matrix@b.matrix_local.inverted() for b in rig.data.bones}
sourcews=[normalize({base.vertex_groups[g.group].name:g.weight for g in v.groups}) for v in base.data.vertices];baseco=[];classes=[];bodyweights=[]
for v,ws in zip(base.data.vertices,sourcews):
 mapmatrix=Matrix([[0.0]*4 for _ in range(4)])
 for n,w in ws.items():mapmatrix+=rig.pose.bones[n].matrix@base_rest[n].inverted()*w
 p=mapmatrix@v.co;side='l' if p.x>=0 else 'r';names=set(ws);arm=sum(w for n,w in ws.items() if any(k in n for k in ['arm','hand','thumb','index','middle','ring','pinky']));leg=sum(w for n,w in ws.items() if any(k in n for k in ['thigh','calf','foot','ball']))
 hand=sum(w for n,w in ws.items() if any(k in n for k in ['hand','thumb','index','middle','ring','pinky']))
 if hand>.5:
  s=13;dominant=max(ws,key=ws.get);chain=[n for n in rig.pose.bones.keys() if n.endswith('_'+side) and (n.startswith(dominant.split('_')[0]+'_') if dominant.startswith(('thumb','index','middle','ring','pinky')) else n=='hand_'+side)]
  chain=list(set(chain+['hand_'+side]));weights=capsules(p,chain)
 elif arm>.35:
  s=10;weights=capsules(p,['upperarm_'+side,'lowerarm_'+side,'clavicle_'+side])
  # White sleeve volume grows around its own axis, preserving a continuous wrist transition.
  upper=rig.pose.bones['upperarm_'+side];lower=rig.pose.bones['lowerarm_'+side];bone=min([upper,lower],key=lambda b:segment_distance(p,b.head,b.tail));d=bone.tail-bone.head;t=max(0,min(1,(p-bone.head).dot(d)/d.length_squared));axis=bone.head+d*t
  inflation=.23*smooth(1.07,1.24,p.z)*(1-smooth(1.46,1.53,p.z));p=axis+(p-axis)*(1+inflation)
 elif leg>.35:
  if p.z<.56:s=14;weights=boot_weights(p,side)
  else:s=11;hip=smooth(.88,1.025,p.z);weights=normalize({'thigh_'+side:1-hip,'pelvis':hip})
  if s==14 and p.z>.14:
   bone=rig.pose.bones['calf_'+side];d=bone.tail-bone.head;t=max(0,min(1,(p-bone.head).dot(d)/d.length_squared));axis=bone.head+d*t;p=axis+(p-axis)*1.06
 else:s=10;weights=torso_weights(p)
 baseco.append(p);classes.append(s);bodyweights.append(weights)
faces=[];slots=[]
for f in base.data.polygons:
 center=sum((baseco[i] for i in f.vertices),Vector())/len(f.vertices)
 if center.z>1.57:continue
 fs=list(f.vertices);faces.append(fs);slots.append(max(set(classes[i] for i in fs),key=lambda s:sum(classes[i]==s for i in fs)))
# Compact unused head vertices before constructing the wardrobe mesh.
used=sorted({i for f in faces for i in f});remap={old:new for new,old in enumerate(used)}
body=part_mesh('Clean anatomical wardrobe',[baseco[i] for i in used],[[remap[i] for i in f] for f in faces],slots,[bodyweights[i] for i in used])
bpy.data.objects.remove(base,do_unlink=True);bpy.data.objects.remove(base_rig,do_unlink=True)
# A single wrapped front robe, attached beneath tunic/belt, with one deliberate opening and broad folds.
def grid_part(name,nu,nv,fn,slot,weightfn,reverse=False,thickness=.004):
 vs=[fn(i/nu,j/nv) for j in range(nv+1) for i in range(nu+1)];fs=[]
 for j in range(nv):
  for i in range(nu):
   a=j*(nu+1)+i;f=(a,a+1,a+nu+2,a+nu+1);fs.append(tuple(reversed(f)) if reverse else f)
 return part_mesh(name,vs,fs,[slot]*len(fs),[weightfn(p) for p in vs],thickness)
def robe(u,t):
 a=.075+(math.tau-.15)*u;rx=.176+.105*t;ry=.136+.075*t;fold=.009*math.cos(a*8)*smooth(0,.5,t)
 bottom=.38+.11*math.sin(a)**2+.035*(1-math.cos(a));z=1.09*(1-t)+bottom*t
 return Vector(((rx+fold)*math.sin(a),.026-(ry+fold)*math.cos(a),z))
robe_obj=grid_part('One wrapped ivory navy robe',64,30,robe,10,lambda p:{'pelvis':1},True)
# One continuous cape: a few long folds and three intentional pointed hem peaks.
def cape(u,t):
 q=2*u-1;width=.165+.29*t**.85;bottom=.32-.05*math.cos(q*math.pi*2)-.04*abs(q)
 return Vector((width*q,.165+.16*t-.20*q*q*t+.012*math.cos(q*math.pi*4)*math.sin(math.pi*t),1.49*(1-t)+bottom*t))
cape_obj=grid_part('One continuous navy cape',64,44,cape,12,torso_weights,False)
# Two clean shoulder shells. Their relief is one large curved form; ornamental linework stays in texture.
shoulders=[]
for side,sign in [('l',1),('r',-1)]:
 vs=[];fs=[];n=32;rows=10
 for j in range(rows+1):
  phi=(.07+j/rows*math.pi*.53)
  for i in range(n):
   a=i/n*math.tau;x=sign*(.205+.108*math.sin(phi)*math.cos(a));y=.061+.115*math.sin(phi)*math.sin(a);z=1.468+.075*math.cos(phi)+.014*math.sin(phi)*max(0,math.cos(a));vs.append(Vector((x,y,z)))
 for j in range(rows):
  for i in range(n):a=j*n+i;b=j*n+(i+1)%n;f=(a,b,b+n,a+n);fs.append(tuple(reversed(f)) if sign>0 else f)
 weights=[{'clavicle_'+side:.40,'upperarm_'+side:.60} for p in vs]
 shoulders.append(part_mesh('Clean shoulder shell '+side,vs,fs,[15]*len(fs),weights,.006))
# Region-specific projections from clean standalone modeling inputs, never the damaged reconstructed donor atlas.
front=bpy.data.images.load(str(front_path),check_existing=False);front.name='Clean keeper front modeling input';front.pack()
back=bpy.data.images.load(str(back_path),check_existing=False);back.name='Clean keeper rear modeling input';back.pack()
def projection_lookup(img,is_front):
 w,h=img.size;px=np.array(img.pixels[:],dtype=np.float32).reshape(h,w,4);rgb=px[:,:,:3]
 valid=(rgb.max(axis=2)>.018) if is_front else ((rgb.max(axis=2)-rgb.min(axis=2)>.035)|(rgb.max(axis=2)<.22)|(rgb.min(axis=2)>.64))
 yy,xx=np.where(valid[::2,::2]);tree=kdtree.KDTree(len(xx))
 for i,(x,y) in enumerate(zip(xx*2,yy*2)):tree.insert((float(x),float(y),0),i)
 tree.balance()
 def uv(x,y_top):
  x=max(0,min(w-1,x));y=max(0,min(h-1,h-1-y_top));ix=int(x);iy=int(y)
  if not valid[iy,ix]:nearest,_,_=tree.find((x,y,0));x=nearest.x;y=nearest.y
  return ((x+.5)/w,(y+.5)/h)
 return uv
frontuv=projection_lookup(front,True);backuv=projection_lookup(back,False)
def region_uv(p,slot,rear=False):
 x,y,z=p;sign=1 if x>=0 else -1;ax=abs(x)
 if slot==12:
  # Match the image's taper and pointed hem smoothly; never snap vertices
  # independently across foreground/background boundaries.
  t=max(0,min(1,(1.49-z)/1.22));q=max(-1,min(1,x/(.165+.29*t**.85)))
  bottom=1200-150*math.sin(math.pi*abs(q))**2
  row=400+t*(bottom-400)
  width=float(np.interp(row,[400,500,650,800,950,1100,1200],[115,148,195,265,350,407,385]))
  return ((543+q*width+.5)/back.size[0],1-(row+.5)/back.size[1])
 if slot==15:
  return backuv(543+sign*(150+(ax-.205)*520),395-(z-1.43)*720) if rear else frontuv(486+sign*(107+(ax-.205)*450),260-(z-1.43)*610)
 if slot==14:
  return backuv(543+sign*(123+(ax-.0983)*610),1400-z*525) if rear else frontuv(486+sign*(98+(ax-.0983)*515),965-z*530)
 if slot==13 or (slot==10 and ax>.205 and z>1.02):
  t=(1.455-z)/.455;center=.1823+.2573*t;offset=(ax-center)*470
  return backuv(543+sign*(195+155*t+offset*1.1),420+345*t) if rear else frontuv(486+sign*(109+110*t+offset),260+230*t)
 if slot==11:
  return backuv(543+sign*(116+(ax-.10)*500),1250-z*250) if rear else frontuv(486+sign*(85+(ax-.10)*440),800-(z-.5)*310)
 if z<1.08:
  return backuv(543+x*750,720+(1.08-z)*760) if rear else frontuv(486+x*560,400+(1.09-z)*500)
 return backuv(543+x*600,410+(1.50-z)*650) if rear else frontuv(486+x*420,400-(z-1.09)*440)
wardrobe=[body,robe_obj,cape_obj]+shoulders
def projection_material(name,img):
 mat=bpy.data.materials.new(name);mat.use_nodes=True;nodes=mat.node_tree.nodes;links=mat.node_tree.links
 tex=nodes.new('ShaderNodeTexImage');tex.image=img;uv=nodes.new('ShaderNodeUVMap');uv.uv_map='FrontProjection'
 links.new(uv.outputs['UV'],tex.inputs['Vector']);links.new(tex.outputs['Color'],nodes['Principled BSDF'].inputs['Base Color'])
 nodes['Principled BSDF'].inputs['Roughness'].default_value=.8
 return mat
projection=projection_material('Clean wardrobe front projection',front)
cape_projection=projection_material('Clean continuous cape projection',back)
for obj in wardrobe:
 obj.data.materials.clear();obj.data.materials.append(cape_projection if obj==cape_obj else projection)
 fu=obj.data.uv_layers.new(name='FrontProjection');slots=obj.data.attributes['appearance_slot']
 for p in obj.data.polygons:
  p.material_index=0;s=slots.data[p.index].value
  for li in p.loop_indices:
   co=obj.data.vertices[obj.data.loops[li].vertex_index].co;fu.data[li].uv=region_uv(co,s,False)
# Join wardrobe once, unwrap finalatlas, and bake clean input colors into right-side2048 native texture.
bpy.ops.object.select_all(action='DESELECT')
for obj in wardrobe:obj.select_set(True)
bpy.context.view_layer.objects.active=body;bpy.ops.object.join();body.name='Simplified keeper wardrobe'
body.data.uv_layers.new(name='WardrobeAtlas');body.data.uv_layers.active_index=len(body.data.uv_layers)-1
bpy.ops.object.mode_set(mode='EDIT');bpy.ops.mesh.select_all(action='SELECT');bpy.ops.uv.smart_project(angle_limit=1.1,island_margin=.004);bpy.ops.object.mode_set(mode='OBJECT')
newimage=bpy.data.images.new('Simplified wardrobe atlas',width=2048,height=2048,alpha=False)
for mat in [projection,cape_projection]:
 nodes=mat.node_tree.nodes;bake_node=nodes.new('ShaderNodeTexImage');bake_node.image=newimage;nodes.active=bake_node
scene=bpy.context.scene;scene.render.engine='CYCLES';scene.cycles.samples=16;scene.render.bake.use_selected_to_active=False;scene.render.bake.use_pass_direct=False;scene.render.bake.use_pass_indirect=False;scene.render.bake.use_pass_color=True;scene.render.bake.margin=12
bpy.ops.object.select_all(action='DESELECT');body.select_set(True);bpy.context.view_layer.objects.active=body
print('SIMPLIFIED_WARDROBE_BAKE_BEGIN',flush=True);bpy.ops.object.bake(type='DIFFUSE');print('SIMPLIFIED_WARDROBE_BAKE_DONE',flush=True)
newimage.filepath_raw=str(O/'SimplifiedWardrobe.png');newimage.file_format='PNG';newimage.save();newimage.pack()
combined=np.concatenate([retainedpixels,np.array(newimage.pixels[:],dtype=np.float32).reshape(2048,2048,4)],axis=1);atlas=bpy.data.images.new('Simplified combined keeper atlas',width=4096,height=2048,alpha=False);atlas.pixels=combined.ravel();atlas.filepath_raw=str(O/'SimplifiedKeeperAlbedo.png');atlas.file_format='PNG';atlas.save();atlas.pack();assert np.array_equal(np.array(atlas.pixels[:],dtype=np.float32).reshape(2048,4096,4)[:,:2048],retainedpixels)
uvvalues=[loop.uv.copy() for loop in body.data.uv_layers['WardrobeAtlas'].data]
while len(body.data.uv_layers):body.data.uv_layers.remove(body.data.uv_layers[-1])
uv=body.data.uv_layers.new(name='UVMap')
for loop,value in zip(uv.data,uvvalues):loop.uv=(.5+value.x*.5,value.y)
oldmaterial=bpy.data.materials['Keeper Reconstruction'];oldmaterial.name='Retained previous body material';oldimage=bpy.data.images['KeeperReconstructionAlbedo'];oldimage.name='Retained previous body atlas';atlas.name='KeeperReconstructionAlbedo'
material=bpy.data.materials.new('Keeper Reconstruction');material.use_nodes=True;tex=material.node_tree.nodes.new('ShaderNodeTexImage');tex.image=atlas;material.node_tree.links.new(tex.outputs['Color'],material.node_tree.nodes['Principled BSDF'].inputs['Base Color']);material.node_tree.nodes['Principled BSDF'].inputs['Roughness'].default_value=.8
body.data.materials.clear();body.data.materials.append(material)
for p in body.data.polygons:p.material_index=0
# Retained Head/Headwear/Hair still point to left-halfUV; Face material retains its separate native atlas.
for i,mat in enumerate(retained.data.materials):
 if mat==oldmaterial:retained.data.materials[i]=material
bpy.ops.object.select_all(action='DESELECT');body.select_set(True);retained.select_set(True);bpy.context.view_layer.objects.active=retained;bpy.ops.object.join();o=bpy.context.object;o.name='Keeper Macro Reconstruction'
# Compact identical material slots and triangulate once before export.
old=list(o.data.materials);canonical=[material,bpy.data.materials['Keeper Reconstruction Face']];indices=[canonical.index(old[p.material_index]) for p in o.data.polygons];o.data.materials.clear()
for mat in canonical:o.data.materials.append(mat)
for p,i in zip(o.data.polygons,indices):p.material_index=i
bm=bmesh.new();bm.from_mesh(o.data);bmesh.ops.triangulate(bm,faces=list(bm.faces),quad_method='BEAUTY',ngon_method='EAR_CLIP');bm.to_mesh(o.data);bm.free()
# Explicit anatomic weights inverse-bound into the untouched65-bone rest rig.
for v in o.data.vertices:
 ws=normalize({o.vertex_groups[g.group].name:g.weight for g in v.groups});skin=Matrix([[0.0]*4 for _ in range(4)])
 for idx in [g.group for g in v.groups]:o.vertex_groups[idx].remove([v.index])
 for name,w in ws.items():o.vertex_groups[name].add([v.index],w,'REPLACE');skin+=posed[name]*w
 v.co=skin.inverted()@v.co
o.parent=rig;o.modifiers.new('Shared65bone anatomical rig','ARMATURE').object=rig
for b in rig.pose.bones:b.matrix_basis.identity()
bpy.context.view_layer.update();o.data.calc_loop_triangles();assert len(o.data.materials)==2;assert all(rig.data.bones[n].matrix_local==m for n,m in rest.items());assert all(1<=len(v.groups)<=4 and abs(sum(g.weight for g in v.groups)-1)<1e-5 for v in o.data.vertices)
slotcounts={}
for entry in o.data.attributes['appearance_slot'].data:slotcounts[entry.value]=slotcounts.get(entry.value,0)+1
assert set(slotcounts)<={1,2,3,10,11,12,13,14,15,16} and 0 not in slotcounts
bpy.ops.wm.save_as_mainfile(filepath=str(O/'SimplifiedKeeper.blend'),compress=True)
scene.cycles.samples=32;scene.render.resolution_x=1080;scene.render.resolution_y=1440;scene.render.resolution_percentage=100;scene.render.film_transparent=False;scene.view_settings.view_transform='AgX';cam=scene.camera;cam.data.type='ORTHO';cam.data.ortho_scale=2.18
for name,loc,raised in [('simplified-front',(0,-5,1),False),('simplified-back',(0,5,1),False),('simplified-point',(3,-5,1.1),True)]:
 for b in rig.pose.bones:b.matrix_basis.identity()
 for side,a in [('l',-65),('r',65)]:rig.pose.bones['upperarm_'+side].rotation_euler.z=math.radians(a)
 if raised:rig.pose.bones['upperarm_l'].rotation_euler.z=math.radians(-20);rig.pose.bones['lowerarm_l'].rotation_mode='XYZ';rig.pose.bones['lowerarm_l'].rotation_euler.z=math.radians(-35)
 cam.location=loc;cam.rotation_euler=(Vector((0,0,1))-cam.location).to_track_quat('-Z','Y').to_euler();scene.render.filepath=str(O/(name+'.png'));bpy.ops.render.render(write_still=True)
receipt={'triangles':len(o.data.loop_triangles),'vertices':len(o.data.vertices),'slots':slotcounts,'body_atlas':[4096,2048],'retained_left2048_exact':True,'bones':65,'materials':2,'wardrobe_color_source':'clean front and back standalone modeling inputs','runtime_verified':False}
(O/'simplified-receipt.json').write_text(json.dumps(receipt,indent=2));print('LGO_SIMPLIFIED_KEEPER',json.dumps(receipt),flush=True)
